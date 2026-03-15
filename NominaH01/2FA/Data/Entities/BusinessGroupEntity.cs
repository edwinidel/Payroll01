using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class BusinessGroupEntity : BaseEntity
    {
        private const string StrLength = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Grupo de Negocio")]
        [Required(ErrorMessage = "El campo {0} es requerido, por favor revise")]
        [StringLength(100, ErrorMessage = StrLength)]
        public string Name { get; set; } = string.Empty;

        [Display(Name ="Descripción")]
        [StringLength(1000,ErrorMessage = StrLength)]
        public string Description { get; set; } = string.Empty ;

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        [Display(Name = "Máximo Empresas")]
        public int? MaxCompanies { get; set; }

        [Display(Name = "Máximo Empleados")]
        public int? MaxEmployees { get; set; }

        public ICollection<CompanyEntity>? Companies { get; set; }
    }
}
