using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("DestajoDocuments")]
    public class DestajoDocumentEntity : BaseEntity
    {
        private const string StrLength = "El Campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Compañía")]
        [Required]
        public int CompanyId { get; set; }

        [Display(Name = "Fecha del Documento")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime DocumentDate { get; set; }

        [Display(Name = "Referencia")]
        [StringLength(50, ErrorMessage = StrLength)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Display(Name = "Responsable Cliente")]
        [StringLength(100, ErrorMessage = StrLength)]
        public string ClientResponsible { get; set; } = string.Empty;

        [Display(Name = "Responsable Empresa")]
        [StringLength(100, ErrorMessage = StrLength)]
        public string CompanyResponsible { get; set; } = string.Empty;

        [Display(Name = "Ruta de Archivo")]
        [StringLength(200, ErrorMessage = StrLength)]
        public string? DocumentPath { get; set; } = string.Empty;

        [Display(Name = "Notas")]
        [StringLength(500, ErrorMessage = StrLength)]
        public string Notes { get; set; } = string.Empty;

        public CompanyEntity? Company { get; set; }
        public ICollection<DestajoProductionEntity> Productions { get; set; } = new List<DestajoProductionEntity>();
    }
}
