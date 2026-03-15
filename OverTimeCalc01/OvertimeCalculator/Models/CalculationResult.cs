using System.Collections.Generic;

namespace OvertimeCalculator.Models
{
    public class CalculationResult
    {
        public int Id { get; set; }
        public List<Range> Rangos { get; set; } = new List<Range>();
        public DateTime FechaMarcacion { get; set; }
        public double Tardanza { get; set; } // in minutes
        public string? Mensaje { get; set; }
    }
}