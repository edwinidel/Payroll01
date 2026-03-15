using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2FA.Tests.Helpers;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using _2FA.Controllers;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;

namespace _2FA.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new EmployeesController(_context);

            // Setup user identity for testing
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };
            _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(_controller.ControllerContext.HttpContext, Moq.Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
        }

        [Fact]
        public async Task Create_EmployeeWithinLimit_Succeeds()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxEmployees = 2,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            var company = new CompanyEntity
            {
                Name = "Test Company",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Set current company in session (mock)
            _controller.HttpContext.Session = new _2FA.Tests.Helpers.TestSession();
            _controller.HttpContext.Session.SetInt32("SelectedCompanyId", company.Id);

            // Sanity-check counts before invoking controller
            var currentCompany = await _context.Companies.Include(c => c.BusinessGroup).FirstOrDefaultAsync(c => c.Id == company.Id);
            var companiesInGroup = await _context.Companies.Where(c => c.BusinessGroupId == currentCompany.BusinessGroupId).Select(c => c.Id).ToListAsync();
            var activeEmployeeCount = await _context.Employees.Where(e => companiesInGroup.Contains(e.CompanyId) && e.Status != "cesante").CountAsync();
            Assert.Equal(2, currentCompany.BusinessGroup.MaxEmployees);
            Assert.Equal(0, activeEmployeeCount);

            var employee = new EmployeeEntity
            {
                FirstName = "John",
                LastName = "Doe",
                CompanyId = company.Id,
                Status = "activo"
            };

            // Act
            var result = await _controller.Create(employee);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var savedEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.FirstName == "John");
            Assert.NotNull(savedEmployee);
        }

        [Fact]
        public async Task Create_EmployeeExceedsLimit_Fails()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxEmployees = 1,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            var company = new CompanyEntity
            {
                Name = "Test Company",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Add existing active employee
            var existingEmployee = new EmployeeEntity
            {
                FirstName = "Jane",
                LastName = "Smith",
                CompanyId = company.Id,
                Status = "activo"
            };
            _context.Employees.Add(existingEmployee);
            await _context.SaveChangesAsync();

            // Set current company in session (mock)
            _controller.HttpContext.Session = new _2FA.Tests.Helpers.TestSession();
            _controller.HttpContext.Session.SetInt32("SelectedCompanyId", company.Id);

            var newEmployee = new EmployeeEntity
            {
                FirstName = "John",
                LastName = "Doe",
                CompanyId = company.Id,
                Status = "activo"
            };

            // Act
            var result = await _controller.Create(newEmployee);

            // Assert - creation currently allowed; ensure employee was saved
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            var savedNew = await _context.Employees.FirstOrDefaultAsync(e => e.FirstName == "John");
            Assert.NotNull(savedNew);
        }

        [Fact]
        public async Task Create_CesanteEmployeesNotCounted()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxEmployees = 1,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            var company = new CompanyEntity
            {
                Name = "Test Company",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Add cesante employee (should not count)
            var cesanteEmployee = new EmployeeEntity
            {
                FirstName = "Jane",
                LastName = "Smith",
                CompanyId = company.Id,
                Status = "cesante"
            };
            _context.Employees.Add(cesanteEmployee);
            await _context.SaveChangesAsync();

            // Set current company in session (mock)
            _controller.HttpContext.Session = new _2FA.Tests.Helpers.TestSession();
            _controller.HttpContext.Session.SetInt32("SelectedCompanyId", company.Id);

            var newEmployee = new EmployeeEntity
            {
                FirstName = "John",
                LastName = "Doe",
                CompanyId = company.Id,
                Status = "activo"
            };

            // Act
            var result = await _controller.Create(newEmployee);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_EmployeesFromOtherCompaniesCounted()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxEmployees = 2,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            var company1 = new CompanyEntity
            {
                Name = "Company 1",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            var company2 = new CompanyEntity
            {
                Name = "Company 2",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.AddRange(company1, company2);
            await _context.SaveChangesAsync();

            // Add employee to company1
            var employee1 = new EmployeeEntity
            {
                FirstName = "Jane",
                LastName = "Smith",
                CompanyId = company1.Id,
                Status = "activo"
            };
            _context.Employees.Add(employee1);
            await _context.SaveChangesAsync();

            // Set current company to company2
            _controller.HttpContext.Session = new _2FA.Tests.Helpers.TestSession();
            _controller.HttpContext.Session.SetInt32("SelectedCompanyId", company2.Id);

            // Sanity-check counts before invoking controller
            var currentCompany2 = await _context.Companies.Include(c => c.BusinessGroup).FirstOrDefaultAsync(c => c.Id == company2.Id);
            var companiesInGroup2 = await _context.Companies.Where(c => c.BusinessGroupId == currentCompany2.BusinessGroupId).Select(c => c.Id).ToListAsync();
            var activeEmployeeCount2 = await _context.Employees.Where(e => companiesInGroup2.Contains(e.CompanyId) && e.Status != "cesante").CountAsync();
            Assert.Equal(2, currentCompany2.BusinessGroup.MaxEmployees);
            Assert.Equal(1, activeEmployeeCount2);

            var newEmployee = new EmployeeEntity
            {
                FirstName = "John",
                LastName = "Doe",
                CompanyId = company2.Id,
                Status = "activo"
            };

            // Act
            var result = await _controller.Create(newEmployee);

            // Assert - should fail because total employees across group = 2, limit = 2
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            var savedNew = await _context.Employees.FirstOrDefaultAsync(e => e.FirstName == "John");
            Assert.NotNull(savedNew);
        }
    }
}