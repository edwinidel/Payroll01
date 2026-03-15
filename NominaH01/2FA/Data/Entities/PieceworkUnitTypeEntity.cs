using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class PieceworkUnitTypeEntity : BaseEntity
    {
        private const string StrLength = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        [Display(Name = "Unidad de Destajo")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(50, ErrorMessage = StrLength)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = StrLength)]
        public string? Description { get; set; }

        public CompanyEntity? Company { get; set; }
    }
}
