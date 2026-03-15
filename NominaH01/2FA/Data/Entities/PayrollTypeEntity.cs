using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollTypes")]
    public class PayrollTypeEntity : BaseEntity
    {
        [Display(Name = "Tipo de Planilla")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Código")]
        [StringLength(1, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public ICollection<LegalDeductionEntity>? LegalDeductions { get; set; }
        public ICollection<PayrollTmpHeaderEntity>? PayrollTmpHeaders { get; set; }
    }
}
