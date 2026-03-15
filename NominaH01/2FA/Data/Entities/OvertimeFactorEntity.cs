using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Data.Entities
{
    [Table("OverTimeFactors")]
    public class OverTimeFactorEntity : BaseEntity
    {
        private const string V = "El campo {0} no puede exceder los {1} caracteres";
        private const string V1 = "El campo {0} es requerido";

        [Display(Name = "Código")]
        [StringLength(100, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Factor")]
        public decimal Factor { get; set; }

        [Display(Name = "Activo?")]
        public bool IsActive { get; set; }

        [Display(Name = "Formula")]
        [StringLength(200, ErrorMessage = V)]
        public string? Formula { get; set; }
        
        [Display(Name = "Identificación")]
        [StringLength(200, ErrorMessage = V)]
        public string? Identify { get; set; }

        public ICollection<PayrollTmpConceptEntity>? PayrollTmpConcepts { get; set; }
    }
}
