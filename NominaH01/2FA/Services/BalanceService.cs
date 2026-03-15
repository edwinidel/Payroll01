using System;
using System.Linq;
using System.Threading.Tasks;
using _2FA.Data;
using _2FA.Models;
using Microsoft.EntityFrameworkCore;

namespace _2FA.Services
{
    public class BalanceService : IBalanceService
    {
        private const decimal VacationAccrualPerMonth = 2.5m; // 30 días / 12 meses
        private const decimal VacationMaxDays = 30m;
        private const decimal DefaultMonthlyHours = 173.33m; // 8 horas * 5 días * 4.333 semanas
        private readonly ApplicationDbContext _context;

        public BalanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveBalanceDto> GetBalancesAsync(int employeeId)
        {
            var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == employeeId)
                ?? throw new InvalidOperationException("Empleado no encontrado");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var start = employee.HiringDate == default
                ? today
                : DateOnly.FromDateTime(employee.HiringDate);

            var endContract = employee.EndOfContract == default
                ? today
                : DateOnly.FromDateTime(employee.EndOfContract);

            var accrualEnd = endContract < today ? endContract : today;

            var totalMonths = Math.Max(0, (accrualEnd.Year - start.Year) * 12 + (accrualEnd.Month - start.Month));

            // Prorrateo del mes actual si corresponde
            var partialMonthDays = accrualEnd.Day;
            var daysInMonth = DateTime.DaysInMonth(accrualEnd.Year, accrualEnd.Month);
            var fractionalMonth = Math.Clamp((decimal)partialMonthDays / daysInMonth, 0, 1);

            var accrued = (totalMonths * VacationAccrualPerMonth) + (VacationAccrualPerMonth * fractionalMonth);
            accrued = Math.Min(VacationMaxDays, Math.Round(accrued, 2));

            // Días tomados vía planillas tipo "Vacaciones"
            var vacationTypeId = await _context.PayrollTypes
                .Where(pt => pt.Name == "Vacaciones")
                .Select(pt => pt.Id)
                .SingleOrDefaultAsync();

            decimal takenDays = 0m;
            if (vacationTypeId > 0)
            {
                var vacPayrolls = await _context.PayrollTmpEmployees
                    .AsNoTracking()
                    .Include(e => e.PayrollTmpHeader)
                    .Where(e => e.EmployeeId == employeeId && e.PayrollTmpHeader != null && e.PayrollTmpHeader.PayrollTypeId == vacationTypeId)
                    .Select(e => e.PayrollTmpHeader!)
                    .Distinct()
                    .ToListAsync();

                foreach (var header in vacPayrolls)
                {
                    var startDate = DateOnly.FromDateTime(header.AbsensestDateStart == default ? header.StartDate : header.AbsensestDateStart);
                    var endDate = DateOnly.FromDateTime(header.AbsensestDateEnd == default ? header.EndDate : header.AbsensestDateEnd);
                    var days = Math.Max(0, (endDate.DayNumber - startDate.DayNumber) + 1);
                    takenDays += days;
                }
            }

            // Promedio 11 meses trabajados (excluyendo planillas de vacaciones)
            decimal bestMonthlyBase = employee.AgreeSalary;
            decimal bestDailyRate = 0m;

            var nonVacationTypeIds = await _context.PayrollTypes
                .Where(pt => pt.Name != "Vacaciones")
                .Select(pt => pt.Id)
                .ToListAsync();

            if (nonVacationTypeIds.Count > 0)
            {
                var monthlyGroups = await _context.PayrollTmpEmployees
                    .AsNoTracking()
                    .Include(e => e.PayrollTmpHeader)
                    .Where(e => e.EmployeeId == employeeId && e.PayrollTmpHeader != null && nonVacationTypeIds.Contains(e.PayrollTmpHeader.PayrollTypeId))
                    .Select(e => new
                    {
                        e.TotalEarnings,
                        e.RegularHours,
                        Month = new DateOnly(e.PayrollTmpHeader!.EndDate.Year, e.PayrollTmpHeader.EndDate.Month, 1)
                    })
                    .GroupBy(x => x.Month)
                    .OrderByDescending(g => g.Key)
                    .Take(11)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Earnings = g.Sum(x => x.TotalEarnings),
                        Hours = g.Sum(x => x.RegularHours)
                    })
                    .ToListAsync();

                if (monthlyGroups.Count > 0)
                {
                    var totalEarnings = monthlyGroups.Sum(m => m.Earnings);
                    var monthsCount = monthlyGroups.Count;
                    var avgMonthly = totalEarnings / monthsCount;

                    var monthlyHours = monthlyGroups.Sum(m => m.Hours);
                    if (monthlyHours <= 0)
                    {
                        monthlyHours = employee.RegularHours > 0 ? employee.RegularHours : DefaultMonthlyHours;
                    }

                    var hourlyFromAverage = monthlyHours > 0 ? avgMonthly / monthlyHours : 0m;
                    var dailyFromAverage = hourlyFromAverage * 8m;

                    var hourlyFromAgreement = employee.HourSalary > 0
                        ? employee.HourSalary
                        : (employee.RegularHours > 0 ? employee.AgreeSalary / employee.RegularHours : employee.AgreeSalary / DefaultMonthlyHours);
                    var dailyFromAgreement = hourlyFromAgreement * 8m;

                    bestDailyRate = Math.Max(dailyFromAverage, dailyFromAgreement);
                    bestMonthlyBase = bestDailyRate >= dailyFromAgreement ? avgMonthly : employee.AgreeSalary;
                }
            }

            var vacationPending = 0m;
            var medicalCertificatesAvailable = 0m;
            var available = Math.Max(0, accrued - takenDays - vacationPending);

            return new LeaveBalanceDto(available, vacationPending, takenDays, bestDailyRate, bestMonthlyBase, medicalCertificatesAvailable);
        }
    }
}
