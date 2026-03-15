using System.ComponentModel.DataAnnotations;

namespace _2FA.Models
{
    public class BusinessGroupLimitsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Grupo de Negocio")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Máximo Empresas")]
        [Range(0, int.MaxValue, ErrorMessage = "El valor debe ser mayor o igual a 0.")]
        public int? MaxCompanies { get; set; }

        [Display(Name = "Máximo Empleados")]
        [Range(0, int.MaxValue, ErrorMessage = "El valor debe ser mayor o igual a 0.")]
        public int? MaxEmployees { get; set; }

        // Additional properties for usage tracking
        [Display(Name = "Empresas Actuales")]
        public int CurrentCompanies { get; set; }

        [Display(Name = "Empleados Actuales")]
        public int CurrentEmployees { get; set; }
    }
}