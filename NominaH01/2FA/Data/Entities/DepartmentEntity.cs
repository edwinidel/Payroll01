using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("Departments")]
    public class DepartmentEntity : BaseEntity
    {
        [Display(Name = "Departamentos")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres")]
        [Required(ErrorMessage = "El cmapo {0} es requerido.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        public ICollection<EmployeeEntity>? Employees { get; set; }
    }
}
