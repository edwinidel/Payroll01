using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PaymentFrequencies")]
    public class PaymentFrequencyEntity : BaseEntity
    {
        [Display(Name = "Frecuencia de Pago")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Cantidad de Días")]
        public int QuantityOfDays { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public ICollection<PaymentGroupEntity>? PaymentGroups { get; set; }
    }
}
