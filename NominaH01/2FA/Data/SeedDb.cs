using System.Threading.Tasks;
using _2FA.Data.Entities;
using _2FA.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _2FA.Data
{
    public class SeedDb(
        ApplicationDbContext dbContext,
        IWebHostEnvironment env,
        IUserHelper userHelper)
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IWebHostEnvironment _env = env;
        private readonly IUserHelper _userHelper = userHelper;

        public async Task SeedAsync()
        {
            try
            {
                await _dbContext.Database.MigrateAsync();

                await CheckBusinessGroupAsync();
                await CheckCountriesAsync();
                await CheckCompaniesAsync();
                await CheckRolesAsycn();
                await CheckUsersAsync();
                await CheckPayRollTypeAsync();
                await CheckTransitBanksAsync();
                await CheckBranchesAsync();
                await CheckCostCentersAsync();
                await CheckDepartmentsAsync();
                await CheckDivisionsAsync();
                await CheckEmployeeTypesAsync();
                await CheckIdentityDocumentTypeAsync();
                await CheckObservationTypesAsync();
                await CheckPaymentFrequencyAsync();
                await CheckPhaseAsync();
                await CheckPositionAsync();
                await CheckProjectAsync();
                await CheckSectionAsync();
                await CheckTypeOfWorkerAsync();
                await CheckSchedulesAsync();
                await CheckBanksAsync();
                await CheckPaymentGroupAsync();
                await CheckEmployeesAsync();
                await CheckPieceworkUnitTypesAsync();
                await CheckPaymentConcepts();
                await CheckLegalDeductionAsync();
                await CheckConceptLegalDeductionAsync();
                await CheckCreditorAsync();
                await CheckLiabilitiesAsync();
                await CheckOverTimeFactorsAsync();
                await UpdateHolidaysAsync();
                await CheckWoringDaysAsync();
                await CheckTypeOfDaysAsync();
                await CheckPayrollVoucherFormatsAsync();
                await CheckDocumentTemplatesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task CheckTypeOfDaysAsync()
        {
            if (!_dbContext.TypeOfDays.Any())
            {
                await _dbContext.TypeOfDays.AddRangeAsync(
                    new TypeOfDayEntity { Code = "DIANOR", Description = "Dia Normal", IsActive = true, Created = DateTime.UtcNow, CreatedBy = "SeedDb" },
                    new TypeOfDayEntity { Code = "DIACOMP", Description = "Dia Compensatorio", IsActive = true, Created = DateTime.UtcNow, CreatedBy = "SeedDb" },
                    new TypeOfDayEntity { Code = "DIALIB", Description = "Dia Libre Trabajado", IsActive = true, Created = DateTime.UtcNow, CreatedBy = "SeedDb" },
                    new TypeOfDayEntity { Code = "FIESNAC", Description = "Fiesta Nacional", IsActive = true, Created = DateTime.UtcNow, CreatedBy = "SeedDb" }
                );

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckWoringDaysAsync()
        {
            if (!_dbContext.TypeOfWorkSchedules.Any())
            {
                await _dbContext.TypeOfWorkSchedules.AddRangeAsync(
                    new TypeOfWorkScheduleEntity { Code = "DIURNO", Description = "Jornada Diurna", IsActive = true, Created = DateTime.UtcNow, CreatedBy = "SeedDb" },
                    new TypeOfWorkScheduleEntity { Code = "NOCTURNO", Description = "Jornada Nocturna", IsActive = true, Created = DateTime.UtcNow, CreatedBy = "SeedDb" },
                    new TypeOfWorkScheduleEntity { Code = "MIXTO", Description = "Jornada Mixta", IsActive = true, Created = DateTime.UtcNow, CreatedBy = "SeedDb" }
                );

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckOverTimeFactorsAsync()
        {
            if (!_dbContext.OverTimeFactors.Any())
            {
                await _dbContext.OverTimeFactors.AddRangeAsync(
                    new OverTimeFactorEntity { Code="JODIUR", Description="JorOrd Diurna 1.0", Factor=1.0m, IsActive=true, Formula="1.0", Identify="Horario de entrada a partir de las 06:00 hasta las 15:00", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JORNOCT", Description="JorOrd Nocturna 1.0", Factor=1.0m, IsActive=true, Formula="1.0", Identify="Horario de entrada a partir de las 18:00 hasta las 06:00 se trabajan 7.0 horas y se cobran 8.0", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JORMIXTA", Description="JorOrd Mixta 1.0", Factor=1.0m, IsActive=true, Formula="1.0", Identify="Horario de entrada a partir de las 15:00 hasta las 18:00 se trabajan 7.5 horas y se cobran 8.0", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="DIURDES", Description="JorOrd Diu DiaDes 1.5", Factor=1.5m, IsActive=true, Formula="1.50", Identify="Trabajo dias domingo o día de descanso según horario del empleado", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="DIURFER", Description="JorOrd Diu DiaFer 2.5", Factor=2.5m, IsActive=true, Formula="2.50", Identify="Trabajo en día feriado según la tabla Holidays", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="DIURCOMP", Description="JorOrd Diu DiaComp Trab 1.5", Factor=1.5m, IsActive=true, Formula="1.50", Identify="Tipo de Día asignado por el usuario", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STDIU", Description="ST Diurno DiaNor 1.25", Factor=1.25m, IsActive=true, Formula="1.25", Identify="Sobretiempo de Jornada Diurna hasta las 18:00", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STNOC", Description="ST Noct. DiaNor 1.75", Factor=1.75m, IsActive=true, Formula="1.75", Identify="Sobretiempo de Jornada Nocturna hasta las 06:00", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STDIUREC", Description="ST Diurno DiaNor Rec 2.1875", Factor=2.1875m, IsActive=true, Formula="1.25 * 1.75", Identify="Sobretiempo Diurno con exceso de 3 horas diarias o 9 horas semanales", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STDIUDES", Description="ST Diurno DiaDes 1.8750", Factor=1.8750m, IsActive=true, Formula="1.50 * 1.25", Identify="Sobretiempo Domingo Jornada Diurna", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STDIUDESREC", Description="ST Diurno DiaDes Rec 3.28125", Factor=3.28125m, IsActive=true, Formula="1.50 * 1.25 * 1.75", Identify="Sobretiempo Domingo Diurno con exceso de 3 horas diarias o 9 horas semanales", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STDIUFER", Description="ST Diurno DiaFer 3.125", Factor=3.125m, IsActive=true, Formula="2.50 * 1.25", Identify="Sobretiempo Diurno Holiday", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STDIUFERREC", Description="ST Diurno DiaFer Rec 5.46875", Factor=5.46875m, IsActive=true, Formula="2.50 * 1.25 * 1.75", Identify="Sobretiempo Diurno Holiday con exceso de 3 horas diarias o 9 horas semanales", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STMIXDIU", Description="ST Mixto DiaNor 1.5", Factor=1.5m, IsActive=true, Formula="1.50", Identify="Sobretiempo de Jornada Diurna desde las 18:00 hasta las 20:00", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STMIXDES", Description="ST Mixto DiaDes 2.25", Factor=2.25m, IsActive=true, Formula="1.50 * 1.50", Identify="Sobretiempo Domingo Jornada Mixta", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STMIXDIUREC", Description="ST Mixto DiaNor Rec 2.625", Factor=2.625m, IsActive=true, Formula="1.50 * 1.75", Identify="Sobretiempo Diurno Mixto con exceso de 3 horas diarias o 9 horas semanales", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STMIXFER", Description="ST Mixto DiaFer 3.75", Factor=3.75m, IsActive=true, Formula="2.50 * 1.50", Identify="Sobretiempo Diurno Mixto Holiday", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STMIXDESREC", Description="ST Mixto DiaDes Rec 3.9375", Factor=3.9375m, IsActive=true, Formula="1.50 * 1.50 * 1.75", Identify="Sobretiempo Nocturno Domingo con exceso de 3 horas diarias o 9 horas semanales", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STMIXFERREC", Description="ST Mixto DiaFer Rec 6.5625", Factor=6.5625m, IsActive=true, Formula="2.50 * 1.50 * 1.75", Identify="Sobretiempo Mixto Holiday con exceso de 3 horas semanales", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STNOCDESREC", Description="ST Noct. DiaDes Rec 4.59375", Factor=4.59375m, IsActive=true, Formula="1.50 * 1.75 * 1.75", Identify="Sobretiempo Domingo Nocturno con exceso de 3 horas diarias o 9 horas semanales", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STNOCREC", Description="ST Noct. DiaNor Rec 3.0625", Factor=3.0625m, IsActive=true, Formula="1.75 * 1.75", Identify="Sobretiempo Nocturno con exceso de 3 horas diarias o 9 horas semanales", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STNOCFER", Description="ST Noct. DiaFer 4.375", Factor=4.375m, IsActive=true, Formula="2.50 * 1.75", Identify="Sobretiempo Nocturno Holiday", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="STNOCFERREC", Description="ST Noct. DiaFer Rec 7.65625", Factor=7.65625m, IsActive=true, Formula="2.50 * 1.75 * 1.75", Identify="Sobretiempo Nocturno Holiday con exceso de 3 horas diarias o 9 horas semanales", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JOMIXFER", Description="JorOrd Mix DiaFer 2.666667", Factor=2.666667m, IsActive=true, Formula="1.066667 * 2.50", Identify="Jornada Mixta Holiday", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JOMIXDOM", Description="JorOrd Mix Domingo 1.6", Factor=1.6m, IsActive=true, Formula="(1.50 * 8) / 7.50", Identify="Jornada Mixta Domingo", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JONOCDOM", Description="JorOrd Noc Domingo 1.714286", Factor=1.714286m, IsActive=true, Formula="1.5 * (8 / 7)", Identify="Jornada Nocturna Domingo", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JONOCFER", Description="JorOrd Noc DiaFer 2.857143", Factor=2.857143m, IsActive=true, Formula="1.142857 * 2.50", Identify="Jornada Nocturna Holiday", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="TARDANZA", Description="Tardanzas", Factor=0m, IsActive=true, Formula="0", Identify="Se ausenta una parte del turno asignado por un tiempo no mayor de 1 hora", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="AUSENCIA", Description="Ausencia", Factor=0m, IsActive=true, Formula="0", Identify="Se ausenta una parte del turno asignado por un tiempo mayor de 1 hora", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JDIUCOMP", Description="Jornada Diurna Solo Comp", Factor=0.5m, IsActive=true, Formula="0.5", Identify="Jornada Diurna Solo el recargo compensatorio", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JORDIUMIX", Description="Jornada Mixta", Factor=1.066667m, IsActive=true, Formula="1.066667", Identify="Jornada Mixta Normal", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JORNOCFER", Description="Jornada Nocturna Dia Feriado", Factor=2.857143m, IsActive=true, Formula="2.857143", Identify="Jornada Nocturna en día feriado", Created=DateTime.UtcNow, CreatedBy="SeedDb" },
                    new OverTimeFactorEntity { Code="JORNOC", Description="Jornada Nocturna", Factor=0m, IsActive=true, Formula="1.142857", Identify="Jornada Nocturna", Created=DateTime.UtcNow, CreatedBy="SeedDb" }
                );

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckLiabilitiesAsync()
        {
            if (!_dbContext.Liabilities.Any())
            {
                await _dbContext.Liabilities.AddAsync(
                    new LiabilityEntity
                    {
                        Code = "DEU01",
                        Created = DateTime.UtcNow,
                        CreditorId = 2,
                        CreatedBy = "SeedDb",
                        Dicsount = 150.00m,
                        EmployeeId = 1,
                        InitialAmount = 1500.00m,
                        Status = "Activo",
                        InitDate = DateTime.Now.AddDays(-5)

                    }
                );
                await _dbContext.Liabilities.AddAsync(
                    new LiabilityEntity
                    {
                        Code = "DEU02",
                        Created = DateTime.UtcNow,
                        CreditorId = 1,
                        CreatedBy = "SeedDb",
                        Dicsount = 125.00m,
                        EmployeeId = 2,
                        InitialAmount = 2000.00m,
                        Status = "Activo",
                        InitDate = DateTime.Now.AddDays(-5)

                    }
                );
                await _dbContext.Liabilities.AddAsync(
                    new LiabilityEntity
                    {
                        Code = "DEU03",
                        Created = DateTime.UtcNow,
                        CreditorId = 2,
                        CreatedBy = "SeedDb",
                        Dicsount = 100.00m,
                        EmployeeId = 2,
                        InitialAmount = 1000.00m,
                        Status = "Activo",
                        InitDate = DateTime.Now.AddDays(-5)

                    }
                );

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckCreditorAsync()
        {
            if (!_dbContext.Creditors.Any())
            {
                await _dbContext.Creditors.AddAsync(
                    new CreditorEntity
                    {
                        CellPhone = "6285-4628",
                        Code = "FINANSOL",
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        FixedPhone = "223-7485",
                        Dv = "01",
                        IsActive = true,
                        Name = "Financiera El Sol, S.A.",
                        RUC = "1245-784-89652"
                    }
                );
                await _dbContext.Creditors.AddAsync(
                    new CreditorEntity
                    {
                        CellPhone = "64319764",
                        Code = "CREDMUNDI",
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        FixedPhone = "231-6341",
                        Dv = "05",
                        IsActive = true,
                        Name = "Créditos Mundiales, S.A.",
                        RUC = "126-3624-84723"
                    }
                );

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckLegalDeductionAsync()
        {
            if (!_dbContext.LegalDeductions.Any())
            {
                var panamaCountryId = await GetCountryByNameAsync("Panamá");

                await _dbContext.LegalDeductions.AddAsync(
                        new LegalDeductionEntity
                        {
                            Code = "SALSOC",
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            DeducFromPayroll = true,
                            Name = "Seguro Social",
                            EmployeeDiscount = 9.75M,
                            EmployerDiscount = 12.25M,
                            PayrollTypeId = await GetPayrollTypeByNameAsync("Regular"),
                            CountryId = panamaCountryId
                        }
                    );

                await _dbContext.LegalDeductions.AddAsync(
                        new LegalDeductionEntity
                        {
                            Code = "SALSOCX",
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            DeducFromPayroll = true,
                            Name = "Seguro Social XIII Mes",
                            EmployeeDiscount = 7.25M,
                            EmployerDiscount = 10.75M,
                            PayrollTypeId = await GetPayrollTypeByNameAsync("XIII Mes"),
                            CountryId = panamaCountryId
                        }
                    );

                await _dbContext.LegalDeductions.AddAsync(
                    new LegalDeductionEntity
                    {
                        Code = "SEGEDUC",
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        DeducFromPayroll = true,
                        Name = "Seguro Educativo",
                        EmployeeDiscount = 1.25M,
                        EmployerDiscount = 1.50M,
                        PayrollTypeId = await GetPayrollTypeByNameAsync("Regular"),
                        CountryId = panamaCountryId
                    }
                );

                await _dbContext.LegalDeductions.AddAsync(
                    new LegalDeductionEntity
                    {
                        Code = "RIEGOP",
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        DeducFromPayroll = false,
                        Name = "Riesgo Profesional",
                        EmployeeDiscount = 0,
                        EmployerDiscount = 0,
                        PayrollTypeId = await GetPayrollTypeByNameAsync("N/A"),
                        CountryId = panamaCountryId
                    }
                );

                await _dbContext.LegalDeductions.AddAsync(
                    new LegalDeductionEntity
                    {
                        Code = "IMPSRTA",
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        DeducFromPayroll = true,
                        Name = "Impuesto Sobre La Renta",
                        EmployeeDiscount = 0,
                        EmployerDiscount = 0,
                        PayrollTypeId = await GetPayrollTypeByNameAsync("Regular"),
                        CountryId = panamaCountryId
                    }
                );

                await _dbContext.SaveChangesAsync();
            }
        }private async Task<int> GetPayrollTypeByNameAsync(string payrollTypeName)
        {
            if (string.IsNullOrWhiteSpace(payrollTypeName))
            {
                return 0;
            }

            var payrollType = await _dbContext.PayrollTypes
                .FirstOrDefaultAsync(pt => pt.Name.Trim() == payrollTypeName.Trim());

            if (payrollType == null)
            {
                return 0; // No encontrado
            }

            return payrollType.Id;
        }


        private async Task CheckPaymentConcepts()
        {
            if (!_dbContext.PaymentConcepts.Any())
            {
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name = "Salario Regular",
                        Code = "SALR0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = true,
                        PayFactor = 1,
                        RecurrentPayment = true,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsPredetermined = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="SobreTiempo",
                        Code = "SBTM0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Ajuste",
                        Code = "AJUS0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Comisión",
                        Code = "COMI0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Prima de Producción",
                        Code = "PRPD0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Bonificación",
                        Code = "BONO0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Gratificación",
                        Code = "GRAT0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Vacación",
                        Code = "VACA0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });

                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="XIII Mes",
                        Code = "13ME0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Mejora al XIII Mes",
                        Code = "13MJ0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Participación de Utilidades",
                        Code = "PART0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Viático",
                        Code = "VIAT0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Transporte",
                        Code = "TRNS0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Gasto de Representación",
                        Code = "GREP001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="GR de Vacaciones",
                        Code = "GRVA0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="GR de XIII Mes",
                        Code = "GR130001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Preaviso",
                        Code = "PRAV0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Prima de Antiguedad",
                        Code = "PRAN0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Indemnización",
                        Code = "INDE0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Ingreso del 6%",
                        Code = "ING60001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsConstruction = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Bono de Asistencia",
                        Code = "BOAS0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsConstruction = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Lluvia",
                        Code = "LLUV0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsConstruction = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Altura hasta 125ms",
                        Code = "ALT10001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsConstruction = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Altura Mayor a 125ms",
                        Code = "ALT20001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsConstruction = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Profundidad",
                        Code = "PROF0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsConstruction = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Tunel",
                        Code = "TUNE0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsConstruction = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Martillo",
                        Code = "MART0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsConstruction = true
                    });
                await _dbContext.PaymentConcepts.AddAsync(
                    new PaymentConceptEntity
                    {
                        Name ="Rastrilleo",
                        Code = "RAST0001",
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        RegularHours = true,
                        ExtraHours = false,
                        PayFactor = 1,
                        RecurrentPayment = false,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        IsConstruction = true
                    });

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckConceptLegalDeductionAsync()
        {
            if (!_dbContext.ConceptLegalDeductions.Any())
            {
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Salario Regular")
                    });

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Salario Regular")
                    });

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Salario Regular")
                    });

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Salario Regular")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto                    
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Sobretiempo")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Sobretiempo")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Sobretiempo")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto                    
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Ajuste")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Ajuste")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Ajuste")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Ajuste")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto                    
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Comisión")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Comisión")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Comisión")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Comisión")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto                    
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Prima de Producción")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Prima de Producción")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Prima de Producción")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Prima de Producción")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Bonificación")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Bonificación")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Bonificación")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Bonificación")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Gratificación")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Gratificación")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Gratificación")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Gratificación")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Vacación")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Vacación")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Vacación")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Vacación")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("XIII Mes")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("XIII Mes")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("XIII Mes")
                    });
                await _dbContext.SaveChangesAsync();

                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("XIII Mes")
                    });

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Mejora al XIIi Mes")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Mejora al XIIi Mes")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Mejora al XIIi Mes")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Mejora al XIIi Mes")
                    });

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Participación de Utilidades")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Participación de Utilidades")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Participación de Utilidades")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Participación de Utilidades")
                    });

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Gasto de Representación")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Gasto de Representación")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Gasto de Representación")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Participación de Utilidades")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("GR de XIII Mes")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("GR de XIII Mes")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("GR de XIII Mes")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("GR de Vacaciones")
                    });

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("GR de Vacaciones")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("GR de Vacaciones")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("GR de Vacaciones")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("GR de Vacaciones")
                    });

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Preaviso")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Preaviso")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Preaviso")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Preaviso")
                    });

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Prima de Antiguedad")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Prima de Antiguedad")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Prima de Antiguedad")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Prima de Antiguedad")
                    });

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Indemnización")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Indemnización")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Indemnización")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Indemnización")
                    });

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Ingreso del 6%")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Ingreso del 6%")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Ingreso del 6%")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Ingreso del 6%")
                    });

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Bono de Asistencia")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Bono de Asistencia")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Bono de Asistencia")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Bono de Asistencia")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Lluvia")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Lluvia")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Lluvia")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Lluvia")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Altura hasta 125ms")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Altura hasta 125ms")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Altura hasta 125ms")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Altura hasta 125ms")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Altura Mayor a 125ms")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Altura Mayor a 125ms")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Altura Mayor a 125ms")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Altura Mayor a 125ms")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Profundidad")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Profundidad")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Profundidad")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Profundidad")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Tunel")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Tunel")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Tunel")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Tunel")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Martillo")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Martillo")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Martillo")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Martillo")
                    });

                await _dbContext.SaveChangesAsync();

                // Concepto 
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Social"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Rastrilleo")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Seguro Educativo"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Rastrilleo")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = false,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Riesgo Profesional"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Rastrilleo")
                    });
                await _dbContext.ConceptLegalDeductions.AddAsync(
                    new ConceptLegalDeductionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        HasSpecialRule = true,
                        LegalDeductionEntityId = await GetLegalDeductionByNameAsync("Impuesto Sobre La Renta"),
                        PaymentConceptId = await GetPaymentConceptByNameAsync("Rastrilleo")
                    });

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<int> GetPaymentConceptByNameAsync(string name)
        {
            if (name == null)
            {
                return 0;
            }

            var paymentConcept = await _dbContext.PaymentConcepts.FirstOrDefaultAsync(x => x.Name == name);

            if (paymentConcept != null)
            {
                return paymentConcept.Id;
            }
            else
            {
                return 0;
            }
        }

        private async Task<int> GetLegalDeductionByNameAsync(string name)
        {
            if (name == null)
            {
                return 0;
            }

            var legalDeduction = await _dbContext.LegalDeductions.FirstOrDefaultAsync(x => x.Name == name);
            if (legalDeduction != null)
            {
                return legalDeduction.Id;
            }
            else
            {
                return 0;
            }

        }

        private async Task CheckPaymentGroupAsync()
        {
            if (!_dbContext.PaymentGroups.Any())
            {
                await _dbContext.PaymentGroups.AddAsync(
                    new PaymentGroupEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "*Sin Asignar*",
                        PaymentFrequency = null,
                        PaymentFrequencyId = 1,
                        LastAbsensestDate = DateTime.MinValue,
                        LastPayDate = DateTime.MinValue,
                        ExtraTimeDate = DateTime.MinValue,
                        BaseHours = 0,
                        QuantityOfDays = 0
                    });
                var paymentFrec = await GetPaymentFrequency("Semanal");
                await _dbContext.PaymentGroups.AddAsync(
                    new PaymentGroupEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Semanal 1",
                        PaymentFrequency = paymentFrec,
                        PaymentFrequencyId = paymentFrec != null ? paymentFrec.Id : 1,
                        LastAbsensestDate = DateTime.Now.AddDays(-7),
                        LastPayDate = DateTime.Now.AddDays(-7),
                        ExtraTimeDate = DateTime.Now.AddDays(-7),
                        BaseHours = 48,
                        QuantityOfDays = paymentFrec?.QuantityOfDays ?? 0
                    });
                paymentFrec = await GetPaymentFrequency("Bisemanal");
                await _dbContext.PaymentGroups.AddAsync(
                    new PaymentGroupEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Bisemanal 1",
                        PaymentFrequency = paymentFrec,
                        PaymentFrequencyId = paymentFrec != null ? paymentFrec.Id : 1,
                        LastAbsensestDate = DateTime.Now.AddDays(-14),
                        LastPayDate = DateTime.Now.AddDays(-14),
                        ExtraTimeDate = DateTime.Now.AddDays(-14),
                        BaseHours = 96,
                        QuantityOfDays = paymentFrec?.QuantityOfDays ?? 0
                    });
                paymentFrec = await GetPaymentFrequency("Quincenal");
                await _dbContext.PaymentGroups.AddAsync(
                    new PaymentGroupEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Quincenal 1",
                        PaymentFrequency = paymentFrec,
                        PaymentFrequencyId = paymentFrec != null ? paymentFrec.Id : 1,
                        LastAbsensestDate = DateTime.Now.AddDays(-15),
                        LastPayDate = DateTime.Now.AddDays(-15),
                        ExtraTimeDate = DateTime.Now.AddDays(-15),
                        BaseHours = 104.28M,
                        QuantityOfDays = paymentFrec?.QuantityOfDays ?? 0
                    });
                await _dbContext.PaymentGroups.AddAsync(
                    new PaymentGroupEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Quincenal Contratistas",
                        PaymentFrequency = paymentFrec,
                        PaymentFrequencyId = paymentFrec != null ? paymentFrec.Id : 1,
                        LastAbsensestDate = DateTime.Now.AddDays(-15),
                        LastPayDate = DateTime.Now.AddDays(-15),
                        ExtraTimeDate = DateTime.Now.AddDays(-15),
                        BaseHours = 104.28M,
                        QuantityOfDays = paymentFrec?.QuantityOfDays ?? 0
                    });
                paymentFrec = await GetPaymentFrequency("Mensual");
                await _dbContext.PaymentGroups.AddAsync(
                    new PaymentGroupEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Mensual 1",
                        PaymentFrequency = paymentFrec,
                        PaymentFrequencyId = paymentFrec != null ? paymentFrec.Id : 1,
                        LastAbsensestDate = DateTime.Now.AddDays(-30),
                        LastPayDate = DateTime.Now.AddDays(-30),
                        ExtraTimeDate = DateTime.Now.AddDays(-30),
                        BaseHours = 208.56M,
                        QuantityOfDays = paymentFrec?.QuantityOfDays ?? 0
                    });

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<PaymentFrequencyEntity?> GetPaymentFrequency(string paymentFrequencieName)
        {
            if (paymentFrequencieName == null)
            {
                return null;
            }

            PaymentFrequencyEntity? paymentFrequency = await _dbContext.PaymentFrequencies
                .FirstOrDefaultAsync(pf => pf.Name == paymentFrequencieName);

            if (paymentFrequency != null)
            {
                return paymentFrequency;
            }
            else
            {
                return null;
            }
        }        

        private async Task CheckPayRollTypeAsync()
        {
            if (!_dbContext.PayrollTypes.Any())
            {
                await _dbContext.PayrollTypes.AddAsync(
                    new PayrollTypeEntity
                    {
                        Code = "N",
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "N/A"
                    });
                await _dbContext.PayrollTypes.AddAsync(
                    new PayrollTypeEntity
                    {
                        Code = "R",
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Regular"
                    });
                await _dbContext.PayrollTypes.AddAsync(
                    new PayrollTypeEntity
                    {
                        Code = "V",
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Vacaciones"
                    });
                await _dbContext.PayrollTypes.AddAsync(
                    new PayrollTypeEntity
                    {
                        Code = "L",
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Liquidación"
                    });
                await _dbContext.PayrollTypes.AddAsync(
                    new PayrollTypeEntity
                    {
                        Code = "X",
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "XIII Mes"
                    });
                await _dbContext.PayrollTypes.AddAsync(
                    new PayrollTypeEntity
                    {
                        Code = "M",
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Miscelánea"
                    });

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckSchedulesAsync()
        {
            if (!_dbContext.Schedules.Any())
            {
                await _dbContext.Schedules.AddAsync(
                    new ScheduleEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "*Sin Asignar*"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckCompaniesAsync()
        {
            if (!_dbContext.Companies.Any())
            {
                await _dbContext.Companies.AddAsync(
                    new CompanyEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Compañía de Prueba S.A.",
                        BusinessGroupId = 1,
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        StateId = await GetStateByNameAsync("Panamá"),
                        CityId = await GetCityByNameAsync("Ciudad de Panamá")
                    });

                await _dbContext.Companies.AddAsync(
                    new CompanyEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Empresa Demo S.A.",
                        BusinessGroupId = 1,
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        StateId = await GetStateByNameAsync("Panamá"),
                        CityId = await GetCityByNameAsync("Ciudad de Panamá")
                    });

                await _dbContext.Companies.AddAsync(
                    new CompanyEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Corporación Test S.A.",
                        BusinessGroupId = 1,
                        CountryId = await GetCountryByNameAsync("Panamá"),
                        StateId = await GetStateByNameAsync("Panamá"),
                        CityId = await GetCityByNameAsync("Ciudad de Panamá")
                    });

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckBusinessGroupAsync()
        {
            if (!_dbContext.BusinessGroups.Any())
            {
                await _dbContext.BusinessGroups.AddAsync(
                    new BusinessGroupEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Grupo Los Caracoles, S.A."
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<int> GetDefaultCompanyIdAsync()
        {
            var defaultCompanyId = await _dbContext.Companies
                .OrderBy(c => c.Id)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (defaultCompanyId == 0)
            {
                throw new InvalidOperationException("No existe una compañía por defecto para asociar catálogos.");
            }

            return defaultCompanyId;
        }

        private async Task CheckBanksAsync()
        {
            if (!_dbContext.Banks.Any())
            {
                await _dbContext.Banks.AddAsync(
                    new BankEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "*Sin Asignar*",
                        TransitBankId = 1,});
                await _dbContext.SaveChangesAsync();
            }
        }   

        public async Task CheckEmployeesAsync()
        {
            if (!_dbContext.Employees.Any())
            {
                var employees = new List<EmployeeEntity>
                {
                    new() { FirstName = "Juan", LastName = "Pérez", CEMAIL = "juan.perez@ejemplo.com",
                                        PositionId = await GetPositionId("Desarrollador"),
                                         ActivityId = 1, DivisionId = 1, SectionId = 1, CostCenterId = 1,
                                         DepartmentId = 1, BranchId = 1, CompanyId = 1, Status = "Activo",
                                         Created = DateTime.UtcNow, CivilStatus = "Soltero",
                                         DateOfBird = new DateTime(1990, 1, 1), Code = "EMP001", ScheduleId = 1,
                                         CodOfClock = "CLOCK001", AgreeSalary = 1000.00M, EmployeeTypeId = 1,
                                         IdDocument = "123456789", IdentityDocumentTypeId = 1, TypeOfWorkerId = 1,
                                         CreatedBy = "SeedDb", ProjectId = 1, PhaseId = 1, Genere = "Masculino",
                                         SalaryType = "Mensual", BankId = GetBankByName("*Sin Asignar*"),
                                         OriginCountryId = await GetCountryByNameAsync("Panamá"),
                                         PaymentGroupId = await GetPaymentGroupIdAsync("Quincenal 1"),
                                         HourSalary = 4.794783m, RegularHours = 104.28m,
                                         SocSecNum = "SS123456789", PayAccount = "BA123456789",
                                         Dv = "00"},
                    new() { FirstName = "Ana", LastName = "Gómez", CEMAIL = "ana.gomez@ejemplo.com",
                                         PositionId = await GetPositionId("Diseñadora"),
                                         ActivityId = 1, DivisionId = 1, SectionId = 1, CostCenterId = 1,
                                         DepartmentId = 1, BranchId = 1, CompanyId = 1, Status = "Activo",
                                         Created = DateTime.UtcNow, CivilStatus = "Soltero",
                                         DateOfBird = new DateTime(1990, 1, 1), Code = "EMP001", ScheduleId = 1,
                                         CodOfClock = "CLOCK001", AgreeSalary = 1500.00M, EmployeeTypeId = 1,
                                         IdDocument = "123456789", IdentityDocumentTypeId = 1, TypeOfWorkerId = 1,
                                         CreatedBy = "SeedDb", ProjectId = 1, PhaseId = 1, Genere = "Femenino",
                                         SalaryType = "Mensual", BankId = GetBankByName("*Sin Asignar*"),
                                         OriginCountryId = await GetCountryByNameAsync("Panamá"),
                                         PaymentGroupId = await GetPaymentGroupIdAsync("Quincenal 1"),
                                         HourSalary = 7.192174m, RegularHours = 104.28m,
                                         SocSecNum = "SS123456789", PayAccount = "BA123456789",
                                         Dv = "00"},
                    new() { FirstName = "Luis", LastName = "Martínez", CEMAIL = "luis.martinez@ejemplo.com",
                                         PositionId = await GetPositionId("Gerente"),
                                         ActivityId = 1, DivisionId = 1, SectionId = 1, CostCenterId = 1,
                                         DepartmentId = 1, BranchId = 1, CompanyId = 1, Status = "Activo",
                                         Created = DateTime.UtcNow, CivilStatus = "Soltero",
                                         DateOfBird = new DateTime(1990, 1, 1), Code = "EMP001", ScheduleId = 1,
                                         CodOfClock = "CLOCK001", AgreeSalary = 1100.00M, EmployeeTypeId = 1,
                                         IdDocument = "123456789", IdentityDocumentTypeId = 1, TypeOfWorkerId = 1,
                                         CreatedBy = "SeedDb", ProjectId = 1, PhaseId = 1, Genere = "Masculino",
                                         SalaryType = "Mensual", BankId = GetBankByName("*Sin Asignar*"),
                                         OriginCountryId = await GetCountryByNameAsync("Panamá"),
                                         PaymentGroupId = await GetPaymentGroupIdAsync("Quincenal 1"),
                                         HourSalary = 5.74262m, RegularHours = 104.28m,
                                         SocSecNum = "SS123456789", PayAccount = "BA123456789",
                                         Dv = "00"},
                    new() { FirstName = "María", LastName = "López", CEMAIL = "maria.lopez@ejemplo.com",
                                         PositionId = await GetPositionId("Analista") ,
                                         ActivityId = 1, DivisionId = 1, SectionId = 1, CostCenterId = 1,
                                         DepartmentId = 1, BranchId = 1, CompanyId = 1, Status = "Activo",
                                         Created = DateTime.UtcNow, CivilStatus = "Soltero",
                                         DateOfBird = new DateTime(1990, 1, 1), Code = "EMP001", ScheduleId = 1,
                                         CodOfClock = "CLOCK001", AgreeSalary = 800.00M, EmployeeTypeId = 1,
                                         IdDocument = "123456789", IdentityDocumentTypeId = 1, TypeOfWorkerId = 1,
                                         CreatedBy = "SeedDb", ProjectId = 1, PhaseId = 1, Genere = "Femenino",
                                         SalaryType = "Mensual", BankId = GetBankByName("*Sin Asignar*"),
                                         OriginCountryId = await GetCountryByNameAsync("Panamá"),
                                         PaymentGroupId = await GetPaymentGroupIdAsync("Quincenal 1"),
                                         HourSalary = 3.835826m, RegularHours = 104.28m,
                                         SocSecNum = "SS123456789", PayAccount = "BA123456789",
                                         Dv = "00"},
                    new() { FirstName = "Carlos", LastName = "Hernández", CEMAIL = "carlos.hernandez@ejemplo.com",
                                         PositionId = await GetPositionId("Tester"),
                                         ActivityId = 1, DivisionId = 1, SectionId = 1, CostCenterId = 1,
                                         DepartmentId = 1, BranchId = 1, CompanyId = 1, Status = "Activo",
                                         Created = DateTime.UtcNow, CivilStatus = "Soltero",
                                         DateOfBird = new DateTime(1990, 1, 1), Code = "EMP001", ScheduleId = 1,
                                         CodOfClock = "CLOCK001", AgreeSalary = 2000.00M, EmployeeTypeId = 1,
                                         IdDocument = "123456789", IdentityDocumentTypeId = 1, TypeOfWorkerId = 1,
                                         CreatedBy = "SeedDb", ProjectId = 1, PhaseId = 1, Genere = "Masculino",
                                         SalaryType = "Mensual", BankId = GetBankByName("*Sin Asignar*"),
                                         OriginCountryId = await GetCountryByNameAsync("Panamá"),
                                         PaymentGroupId = await GetPaymentGroupIdAsync("Quincenal 1"),
                                         HourSalary = 9.589566m, RegularHours = 104.28m,
                                         SocSecNum = "SS123456789", PayAccount = "BA123456789",
                                         Dv = "00"},
                    // Contrato 1 - Servicios de Consultoría TI
                    new() { FirstName = "Roberto", LastName = "Silva", CEMAIL = "roberto.silva.contractor@ejemplo.com",
                                         PositionId = await GetPositionId("Consultor Senior"),
                                         ActivityId = 1, DivisionId = 1, SectionId = 1, CostCenterId = 1,
                                         DepartmentId = 1, BranchId = 1, CompanyId = 1, Status = "Activo",
                                         Created = DateTime.UtcNow, CivilStatus = "Soltero",
                                         DateOfBird = new DateTime(1985, 3, 15), Code = "CTR001", ScheduleId = 1,
                                         CodOfClock = "CLOCKCTR1", AgreeSalary = 3000.00M, EmployeeTypeId = 1,
                                         IdDocument = "987654321", IdentityDocumentTypeId = 1, TypeOfWorkerId = 1,
                                         CreatedBy = "SeedDb", ProjectId = 1, PhaseId = 1, Genere = "Masculino",
                                         SalaryType = "Mensual", BankId = GetBankByName("*Sin Asignar*"),
                                         OriginCountryId = await GetCountryByNameAsync("Panamá"),
                                         PaymentGroupId = await GetPaymentGroupIdAsync("Quincenal Contratistas"),
                                         HourSalary = 14.384531m, RegularHours = 104.28m, IsContractor = true,
                                         SocSecNum = "SS123456789", PayAccount = "BA123456789",
                                         Dv = "00"},
                    // Contrato 2 - Servicios de Diseño Web
                    new() { FirstName = "Elena", LastName = "Vargas", CEMAIL = "elena.vargas.contractor@ejemplo.com",
                                         PositionId = await GetPositionId("Diseñadora Web"),
                                         ActivityId = 1, DivisionId = 1, SectionId = 1, CostCenterId = 1,
                                         DepartmentId = 1, BranchId = 1, CompanyId = 1, Status = "Activo",
                                         Created = DateTime.UtcNow, CivilStatus = "Casada",
                                         DateOfBird = new DateTime(1988, 7, 22), Code = "CTR002", ScheduleId = 1,
                                         CodOfClock = "CLOCKCTR2", AgreeSalary = 2500.00M, EmployeeTypeId = 1,
                                         IdDocument = "456789123", IdentityDocumentTypeId = 1, TypeOfWorkerId = 1,
                                         CreatedBy = "SeedDb", ProjectId = 1, PhaseId = 1, Genere = "Femenino",
                                         SalaryType = "Mensual", BankId = GetBankByName("*Sin Asignar*"),
                                         OriginCountryId = await GetCountryByNameAsync("Panamá"),
                                         PaymentGroupId = await GetPaymentGroupIdAsync("Quincenal Contratistas"),
                                         HourSalary = 11.986959m, RegularHours = 104.28m, IsContractor = true,
                                         SocSecNum = "SS123456789", PayAccount = "BA123456789",
                                         Dv = "00"},
                    // Contrato 3 - Servicios de Análisis de Datos
                    new() { FirstName = "Diego", LastName = "Morales", CEMAIL = "diego.morales.contractor@ejemplo.com",
                                         PositionId = await GetPositionId("Analista de Datos"),
                                         ActivityId = 1, DivisionId = 1, SectionId = 1, CostCenterId = 1,
                                         DepartmentId = 1, BranchId = 1, CompanyId = 1, Status = "Activo",
                                         Created = DateTime.UtcNow, CivilStatus = "Divorciado",
                                         DateOfBird = new DateTime(1982, 11, 8), Code = "CTR003", ScheduleId = 1,
                                         CodOfClock = "CLOCKCTR3", AgreeSalary = 2800.00M, EmployeeTypeId = 1,
                                         IdDocument = "321654987", IdentityDocumentTypeId = 1, TypeOfWorkerId = 1,
                                         CreatedBy = "SeedDb", ProjectId = 1, PhaseId = 1, Genere = "Masculino",
                                         SalaryType = "Mensual", BankId = GetBankByName("*Sin Asignar*"),
                                         OriginCountryId = await GetCountryByNameAsync("Panamá"),
                                         PaymentGroupId = await GetPaymentGroupIdAsync("Quincenal Contratistas"),
                                         HourSalary = 13.441547m, RegularHours = 104.28m, IsContractor = true,
                                         SocSecNum = "SS123456789", PayAccount = "BA123456789",
                                         Dv = "00"}

                };
                _dbContext.Employees.AddRange(employees);
                _dbContext.SaveChanges();
            }
        }

        private async Task<int> GetPaymentGroupIdAsync(string paymentGroupName)
        {
            if (paymentGroupName == null)
            {
                return 1; // Default value for null payment group
            }

            var paymentGroup = await _dbContext.PaymentGroups.FirstOrDefaultAsync(pg => pg.Name == paymentGroupName);

            if (paymentGroup == null)
            {
                return 1;
            }
            else
            {
                return paymentGroup.Id;
            }
        }

        private int GetBankByName(string bankName)
        {
            var bank = _dbContext.Banks.FirstOrDefault(b => b.Name == bankName);
            if (bank != null)
            {
                return bank.Id;
            }
            else
            {
                // Handle the case where the bank is not found, e.g., throw an exception or return a default value.
                throw new Exception($"Bank '{bankName}' not found.");
            }
        }

        private async Task<int> GetCountryByNameAsync(string countryName)
        {
            var country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Name_es == countryName);
            if (country != null)
            {
                return country.Id;
            }
            else
            {
                // Handle the case where the country is not found, e.g., throw an exception or return a default value.
                throw new Exception($"Country '{countryName}' not found.");
            }
        }

        private async Task<int> GetStateByNameAsync(string stateName)
        {
            var state = await _dbContext.States.FirstOrDefaultAsync(s => s.Name == stateName);
            if (state != null)
            {
                return state.Id;
            }
            else
            {
                // Handle the case where the state is not found, e.g., throw an exception or return a default value.
                throw new Exception($"State '{stateName}' not found.");
            }
        }

        private async Task<int> GetCityByNameAsync(string cityName)
        {
            var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Name == cityName);
            if (city != null)
            {
                return city.Id;
            }
            else
            {
                // Handle the case where the city is not found, e.g., throw an exception or return a default value.
                throw new Exception($"City '{cityName}' not found.");
            }
        }


        private async Task<int> GetPositionId(string positionName)
        {
            var position = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name == positionName);

            if (position == null)
            {
                if (position == null)
                {
                    position = new PositionEntity
                    {
                        Name = positionName,
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true
                    };
                    _dbContext.Positions.Add(position);
                    await _dbContext.SaveChangesAsync();
                }
                return position.Id;
            }
            return position.Id;
        }   

        private async Task CheckTransitBanksAsync()
        {
            if (!_dbContext.TransitBanks.Any())
            {
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity {Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, Name = "*Sin Asignar*"}
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 13, Name = "Banco Nacional de Panamá" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 770, Name = "CAJA DE AHORROS" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 2529, Name = "CACECHI" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 518, Name = "BICSA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1397, Name = "BCT BANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1698, Name = "BANCO LA HIPOTECARIA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1164, Name = "BANK OF CHINA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1614, Name = "BANISI, S.A." }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1588, Name = "BANESCO" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1258, Name = "CANAL BANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1517, Name = "BANCO PICHINCHA PANAMA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 71, Name = "BANCO GENERAL" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1724, Name = "BANCO FICOHSA PANAMA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1384, Name = "BAC INTERNATIONAL BANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1562, Name = "BANCO DELTA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1504, Name = "BANCO AZTECA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1083, Name = "BANCO ALIADO" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1575, Name = "Banco LAFISE PANAMA, S.A." }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1782, Name = "BIBANK PANAMÁ" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 181, Name = "DAVIVIENDA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 5034, Name = "COOPRAC, R.L." }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 5021, Name = "ECASESO" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1805, Name = "ATLAS BANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 916, Name = "BANCO DEL PACÍFICO (PANAMÁ), S.A." }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 5018, Name = "EDIOACC, R.L" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1656, Name = "BBP BANK, S.A." }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1753, Name = "BANCOLOMBIA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1708, Name = "UNIBANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 408, Name = "TOWERBANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1494, Name = "ST. GEORGES BANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 424, Name = "THE BANK OF NOVA SCOTIA" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 39, Name = "CITIBANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 2532, Name = "COEDUCO" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 2516, Name = "COOESAN" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 2503, Name = "COOPEDUC" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 5005, Name = "COOPERATIVA CRISTOBAL" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 712, Name = "COOPERATIVA DE PROFESIONALES" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 2545, Name = "COOPEVE0" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1106, Name = "CREDICORP BANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1151, Name = "GLOBAL BANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 26, Name = "BANISTMO S.A." }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1630, Name = "MERCANTIL BANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1067, Name = "METROBANK S.A." }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1478, Name = "MMG BANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 372, Name = "MULTIBANK" }
                    );
                await _dbContext.TransitBanks.AddAsync(
                    new TransitBankEntity { Created = DateTime.UtcNow, CreatedBy = "SeedDb", IsActive = true, TransitId = 1672, Name = "PRIVAL BANK" }
                    );

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckBranchesAsync()
        {
            if(!_dbContext.Branches.Any())
            {
                var defaultCompanyId = await GetDefaultCompanyIdAsync();
                await _dbContext.Branches.AddAsync(
                    new BranchEntity 
                    { 
                        Created = DateTime.UtcNow, 
                        CreatedBy = "SeedDb", 
                        IsActive = true,
                        CompanyId = defaultCompanyId,
                        Name= "*Sin Asignar*" 
                    });
                await _dbContext.SaveChangesAsync();
            }
            if(!_dbContext.Activities.Any())
            {
                await _dbContext.Activities.AddAsync(
                    new ActivityEntity 
                    { 
                        Created = DateTime.UtcNow, 
                        CreatedBy = "SeedDb", 
                        IsActive = true, 
                        Name= "*Sin Asignar*" 
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckCostCentersAsync()
        {
            if (!_dbContext.CostCenters.Any())
            {
                var defaultCompanyId = await GetDefaultCompanyIdAsync();
                await _dbContext.CostCenters.AddAsync(
                    new CostCenterEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        CompanyId = defaultCompanyId,
                        Name = "*Sin Asignar*"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckDepartmentsAsync()
        {
            if (!_dbContext.Departments.Any())
            {
                var defaultCompanyId = await GetDefaultCompanyIdAsync();
                await _dbContext.Departments.AddAsync(
                    new DepartmentEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        CompanyId = defaultCompanyId,
                        Name = "*Sin Asignar*"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckDivisionsAsync()
        {
            if (!_dbContext.Divisions.Any())
            {
                var defaultCompanyId = await GetDefaultCompanyIdAsync();
                await _dbContext.Divisions.AddAsync(
                    new DivisionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        CompanyId = defaultCompanyId,
                        Name = "*Sin Asignar*"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckEmployeeTypesAsync()
        {
            if (!_dbContext.EmployeeTypes.Any())
            {
                await _dbContext.EmployeeTypes.AddAsync(
                    new EmployeeTypeEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        Name = "*Sin Asignar*"
                    });
                await _dbContext.EmployeeTypes.AddAsync(
                    new EmployeeTypeEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        Name = "Permanente"
                    });
                await _dbContext.EmployeeTypes.AddAsync(
                    new EmployeeTypeEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        Name = "Eventual"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckIdentityDocumentTypeAsync()
        {
            if (!_dbContext.IdentityDocumentTypes.Any())
            {
                await _dbContext.IdentityDocumentTypes.AddAsync(
                    new IdentityDocumentTypeEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Cédula"
                    });
                await _dbContext.IdentityDocumentTypes.AddAsync(
                    new IdentityDocumentTypeEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Pasaporte"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckObservationTypesAsync()
        {
            if (!_dbContext.ObservationTypes.Any())
            {
                await _dbContext.ObservationTypes.AddAsync(
                    new ObservationTypeEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Nota"
                    });
                await _dbContext.ObservationTypes.AddAsync(
                    new ObservationTypeEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Llamado de Atención"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckPaymentFrequencyAsync()
        {
            if (!_dbContext.PaymentFrequencies.Any())
            {
                await _dbContext.PaymentFrequencies.AddAsync(
                    new PaymentFrequencyEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Semanal",
                        QuantityOfDays = 7
                    });
                await _dbContext.PaymentFrequencies.AddAsync(
                    new PaymentFrequencyEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Bisemanal",
                        QuantityOfDays = 14
                    });
                await _dbContext.PaymentFrequencies.AddAsync(
                    new PaymentFrequencyEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Quincenal",
                        QuantityOfDays = 15
                    });
                await _dbContext.PaymentFrequencies.AddAsync(
                    new PaymentFrequencyEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Semanal",
                        QuantityOfDays = 30
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckPhaseAsync()
        {
            if (!_dbContext.Phases.Any())
            {
                var defaultCompanyId = await GetDefaultCompanyIdAsync();
                await _dbContext.Phases.AddAsync(
                    new PhaseEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        CompanyId = defaultCompanyId,
                        Name = "*Sin Asignar*"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckPositionAsync()
        {
            if (!_dbContext.Positions.Any())
            {
                await _dbContext.Positions.AddAsync(
                    new PositionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "*Sin Asignar*"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckProjectAsync()
        {
            if (!_dbContext.Projects.Any())
            {
                var defaultCompanyId = await GetDefaultCompanyIdAsync();
                await _dbContext.Projects.AddAsync(
                    new ProjectEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        CompanyId = defaultCompanyId,
                        Name = "*Sin Asignar*"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckSectionAsync()
        {
            if (!_dbContext.Sections.Any())
            {
                var defaultCompanyId = await GetDefaultCompanyIdAsync();
                await _dbContext.Sections.AddAsync(
                    new SectionEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        CompanyId = defaultCompanyId,
                        Name = "*Sin Asignar*"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckTypeOfWorkerAsync()
        {
            if (!_dbContext.TypeOfWorkers.Any())
            {
                await _dbContext.TypeOfWorkers.AddAsync(
                    new TypeOfWorkerEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Normal"
                    });
                await _dbContext.TypeOfWorkers.AddAsync(
                    new TypeOfWorkerEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Construcción"
                    });
                await _dbContext.TypeOfWorkers.AddAsync(
                    new TypeOfWorkerEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Porturio"
                    });
                await _dbContext.TypeOfWorkers.AddAsync(
                    new TypeOfWorkerEntity
                    {
                        Created = DateTime.UtcNow,
                        CreatedBy = "SeedDb",
                        IsActive = true,
                        Name = "Destajo"
                    });
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckRolesAsycn()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Usuario");
            await _userHelper.CheckRoleAsync("Administrator");
        }

        private async Task CheckUsersAsync()
        {
            AppUser adminUser = await CheckUserAsync("edwinidel@gmail.com", "edwinidel@gmail.com", "+507 6480 0168", "Administrator");

            // Ensure seeded users have a PasswordLastChanged timestamp and associate admin with all companies
            if (adminUser != null)
            {
                // set PasswordLastChanged for admin if not set
                if (adminUser.PasswordLastChanged == null)
                {
                    adminUser.PasswordLastChanged = DateTime.UtcNow;
                    await _userHelper.UpdateUserAsync(adminUser);
                }

                var companies = await _dbContext.Companies.ToListAsync();
                foreach (var company in companies)
                {
                    var userCompanyExists = await _dbContext.UserCompanies.AnyAsync(uc => uc.UserId == adminUser.Id && uc.CompanyId == company.Id);
                    if (!userCompanyExists)
                    {
                        var userCompany = new UserCompanyEntity
                        {
                            UserId = adminUser.Id,
                            CompanyId = company.Id,
                            Created = DateTime.UtcNow,
                            CreatedBy = "SeedDb"
                        };
                        _dbContext.UserCompanies.Add(userCompany);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }

            // For any existing users without PasswordLastChanged set, set it to now
            var allUsers = await _dbContext.Users.ToListAsync();
            foreach (var u in allUsers)
            {
                if (u.PasswordLastChanged == null)
                {
                    u.PasswordLastChanged = DateTime.UtcNow;
                    _dbContext.Users.Update(u);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        private async Task<AppUser> CheckUserAsync(
            string userName,
            string email,
            string phone,
            string role)
        {
            AppUser user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = userName,
                    Email = email,
                    PhoneNumber = phone,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = userName.ToUpper(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Edwin",
                    LastName = "Delgado",
                    BusinessGroupId = 1 // Default to first business group
                };

                await _userHelper.AddUserAsync(user, "Edwin1624*");
                await _userHelper.AddUserToRoleAsync(user, role);
            }

            return user;
        }

        private async Task CheckCountriesAsync()
        {
            var countriesCount = _dbContext.Countries.Count();

            if (countriesCount > 0)
            {
                return;
            }

            var countriesJsonPath = Path.Combine(_env.WebRootPath, "Files", "countries.json");
            var countriesJson = File.ReadAllText(countriesJsonPath);
            var statesJsonPath = Path.Combine(_env.WebRootPath, "Files", "states.json");
            var statesJson = File.ReadAllText(statesJsonPath);
            var citiesJsonPath = Path.Combine(_env.WebRootPath, "Files", "cities.json");
            var citiesJson = File.ReadAllText(citiesJsonPath);

            var countries = JsonConvert.DeserializeObject<List<CountryEntity>>(countriesJson);
            var states = JsonConvert.DeserializeObject<List<StateEntity>>(statesJson);
            var cities = JsonConvert.DeserializeObject<List<CityEntity>>(citiesJson);

            //Countries
            var countryId = _dbContext.Countries.Count();

            if (countryId == 0)
            {
                foreach (var country in countries)
                {
                    var countryEntity = await GetCountryDataAsync(country);

                    //_dbContext.Entry(countryEntity).State = EntityState.Unchanged;
                    _dbContext.Countries.Add((CountryEntity)countryEntity);
                }
                await _dbContext.SaveChangesAsync();
            }

            //States
            var stateId = _dbContext.States.Count();

            if (stateId == 0)
            {
                foreach (var state in states)
                {
                    var idCountry = countries.Where(c => c.Id == state.CountryId);

                    if (idCountry != null)
                    {
                        state.Created = DateTime.UtcNow;
                        _dbContext.States.Add(state);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }

            //Cities
            var cityId = _dbContext.Cities.Count();

            if (cityId == 0)
            {
                foreach (var city in cities)
                {
                    var idState = states.Where(s => s.Id == city.StateId);

                    if (idState.Any())
                    {
                        city.Created = DateTime.UtcNow;
                        _dbContext.Cities.Add(city);
                        //await _dbContext.SaveChangesAsync();
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<object> GetCountryDataAsync(CountryEntity country)
        {
            string apiUrl = $"https://restcountries.com/v3.1/alpha/{country.Iso2}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var countryData = JArray.Parse(jsonResponse)[0]; // Tomamos el primer elemento del array

                        var timezones = countryData["timezones"];
                        //Console.WriteLine($"\nPaís: {name}");
                        Console.WriteLine("Zonas horarias:");
                        foreach (var timezone in timezones)
                        {
                            var timezoneString = timezone.ToString();
                            if (timezoneString.Contains("/"))
                            {
                                timezoneString = timezoneString.Split('/')[1];
                            }
                            else if (timezoneString.Contains("Etc/"))
                            {
                                timezoneString = timezoneString.Replace("Etc/", "");
                            }
                            else if (timezoneString.Contains("GMT"))
                            {
                                timezoneString = timezoneString.Replace("GMT", "GMT+0");
                            }
                            else if (timezoneString.Contains("UTC"))
                            {
                                timezoneString = timezoneString.Replace("UTC", "UTC+0");
                            }

                            await _dbContext.CountryTimeZones.AddAsync(new CountryTimeZoneEntity
                            {
                                CountryId = country.Id,
                                TimeZone = timezoneString,
                                Created = DateTime.UtcNow
                            });
                            await _dbContext.SaveChangesAsync();
                        }

                        var capital = countryData["capital"]?.ToString() ?? string.Empty;
                        // Encontramos la primera comilla doble de apertura
                        int startIndex = capital.IndexOf('"');
                        // Encontramos la primera comilla doble de cierre después de la de apertura
                        int endIndex = capital.IndexOf('"', startIndex + 1);

                        string firstCapital = "";

                        if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
                        {
                            // Extraer el texto entre las dos primeras comillas dobles
                            firstCapital = capital.Substring(startIndex + 1, endIndex - (startIndex + 1));
                        }
                        if (firstCapital.Length !> 50)
                        {
                            country.Capital = firstCapital;
                        }
                         
                        //country.Currency = countryData["currencies"]?.First?.First?.ToString() ?? string.Empty;
                        //var currency_symbol = countryData["currencies"]?.First?.First?["symbol"]?.ToString() ?? string.Empty;
                        //var bytes = System.Text.Encoding.UTF8.GetBytes(currency_symbol);
                        //country.Currency_symbol = System.Text.Encoding.UTF8.GetString(bytes);
                        //country.Currency_name = countryData["currencies"]?.First?.First?["name"]?.ToString() ?? string.Empty;
                        country.Tld = countryData["tld"]?.First?.ToString() ?? string.Empty;
                        country.Region = countryData["region"]?.ToString() ?? string.Empty;
                        country.Subregion = countryData["subregion"]?.ToString() ?? string.Empty;
                        country.Nationality = countryData["demonym"]?.ToString() ?? string.Empty;
                        country.Latitude = countryData["latlng"]?.FirstOrDefault()?.ToString() ?? "0";
                        country.Longitude = countryData["latlng"]?.Skip(1).FirstOrDefault()?.ToString() ?? "0";
                        country.Flag = countryData["flags"]?["png"]?.ToString() ?? string.Empty;
                        country.CoatOfArms = countryData["coatOfArms"]?["png"]?.ToString() ?? string.Empty;
                        country.Created = DateTime.UtcNow;

                        return country;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - País no encontrado.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Excepción: {ex.Message}");
                }
            }
            return country;
        }

        private async Task UpdateHolidaysAsync()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var holidayService = new _2FA.Services.HolidayService(_dbContext);

                // Update holidays for current year
                var updatedCount = await holidayService.UpdateHolidaysFromApiAsync(currentYear, "PA");

                if (updatedCount > 0)
                {
                    Console.WriteLine($"Se actualizaron {updatedCount} días festivos para el año {currentYear}");
                }
                else
                {
                    Console.WriteLine($"Los días festivos para el año {currentYear} ya están actualizados");
                }

                // Optionally update next year as well
                var nextYear = currentYear + 1;
                var nextYearUpdated = await holidayService.UpdateHolidaysFromApiAsync(nextYear, "PA");

                if (nextYearUpdated > 0)
                {
                    Console.WriteLine($"Se actualizaron {nextYearUpdated} días festivos para el año {nextYear}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar días festivos: {ex.Message}");
            }
        }

        private async Task CheckPayrollVoucherFormatsAsync()
        {
            if (!_dbContext.PayrollVoucherFormats.Any())
            {
                var regularPayrollTypeId = await GetPayrollTypeByNameAsync("Regular");

                if (regularPayrollTypeId > 0)
                {
                    // Template 1: Simple format
                    await _dbContext.PayrollVoucherFormats.AddAsync(
                        new PayrollVoucherFormatEntity
                        {
                            CompanyId = 1, // Default company
                            PayrollTypeId = regularPayrollTypeId,
                            Name = "Voucher Básico",
                            FormatTemplate = @"
COMPROBANTE DE PAGO

Empresa: {CompanyName}
Empleado: {EmployeeName}
Código: {EmployeeCode}
Período: {PayrollPeriod}

INGRESOS:
Salario Base: {BaseSalary}
Horas Extras: {OvertimeAmount}
Otros Ingresos: {OtherEarnings}
Total Ingresos: {TotalEarnings}

DEDUCCIONES LEGALES:
Seguro Social: {SocialSecurity}
Seguro Educativo: {EducationalInsurance}
Impuesto Sobre La Renta: {IncomeTax}
Total Deducciones Legales: {TotalLegalDeductions}

OTRAS DEDUCCIONES:
{OtherDeductions}

NETO A PAGAR: {NetPay}

Fecha de Emisión: {IssueDate}

___________________________
Firma del Empleado
                            ",
                            IsActive = true,
                            Created = DateTime.UtcNow,
                            CreatedBy = "SeedDb"
                        });

                    // Template 2: Detailed format
                    await _dbContext.PayrollVoucherFormats.AddAsync(
                        new PayrollVoucherFormatEntity
                        {
                            CompanyId = 1,
                            PayrollTypeId = regularPayrollTypeId,
                            Name = "Voucher Detallado",
                            FormatTemplate = @"
================================================================================
                          COMPROBANTE DE PAGO
================================================================================

DATOS DE LA EMPRESA:
Nombre: {CompanyName}
RUC: {CompanyRUC}

DATOS DEL EMPLEADO:
Nombre Completo: {EmployeeName}
Código: {EmployeeCode}
Cargo: {EmployeePosition}
Fecha de Ingreso: {HireDate}

PERIODO DE NÓMINA:
Desde: {PeriodStart}
Hasta: {PeriodEnd}
Tipo: {PayrollType}

DETALLE DE INGRESOS:
--------------------------------------------------------------------------------
Concepto                          | Cantidad | Monto Unitario | Total
--------------------------------------------------------------------------------
Salario Base                      | {BaseHours} | {HourlyRate} | {BaseSalary}
Horas Ordinarias                 | {RegularHours} | {HourlyRate} | {RegularAmount}
Horas Extras                     | {OvertimeHours} | {OvertimeRate} | {OvertimeAmount}
Bonificaciones                   | - | - | {Bonuses}
Vacaciones                       | - | - | {VacationPay}
Total Ingresos                   |           |           | {TotalEarnings}

DEDUCCIONES LEGALES:
--------------------------------------------------------------------------------
Seguro Social (Empleado)         | {SocialSecurityRate}% | {SocialSecurityBase} | {SocialSecurity}
Seguro Educativo (Empleado)      | {EducationalInsuranceRate}% | {EducationalInsuranceBase} | {EducationalInsurance}
Impuesto Sobre La Renta          | {IncomeTaxRate}% | {IncomeTaxBase} | {IncomeTax}
Total Deducciones Legales        |           |           | {TotalLegalDeductions}

OTRAS DEDUCCIONES:
--------------------------------------------------------------------------------
Préstamos                        | - | - | {Loans}
Descuentos Varios                | - | - | {OtherDeductions}
Total Otras Deducciones          |           |           | {TotalOtherDeductions}

RESUMEN FINAL:
--------------------------------------------------------------------------------
Total Ingresos: {TotalEarnings}
Total Deducciones: {TotalDeductions}
Neto a Pagar: {NetPay}

Fecha de Emisión: {IssueDate}
Preparado por: {PreparedBy}

___________________________     ___________________________
Firma del Empleado              Firma del Empleador
                            ",
                            IsActive = true,
                            Created = DateTime.UtcNow,
                            CreatedBy = "SeedDb"
                        });

                    // Template 3: Compact format
                    await _dbContext.PayrollVoucherFormats.AddAsync(
                        new PayrollVoucherFormatEntity
                        {
                            CompanyId = 1,
                            PayrollTypeId = regularPayrollTypeId,
                            Name = "Voucher Compacto",
                            FormatTemplate = @"
COMPROBANTE DE PAGO - {CompanyName}

Empleado: {EmployeeName} | Código: {EmployeeCode}
Período: {PayrollPeriod}

INGRESOS           | DEDUCCIONES
-------------------|-------------------
Salario: {BaseSalary} | SS: {SocialSecurity}
Extras: {OvertimeAmount} | SE: {EducationalInsurance}
Otros: {OtherEarnings} | ISR: {IncomeTax}
                   | Otras: {OtherDeductions}
TOTAL: {TotalEarnings} | TOTAL: {TotalDeductions}

NETO A PAGAR: {NetPay}

Emitido: {IssueDate}

___________________________
Recibí conforme: {EmployeeName}
                            ",
                            IsActive = true,
                            Created = DateTime.UtcNow,
                            CreatedBy = "SeedDb"
                        });

                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task CheckDocumentTemplatesAsync()
        {
            if (!_dbContext.DocumentTemplates.Any())
            {
                await _dbContext.DocumentTemplates.AddAsync(new DocumentTemplateEntity
                {
                    Name = "Carta de trabajo",
                    Content = @"<div style='font-family: Arial, Helvetica, sans-serif;'>
<p style='text-align:center;'><img src='{{Company.Logo}}' style='max-height:100px' alt='logo' /></p>
<p>Fecha: {{Today}}</p>
<p>Señor(a): <strong>{{Employee.FullName}}</strong></p>
<p>Por medio de la presente certificamos que el(la) señor(a) <strong>{{Employee.FullName}}</strong>, identificado con <strong>{{Employee.DocumentId}}</strong>, labora en la empresa <strong>{{Company.Name}}</strong> desde la fecha <strong>{{Employee.StartDate}}</strong>, desempeñándose como <strong>{{Employee.Position}}</strong>.</p>
<p>Salario mensual: <strong>{{Employee.BaseSalary}}</strong></p>
<p>Atentamente,</p>
<p><img src='{{Signature}}' style='max-height:80px' alt='signature' /></p>
<p><strong>{{Company.Name}}</strong></p>
</div>",
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    CreatedBy = "SeedDb"
                });

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task CheckPieceworkUnitTypesAsync()
        {
            var companies = await _dbContext.Companies.ToListAsync();
            if (!companies.Any())
            {
                return;
            }

            var defaultNames = new[] { "No Aplica", "Libras", "Kilos", "Toneladas", "Parcelas" };

            foreach (var company in companies)
            {
                foreach (var name in defaultNames)
                {
                    var exists = await _dbContext.PieceworkUnitTypes
                        .AnyAsync(p => p.CompanyId == company.Id && p.Name == name);

                    if (!exists)
                    {
                        _dbContext.PieceworkUnitTypes.Add(new PieceworkUnitTypeEntity
                        {
                            CompanyId = company.Id,
                            Name = name,
                            Description = name,
                            Created = DateTime.UtcNow,
                            CreatedBy = "SeedDb"
                        });
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            foreach (var company in companies)
            {
                var noAplicaId = await _dbContext.PieceworkUnitTypes
                    .Where(p => p.CompanyId == company.Id && p.Name == "No Aplica")
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                if (noAplicaId == 0)
                {
                    continue;
                }

                var employees = await _dbContext.Employees
                    .Where(e => e.CompanyId == company.Id && (e.PieceworkUnitTypeId == null || e.PieceworkUnitTypeId == 0))
                    .ToListAsync();

                foreach (var employee in employees)
                {
                    employee.PieceworkUnitTypeId = noAplicaId;
                    employee.UnitType = "No Aplica";
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}