using System.ComponentModel.DataAnnotations;

namespace OvertimeCalculator.Models
{
    public class Marcacion
    {
        [Required]
        public string Id { get; set; } = string.Empty;  // Ej. "Entrada", "Salida", "Pausa", etc.
        [Required]
        public DateTime Hora { get; set; }
    }
}