using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Creado")]
        public DateTime Created { get; set; }

        [Display(Name = "Creado por")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Modificado")]
        public DateTime? Modified { get; set; }

        [Display(Name ="Modificado por")]
        public string ModifiedBy { get; set;} = string.Empty;

        [Display(Name = "Borrado")]
        public DateTime? Deleted { get; set; }

        [Display(Name ="Borrado por")]
        public string DeletedBy { get; set;} = string.Empty;

        [Display(Name = "¿Está borrado?")]
        public bool IsDeleted { get; set; }

        public DateTime CreatedLocal => Created.ToLocalTime();

        public DateTime? ModifiedLocal => Modified?.ToLocalTime();

        public DateTime? DeletedLocal => Deleted?.ToLocalTime();
    }
}
