using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("CountryTimeZones")]
    public class CountryTimeZoneEntity : BaseEntity
    {
        private const string StrLength = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "País")]
        public int CountryId { get; set; }

        [Display(Name = "Zona Horaria")]
        [StringLength(20, ErrorMessage = StrLength)]
        public string TimeZone { get; set; } = string.Empty;

        public ICollection<CountryEntity>? Countries { get; set; }
    }
}
