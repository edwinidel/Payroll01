using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CompaniesControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CompaniesController _controller;

        public CompaniesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new CompaniesController(_context);

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
        public async Task Create_CompanyWithinLimit_Succeeds()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 2,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);
            await _context.SaveChangesAsync();

            var company = new CompanyEntity
            {
                Name = "Test Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };

            // Act
            var result = await _controller.Create(company);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var savedCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Name == "Test Company");
            Assert.NotNull(savedCompany);
        }

        [Fact]
        public async Task Create_CompanyExceedsLimit_Fails()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 1,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            // Add existing company
            var existingCompany = new CompanyEntity
            {
                Name = "Existing Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };
            _context.Companies.Add(existingCompany);
            await _context.SaveChangesAsync();

            var newCompany = new CompanyEntity
            {
                Name = "New Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };

            // Act
            var result = await _controller.Create(newCompany);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            var errors = _controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Assert.True(errors.Any(msg => msg.Contains("ha alcanzado el límite máximo")));
        }

        [Fact]
        public async Task Create_DeletedCompaniesNotCounted()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 1,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            // Add deleted company (should not count)
            var deletedCompany = new CompanyEntity
            {
                Name = "Deleted Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = false,
                IsDeleted = true
            };
            _context.Companies.Add(deletedCompany);
            await _context.SaveChangesAsync();

            var newCompany = new CompanyEntity
            {
                Name = "New Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };

            // Act
            var result = await _controller.Create(newCompany);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_InactiveCompaniesNotCounted()
        {
            // Arrange
            var businessGroup = new BusinessGroupEntity
            {
                Name = "Test Group",
                MaxCompanies = 1,
                IsActive = true
            };
            _context.BusinessGroups.Add(businessGroup);

            // Add inactive company (should not count)
            var inactiveCompany = new CompanyEntity
            {
                Name = "Inactive Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = false,
                IsDeleted = false
            };
            _context.Companies.Add(inactiveCompany);
            await _context.SaveChangesAsync();

            var newCompany = new CompanyEntity
            {
                Name = "New Company",
                BusinessGroupId = businessGroup.Id,
                IsActive = true
            };

            // Act
            var result = await _controller.Create(newCompany);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}