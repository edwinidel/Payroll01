using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PaymentGroups")]
    public class PaymentGroupEntity : BaseEntity
    {
        [Display(Name = "Grupo de Pago")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Horas Base")]
        public decimal BaseHours { get; set; }

        [Display(Name = "Frecuencia de Pago")]
        public int PaymentFrequencyId { get; set; }

        [Display(Name = "Ultimo Pago")]
        [DataType(DataType.Date)]
        public DateTime? LastPayDate { get; set; }

        [Display(Name = "Fecha de Ausencias")]
        [DataType(DataType.Date)]
        public DateTime? LastAbsensestDate { get; set; }

        [Display(Name = "Fecha de Sobretiempo")]
        [DataType(DataType.Date)]
        public DateTime? ExtraTimeDate { get; set; }

        [Display(Name = "Cantidad de Días")]
        public int QuantityOfDays { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public PaymentFrequencyEntity? PaymentFrequency { get; set; }
        public ICollection<PayrollTmpHeaderEntity>? PayrrollTmps { get; set; }
        public ICollection<EmployeeEntity>? Employees { get; set; }

    }
}
