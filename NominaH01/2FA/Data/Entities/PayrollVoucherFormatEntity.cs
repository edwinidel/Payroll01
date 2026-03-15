using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollVoucherFormats")]
    public class PayrollVoucherFormatEntity : BaseEntity
    {
        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        [Display(Name = "Tipo de Planilla")]
        public int PayrollTypeId { get; set; }

        [Display(Name = "Nombre del Formato")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Plantilla del Formato")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string FormatTemplate { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public CompanyEntity? Company { get; set; }
        public PayrollTypeEntity? PayrollType { get; set; }
    }
}