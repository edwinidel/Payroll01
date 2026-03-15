using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OvertimeCalculator.Data;
using OvertimeCalculator.Dtos;
using OvertimeCalculator.Models;
using OvertimeCalculator.Services;
using System.Text.Json;
using System.Linq;

namespace OvertimeCalculator.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0", Deprecated = true)]
    public class OvertimeController : ControllerBase
    {
        private readonly OvertimeDbContext _context;
        private readonly ILogger<OvertimeController> _logger;
        private readonly OvertimeCalculatorService _service;

        public OvertimeController(OvertimeDbContext context, ILogger<OvertimeController> logger, OvertimeCalculatorService service)
        {
            _context = context;
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Calculates overtime hours for an employee based on their work schedule and daily hours worked.
        /// </summary>
        /// <param name="input">The employee input data containing work schedule information and time tracking data.</param>
        /// <returns>A CalculationResult object containing computed regular hours, extra hours (diurnal/nocturnal), 
        /// excess hours, and special day multipliers.</returns>
        /// <remarks>
        /// This endpoint:
        /// - Requires JWT authentication
        /// - Validates input data against multiple constraints
        /// - Automatically detects overlapping shifts and deactivates conflicting previous calculations
        /// - Saves calculation history for audit trail
        /// - Applies special day factors (Sunday 1.5x, Holiday 2.5x)
        /// </remarks>
        [HttpPost("calculate")]
        [Authorize]
        public async Task<IActionResult> Calculate([FromBody] EmployeeInput input)
        {
            if (input == null)
            {
                return BadRequest("Payload vacío");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El ModelState es inválido para el empleado {EmployeeCode}", input.Código);
                return BadRequest(ModelState);
            }

            return await ProcessCalculation(input);
        }

        [HttpPost("calculate-overtime-simple")]
        [Authorize]
        public async Task<IActionResult> CalculateOvertimeSimple([FromBody] OvertimeSimpleInput input)
        {
            if (input == null)
            {
                return BadRequest("Payload vacío");
            }

            if (input.HoraSalida <= input.HoraEntrada)
            {
                return BadRequest("La hora de salida debe ser posterior a la hora de entrada.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El ModelState es inválido para el empleado {EmployeeCode}", input.Codigo);
                return BadRequest(ModelState);
            }

            var mapped = new EmployeeInput
            {
                Código = input.Codigo,
                Nombre = input.Nombre,
                Apellido = input.Apellido,
                Cédula = input.Cedula,
                SalarioPorHora = input.SalarioPorHora,
                Compañía = input.Compania,
                Sucursal = input.Sucursal,
                Departamento = input.Departamento,
                CentroDeCosto = input.CentroDeCosto,
                Proyecto = input.Proyecto,
                Fase = input.Fase,
                Actividad = input.Actividad,
                TipoDeDía = string.IsNullOrWhiteSpace(input.TipoDeDia) ? "Regular" : input.TipoDeDia!,
                TipoDeHorario = string.IsNullOrWhiteSpace(input.TipoDeHorario) ? "Diurno" : input.TipoDeHorario!,
                TiempoComida = input.TiempoComida ?? 0,
                IniciaHorario = input.HoraEntrada.ToString("HH:mm"),
                FinHorario = input.HoraEntrada.ToString("HH:mm"),
                HoraEntrada = input.HoraEntrada,
                HoraSalida = input.HoraSalida,
                Marcaciones = new List<Marcacion>
                {
                    new() { Id = "Entrada", Hora = input.HoraEntrada },
                    new() { Id = "Salida", Hora = input.HoraSalida }
                },
                PeriodoGraciaEntradaAntes = 0,
                PeriodoGraciaEntradaDespues = 0,
                PeriodoGraciaSalidaAntes = 0,
                PeriodoGraciaSalidaDespues = 0
            };

            return await ProcessCalculation(mapped);
        }

        [HttpPost("deactivate-calculation")]
        [Authorize]
        public async Task<IActionResult> DeactivateCalculation([FromBody] DeactivateOvertimeCalculationInput input)
        {
            if (input == null)
            {
                return BadRequest("Payload vacío");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (input.HoraSalida <= input.HoraEntrada)
            {
                return BadRequest("La hora de salida debe ser posterior a la hora de entrada.");
            }

            var existingHistories = await _context.CalculationHistories
                .Include(h => h.EmployeeInputHeader)
                    .ThenInclude(h => h.Details)
                .Where(h => h.Código == input.Codigo
                    && h.FechaMarcacion.Date == input.FechaMarcacion.Date
                    && h.IsActive)
                .ToListAsync();

            var deactivatedCount = 0;

            foreach (var history in existingHistories)
            {
                var existingEntry = history.EmployeeInputHeader?.Details?
                    .FirstOrDefault(d => d.IdMarcacion == "Entrada")?.Hora;
                var existingExit = history.EmployeeInputHeader?.Details?
                    .FirstOrDefault(d => d.IdMarcacion == "Salida")?.Hora;

                if (existingEntry.HasValue
                    && existingExit.HasValue
                    && existingEntry.Value == input.HoraEntrada
                    && existingExit.Value == input.HoraSalida)
                {
                    history.IsActive = false;
                    deactivatedCount++;
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Se desactivaron {Count} cálculos OTC para empleado {EmployeeCode} en fecha {Date}", deactivatedCount, input.Codigo, input.FechaMarcacion.Date);

            return Ok(new { Deactivated = deactivatedCount });
        }

        private async Task<IActionResult> ProcessCalculation(EmployeeInput input)
        {
            // Log request details for connection troubleshooting
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            var requestMethod = HttpContext.Request.Method;
            var requestPath = HttpContext.Request.Path;
            _logger.LogInformation("Solicitud recibida - Método: {Method}, Ruta: {Path}, IP: {IP}, UserAgent: {UserAgent}, Empleado: {EmployeeCode}, Compañía: {Company}, TipoDeDía: {DayType}, TipoDeHorario: {ScheduleType}, Marcaciones: {MarcacionesCount}",
                requestMethod, requestPath, ipAddress, userAgent, input.Código, input.Compañía, input.TipoDeDía, input.TipoDeHorario, input.Marcaciones?.Count ?? 0);

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
                _logger.LogInformation("Calculando Sobre Teimpo para el empleado {EmployeeCode} (User: {UserId})",
                    input.Código, userId);

                var result = _service.Calculate(input);

                // Save to history
                var totalRegulares = result.Rangos.Where(r => r.Id == "Ordinaria").Sum(r => r.Horas);
                var totalExtras = result.Rangos.Where(r => r.Id != "Ordinaria").Sum(r => r.Horas);
                int userIdInt = int.Parse(userId);
                // Save EmployeeInput to Header and Details
                var header = new EmployeeInputHeader
                {
                    Compañía = input.Compañía,
                    TipoDeDía = input.TipoDeDía,
                    TipoDeHorario = input.TipoDeHorario
                };
                _context.EmployeeInputHeaders.Add(header);
                await _context.SaveChangesAsync(); // Save to get ID

                // Save all marcaciones as details
                if (input.Marcaciones != null && input.Marcaciones.Any())
                {
                    foreach (var marcacion in input.Marcaciones)
                    {
                        var detail = new EmployeeInputDetail
                        {
                            HeaderId = header.Id,
                            IdMarcacion = marcacion.Id,
                            Hora = marcacion.Hora,
                            Horas = 0  // Calculate if needed
                        };
                        _context.EmployeeInputDetails.Add(detail);
                    }
                }
                else
                {
                    // Fallback to HoraEntrada and HoraSalida
                    if (input.HoraEntrada == null || input.HoraSalida == null)
                    {
                        return BadRequest("Se debe proporcionar Marcaciones u HoraEntrada/HoraSalida.");
                    }
                    var entryDetail = new EmployeeInputDetail
                    {
                        HeaderId = header.Id,
                        IdMarcacion = "Entrada",
                        Hora = input.HoraEntrada.Value,
                        Horas = 0
                    };
                    _context.EmployeeInputDetails.Add(entryDetail);

                    var exitDetail = new EmployeeInputDetail
                    {
                        HeaderId = header.Id,
                        IdMarcacion = "Salida",
                        Hora = input.HoraSalida.Value,
                        Horas = 0
                    };
                    _context.EmployeeInputDetails.Add(exitDetail);
                }

                var history = new CalculationHistory
                {
                    UserId = userIdInt,
                    EmployeeInputHeaderId = header.Id,
                    CalculationResultJson = JsonSerializer.Serialize(result),
                    FechaMarcacion = result.FechaMarcacion,
                    Compañía = input.Compañía,
                    TipoDeDía = input.TipoDeDía,
                    TipoDeHorario = input.TipoDeHorario,
                    TotalHorasRegulares = totalRegulares,
                    TotalHorasExtras = totalExtras,
                    Tardanza = result.Tardanza,
                    Código = input.Código,
                    IsActive = true
                };

                // Inactivar cálculos previos para el mismo empleado, fecha y horas
                DateTime entryTimeForQuery, exitTimeForQuery;
                if (input.Marcaciones != null && input.Marcaciones.Any())
                {
                    var sortedMarc = input.Marcaciones.OrderBy(m => m.Hora).ToList();
                    entryTimeForQuery = sortedMarc.First().Hora;
                    exitTimeForQuery = sortedMarc.Last().Hora;
                }
                else
                {
                    entryTimeForQuery = input.HoraEntrada ?? DateTime.MinValue;
                    exitTimeForQuery = input.HoraSalida ?? DateTime.MinValue;
                }

                var existingHistories = _context.CalculationHistories
                    .Include(h => h.EmployeeInputHeader)
                        .ThenInclude(h => h.Details)
                    .Where(h => h.Código == input.Código && h.FechaMarcacion.Date == result.FechaMarcacion.Date && h.IsActive)
                    .ToList();

                string overlapMessage = string.Empty;

                foreach (var existing in existingHistories)
                {
                    // Obtener las horas de entrada y salida del turno existente
                    var existingEntry = existing.EmployeeInputHeader?.Details?.FirstOrDefault(d => d.IdMarcacion == "Entrada")?.Hora;
                    var existingExit = existing.EmployeeInputHeader?.Details?.FirstOrDefault(d => d.IdMarcacion == "Salida")?.Hora;
                    
                    if (existingEntry.HasValue && existingExit.HasValue)
                    {
                        // Verificar si hay traslape entre turnos
                        // Traslape ocurre si: new_start < existing_end AND new_end > existing_start
                        if (entryTimeForQuery < existingExit.Value && exitTimeForQuery > existingEntry.Value)
                        {
                            existing.IsActive = false;
                            overlapMessage = $"Se desactivó un cálculo anterior (entrada: {existingEntry.Value:HH:mm}, salida: {existingExit.Value:HH:mm}) porque se traslapaba con el nuevo cálculo.";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(overlapMessage))
                {
                    result.Mensaje = overlapMessage;
                }

                _context.CalculationHistories.Add(history);
                await _context.SaveChangesAsync();

                // Add ID to result
                result.Id = history.Id;

                _logger.LogInformation("Cálculo completado exitosamente para el empleado {EmployeeCode} - ID: {CalculationId}, HorasRegulares: {RegularHours}, HorasExtras: {ExtraHours}, Mensaje: {Message}", input.Código, result.Id, totalRegulares, totalExtras, result.Mensaje ?? "Ninguno");

                return Ok(result);

            }
            catch (Exception ex)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                _logger.LogError(ex, "Error en cálculo de sobre tiempo para el empleado {EmployeeCode} por el usuario {UserId}", 
                    input?.Código, userId);
                
                return BadRequest(new { Error = "Un error ocurrió mientras se procesaba el cálculo", Message = ex.Message });
            }
        }
    }
}