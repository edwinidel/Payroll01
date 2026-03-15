using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PaymentConcepts")]
    public class PaymentConceptEntity : BaseEntity
    {
        [Display(Name = "Concepto de Pago")]
        [StringLength(50, ErrorMessage = "El concepto {0} no puede exceder los {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Concepto de Pago")]
        [StringLength(10, ErrorMessage = "El concepto {0} no puede exceder los {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "País")]
        public int CountryId { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        [Display(Name = "¿Hrs Regulares?")]
        public bool RegularHours { get; set; }

        [Display(Name = "¿Hrs Extras?")]
        public bool ExtraHours { get; set; }

        [Display(Name = "Factor de Pago")]
        public decimal PayFactor { get; set; }

        [Display(Name = "¿Acepta Pago Recurrente?")]
        public bool RecurrentPayment { get; set; }

        [Display(Name = "Predeterminado")]
        public bool IsPredetermined { get; set; }

        [Display(Name = "¿Es Construcción?")]
        public bool IsConstruction { get; set; }

        [Display(Name = "Cuenta Contable")]
        public int? AccountId { get; set; }

        public ICollection<FixedPaymentEntity>? FixedPayments { get; set; }
        public CountryEntity? Country { get; set; }
        public AccountEntity? Account { get; set; }
    }
}
