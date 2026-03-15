using System.ComponentModel.DataAnnotations;
using OvertimeCalculator.Validators;

namespace OvertimeCalculator.Models
{
    /// <summary>
    /// Represents employee work schedule and time tracking data for overtime calculation.
    /// </summary>
    /// <remarks>
    /// This model encapsulates all input required to calculate overtime hours including:
    /// - Employee identification and personal information
    /// - Work schedule details (start/end times, time breaks)
    /// - Day type classification (regular, Sunday, holiday, national mourning)
    /// - Schedule type (diurnal, mixed, nocturnal)
    /// - Time tracking records (marcaciones)
    /// 
    /// All properties include comprehensive validation attributes to ensure data integrity.
    /// </remarks>
    public class EmployeeInput
    {
        [Required(ErrorMessage = "Employee code is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Employee code must be between 1 and 50 characters")]
        public string Código { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 100 characters")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 100 characters")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID is required")]
        [StringLength(20, ErrorMessage = "ID cannot exceed 20 characters")]
        public string Cédula { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hourly salary is required")]
        [Range(0.01, 10000, ErrorMessage = "Hourly salary must be between 0.01 and 10000")]
        [System.Text.Json.Serialization.JsonPropertyName("Salario por Hora")]
        public decimal SalarioPorHora { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Company must be a positive number")]
        public int Compañía { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch must be a positive number")]
        public int Sucursal { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Department must be a positive number")]
        public int Departamento { get; set; }

        [Required(ErrorMessage = "Cost center is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Cost center must be a positive number")]
        [System.Text.Json.Serialization.JsonPropertyName("Centro de Costo")]
        public int CentroDeCosto { get; set; }

        [Required(ErrorMessage = "Project is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Project must be a positive number")]
        public int Proyecto { get; set; }

        [Required(ErrorMessage = "Phase is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Phase must be a positive number")]
        public int Fase { get; set; }

        [Required(ErrorMessage = "Activity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Activity must be a positive number")]
        public int Actividad { get; set; }

        [Required(ErrorMessage = "Day type is required")]
        [ValidDayType(ErrorMessage = "Invalid day type")]
        [System.Text.Json.Serialization.JsonPropertyName("Tipo de Día")]
        public string TipoDeDía { get; set; } = string.Empty;

        [Required(ErrorMessage = "Schedule type is required")]
        [RegularExpression("^(Diurno|Mixto|Nocturno)$", ErrorMessage = "Schedule type must be: Diurno, Mixto, or Nocturno")]
        [System.Text.Json.Serialization.JsonPropertyName("Tipo de Horario")]
        public string TipoDeHorario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lunch time is required")]
        [Range(0, 480, ErrorMessage = "Lunch time must be between 0 and 480 minutes")]
        [System.Text.Json.Serialization.JsonPropertyName("Tiempo Comida")]
        public int TiempoComida { get; set; }

        [Required(ErrorMessage = "Schedule start time is required")]
        [ValidTime(ErrorMessage = "Schedule start time must be in HH:mm format")]
        [System.Text.Json.Serialization.JsonPropertyName("Inicia Horario")]
        public string IniciaHorario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Schedule end time is required")]
        [ValidTime(ErrorMessage = "Schedule end time must be in HH:mm format")]
        [System.Text.Json.Serialization.JsonPropertyName("Fin Horario")]
        public string FinHorario { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("Hora Entrada")]
        public DateTime? HoraEntrada { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("Hora Salida")]
        public DateTime? HoraSalida { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("Marcaciones")]
        public List<Marcacion> Marcaciones { get; set; } = new List<Marcacion>();

        [Required(ErrorMessage = "Grace period (before entry) is required")]
        [NonNegative(ErrorMessage = "Grace period (before entry) cannot be negative")]
        [Range(0, 60, ErrorMessage = "Grace period (before entry) must be between 0 and 60 minutes")]
        [System.Text.Json.Serialization.JsonPropertyName("Periodo de Gracia Entrada Antes")]
        public int PeriodoGraciaEntradaAntes { get; set; }

        [Required(ErrorMessage = "Grace period (after entry) is required")]
        [NonNegative(ErrorMessage = "Grace period (after entry) cannot be negative")]
        [Range(0, 60, ErrorMessage = "Grace period (after entry) must be between 0 and 60 minutes")]
        [System.Text.Json.Serialization.JsonPropertyName("Periodo de Gracia Entrada Después")]
        public int PeriodoGraciaEntradaDespues { get; set; }

        [Required(ErrorMessage = "Grace period (before exit) is required")]
        [NonNegative(ErrorMessage = "Grace period (before exit) cannot be negative")]
        [Range(0, 60, ErrorMessage = "Grace period (before exit) must be between 0 and 60 minutes")]
        [System.Text.Json.Serialization.JsonPropertyName("Periodo de Gracia Salida Antes")]
        public int PeriodoGraciaSalidaAntes { get; set; }

        [Required(ErrorMessage = "Grace period (after exit) is required")]
        [NonNegative(ErrorMessage = "Grace period (after exit) cannot be negative")]
        [Range(0, 60, ErrorMessage = "Grace period (after exit) must be between 0 and 60 minutes")]
        [System.Text.Json.Serialization.JsonPropertyName("Periodo de Gracia Salida Después")]
        public int PeriodoGraciaSalidaDespues { get; set; }
    }
}