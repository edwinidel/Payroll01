using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OvertimeCalculator.Models
{
    public class EmployeeInputDetail
    {
        [Key]
        public int Id { get; set; }
        public int HeaderId { get; set; }
        [ForeignKey("HeaderId")]
        public EmployeeInputHeader Header { get; set; } = null!;

        // Campos de cada marcación (basado en Range)
        public string IdMarcacion { get; set; } = string.Empty;  // Ej. "Entrada", "Salida", etc.
        public DateTime Hora { get; set; }
        public double Horas { get; set; }  // Si aplica
    }
}