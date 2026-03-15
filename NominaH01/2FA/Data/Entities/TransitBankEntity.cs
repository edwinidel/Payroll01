using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("TransitBanks")]
    public class TransitBankEntity : BaseEntity
    {
        private const string StrLength = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Banco")]
        [StringLength(100, ErrorMessage = StrLength)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Ruta y Tránsito")]
        public int TransitId { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public ICollection<BankEntity>? Banks { get; set; }
    }
}
