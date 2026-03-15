using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class EmployeeTypeEntity : BaseEntity
    {
        [Display(Name = "Tipo de Empleado")]
        [StringLength(30, ErrorMessage = "El Campo {0} no puede exceder los {1} caracteres.")]
        [Required(ErrorMessage = "el campo {0} es requerido, por favor verifique.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; } = true;

        public ICollection<EmployeeEntity>? Employees { get; set; }
    }
}
