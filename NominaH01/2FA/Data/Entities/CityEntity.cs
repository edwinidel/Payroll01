using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    [Table("Cities")]
    public class CityEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Provincia")]
        public int StateId { get; set; }

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
        public StateEntity? State { get; set; }

        public ICollection<CompanyEntity>? Companies { get; set; }
        public ICollection<BranchEntity>? Branches { get; set; }

    }

}
