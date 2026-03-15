using System;
using System.ComponentModel.DataAnnotations;

namespace OvertimeCalculator.Dtos
{
    /// <summary>
    /// Minimal payload for sending overtime-only ranges. Assumes provided entry/exit are overtime boundaries.
    /// </summary>
    public class OvertimeSimpleInput
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000)]
        public decimal SalarioPorHora { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Compania { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Sucursal { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Departamento { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CentroDeCosto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Proyecto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Fase { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Actividad { get; set; }

        [Required]
        public DateTime HoraEntrada { get; set; }

        [Required]
        public DateTime HoraSalida { get; set; }

        public string? TipoDeDia { get; set; }

        public string? TipoDeHorario { get; set; }

        [Range(0, 480)]
        public int? TiempoComida { get; set; }
    }
}
