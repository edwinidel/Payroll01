using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class ShiftSegmentEntity : BaseEntity
    {
        [Display(Name = "Turno")]
        public int ShiftId { get; set; }

        [Display(Name = "Hora de Inicio")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Hora de Fin")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Tipo de Segmento")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string SegmentType { get; set; } = "Trabajo"; // Trabajo, Comida, etc.

        [Display(Name = "Orden")]
        public int Order { get; set; }

        public ShiftEntity? Shift { get; set; }
    }
}