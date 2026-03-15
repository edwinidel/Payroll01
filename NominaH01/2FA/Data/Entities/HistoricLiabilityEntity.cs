using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Data.Entities
{
    [Table("HistoricLiabilities")]
    public class HistoricLiabilityEntity : BaseEntity
    {
        [Display(Name = "Nomina")]
        public int PayrollTmpHeaderId { get; set; }

        [Display(Name = "Empleado")]
        public int EmployeeId { get; set; }

        [Display(Name = "Deuda")]
        public int LiabilityId { get; set; }

        [Display(Name = "Monto a Descontar")]
        public decimal AmountToDiscount { get; set; }

        [Display(Name = "Monto Descontado")]
        public decimal DiscountedAmount { get; set; }


        public PayrollTmpHeaderEntity? PayrollTmpHeader { get; set; }
        public EmployeeEntity? Employee { get; set; }
        public LiabilityEntity? Liability { get; set; }
    }
}