using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("TypeOfWorkers")]
    public class TypeOfWorkerEntity : BaseEntity
    {
        [Display(Name = "Tipo de Trabajador")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public ICollection<EmployeeEntity>? Employees { get; set; }
    }
}
