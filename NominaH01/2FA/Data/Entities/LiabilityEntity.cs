using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Data.Entities
{
    [Table("Liabitities")]
    public class LiabilityEntity : BaseEntity
    {
        private const string V = "El campo {0} no puede exceder los {1} caracteres";
        private const string V1 = "El campo {0} es requerido";

        [Display(Name = "Código")]
        [StringLength(100, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Empleado")]
        public int EmployeeId {get; set;}

        [Display(Name = "Acreedor")]
        public int CreditorId {get; set;}

        [Display(Name = "Monto Inicial")]
        public decimal InitialAmount {get; set;}

        [Display(Name = "Descuento")]
        public decimal Dicsount {get; set;}

        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime InitDate { get; set; }

        [Display(Name = "Estatus")]
        [StringLength(20, ErrorMessage = V)]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Referencia")]
        [StringLength(2000, ErrorMessage = V)]
        public string Reference { get; set; } = string.Empty;

        [Display(Name = "ID Externo")]
        [StringLength(20, ErrorMessage = V)]
        public string ExternalId { get; set; } = string.Empty;

        [Display(Name = "% Máximo")]
        public decimal MaxPercentage { get; set; }

        public EmployeeEntity? Employee { get; set; }
        public CreditorEntity? Creditor {get; set;}

    }
}
