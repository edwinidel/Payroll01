using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class ShiftEntity : BaseEntity
    {
        [Display(Name = "Turno")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Segmentos del Turno")]
        public ICollection<ShiftSegmentEntity>? ShiftSegments { get; set; }

        // Property for form binding
        public List<ShiftSegmentEntity>? Segments { get; set; }

        [Display(Name = "Horario")]
        public int ScheduleId { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; } = true;

        public ScheduleEntity? Schedule { get; set; }

        public ICollection<ShiftAssignmentEntity>? ShiftAssignments { get; set; }
    }
}