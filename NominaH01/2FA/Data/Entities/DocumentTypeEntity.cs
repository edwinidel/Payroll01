using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class DocumentTypeEntity : BaseEntity
    {
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? Description { get; set; }

        // Optional stored signature path under wwwroot/uploads/signatures/
        [Display(Name = "Firma")]
        public string? SignaturePath { get; set; }

        [Display(Name = "Nombre del firmante")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? SignerName { get; set; }

        [Display(Name = "Cargo del firmante")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? SignerTitle { get; set; }
    }
}
