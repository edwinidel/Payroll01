using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Data.Entities
{
    [Table("HistoricTmpLiabilities")]
    public class HistoricTmpLiabilityEntity : BaseEntity
    {
        [Display(Name = "Nomina")]
        public int PayrollTmpHeaderId { get; set; }

        [Display(Name = "Empleado")]
        public int PayrollTmpEmployeeId { get; set; }

        [Display(Name = "Deuda")]
        public int LiabilityId { get; set; }

        [Display(Name = "Monto a Descontar")]
        public decimal AmountToDiscount { get; set; }

        [Display(Name = "Monto Descontado")]
        public decimal DiscountedAmount { get; set; }


        public PayrollTmpHeaderEntity? PayrrollTmpHeader { get; set; }
        public PayrollTmpEmployeeEntity? PayrollTmpEmployee { get; set; }
        public LiabilityEntity? Liability { get; set; }


    }
}
