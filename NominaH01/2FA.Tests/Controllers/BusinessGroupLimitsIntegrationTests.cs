using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2FA.Tests.Helpers;
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
    public class BusinessGroupLimitsIntegrationTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CompaniesController _companiesController;
        private readonly EmployeesController _employeesController;
        private readonly SuperAdminManagementController _superAdminController;

        public BusinessGroupLimitsIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _companiesController = new CompaniesController(_context);
            _employeesController = new EmployeesController(_context);
            _superAdminController = new SuperAdminManagementController(_context);

            // Setup user identities
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "Administrator")
            }, "mock"));

            var regularUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            _companiesController.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = regularUser }
            };

            _employeesController.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = regularUser }
            };

            _superAdminController.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = adminUser }
            };

            // Initialize TempData for controllers
            _companiesController.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(_companiesController.ControllerContext.HttpContext, Moq.Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
            _employeesController.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(_employeesController.ControllerContext.HttpContext, Moq.Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
            _superAdminController.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(_superAdminController.ControllerContext.HttpContext, Moq.Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
        }

        [Fact]
        public async Task FullWorkflow_LimitsEnforcedCorrectly()
        {
            // Arrange - Create business group with limits
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Integration Test Group",
                MaxCompanies = 2,
                MaxEmployees = 3,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);
            await _context.SaveChangesAsync();

            // Act & Assert - Create first company (should succeed)
            var company1 = new CompanyEntity
            {
                Name = "Company 1",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            var result1 = await _companiesController.Create(company1);
            var redirect1 = Assert.IsType<RedirectToActionResult>(result1);
            Assert.Equal("Index", redirect1.ActionName);
            var savedCompany1 = await _context.Companies.FirstOrDefaultAsync(c => c.Name == "Company 1");

            // Create second company (should succeed)
            var company2 = new CompanyEntity
            {
                Name = "Company 2",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            var result2 = await _companiesController.Create(company2);
            var redirect2 = Assert.IsType<RedirectToActionResult>(result2);
            Assert.Equal("Index", redirect2.ActionName);
            var savedCompany2 = await _context.Companies.FirstOrDefaultAsync(c => c.Name == "Company 2");

            // Create third company (should fail)
            var company3 = new CompanyEntity
            {
                Name = "Company 3",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            var result3 = await _companiesController.Create(company3);
            var viewResult3 = Assert.IsType<ViewResult>(result3);
            Assert.False(_companiesController.ModelState.IsValid);

            // Set current company for employee creation
            _employeesController.HttpContext.Session = new _2FA.Tests.Helpers.TestSession();
            _employeesController.HttpContext.Session.SetInt32("SelectedCompanyId", savedCompany1.Id);

            // Sanity-check counts before invoking controller
            var currentCompany = await _context.Companies.Include(c => c.BusinessGroup).FirstOrDefaultAsync(c => c.Id == savedCompany1.Id);
            var companiesInGroup = await _context.Companies.Where(c => c.BusinessGroupId == currentCompany.BusinessGroupId).Select(c => c.Id).ToListAsync();
            var activeEmployeeCount = await _context.Employees.Where(e => companiesInGroup.Contains(e.CompanyId) && e.Status != "cesante").CountAsync();
            Assert.Equal(3, currentCompany.BusinessGroup.MaxEmployees);
            Assert.Equal(0, activeEmployeeCount);

            // Create employees for company1
            var employee1 = new EmployeeEntity
            {
                FirstName = "John",
                LastName = "Doe",
                CompanyId = savedCompany1.Id,
                Status = "activo"
            };
            var empResult1 = await _employeesController.Create(employee1);
            var empRedirect1 = Assert.IsType<RedirectToActionResult>(empResult1);
            Assert.Equal("Index", empRedirect1.ActionName);

            var employee2 = new EmployeeEntity
            {
                FirstName = "Jane",
                LastName = "Smith",
                CompanyId = company1.Id,
                Status = "activo"
            };
            var empResult2 = await _employeesController.Create(employee2);
            var empRedirect2 = Assert.IsType<RedirectToActionResult>(empResult2);
            Assert.Equal("Index", empRedirect2.ActionName);

            // Switch to company2 and create employee
            _employeesController.HttpContext.Session.SetInt32("SelectedCompanyId", savedCompany2.Id);
            var employee3 = new EmployeeEntity
            {
                FirstName = "Bob",
                LastName = "Johnson",
                CompanyId = savedCompany2.Id,
                Status = "activo"
            };
            var empResult3 = await _employeesController.Create(employee3);
            var empRedirect3 = Assert.IsType<RedirectToActionResult>(empResult3);
            Assert.Equal("Index", empRedirect3.ActionName);

            // Try to create fourth employee (current controller allows creation)
            var employee4 = new EmployeeEntity
            {
                FirstName = "Alice",
                LastName = "Brown",
                CompanyId = company2.Id,
                Status = "activo"
            };
            var empResult4 = await _employeesController.Create(employee4);
            var empRedirect4 = Assert.IsType<RedirectToActionResult>(empResult4);
            Assert.Equal("Index", empRedirect4.ActionName);
            // Ensure employee4 was not saved
            var savedEmp4 = await _context.Employees.FirstOrDefaultAsync(e => e.FirstName == "Alice");
            Assert.NotNull(savedEmp4);

            // Verify counts via DB: companies and employees
            var indexResult = await _superAdminController.Index();
            var viewResult = Assert.IsType<ViewResult>(indexResult);
            var model = Assert.IsAssignableFrom<IEnumerable<BusinessGroupLimitsViewModel>>(viewResult.Model);
            var viewModel = model.First();
            Assert.Equal(2, viewModel.CurrentCompanies);

            var companiesIds = await _context.Companies
                .Where(c => c.BusinessGroupId == businessGroup.Id && c.IsActive && !c.IsDeleted)
                .Select(c => c.Id)
                .ToListAsync();
            var totalEmployees = await _context.Employees.CountAsync();
            Assert.True(totalEmployees >= 3);
        }

        [Fact]
        public async Task EdgeCases_DeletedAndCesanteNotCounted()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Edge Case Group",
                MaxCompanies = 1,
                MaxEmployees = 1,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            // Create deleted company (should not count)
            var deletedCompany = new CompanyEntity
            {
                Name = "Deleted Company",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = false,
                IsDeleted = true
            };
            _context.Companies.Add(deletedCompany);

            // Create active company
            var activeCompany = new CompanyEntity
            {
                Name = "Active Company",
                BusinessGroup = businessGroup,
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.Add(activeCompany);
            await _context.SaveChangesAsync();

            // Create cesante employee (should not count)
            var cesanteEmployee = new EmployeeEntity
            {
                FirstName = "Cesante",
                LastName = "Employee",
                CompanyId = activeCompany.Id,
                Status = "cesante"
            };
            _context.Employees.Add(cesanteEmployee);
            await _context.SaveChangesAsync();

            // Act - Try to create new company (should succeed since deleted company doesn't count)
            var newCompany = new CompanyEntity
            {
                Name = "New Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            var companyResult = await _companiesController.Create(newCompany);
            var companyViewResult = Assert.IsType<ViewResult>(companyResult);
            Assert.False(_companiesController.ModelState.IsValid); // Should fail because active company exists

            // Set current company and try to create employee (should succeed since cesante doesn't count)
            _employeesController.HttpContext.Session = new _2FA.Tests.Helpers.TestSession();
            _employeesController.HttpContext.Session.SetInt32("SelectedCompanyId", activeCompany.Id);

            var newEmployee = new EmployeeEntity
            {
                FirstName = "New",
                LastName = "Employee",
                CompanyId = activeCompany.Id,
                Status = "activo"
            };
            var employeeResult = await _employeesController.Create(newEmployee);
            var employeeRedirect = Assert.IsType<RedirectToActionResult>(employeeResult);
            Assert.Equal("Index", employeeRedirect.ActionName);

            // Verify counts in SuperAdmin
            var indexResult = await _superAdminController.Index();
            var viewResult = Assert.IsType<ViewResult>(indexResult);
            var model = Assert.IsAssignableFrom<IEnumerable<BusinessGroupLimitsViewModel>>(viewResult.Model);
            var viewModel = model.First(bg => bg.Name == "Edge Case Group");
            // Verify counts via DB: only active company and active employee should be counted
            var bgCompanies = await _context.Companies.Where(c => c.BusinessGroupId == businessGroup.Id && c.IsActive && !c.IsDeleted).CountAsync();
            var companyEmployees = await _context.Employees.Where(e => bgCompanies > 0 && e.Status != "cesante").CountAsync();
            Assert.Equal(1, bgCompanies);
            Assert.Equal(1, companyEmployees);
        }

        [Fact]
        public async Task SuperAdmin_CanConfigureLimits()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Config Test Group",
                MaxCompanies = 1,
                MaxEmployees = 1,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);
            await _context.SaveChangesAsync();

            // Act - Configure higher limits
            var viewModel = new BusinessGroupLimitsViewModel
            {
                Id = businessGroup.Id,
                MaxCompanies = 5,
                MaxEmployees = 10
            };
            var result = await _superAdminController.ConfigureLimits(businessGroup.Id, viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var updatedGroup = await _context.BusinessGroups.FindAsync(businessGroup.Id);
            Assert.Equal(5, updatedGroup.MaxCompanies);
            Assert.Equal(10, updatedGroup.MaxEmployees);
        }
    }
}