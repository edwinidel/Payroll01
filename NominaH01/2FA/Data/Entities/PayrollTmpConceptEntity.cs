using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollTmpConcepts")]
    public class PayrollTmpConceptEntity : BaseEntity
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

        [Display(Name = "Factor de Pago")]
        public decimal PayFactor { get; set; }

        [Display(Name = "Monto Total")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Tipo de Planilla")]
        public int PayrollTmpHeaderId { get; set; }

        public PayrollTmpHeaderEntity? PayrrollTmpHeader { get; set; }
        public PayrollTmpEmployeeEntity? PayrollTmpEmployee { get; set; }
        public PaymentConceptEntity? PaymentConcept { get; set; }
   }
}