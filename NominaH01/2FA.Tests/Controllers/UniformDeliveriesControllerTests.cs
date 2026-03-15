using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using _2FA.Controllers;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Tests.Helpers;

namespace _2FA.Tests.Controllers
{
    public class UniformDeliveriesControllerTests
    {
        private UniformDeliveriesController BuildController(
            out ApplicationDbContext context,
            out CompanyEntity company,
            out EmployeeEntity employee,
            out TestSession session)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new ApplicationDbContext(options);

            company = new CompanyEntity { Name = "Test Co", IsActive = true };
            employee = new EmployeeEntity
            {
                FirstName = "Ana",
                LastName = "García",
                Company = company,
                CompanyId = company.Id,
                Status = "activo"
            };

            context.Companies.Add(company);
            context.Employees.Add(employee);
            context.SaveChanges();

            var controller = new UniformDeliveriesController(context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "tester"),
                new Claim(ClaimTypes.Name, "tester")
            }, "mock"));

            session = new TestSession();
            session.SetInt32("SelectedCompanyId", company.Id);

            var httpContext = new DefaultHttpContext
            {
                User = user,
                Session = session
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            return controller;
        }

        [Fact]
        public async Task Index_FiltersByCompany_ExcludesDeleted()
        {
            var controller = BuildController(out var context, out var company, out var employee, out _);

            var otherCompany = new CompanyEntity { Name = "Other Co", IsActive = true };
            context.Companies.Add(otherCompany);
            context.SaveChanges();

            var valid = new UniformDeliveryEntity
            {
                CompanyId = company.Id,
                EmployeeId = employee.Id,
                ItemName = "Camisa",
                DeliveryDate = DateTime.UtcNow.Date,
                ValidityDays = 30,
                ExpirationDate = DateTime.UtcNow.Date.AddDays(30)
            };
            var deleted = new UniformDeliveryEntity
            {
                CompanyId = company.Id,
                EmployeeId = employee.Id,
                ItemName = "Pantalón",
                DeliveryDate = DateTime.UtcNow.Date,
                ValidityDays = 30,
                ExpirationDate = DateTime.UtcNow.Date.AddDays(30),
                IsDeleted = true
            };
            var otherCompanyDelivery = new UniformDeliveryEntity
            {
                CompanyId = otherCompany.Id,
                EmployeeId = employee.Id,
                ItemName = "Zapatos",
                DeliveryDate = DateTime.UtcNow.Date,
                ValidityDays = 30,
                ExpirationDate = DateTime.UtcNow.Date.AddDays(30)
            };

            context.UniformDeliveries.AddRange(valid, deleted, otherCompanyDelivery);
            context.SaveChanges();

            var result = await controller.Index(null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<UniformDeliveryEntity>>(result.Model);

            Assert.Single(model);
            Assert.Equal("Camisa", model.First().ItemName);
        }

        [Fact]
        public async Task Create_SetsCompanyAndExpiration_Saves()
        {
            var controller = BuildController(out var context, out var company, out var employee, out _);

            var deliveryDate = new DateTime(2025, 1, 1);
            var validityDays = 10;

            var payload = new UniformDeliveryEntity
            {
                EmployeeId = employee.Id,
                ItemName = "Camisa",
                Brand = "Marca X",
                Size = "M",
                Color = "Azul",
                Quantity = 1,
                DeliveryDate = deliveryDate,
                ValidityDays = validityDays,
                ExpirationDate = deliveryDate // will be recalculated
            };

            var result = await controller.Create(payload);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var saved = context.UniformDeliveries.Single();
            Assert.Equal(company.Id, saved.CompanyId);
            Assert.Equal(deliveryDate.AddDays(validityDays), saved.ExpirationDate);
            Assert.Equal("tester", saved.CreatedBy);
        }

        [Fact]
        public async Task Edit_RecalculatesExpiration_Updates()
        {
            var controller = BuildController(out var context, out var company, out var employee, out _);

            var existing = new UniformDeliveryEntity
            {
                CompanyId = company.Id,
                EmployeeId = employee.Id,
                ItemName = "Pantalón",
                DeliveryDate = new DateTime(2025, 2, 1),
                ValidityDays = 5,
                ExpirationDate = new DateTime(2025, 2, 6)
            };
            context.UniformDeliveries.Add(existing);
            context.SaveChanges();

            var updated = new UniformDeliveryEntity
            {
                Id = existing.Id,
                CompanyId = company.Id,
                EmployeeId = employee.Id,
                ItemName = "Pantalón",
                DeliveryDate = new DateTime(2025, 2, 10),
                ValidityDays = 20,
                ExpirationDate = existing.ExpirationDate // will be recalculated
            };

            var result = await controller.Edit(existing.Id, updated);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var saved = context.UniformDeliveries.Single();
            Assert.Equal(new DateTime(2025, 3, 2), saved.ExpirationDate);
            Assert.NotNull(saved.Modified);
        }

        [Fact]
        public async Task DeleteConfirmed_MarksAsDeleted()
        {
            var controller = BuildController(out var context, out var company, out var employee, out _);

            var delivery = new UniformDeliveryEntity
            {
                CompanyId = company.Id,
                EmployeeId = employee.Id,
                ItemName = "Zapatos",
                DeliveryDate = DateTime.UtcNow.Date,
                ValidityDays = 15,
                ExpirationDate = DateTime.UtcNow.Date.AddDays(15)
            };
            context.UniformDeliveries.Add(delivery);
            context.SaveChanges();

            var result = await controller.DeleteConfirmed(delivery.Id);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var saved = context.UniformDeliveries.Single();
            Assert.True(saved.IsDeleted);
            Assert.NotNull(saved.Deleted);
        }
    }
}
