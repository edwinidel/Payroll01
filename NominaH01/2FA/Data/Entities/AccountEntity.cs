using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("Accounts")]
    public class AccountEntity : BaseEntity
    {
        [Display(Name = "Número de Cuenta")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string AccountNumber { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres")]
        public string? Description { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        public CompanyEntity? Company { get; set; }
    }
}