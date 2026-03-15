using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("ObservationTypes")]
    public class ObservationTypeEntity : BaseEntity
    {
        [Display(Name = "Tipo de Observaciones")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        [Display(Name = "Documento")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string? DocumentPath { get; set; } = string.Empty;
        public ICollection<EmployeeEntity>? Employees { get; set; }
        public ICollection<EmployeeObservationEntity>? EmployeeObservations { get; set; }
    }
}
