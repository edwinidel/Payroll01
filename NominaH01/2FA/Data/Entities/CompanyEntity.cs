using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class CompanyEntity : BaseEntity
    {
        private const string StrLength = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Compañía")]
        [Required(ErrorMessage = "El campo {0} es requerido, por favor revise.")]
        [StringLength(100, ErrorMessage = StrLength)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Grupo de Negocio")]
        public int BusinessGroupId { get; set; }

        [Display(Name = "Teléfono Fijo")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string FixedPhone { get; set; } = string.Empty;

        [Display(Name = "Celular")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string CellPhone { get; set; } = string.Empty;

        [Display(Name = "País")]
        public int? CountryId { get; set; }

        [Display(Name = "Provincia")]
        public int? StateId { get; set; }

        [Display(Name = "Ciudad")]
        public int? CityId { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(200, ErrorMessage = StrLength)]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = StrLength)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "RUC")]
        public string Ruc { get; set; } = string.Empty;

        [Display(Name = "DV")]
        [StringLength(2, ErrorMessage = StrLength)]
        public string Dv { get; set; } = string.Empty;

        [Display(Name = "Riesgo Profesional")]
        public decimal ProfessionalRisk { get; set; }

        [Display(Name = "¿Activa?")]
        public bool IsActive { get; set; }

        [Display(Name = "Banco para Pago")]
        public int? PaymentBankId { get; set; }

        [Display(Name = "Formato de Voucher")]
        public int? VoucherFormatId { get; set; }

        public BusinessGroupEntity? BusinessGroup { get; set; }
        public CountryEntity? Country { get; set; }
        public StateEntity? State { get; set; }
        public CityEntity? City { get; set; }
        public BankEntity? PaymentBank { get; set; }
        public PayrollVoucherFormatEntity? VoucherFormat { get; set; }
        public ICollection<PayrollVoucherFormatEntity>? PayrollVoucherFormats { get; set; }
        public ICollection<CompanyPayrollVoucherEntity>? CompanyPayrollVouchers { get; set; }
        public ICollection<PayrollTmpHeaderEntity>? PayrollTmpHeaders { get; set; }

    }
}
