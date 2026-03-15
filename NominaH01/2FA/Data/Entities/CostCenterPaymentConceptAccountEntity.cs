using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("CostCenterPaymentConceptAccounts")]
    public class CostCenterPaymentConceptAccountEntity : BaseEntity
    {
        [Display(Name = "Centro de Costo")]
        public int CostCenterId { get; set; }

        [Display(Name = "Concepto de Pago")]
        public int PaymentConceptId { get; set; }

        [Display(Name = "Cuenta Contable")]
        public int AccountId { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres")]
        public string? Description { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public CostCenterEntity? CostCenter { get; set; }
        public PaymentConceptEntity? PaymentConcept { get; set; }
        public AccountEntity? Account { get; set; }
    }
}