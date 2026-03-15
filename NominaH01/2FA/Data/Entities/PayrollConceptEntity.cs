using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollConcepts")]
    public class PayrollConceptEntity : BaseEntity
    {
        [Required]
        public int PayrollTmpEmployeeId { get; set; }

        [Required]
        public int PaymentConceptId { get; set; }

        [Display(Name = "Cantidad de Horas")]
        public decimal Hours { get; set; }

        [Display(Name = "Monto Unitario")]
        [DataType(DataType.Currency)]
        public decimal UnitAmount { get; set; }

        [Display(Name = "Monto Total")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Notas")]
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
 
        public PayrollTmpEmployeeEntity? PayrollTmpEmployee { get; set; }
        public PaymentConceptEntity? PaymentConcept { get; set; }
   }

}