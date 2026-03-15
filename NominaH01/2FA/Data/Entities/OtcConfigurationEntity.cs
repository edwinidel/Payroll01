using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("OtcConfigurations")]
    public class OtcConfigurationEntity : BaseEntity
    {
        private const string RequiredMessage = "El campo {0} es requerido";
        private const string MaxLengthMessage = "El campo {0} no puede exceder los {1} caracteres";

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(150, ErrorMessage = MaxLengthMessage)]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(50, ErrorMessage = MaxLengthMessage)]
        [Display(Name = "Versión")]
        public string Version { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(500, ErrorMessage = MaxLengthMessage)]
        [RegularExpression(@"^https?://.+", ErrorMessage = "El campo {0} debe iniciar con http:// o https://")]
        [Display(Name = "Endpoint de registro")]
        public string RegisterEndpoint { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(500, ErrorMessage = MaxLengthMessage)]
        [RegularExpression(@"^https?://.+", ErrorMessage = "El campo {0} debe iniciar con http:// o https://")]
        [Display(Name = "Endpoint de inicio de sesión")]
        public string LoginEndpoint { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(500, ErrorMessage = MaxLengthMessage)]
        [RegularExpression(@"^https?://.+", ErrorMessage = "El campo {0} debe iniciar con http:// o https://")]
        [Display(Name = "Endpoint de cálculo")]
        public string CalculateEndpoint { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(150, ErrorMessage = MaxLengthMessage)]
        [Display(Name = "Usuario")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(150, ErrorMessage = MaxLengthMessage)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        public string ValidationToken { get; set; } = string.Empty;

        [NotMapped]
        public string ValidationSessionKey { get; set; } = string.Empty;
    }
}
