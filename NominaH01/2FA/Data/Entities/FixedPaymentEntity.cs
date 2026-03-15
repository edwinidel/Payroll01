using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class FixedPaymentEntity : BaseEntity
    {
        private const string StrLength = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Concepto de Pago")]
        public int PaymentConceptId { get; set; }

        [Display(Name = "Empleado")]
        public int EmployeeId { get; set; }

        [Display(Name = "Frec. de Pago")]
        [StringLength(1, ErrorMessage = StrLength)]
        public string PaymentFrequency { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        [Display(Name = "Monto a Pagar")]
        [DataType(DataType.Currency)]
        public decimal PayAmount { get; set; }

        [Display(Name ="Inicio del Pago")]
        [DataType(DataType.Date)]
        public DateTime InitDate { get; set; }

        [Display(Name = "Fin del Pago")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Comentario")]
        [StringLength(1000, ErrorMessage = StrLength)]
        public string Comentary { get; set; } = string.Empty;

        [Display(Name = "Autorizado por")]
        [StringLength(50, ErrorMessage = StrLength)]
        public string AuthorizeBy { get; set; } = string.Empty;



        public PaymentConceptEntity? PaymentConcept { get; set; }
        public EmployeeEntity? Employee { get; set; }
    }
}
