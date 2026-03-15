using System.ComponentModel.DataAnnotations;

namespace OvertimeCalculator.Dtos
{
    public class DeactivateOvertimeCalculationInput
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public DateTime FechaMarcacion { get; set; }

        [Required]
        public DateTime HoraEntrada { get; set; }

        [Required]
        public DateTime HoraSalida { get; set; }
    }
}
