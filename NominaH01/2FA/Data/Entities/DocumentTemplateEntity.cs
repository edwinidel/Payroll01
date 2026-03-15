using System;
using System.ComponentModel.DataAnnotations;
namespace _2FA.Data.Entities
{
    public class DocumentTemplateEntity : BaseEntity
    {
        // Inherits Id and auditing fields from BaseEntity
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Nombre")]
        [StringLength(100,ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(500,ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string Description { get; set; }

        // HTML content (Summernote)
        [Display(Name = "Contenido")]
        public string Content { get; set; } = string.Empty;
       
        // Optional stored logo and signature relative paths under wwwroot/uploads/templates/
        [Display(Name = "Logo")]
        public string? LogoPath { get; set; }

        [Display(Name = "Firma")]
        public string? SignaturePath { get; set; }

        // Optional company scoping
        [Display(Name = "Compañía")]
        public int? CompanyId { get; set; }

        // Relationship to a document type which may contain signer metadata
        [Display(Name = "Tipo de Documento")]
        public int? DocumentTypeId { get; set; }

        public DocumentTypeEntity? DocumentType { get; set; }
    }
}
