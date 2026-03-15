using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class HoliDayEntity : BaseEntity
    {
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime Date { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres.")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
    }
}