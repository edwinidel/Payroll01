using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class ScheduleEntity : BaseEntity
    {
        [Display(Name = "Horario")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; } = true;

        public ICollection<EmployeeEntity>? Employees { get; set; }
        public ICollection<ShiftEntity>? Shifts { get; set; }
        public ICollection<ShiftAssignmentEntity>? ShiftAssignments { get; set; }
    }
}
