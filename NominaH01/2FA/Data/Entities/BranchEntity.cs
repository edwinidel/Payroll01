using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("Branches")]
    public class BranchEntity : BaseEntity
    {
        private const string StrLength = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Sucursal")]
        [StringLength(100, ErrorMessage = StrLength)]
        [Required(ErrorMessage = "El cmapo {0} es requerido.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

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
        public ICollection<EmployeeEntity>? Employees { get; set; }
        public CountryEntity? Country { get; set; }
        public StateEntity? State { get; set; } 
        public CityEntity? City { get; set; }
    }
}
