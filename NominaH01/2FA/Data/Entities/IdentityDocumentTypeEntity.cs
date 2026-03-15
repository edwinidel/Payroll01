using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class IdentityDocumentTypeEntity : BaseEntity
    {
        [Display(Name = "Tipo de Documento de Identidad")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public ICollection<EmployeeEntity>? Employees { get; set; }
    }
}
