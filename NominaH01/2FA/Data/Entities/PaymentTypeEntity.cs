using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class PaymentTypeEntity : BaseEntity
    {
        [Display(Name = "Tipo de Planilla")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }
    }
}
