using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace OvertimeCalculator.Models
{
    public class CalculationHistory
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EmployeeInputHeaderId { get; set; }
        [ForeignKey("EmployeeInputHeaderId")]
        public EmployeeInputHeader EmployeeInputHeader { get; set; } = null!;
        public string CalculationResultJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Individual fields for reporting
        public DateTime FechaMarcacion { get; set; }
        public int Compañía { get; set; }
        public string TipoDeDía { get; set; } = string.Empty;
        public string TipoDeHorario { get; set; } = string.Empty;
        public double TotalHorasRegulares { get; set; }
        public double TotalHorasExtras { get; set; }
        public double Tardanza { get; set; }
        public string Código { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Para acceso fácil, reconstruir desde Header y Details si es necesario
        public CalculationResult CalculationResult => JsonSerializer.Deserialize<CalculationResult>(CalculationResultJson) ?? new CalculationResult();
    }
}