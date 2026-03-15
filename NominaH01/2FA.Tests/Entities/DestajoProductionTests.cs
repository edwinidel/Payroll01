using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Tests.Entities
{
    public class DestajoProductionTests
    {
        [Fact]
        public async Task Save_CalculatesTotalAmount()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                // arrange: create an employee first
                var employee = new EmployeeEntity { FirstName = "Worker", LastName = "A" };
                context.Employees.Add(employee);
                await context.SaveChangesAsync();

                var prod = new DestajoProductionEntity
                {
                    EmployeeId = employee.Id,
                    ProductionDate = new DateTime(2025, 1, 1),
                    UnitsProduced = 12.345m,
                    UnitValue = 2.50m
                };

                // act
                context.DestajoProductions.Add(prod);
                await context.SaveChangesAsync();

                // assert
                var saved = context.DestajoProductions.FirstOrDefault();
                Assert.NotNull(saved);
                // Total should be rounded to 2 decimals per calculation
                Assert.Equal(decimal.Round(12.345m * 2.50m, 2, MidpointRounding.AwayFromZero), saved.TotalAmount);
            }
        }

        [Fact]
        public async Task Update_RecalculatesTotalAmount()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var employee = new EmployeeEntity { FirstName = "Worker", LastName = "B" };
                context.Employees.Add(employee);
                await context.SaveChangesAsync();

                var prod = new DestajoProductionEntity
                {
                    EmployeeId = employee.Id,
                    ProductionDate = new DateTime(2025, 2, 1),
                    UnitsProduced = 10m,
                    UnitValue = 1.00m
                };

                context.DestajoProductions.Add(prod);
                await context.SaveChangesAsync();

                // change units and save
                prod.UnitsProduced = 20.5m;
                context.DestajoProductions.Update(prod);
                await context.SaveChangesAsync();

                var saved = context.DestajoProductions.FirstOrDefault();
                Assert.NotNull(saved);
                Assert.Equal(decimal.Round(20.5m * 1.00m, 2, MidpointRounding.AwayFromZero), saved.TotalAmount);
            }
        }
    }
}
