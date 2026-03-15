using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("BankDepositStructures")]
    public class BankDepositStructureEntity : BaseEntity
    {
        private const string Requ = "El campo {0} es requerido";

        [Display(Name = "Nombre de Estructura")]
        [Required(ErrorMessage = Requ)]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string StructureName { get; set; } = string.Empty;

        [Display(Name = "Tipo de Depósito Bancario")]
        [Required(ErrorMessage = Requ)]
        public BankDepositType BankDepositType { get; set; }

        [Display(Name = "Tipo de Archivo")]
        [Required(ErrorMessage = Requ)]
        public FileType FileType { get; set; }

        public ICollection<BankDepositFieldEntity> Fields { get; set; } = [];
    }
}