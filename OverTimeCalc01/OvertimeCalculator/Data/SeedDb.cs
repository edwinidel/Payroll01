using OvertimeCalculator.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace OvertimeCalculator.Data
{
    public static class SeedDb
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<OvertimeDbContext>();
                
                try
                {
                    // Apply migrations to ensure database is up to date
                    await context.Database.MigrateAsync();

                    // Seed Roles
                    await SeedRolesAsync(context);

                    // Seed DayFactors
                    await SeedDayFactorsAsync(context);

                    // Seed SuperUser
                    await SeedSuperUserAsync(context);

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error during database seeding", ex);
                }
            }
        }

        private static async Task SeedRolesAsync(OvertimeDbContext context)
        {
            if (context.Roles.Any())
                return;

            var roles = new List<Role>
            {
                new Role
                {
                    Name = "SuperAdmin",
                    Description = "Super Administrator with full system access"
                },
                new Role
                {
                    Name = "Admin",
                    Description = "Administrator with management capabilities"
                },
                new Role
                {
                    Name = "Manager",
                    Description = "Manager who can view and process overtime calculations"
                },
                new Role
                {
                    Name = "Employee",
                    Description = "Regular employee who can view own information"
                }
            };

            await context.Roles.AddRangeAsync(roles);
        }

        private static async Task SeedDayFactorsAsync(OvertimeDbContext context)
        {
            if (context.DayFactors.Any())
                return;

            var dayFactors = new List<DayFactor>
            {
                new DayFactor
                {
                    DayType = "Regular",
                    Factor = 1.0,
                    Description = "Regular working day"
                },
                new DayFactor
                {
                    DayType = "Mixto",
                    Factor = 1.066666666666667,
                    Description = "Mixed working day"
                },
                new DayFactor
                {
                    DayType = "Nocturno",
                    Factor = 1.142857142857143,
                    Description = "Night working day"
                },
                new DayFactor
                {
                    DayType = "Domingo",
                    Factor = 1.5,
                    Description = "Sunday - holiday factor"
                },
                new DayFactor
                {
                    DayType = "Fiesta",
                    Factor = 2.5,
                    Description = "Public holiday - double pay plus half"
                },
                new DayFactor
                {
                    DayType = "Duelo Nacional",
                    Factor = 2.5,
                    Description = "National mourning day - double pay plus half"
                },
                new DayFactor
                {
                    DayType = "Domingo Compensatorio",
                    Factor = 2.0,
                    Description = "Compensatory Sunday - double pay"
                }
            };

            await context.DayFactors.AddRangeAsync(dayFactors);
        }

        private static async Task SeedSuperUserAsync(OvertimeDbContext context)
        {
            if (context.Users.Any(u => u.Username == "edwinidel@gmail.com"))
                return;

            // Get or create SuperAdmin role
            var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
            if (superAdminRole == null)
            {
                superAdminRole = new Role
                {
                    Name = "SuperAdmin",
                    Description = "Super Administrator with full system access"
                };
                context.Roles.Add(superAdminRole);
                await context.SaveChangesAsync();
            }

            var superUser = new User
            {
                Username = "edwinidel@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Edwin123*"),
                RoleId = superAdminRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(superUser);
        }
    }
}
