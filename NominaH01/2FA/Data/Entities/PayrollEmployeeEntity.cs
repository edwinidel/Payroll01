using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollEmployees")]
    public class PayrollEmployeeEntity : BaseEntity
    {
        [Required]
        public int PayrollTmpHeaderId { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public EmployeeEntity? Employee { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalEarnings { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalDeductions { get; set; }

        [DataType(DataType.Currency)]
        public decimal NetPay { get; set; }

        public PayrollHeaderEntity? PayrollHeader { get; set; }
        public ICollection<PayrollConceptEntity> Concepts { get; set; } = [];
        public ICollection<PayrollLegalDeductionEntity> LegalDeductions { get; set; } = [];
    }
}