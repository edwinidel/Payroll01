using System.ComponentModel.DataAnnotations;

namespace _2FA.Models
{
    public class AccountImportViewModel
    {
        public int RowNumber { get; set; }

        [Required(ErrorMessage = "El número de cuenta es requerido")]
        [StringLength(50, ErrorMessage = "El número de cuenta no puede exceder los 50 caracteres")]
        public string AccountNumber { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres")]
        public string Description { get; set; } = string.Empty;

        public bool IsValid { get; set; } = true;

        public string ErrorMessage { get; set; } = string.Empty;
    }
}