using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("OvertimeCodes")]
    public class OvertimeCodeEntity : BaseEntity
    {
        [Required]
        [Display(Name = "Código de Horas Extra")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Descripción")]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Factor de Pago")]
        [Range(0.0, 10.0)]
        public decimal PayFactor { get; set; } = 1.0m;

        [Display(Name = "Combinación")]
        [StringLength(200)]
        public string Combination { get; set; } = string.Empty;

        [Display(Name = "Identificación")]
        [StringLength(200)]
        public string Identification { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
    }
}