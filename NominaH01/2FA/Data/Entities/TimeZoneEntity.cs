using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    [Table("TimeZones")]
    public class TimeZoneEntity : BaseEntity
    {
        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "GmtOffset")]
        public int GmtOffset { get; set; }

        [Display(Name = "DaylightSavingOffset")]
        public int DaylightSavingOffset { get; set; }

        public ICollection<CountryEntity>? Countries { get; set; }
    }
}
