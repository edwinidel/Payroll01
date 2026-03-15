using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollTmpOvertime")]
    public class PayrollTmpOvertimeEntity : BaseEntity
    {
        [Required]
        public int PayrollTmpEmployeeId { get; set; }

        [Required]
        public int PayrollTmpHeaderId { get; set; }

        [Display(Name = "Fecha del Sobretiempo")]
        [DataType(DataType.Date)]
        public DateTime OvertimeDate { get; set; }

        [Display(Name = "Hora de Entrada")]
        [DataType(DataType.Time)]
        public TimeSpan EntryTime { get; set; }

        [Display(Name = "Hora de Salida")]
        [DataType(DataType.Time)]
        public TimeSpan ExitTime { get; set; }

        [Display(Name = "Horas Calculadas")]
        public decimal CalculatedHours { get; set; }

        [Display(Name = "Código del Factor")]
        [StringLength(100)]
        public string FactorCode { get; set; } = string.Empty;

        [Display(Name = "Factor Aplicado")]
        public decimal AppliedFactor { get; set; }

        [Display(Name = "Monto por Hora")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Monto Total")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Es Domingo")]
        public bool IsSunday { get; set; }

        [Display(Name = "Es Feriado")]
        public bool IsHoliday { get; set; }

        [Display(Name = "Acumulado Semanal")]
        public decimal WeeklyAccumulated { get; set; }

        [Display(Name = "Tipo de Día")]
        public int? TypeOfDayId { get; set; }

        [Display(Name = "Tipo de Jornada")]
        public int? TypeOfWorkScheduleId { get; set; }

        // Navigation properties
        public PayrollTmpEmployeeEntity? PayrollTmpEmployee { get; set; }
        public PayrollTmpHeaderEntity? PayrollTmpHeader { get; set; }
        public OverTimeFactorEntity? OverTimeFactor { get; set; }
        public TypeOfDayEntity? TypeOfDay { get; set; }
        public TypeOfWorkScheduleEntity? TypeOfWorkSchedule { get; set; }
    }
}