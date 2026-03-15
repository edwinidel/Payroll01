using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollTmpLegalDeductions")]
    public class PayrollTmpLegalDeductionEntity : BaseEntity
    {
        [Display(Name = "Planilla")]
        public int PayrollTmpHeaderId { get; set; }

        [Display(Name = "Empleado")]
        public int PayrollTmpEmployeeId { get; set; }

        [Display(Name = "Descuento Legal")]
        public int LegalDeductionId { get; set; }

        [Display(Name = "Monto")]
        public decimal Amount { get; set; }

        public PayrollTmpEmployeeEntity? PayrollTmpEmployee { get; set; }
        public LegalDeductionEntity? LegalDeduction { get; set; }
        public PayrollTmpHeaderEntity? PayrollTmpHeader { get; set; }
        
    }
}