using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class PositionEntity : BaseEntity
    {
        [Display(Name = "Cargo")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede escedeer los {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public ICollection<EmployeeEntity>? Employees { get; set; }
    }
}
