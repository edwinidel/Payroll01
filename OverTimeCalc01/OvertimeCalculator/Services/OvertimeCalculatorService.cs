using System;
using System.Collections.Generic;
using System.Linq;
using OvertimeCalculator.Data;
using OvertimeCalculator.Models;
using Microsoft.EntityFrameworkCore;
using RangeModel = OvertimeCalculator.Models.Range;

namespace OvertimeCalculator.Services
{
    public class OvertimeCalculatorService
    {
        private readonly OvertimeDbContext _context;

        public OvertimeCalculatorService(OvertimeDbContext context)
        {
            _context = context;
        }

        public CalculationResult Calculate(EmployeeInput input)
        {
            var result = new CalculationResult();

            // Calculate entry and exit times, and total worked hours
            DateTime entryTime, exitTime;
            double totalWorkedHours;
            if (input.Marcaciones != null && input.Marcaciones.Any())
            {
                var sortedMarc = input.Marcaciones.OrderBy(m => m.Hora).ToList();
                entryTime = sortedMarc.First().Hora;
                exitTime = sortedMarc.Last().Hora;
                TimeSpan totalWorkedSpan = TimeSpan.Zero;
                for (int i = 0; i < sortedMarc.Count - 1; i += 2)
                {
                    if (i + 1 < sortedMarc.Count)
                    {
                        totalWorkedSpan += sortedMarc[i + 1].Hora - sortedMarc[i].Hora;
                    }
                }
                totalWorkedSpan -= TimeSpan.FromMinutes(input.TiempoComida);
                totalWorkedHours = totalWorkedSpan.TotalHours;
                if (input.Marcaciones.Count % 2 != 0)
                    throw new ArgumentException("El número de marcaciones debe ser par para calcular periodos de trabajo");
            }
            else
            {
                if (input.HoraEntrada == null || input.HoraSalida == null)
                    throw new ArgumentException("Se debe proporcionar Marcaciones con HoraEntrada/HoraSalida");
                entryTime = input.HoraEntrada.Value;
                exitTime = input.HoraSalida.Value;
                totalWorkedHours = (exitTime - entryTime - TimeSpan.FromMinutes(input.TiempoComida)).TotalHours;
            }

            result.FechaMarcacion = entryTime.Date;

            // Get day factor from database, default to 1.0 if not found
            var dayFactorEntity = _context.DayFactors
                .FirstOrDefault(df => df.DayType == input.TipoDeDía && df.IsActive);
            double dayFactor = dayFactorEntity?.Factor ?? 1.0;

            // Check if it's a special day (entire shift considered overtime)
            bool isSpecialDay = new[] { "Domingo Compensatorio", "Fiesta", "Duelo Nacional" }
                .Contains(input.TipoDeDía);

            // Determine schedule factor
            double scheduleFactor = input.TipoDeHorario switch
            {
                "Diurno" => 1.0,
                "Mixto" => 8.0 / 7.5,
                "Nocturno" => 8.0 / 7.0,
                _ => 1.0
            };

            // Parse times
            TimeSpan scheduledStart = TimeSpan.Parse(input.IniciaHorario);
            TimeSpan scheduledEnd = TimeSpan.Parse(input.FinHorario);
            TimeSpan workedEnd = exitTime.TimeOfDay;
            TimeSpan lunch = TimeSpan.FromMinutes(input.TiempoComida);

            // Handle schedules that span midnight
            if (scheduledEnd < scheduledStart)
            {
                scheduledEnd = scheduledEnd.Add(TimeSpan.FromDays(1));
            }

            // Handle case where workedEnd is on the next day
            if (workedEnd < scheduledStart)
            {
                workedEnd = workedEnd.Add(TimeSpan.FromDays(1));
            }

            // Calculate tardiness
            DateTime scheduledStartDT = result.FechaMarcacion.Date + scheduledStart;
            DateTime graceStart = scheduledStartDT - TimeSpan.FromMinutes(input.PeriodoGraciaEntradaAntes);
            DateTime graceEnd = scheduledStartDT + TimeSpan.FromMinutes(input.PeriodoGraciaEntradaDespues);
            DateTime actualEntry = entryTime;
            if (actualEntry > graceEnd)
            {
                result.Tardanza = (actualEntry - scheduledStartDT).TotalMinutes;
            }
            else
            {
                result.Tardanza = 0;
            }

            // Calculate hours
            double scheduledNetHours = (scheduledEnd - scheduledStart - lunch).TotalHours;
            double totalWorkedNetHours = (workedEnd - scheduledStart - lunch).TotalHours;
            double regularHours = Math.Min(scheduledNetHours, totalWorkedNetHours);
            double extraHours = Math.Max(0, totalWorkedNetHours - regularHours);

            // Subtract tardiness from regular hours
            double tardanzaHours = result.Tardanza / 60.0;
            regularHours = Math.Max(0, regularHours - tardanzaHours);

            // Add regular range
            result.Rangos.Add(new RangeModel { Id = "Ordinaria", Horas = regularHours, Factor = dayFactor * scheduleFactor });

            if (extraHours > 0)
            {
                var weeklyAccumulatedExtraHours = GetWeeklyAccumulatedExtraHours(input.Código, entryTime, exitTime);
                TimeSpan extraStart = scheduledEnd;
                TimeSpan extraEnd = workedEnd;

                // Define diurnal and nocturnal boundary
                TimeSpan diurnalEnd = TimeSpan.Parse("18:00");

                var segments = new List<(TimeSpan Start, TimeSpan End, double OvertimeFactor, string SegmentType)>();

                // Split extra hours into diurnal (until 18:00) and nocturnal (after 18:00)
                if (extraStart < diurnalEnd)
                {
                    TimeSpan segEnd = extraEnd < diurnalEnd ? extraEnd : diurnalEnd;
                    segments.Add((extraStart, segEnd, 1.25, "Extra Diurna")); // 1.25 for diurnal overtime
                }

                if (extraEnd > diurnalEnd)
                {
                    TimeSpan segStart = extraStart > diurnalEnd ? extraStart : diurnalEnd;
                    segments.Add((segStart, extraEnd, 1.50, "Extra Nocturna")); // 1.50 for nocturnal overtime
                }

                // Calculate ranges with correct factor multiplication.
                // A 75% surcharge applies either when daily overtime exceeds 3 hours
                // or when cumulative weekly overtime (Monday-Sunday) exceeds 9 hours.
                double accumulatedExtraDaily = 0;
                double accumulatedExtraCurrentWeek = 0;

                foreach (var seg in segments)
                {
                    double segHours = (seg.End - seg.Start).TotalHours;
                    double overtimeFactor = seg.OvertimeFactor;
                    string id = seg.SegmentType;
                    
                    // For special days: overtimeFactor × (dayFactor - 0.5) × 1.75
                    // For regular days: dayFactor × overtimeFactor
                    double baseFactor = isSpecialDay
                        ? (overtimeFactor * (dayFactor - 0.5) * 1.75)
                        : (dayFactor * overtimeFactor);

                    var dailyHoursBeforeExcess = Math.Max(0, 3 - accumulatedExtraDaily);
                    var weeklyHoursBeforeExcess = Math.Max(0, 9 - (weeklyAccumulatedExtraHours + accumulatedExtraCurrentWeek));
                    var normalInSeg = Math.Min(segHours, Math.Min(dailyHoursBeforeExcess, weeklyHoursBeforeExcess));

                    if (normalInSeg > 0)
                    {
                        result.Rangos.Add(new RangeModel { Id = id, Horas = normalInSeg, Factor = baseFactor });
                    }

                    var excessHours = segHours - normalInSeg;
                    if (excessHours > 0)
                    {
                        double excessFactor = isSpecialDay
                            ? baseFactor
                            : (dayFactor * overtimeFactor * 1.75);

                        result.Rangos.Add(new RangeModel { Id = "Excedente", Horas = excessHours, Factor = excessFactor });
                    }

                    accumulatedExtraDaily += segHours;
                    accumulatedExtraCurrentWeek += segHours;
                }
            }

            return result;
        }

        private double GetWeeklyAccumulatedExtraHours(string employeeCode, DateTime currentEntry, DateTime currentExit)
        {
            var weekStart = GetWeekStart(currentEntry.Date);
            var weekEndExclusive = weekStart.AddDays(7);

            var histories = _context.CalculationHistories
                .AsNoTracking()
                .Include(h => h.EmployeeInputHeader)
                    .ThenInclude(h => h.Details)
                .Where(h => h.Código == employeeCode
                            && h.IsActive
                            && h.FechaMarcacion >= weekStart
                            && h.FechaMarcacion < weekEndExclusive)
                .ToList();

            double accumulated = 0;

            foreach (var history in histories)
            {
                var details = history.EmployeeInputHeader?.Details?
                    .OrderBy(d => d.Hora)
                    .ToList();

                if (details != null && details.Count >= 2)
                {
                    var existingEntry = details.First().Hora;
                    var existingExit = details.Last().Hora;
                    if (currentEntry < existingExit && currentExit > existingEntry)
                    {
                        continue;
                    }
                }

                accumulated += history.CalculationResult.Rangos
                    .Where(r => !string.Equals(r.Id, "Ordinaria", StringComparison.OrdinalIgnoreCase))
                    .Sum(r => r.Horas);
            }

            return accumulated;
        }

        private static DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + ((int)date.DayOfWeek - (int)DayOfWeek.Monday)) % 7;
            return date.Date.AddDays(-diff);
        }
    }
}