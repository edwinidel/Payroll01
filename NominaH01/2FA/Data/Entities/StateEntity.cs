using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    [Table("States")]
    public class StateEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "País")]
        public int CountryId { get; set; }

        [Display(Name = "Provincia")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Código")]
        public string State_code { get; set; } = string.Empty;

        [Display(Name = "Latitud")]
        public string Latitude { get; set; } = string.Empty;

        [Display(Name = "Longitud")]
        public string Longitude { get; set; } = string.Empty;

        [Display(Name = "Tipo")]
        public string Type { get; set; } = string.Empty;

        [Display(Name = "Creado")]
        public DateTime Created { get; set; }

        [Display(Name = "Modificado")]
        public DateTime? Modified { get; set; }

        [Display(Name = "Borrado")]
        public DateTime? Deleted { get; set; }

        [Display(Name = "¿Está borrado?")]
        public bool IsDeleted { get; set; }

        public DateTime CreatedLocal => Created.ToLocalTime();

        public DateTime? ModifiedLocal => Modified?.ToLocalTime();

        public DateTime? DeletedLocal => Deleted?.ToLocalTime();
        public ICollection<CityEntity>? Cities { get; set; }
        public ICollection<CompanyEntity>? Companies { get; set; }
        public ICollection<BranchEntity>? Branches { get; set; }

        public CountryEntity? Country { get; set; }


        public int? CitiesCount => Cities?.Where(s => !s.IsDeleted).ToList().Count;

    }

}
