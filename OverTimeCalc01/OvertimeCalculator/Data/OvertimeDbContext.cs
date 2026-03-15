using Microsoft.EntityFrameworkCore;
using OvertimeCalculator.Models;

namespace OvertimeCalculator.Data
{
    public class OvertimeDbContext : DbContext
    {
        public OvertimeDbContext(DbContextOptions<OvertimeDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<DayFactor> DayFactors { get; set; }
        public DbSet<CalculationHistory> CalculationHistories { get; set; }
        public DbSet<EmployeeInputHeader> EmployeeInputHeaders { get; set; }
        public DbSet<EmployeeInputDetail> EmployeeInputDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add unique constraint on DayFactor DayType
            modelBuilder.Entity<DayFactor>()
                .HasIndex(df => df.DayType)
                .IsUnique()
                .HasDatabaseName("IX_DayFactor_DayType");

            // Create indexes for frequent queries
            modelBuilder.Entity<CalculationHistory>()
                .HasIndex(h => h.Código)
                .HasDatabaseName("IX_CalculationHistory_Código");

            modelBuilder.Entity<CalculationHistory>()
                .HasIndex(h => h.FechaMarcacion)
                .HasDatabaseName("IX_CalculationHistory_FechaMarcacion");

            modelBuilder.Entity<CalculationHistory>()
                .HasIndex(h => new { h.Código, h.FechaMarcacion })
                .HasDatabaseName("IX_CalculationHistory_Código_FechaMarcacion");

            // Add explicit foreign key
            modelBuilder.Entity<CalculationHistory>()
                .HasOne(h => h.EmployeeInputHeader)
                .WithMany()
                .HasForeignKey(h => h.EmployeeInputHeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeInputDetail>()
                .HasOne(d => d.Header)
                .WithMany(h => h.Details)
                .HasForeignKey(d => d.HeaderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}