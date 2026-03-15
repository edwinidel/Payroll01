using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    [Table("Countries")]
    public class CountryEntity
    {
        private const string StrLength = "El campo {0} no puede tener más de {1} caracteres.";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "País")]
        //[Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "ISO 3")]
        [MaxLength(3, ErrorMessage = StrLength)]
        public string Iso3 { get; set; } = string.Empty;

        [Display(Name = "ISO 2")]
        [MaxLength(2, ErrorMessage = StrLength)]
        public string Iso2 { get; set; } = string.Empty;

        [Display(Name = "Código Numérico")]
        public string Numeric_code { get; set; } = string.Empty;

        [Display(Name = "Código Telefónico")]
        [MaxLength(10, ErrorMessage = StrLength)]
        public string Phone_code { get; set; } = string.Empty;

        [Display(Name = "Capital")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Capital { get; set; } = string.Empty;

        [Display(Name = "Moneda")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Currency { get; set; } = string.Empty;

        [Display(Name = "Nombre de la Moneda")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Currency_name { get; set; } = string.Empty;

        [Display(Name = "Símbolo de Moneda")]
        [Column(TypeName = "nvarchar(200)")]
        public string Currency_symbol { get; set; } = string.Empty;

        [Display(Name = "Tld")]
        [MaxLength(100, ErrorMessage = StrLength)]
        public string Tld { get; set; } = string.Empty;

        [Display(Name = "Nativo")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Native { get; set; } = string.Empty;

        [Display(Name = "Región")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Region { get; set; } = string.Empty;

        [Display(Name = "ID de Región")]
        public int Region_id { get; set; }

        [Display(Name = "Sub Región")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Subregion { get; set; } = string.Empty;

        [Display(Name = "ID de Sub Región")]
        public int Subregion_id { get; set; }

        [Display(Name = "Nacionalidad")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Nationality { get; set; } = string.Empty;

        [Display(Name = "Zona Horaria")]
        public int? TimeZoneId { get; set; }

        [Display(Name = "Latitud")]
        [MaxLength(30, ErrorMessage = StrLength)]
        public string Latitude { get; set; } = string.Empty;

        [Display(Name = "Longitud")]
        [MaxLength(30, ErrorMessage = StrLength)]
        public string Longitude { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = StrLength)]
        public string Emoji { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = StrLength)]
        public string EmojiU { get; set; } = string.Empty;

        [Display(Name = "País")]
        [MaxLength(100, ErrorMessage = StrLength)]
        public string Name_es { get; set; } = string.Empty;

        [Display(Name = "Country")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Name_en { get; set; } = string.Empty;

        [Display(Name = "Pays")]
        [MaxLength(50, ErrorMessage = StrLength)]
        public string Name_fr { get; set; } = string.Empty;

        [Display(Name = "Bandera")]
        [MaxLength(200, ErrorMessage = StrLength)]
        public string Flag { get; set; } = string.Empty;

        [Display(Name = "Escudo")]
        [MaxLength(200, ErrorMessage = StrLength)]
        public string CoatOfArms { get; set; } = string.Empty;

        [Display(Name = "Creado")]
        public DateTime Created { get; set; }

        [Display(Name = "Creado por")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Modificado")]
        public DateTime? Modified { get; set; }

        [Display(Name = "Modificado por")]
        public string ModifiedBy { get; set; } = string.Empty;

        [Display(Name = "Borrado")]
        public DateTime? Deleted { get; set; }

        [Display(Name = "Borrado por")]
        public string DeletedBy { get; set; } = string.Empty;

        [Display(Name = "¿Está borrado?")]
        public bool IsDeleted { get; set; }

        [Display(Name = "Zona Horaria")]
        public List<CountryTimeZoneEntity> CountryTimeZones { get; set; } = new List<CountryTimeZoneEntity>();

        public DateTime CreatedLocal => Created.ToLocalTime();
        public DateTime? ModifiedLocal => Modified?.ToLocalTime();
        public DateTime? DeletedLocal => Deleted?.ToLocalTime();
        public ICollection<StateEntity>? States { get; set; }
        public TimeZoneEntity? TimeZone { get; set; }

        public int? StatesCount => States?.Where(s => !s.IsDeleted).ToList().Count;
        public int? QStates => States?.Where(s => s.IsDeleted).ToList().Count;
        public ICollection<EmployeeEntity>? Employees { get; set; }
        public ICollection<CompanyEntity>? Companies { get; set; }
        public ICollection<BranchEntity>? Branches { get; set; }
        public ICollection<PaymentConceptEntity>? PaymentConcepts { get; set; }
        public ICollection<LegalDeductionEntity>? LegalDeductions { get; set; }
        public ICollection<CountryEntity>? Countries { get; set; }

    }
}
