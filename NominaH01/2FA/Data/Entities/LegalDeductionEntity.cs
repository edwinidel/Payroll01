using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Data.Entities
{
    [Table("LegalDeductions")]
    public class LegalDeductionEntity : BaseEntity
    {
        private const string V = "El campo {0} no puede exceder los {1} caracteres";
        private const string V1 = "El campo {0} es requerido";

        [Display(Name = "Código")]
        [StringLength(10, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Concepto")]
        [StringLength(100, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Desc. Empleados")]
        [Range(1, 100, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        public decimal EmployeeDiscount { get; set; }

        [Display(Name = "Desc. Empleador")]
        [Range(1, 100, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        public decimal EmployerDiscount { get; set; }

        [Display(Name = "Tipo de Planilla")]
        public int PayrollTypeId { get; set; }

        [Display(Name = "País")]
        public int? CountryId { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        [Display(Name = "¿Descontar en planilla?")]
        public bool DeducFromPayroll { get; set; } = true;

        public PayrollTypeEntity? PayrollType { get; set; }
        public CountryEntity? Country { get; set; }
        public ICollection<PayrollTmpLegalDeductionEntity>? PayrollTmpLegalDeductions { get; set; }

    }
}