using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Data.Entities
{
    [Table("Creditors")]
    public class CreditorEntity : BaseEntity
    {
        private const string V = "El campo {0} no puede exceder los {1} caracteres";
        private const string V1 = "El campo {0} es requerido";

        [Display(Name = "Acreedor")]
        [StringLength(100, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Código")]
        [StringLength(20, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "RUC")]
        [StringLength(20, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string RUC { get; set; } = string.Empty;

        [Display(Name = "DV")]
        [StringLength(2, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string Dv { get; set; } = string.Empty;

        [Display(Name = "Teléfono Fijo")]
        [StringLength(20, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string FixedPhone { get; set; } = string.Empty;

        [Display(Name = "Celular")]
        [StringLength(20, ErrorMessage = V)]
        [Required(ErrorMessage = V1)]
        public string CellPhone { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }
    }
}