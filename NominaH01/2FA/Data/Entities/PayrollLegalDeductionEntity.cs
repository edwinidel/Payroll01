using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollLegalDeductions")]
    public class PayrollLegalDeductionEntity : BaseEntity
    {
        [Display(Name = "Planilla")]
        public int PayrollHeaderId { get; set; }

        [Display(Name = "Empleado")]
        public int PayrollEmployeeId { get; set; }

        [Display(Name = "Descuento Legal")]
        public int LegalDeductionId { get; set; }

        [Display(Name = "Monto")]
        public decimal Amount { get; set; }

        public PayrollEmployeeEntity? PayrollEmployee { get; set; }
        public LegalDeductionEntity? LegalDeduction { get; set; }
        public PayrollHeaderEntity? PayrollHeader { get; set; }
        
    }
}