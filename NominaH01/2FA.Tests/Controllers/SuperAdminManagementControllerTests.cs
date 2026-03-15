using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class SuperAdminManagementControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly SuperAdminManagementController _controller;

        public SuperAdminManagementControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new SuperAdminManagementController(_context);

            // Setup admin user identity for testing
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "Administrator")
            }, "mock"));
            _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };
            _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(_controller.ControllerContext.HttpContext, Moq.Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
        }

        [Fact]
        public async Task Index_ReturnsViewWithBusinessGroups()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 5,
                MaxEmployees = 10,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            var company = new CompanyEntity
            {
                Name = "Test Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.Add(company);

            var employee = new EmployeeEntity
            {
                FirstName = "John",
                LastName = "Doe",
                CompanyId = company.Id,
                Status = "activo"
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BusinessGroupLimitsViewModel>>(viewResult.Model);
            var viewModel = model.First();
            Assert.Equal("Test Group", viewModel.Name);
            Assert.Equal(5, viewModel.MaxCompanies);
            Assert.Equal(10, viewModel.MaxEmployees);
            Assert.Equal(1, viewModel.CurrentCompanies);
            Assert.Equal(1, viewModel.CurrentEmployees);
        }

        [Fact]
        public async Task Index_DeletedCompaniesNotCounted()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 5,
                MaxEmployees = 10,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            var activeCompany = new CompanyEntity
            {
                Name = "Active Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true,
                IsDeleted = false
            };
            var deletedCompany = new CompanyEntity
            {
                Name = "Deleted Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = false,
                IsDeleted = true
            };
            _context.Companies.AddRange(activeCompany, deletedCompany);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BusinessGroupLimitsViewModel>>(viewResult.Model);
            var viewModel = model.First();
            Assert.Equal(1, viewModel.CurrentCompanies); // Only active company counted
        }

        [Fact]
        public async Task Index_CesanteEmployeesNotCounted()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 5,
                MaxEmployees = 10,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            var company = new CompanyEntity
            {
                Name = "Test Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.Add(company);

            var activeEmployee = new EmployeeEntity
            {
                FirstName = "John",
                LastName = "Doe",
                CompanyId = company.Id,
                Status = "activo"
            };
            var cesanteEmployee = new EmployeeEntity
            {
                FirstName = "Jane",
                LastName = "Smith",
                CompanyId = company.Id,
                Status = "cesante"
            };
            _context.Employees.AddRange(activeEmployee, cesanteEmployee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BusinessGroupLimitsViewModel>>(viewResult.Model);
            var viewModel = model.First();
            Assert.Equal(1, viewModel.CurrentEmployees); // Only active employee counted
        }

        [Fact]
        public async Task ConfigureLimits_ValidLimits_Succeeds()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 5,
                MaxEmployees = 10,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);
            await _context.SaveChangesAsync();

            var viewModel = new BusinessGroupLimitsViewModel
            {
                Id = businessGroup.Id,
                MaxCompanies = 10,
                MaxEmployees = 20
            };

            // Act
            var result = await _controller.ConfigureLimits(businessGroup.Id, viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var updatedGroup = await _context.BusinessGroups.FindAsync(businessGroup.Id);
            Assert.Equal(10, updatedGroup.MaxCompanies);
            Assert.Equal(20, updatedGroup.MaxEmployees);
        }

        [Fact]
        public async Task ConfigureLimits_LimitsBelowCurrentUsage_Fails()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 5,
                MaxEmployees = 10,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            var company = new CompanyEntity
            {
                Name = "Test Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.Add(company);

            var employee = new EmployeeEntity
            {
                FirstName = "John",
                LastName = "Doe",
                CompanyId = company.Id,
                Status = "activo"
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var viewModel = new BusinessGroupLimitsViewModel
            {
                Id = businessGroup.Id,
                MaxCompanies = 0, // Below current usage of 1
                MaxEmployees = 0  // Below current usage of 1
            };

            // Act
            var result = await _controller.ConfigureLimits(businessGroup.Id, viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Contains(_controller.ModelState["MaxCompanies"].Errors,
                e => e.ErrorMessage.Contains("menor al número actual"));
            Assert.Contains(_controller.ModelState["MaxEmployees"].Errors,
                e => e.ErrorMessage.Contains("menor al número actual"));
        }

        [Fact]
        public async Task UsageReport_ReturnsViewWithUsageData()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 5,
                MaxEmployees = 10,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            var company = new CompanyEntity
            {
                Name = "Test Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.Add(company);

            var employee = new EmployeeEntity
            {
                FirstName = "John",
                LastName = "Doe",
                CompanyId = company.Id,
                Status = "activo"
            };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UsageReport();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BusinessGroupLimitsViewModel>>(viewResult.Model);
            var viewModel = model.First();
            Assert.Equal("Test Group", viewModel.Name);
            Assert.Equal(1, viewModel.CurrentCompanies);
            Assert.Equal(1, viewModel.CurrentEmployees);
        }
    }
}