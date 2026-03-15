using _2FA.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _2FA.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmployeeEntity>(entity => {
                entity.Property(e => e.AdditionalIsr).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AgreeSalary).HasColumnType("decimal(18,6)");
                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18,2)");
                entity.Property(e => e.HourSalary).HasColumnType("decimal(18,6)");
                entity.Property(e => e.IsrFixed).HasColumnType("decimal(18,2)");
                entity.Property(e => e.RegularHours).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitValue).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<CompanyEntity>(entity => {
                entity.Property(e => e.ProfessionalRisk).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PayrollTmpLegalDeductionEntity>(entity => {
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PayrollTmpEmployeeEntity>(entity =>
            {
                entity.Property(e => e.NetPay).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AgreeSalary).HasColumnType("decimal(18,6)");
                entity.Property(e => e.HourlySalary).HasColumnType("decimal(18,6)");
                entity.Property(e => e.RegularHours).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PayrollTmpConceptEntity>(entity =>
            {
                entity.Property(e => e.Hours).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PayFactor).HasColumnType("decimal(18,6)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitAmount).HasColumnType("decimal(18,6)");
            });

            modelBuilder.Entity<PayrollTmpOvertimeEntity>(entity =>
            {
                entity.Property(e => e.CalculatedHours).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AppliedFactor).HasColumnType("decimal(18,6)");
                entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,6)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.WeeklyAccumulated).HasColumnType("decimal(18,2)");
            });


            modelBuilder.Entity<LiabilityEntity>(entity =>
            {
                entity.Property(e => e.InitialAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Dicsount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MaxPercentage).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<BranchEntity>(entity =>
            {
                entity.Property(e => e.ProfessionalRisk).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<HistoricLiabilityEntity>(entity =>
            {
                entity.Property(e => e.AmountToDiscount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountedAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<HistoricTmpLiabilityEntity>(entity =>
            {
                entity.Property(e => e.AmountToDiscount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountedAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<LegalDeductionEntity>(entity =>
            {
                entity.Property(e => e.EmployeeDiscount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EmployerDiscount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<LegalDeductionEntity>()
                .HasOne(ld => ld.Country)
                .WithMany(c => c.LegalDeductions)
                .HasForeignKey(ld => ld.CountryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OverTimeFactorEntity>(entity =>
            {
                entity.Property(e => e.Factor).HasColumnType("decimal(18,6)");
            });

            modelBuilder.Entity<OvertimeCodeEntity>(entity =>
            {
                entity.Property(e => e.PayFactor).HasColumnType("decimal(18,6)");
            });

            modelBuilder.Entity<PayrollConceptEntity>(entity =>
            {
                entity.Property(e => e.Hours).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitAmount).HasColumnType("decimal(18,6)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PayrollEmployeeEntity>(entity =>
            {
                entity.Property(e => e.TotalEarnings).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalDeductions).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NetPay).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PayrollHeaderEntity>(entity =>
            {
                entity.Property(e => e.UnitValue).HasColumnType("decimal(18,6)");
            });

            modelBuilder.Entity<PayrollLegalDeductionEntity>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PayrollTmpEmployeeEntity>(entity =>
            {
                entity.Property(e => e.RegularSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OverTimeSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OtherPayment).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalEarnings).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalLegalDiscount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalOtherDiscount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalDeductions).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitValue).HasColumnType("decimal(18,6)");
                entity.Property(e => e.UnitsProduced).HasColumnType("decimal(18,3)");
                entity.Property(e => e.DestajoAmount).HasColumnType("decimal(18,2)");
            });

            // Configure the one-to-one relationship between EmployeeEntity and EmployeeObservationEntity
            modelBuilder.Entity<EmployeeEntity>()
                .HasOne(e => e.EmployeeObservation) // An Employee has one EmployeeObservation
                .WithOne(eo => eo.Employee)         // An EmployeeObservation has one Employee
                .HasForeignKey<EmployeeObservationEntity>(eo => eo.EmployeeId);

            // Avoid cascade cycles for observation history
            modelBuilder.Entity<EmployeeObservationHistoryEntity>(entity =>
            {
                entity.HasOne(h => h.Employee)
                    .WithMany()
                    .HasForeignKey(h => h.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Company)
                    .WithMany()
                    .HasForeignKey(h => h.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.ObservationType)
                    .WithMany()
                    .HasForeignKey(h => h.ObservationTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
                modelBuilder.Entity<FixedPaymentEntity>(entity => {
                entity.Property(e => e.PayAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<PaymentConceptEntity>()
                .HasOne(pc => pc.Country) // Indica que un PaymentConcept tiene un solo Country
                .WithMany(c => c.PaymentConcepts) // Indica que un Country puede tener muchos PaymentConcepts
                .HasForeignKey(pc => pc.CountryId) // La columna de la clave foránea
                .OnDelete(DeleteBehavior.NoAction); // <--- ¡Esta es la clave!
                modelBuilder.Entity<PaymentConceptEntity>(entity => {
                entity.Property(e => e.PayFactor).HasColumnType("decimal(18,2)");

            });

            modelBuilder.Entity<FixedPaymentEntity>(entity => {
                entity.Property(e => e.PayAmount).HasColumnType("decimal(18,2)");

            });

            modelBuilder.Entity<PaymentGroupEntity>(entity => {
                entity.Property(e => e.BaseHours).HasColumnType("decimal(18,2)");

            });

            modelBuilder.Entity<PayrollTmpHeaderEntity>()
                .HasOne(e => e.Company)
                .WithMany()
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeEntity>()
                .HasOne(e => e.Activity)
                .WithMany() 
                .HasForeignKey(e => e.ActivityId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<EmployeeEntity>()
                .HasOne(e => e.PaymentGroup)
                .WithMany() 
                .HasForeignKey(e => e.PaymentGroupId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<LiabilityEntity>()
                .HasOne(e => e.Employee)
                .WithMany() 
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LegalDeductionEntity>()
                .HasOne(e => e.PayrollType)
                .WithMany()
                .HasForeignKey(e => e.PayrollTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollTmpHeaderEntity>()
                .HasOne(e => e.PayrollType)
                .WithMany()
                .HasForeignKey(e => e.PayrollTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConceptLegalDeductionEntity>()
                .HasOne(e => e.PaymentConcept)
                .WithMany()
                .HasForeignKey(e => e.PaymentConceptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConceptLegalDeductionEntity>()
                .HasOne(e => e.LegalDeduction)
                .WithMany()
                .HasForeignKey(e => e.LegalDeductionEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollTmpLegalDeductionEntity>()
                .HasOne(e => e.PayrollTmpEmployee)
                .WithMany()
                .HasForeignKey(e => e.PayrollTmpEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollTmpOvertimeEntity>()
                .HasOne(e => e.PayrollTmpHeader)
                .WithMany()
                .HasForeignKey(e => e.PayrollTmpHeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PieceworkUnitTypeEntity>()
                .HasOne(p => p.Company)
                .WithMany()
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PayrollTmpOvertimeEntity>()
                .HasOne(e => e.TypeOfDay)
                .WithMany()
                .HasForeignKey(e => e.TypeOfDayId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollTmpOvertimeEntity>()
                .HasOne(e => e.TypeOfWorkSchedule)
                .WithMany()
                .HasForeignKey(e => e.TypeOfWorkScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ShiftEntity relationships
            modelBuilder.Entity<ShiftEntity>()
                .HasOne(s => s.Schedule)
                .WithMany(sch => sch.Shifts)
                .HasForeignKey(s => s.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ShiftSegmentEntity relationships
            modelBuilder.Entity<ShiftSegmentEntity>()
                .HasOne(ss => ss.Shift)
                .WithMany(s => s.ShiftSegments)
                .HasForeignKey(ss => ss.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ShiftAssignmentEntity relationships
            modelBuilder.Entity<ShiftAssignmentEntity>()
                .HasOne(sa => sa.Employee)
                .WithMany(e => e.ShiftAssignments)
                .HasForeignKey(sa => sa.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShiftAssignmentEntity>()
                .HasOne(sa => sa.Shift)
                .WithMany(s => s.ShiftAssignments)
                .HasForeignKey(sa => sa.ShiftId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShiftAssignmentEntity>()
                .HasOne(sa => sa.Schedule)
                .WithMany(sch => sch.ShiftAssignments)
                .HasForeignKey(sa => sa.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure UserCompanyEntity relationships
            modelBuilder.Entity<UserCompanyEntity>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCompanies)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCompanyEntity>()
                .HasOne(uc => uc.Company)
                .WithMany()
                .HasForeignKey(uc => uc.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure CostCenterPaymentConceptAccount relationships
            modelBuilder.Entity<CostCenterPaymentConceptAccountEntity>()
                .HasOne(ccpca => ccpca.CostCenter)
                .WithMany(c => c.CostCenterPaymentConceptAccounts)
                .HasForeignKey(ccpca => ccpca.CostCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CostCenterPaymentConceptAccountEntity>()
                .HasOne(ccpca => ccpca.PaymentConcept)
                .WithMany()
                .HasForeignKey(ccpca => ccpca.PaymentConceptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CostCenterPaymentConceptAccountEntity>()
                .HasOne(ccpca => ccpca.Account)
                .WithMany()
                .HasForeignKey(ccpca => ccpca.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentConceptEntity>()
                .HasOne(pc => pc.Account)
                .WithMany()
                .HasForeignKey(pc => pc.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AccountEntity>()
                .HasOne(a => a.Company)
                .WithMany()
                .HasForeignKey(a => a.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PayrollVoucherFormatEntity relationships
            modelBuilder.Entity<PayrollVoucherFormatEntity>()
                .HasOne(pvf => pvf.Company)
                .WithMany()
                .HasForeignKey(pvf => pvf.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PayrollVoucherFormatEntity>()
                .HasOne(pvf => pvf.PayrollType)
                .WithMany()
                .HasForeignKey(pvf => pvf.PayrollTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PayrollLegalDeductionEntity relationships
            modelBuilder.Entity<PayrollLegalDeductionEntity>()
                .HasOne(e => e.PayrollHeader)
                .WithMany()
                .HasForeignKey(e => e.PayrollHeaderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PayrollLegalDeductionEntity>()
                .HasOne(e => e.PayrollEmployee)
                .WithMany(e => e.LegalDeductions)
                .HasForeignKey(e => e.PayrollEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PayrollLegalDeductionEntity>()
                .HasOne(e => e.LegalDeduction)
                .WithMany()
                .HasForeignKey(e => e.LegalDeductionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure PayrollEmployeeEntity relationships
            modelBuilder.Entity<PayrollEmployeeEntity>()
                .HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure PayrollHeaderEntity relationships
            modelBuilder.Entity<PayrollHeaderEntity>()
                .HasOne(e => e.PaymentGroup)
                .WithMany()
                .HasForeignKey(e => e.PaymentGroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PayrollHeaderEntity>()
                .HasOne(e => e.Company)
                .WithMany()
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure PayrollTmpEmployeeEntity Employee relationship
            modelBuilder.Entity<PayrollTmpEmployeeEntity>()
                .HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
            // Configure PayrollTmpEmployeeEntity Employee relationship
            modelBuilder.Entity<PayrollTmpEmployeeEntity>()
                .HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure BankDepositFieldEntity relationships
            modelBuilder.Entity<BankDepositFieldEntity>()
                .HasOne(bdf => bdf.BankDepositStructure)
                .WithMany(bds => bds.Fields)
                .HasForeignKey(bdf => bdf.BankDepositStructureId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BankDepositFieldEntity>()
                .HasOne(bdf => bdf.Company)
                .WithMany()
                .HasForeignKey(bdf => bdf.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // DocumentTemplate -> DocumentType (optional)
            modelBuilder.Entity<DocumentTemplateEntity>()
                .HasOne(dt => dt.DocumentType)
                .WithMany()
                .HasForeignKey(dt => dt.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // DestajoProduction -> Employee
            modelBuilder.Entity<DestajoProductionEntity>()
                .HasOne(dp => dp.Employee)
                .WithMany()
                .HasForeignKey(dp => dp.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index to speed up queries and help prevent duplicates per day
            modelBuilder.Entity<DestajoProductionEntity>()
                .HasIndex(dp => new { dp.EmployeeId, dp.ProductionDate });

            modelBuilder.Entity<PaystubEntity>(entity =>
            {
                entity.Property(e => e.Gross).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Net).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.PayrollHeader)
                    .WithMany()
                    .HasForeignKey(e => e.PayrollHeaderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CertificateRequestEntity>(entity =>
            {
                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<NotificationEntity>(entity =>
            {
                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<NotificationRuleEntity>();

            modelBuilder.Entity<UniformDeliveryEntity>(entity =>
            {
                entity.HasOne(e => e.Company)
                    .WithMany()
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.CompanyId, e.EmployeeId, e.ExpirationDate });
            });

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            CalculateDestajoTotals();
            return base.SaveChanges();
        }

        public override System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            CalculateDestajoTotals();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void CalculateDestajoTotals()
        {
            var entries = ChangeTracker.Entries<DestajoProductionEntity>()
                .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added || e.State == Microsoft.EntityFrameworkCore.EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = entry.Entity;
                entity.TotalAmount = decimal.Round(entity.UnitsProduced * entity.UnitValue, 2, MidpointRounding.AwayFromZero);
            }
        }

        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<ActivityEntity> Activities { get; set; }
        public DbSet<BankEntity> Banks { get; set; }
        public DbSet<BranchEntity> Branches { get; set; }
        public DbSet<BusinessGroupEntity> BusinessGroups { get; set; }
        public DbSet<CityEntity> Cities { get; set; }
        public DbSet<ConceptLegalDeductionEntity> ConceptLegalDeductions { get; set; }
        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<CompanyPayrollVoucherEntity> CompanyPayrollVouchers { get; set; }
        public DbSet<CostCenterPaymentConceptAccountEntity> CostCenterPaymentConceptAccounts { get; set; }
        public DbSet<CostCenterEntity> CostCenters { get; set; }
        public DbSet<CountryEntity> Countries { get; set; }
        public DbSet<CountryTimeZoneEntity> CountryTimeZones { get; set; }
        public DbSet<CreditorEntity> Creditors { get; set; }
        public DbSet<DepartmentEntity> Departments { get; set; }
        public DbSet<DivisionEntity> Divisions { get; set; }
        public DbSet<EmployeeEntity> Employees { get; set; }
        public DbSet<EmployeeObservationEntity> EmployeeObservations { get; set; }
        public DbSet<EmployeeObservationHistoryEntity> EmployeeObservationHistories { get; set; }
        public DbSet<EmployeeTypeEntity> EmployeeTypes { get; set; }
        public DbSet<FixedPaymentEntity> FixedPayments { get; set; }
        public DbSet<HistoricLiabilityEntity> HistoricLiabilities { get; set; }
        public DbSet<HistoricTmpLiabilityEntity> HistoricTmpLiabilities { get; set; }
        public DbSet<HoliDayEntity> HoliDays { get; set; }
        public DbSet<IdentityDocumentTypeEntity> IdentityDocumentTypes { get; set; }
        public DbSet<LegalDeductionEntity> LegalDeductions  { get; set; }
        public DbSet<LiabilityEntity> Liabilities { get; set; }
        public DbSet<ObservationTypeEntity> ObservationTypes { get; set; }
        public DbSet<OverTimeFactorEntity> OverTimeFactors { get; set; }
        public DbSet<PaymentConceptEntity> PaymentConcepts { get; set; }
        public DbSet<PaymentFrequencyEntity> PaymentFrequencies { get; set; }
        public DbSet<PaymentGroupEntity> PaymentGroups { get; set; }
        public DbSet<PayrollHeaderEntity> PayrollHeaders { get; set; }
        public DbSet<PayrollTypeEntity> PayrollTypes { get; set; }
        public DbSet<PayrollEmployeeEntity> PayrollEmployees { get; set; }
        public DbSet<PayrollLegalDeductionEntity> PayrollLegalDeductions { get; set; }
        public DbSet<PayrollConceptEntity> PayrollConcepts { get; set; }
        public DbSet<PayrollTmpHeaderEntity> PayrollTmpHeaders { get; set; }
        public DbSet<PayrollTmpEmployeeEntity> PayrollTmpEmployees { get; set; }
        public DbSet<PayrollTmpConceptEntity> PayrollTmpConcepts { get; set; }
        public DbSet<PayrollTmpLegalDeductionEntity> PayrollTmpLegalDeductions { get; set; }
        public DbSet<PayrollTmpOvertimeEntity> PayrollTmpOvertime { get; set; }
        public DbSet<PhaseEntity> Phases { get; set; }
        public DbSet<PositionEntity> Positions { get; set; }
        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<ScheduleEntity> Schedules { get; set; }
        public DbSet<ShiftEntity> Shifts { get; set; }
        public DbSet<ShiftSegmentEntity> ShiftSegments { get; set; }
        public DbSet<ShiftAssignmentEntity> ShiftAssignments { get; set; }
        public DbSet<StateEntity> States { get; set; }
        public DbSet<TimeZoneEntity> TimeZones { get; set; }
        public DbSet<TransitBankEntity> TransitBanks { get; set; }
        public DbSet<TypeOfWorkerEntity> TypeOfWorkers { get; set; }
        public DbSet<TypeOfWorkScheduleEntity> TypeOfWorkSchedules { get; set; } = default!;
        public DbSet<SectionEntity> Sections { get; set; } = default!;
        public DbSet<LegalDeductionEntity> LegalDeductionEntity { get; set; } = default!;
        public DbSet<UserCompanyEntity> UserCompanies { get; set; } = default!;
        public DbSet<TypeOfDayEntity> TypeOfDays { get; set; } = default!;
        public DbSet<PayrollVoucherFormatEntity> PayrollVoucherFormats { get; set; } = default!;
        public DbSet<BankDepositStructureEntity> BankDepositStructures { get; set; } = default!;
        public DbSet<BankDepositFieldEntity> BankDepositFields { get; set; } = default!;
        public DbSet<DocumentTypeEntity> DocumentTypes { get; set; } = default!;
        public DbSet<DocumentTemplateEntity> DocumentTemplates { get; set; } = default!;
        public DbSet<DocumentTypeSignaturesEntity> DocumentTypeSignatures { get; set; } = default!;
        public DbSet<DestajoProductionEntity> DestajoProductions { get; set; } = default!;
        public DbSet<DestajoDocumentEntity> DestajoDocuments { get; set; } = default!;
        public DbSet<PieceworkUnitTypeEntity> PieceworkUnitTypes { get; set; } = default!;
        public DbSet<OtcConfigurationEntity> OtcConfigurations { get; set; } = default!;
        public DbSet<OvertimeCodeEntity> OvertimeCodes { get; set; } = default!;
        public DbSet<PaystubEntity> Paystubs { get; set; } = default!;
        public DbSet<CertificateRequestEntity> CertificateRequests { get; set; } = default!;
        public DbSet<NotificationEntity> Notifications { get; set; } = default!;
        public DbSet<NotificationRuleEntity> NotificationRules { get; set; } = default!;
        public DbSet<UniformDeliveryEntity> UniformDeliveries { get; set; } = default!;
    }
}
