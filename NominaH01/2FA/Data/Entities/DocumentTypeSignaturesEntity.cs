using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    public class DocumentTypeSignaturesEntity : BaseEntity
    {
        [Display(Name = "Tipo de Documento")]
        public int DocumentTypeId { get; set; }

        [ForeignKey("DocumentTypeId")]
        public virtual DocumentTypeEntity DocumentType { get; set; } = null!;

        [Display(Name = "Nombre del Firmante")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string SignerName { get; set; } = string.Empty;

        [Display(Name = "Cargo del Firmante")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? SignerTitle { get; set; }

        [Display(Name = "Firma Digital")]
        public byte[] SignatureData { get; set; } = Array.Empty<byte>();

        [Display(Name = "Tipo de Archivo")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string ContentType { get; set; } = string.Empty;

        [Display(Name = "Nombre del Archivo")]
        [StringLength(255, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string FileName { get; set; } = string.Empty;

        [Display(Name = "Hash de Seguridad")]
        [StringLength(128, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? SecurityHash { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Orden de Firma")]
        public int SignatureOrder { get; set; } = 1;
    }
}