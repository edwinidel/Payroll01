using System.ComponentModel.DataAnnotations;

namespace OvertimeCalculator.Models
{
    public class EmployeeInputHeader
    {
        [Key]
        public int Id { get; set; }
        public int Compañía { get; set; }
        public string TipoDeDía { get; set; } = string.Empty;
        public string TipoDeHorario { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación con detalles
        public ICollection<EmployeeInputDetail> Details { get; set; } = new List<EmployeeInputDetail>();
    }
}