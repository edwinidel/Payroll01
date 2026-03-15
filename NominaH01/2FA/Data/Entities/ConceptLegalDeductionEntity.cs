using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Data.Entities
{
    [Table("ConceptLegalDeductions")]
    public class ConceptLegalDeductionEntity : BaseEntity
    {
        [Display(Name = "Concepto")]
        public int PaymentConceptId { get; set; }
        public PaymentConceptEntity? PaymentConcept { get; set; }

        [Display(Name = "Descuento Legal")]
        public int LegalDeductionEntityId { get; set; }

        [Display(Name = "¿Regla Especial?")]
        public bool HasSpecialRule { get; set; }

        [Display(Name = "Regla Especial")]
        public string? SpecialRule { get; set; } = string.Empty;

        public LegalDeductionEntity? LegalDeduction { get; set; }

    }
}
