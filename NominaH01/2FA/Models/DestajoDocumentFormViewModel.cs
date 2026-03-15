using System.ComponentModel.DataAnnotations;
using _2FA.Data.Entities;

namespace _2FA.Models
{
    public class DestajoDocumentFormViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Compañía")]
        [Required]
        public int CompanyId { get; set; }

        [Display(Name = "Fecha del Documento")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime DocumentDate { get; set; }

        [Display(Name = "Referencia")]
        [StringLength(50)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Display(Name = "Responsable Cliente")]
        [StringLength(100)]
        public string ClientResponsible { get; set; } = string.Empty;

        [Display(Name = "Responsable Empresa")]
        [StringLength(100)]
        public string CompanyResponsible { get; set; } = string.Empty;

        [Display(Name = "Ruta de Archivo")]
        [StringLength(200)]
        public string? DocumentPath { get; set; } = string.Empty;

        [Display(Name = "Notas")]
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        [Display(Name = "Unidades (todas las líneas)")]
        [Range(0, double.MaxValue)]
        public decimal UnitsProduced { get; set; }

        public List<DestajoDocumentEmployeeLine> Lines { get; set; } = new();
    }

    public class DestajoDocumentEmployeeLine
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string DocumentId { get; set; } = string.Empty;
        public bool Selected { get; set; }

        [Display(Name = "Unidades"), Range(0, double.MaxValue)]
        public decimal UnitsProduced { get; set; }

        [Display(Name = "Valor Unitario"), Range(0, double.MaxValue)]
        public decimal UnitValue { get; set; }
    }
}
