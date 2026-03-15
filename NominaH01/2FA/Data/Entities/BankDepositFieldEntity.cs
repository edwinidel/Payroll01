using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("BankDepositFields")]
    public class BankDepositFieldEntity : BaseEntity
    {
        private const string Requ = "El campo {0} es requerido";
        private const string V = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Estructura de Depósito Bancario")]
        [Required(ErrorMessage = Requ)]
        public int BankDepositStructureId { get; set; }

        [Display(Name = "Compañía")]
        [Required(ErrorMessage = Requ)]
        public int CompanyId { get; set; }

        [Display(Name = "Nombre del Campo")]
        [Required(ErrorMessage = Requ)]
        [StringLength(100, ErrorMessage = V)]
        public string FieldName { get; set; } = string.Empty;

        [Display(Name = "Contenido del Campo")]
        [Required(ErrorMessage = Requ)]
        [StringLength(200, ErrorMessage = V)]
        public string FieldContent { get; set; } = string.Empty;

        [Display(Name = "Orden")]
        [Required(ErrorMessage = Requ)]
        public int Order { get; set; }

        [Display(Name = "Ignorar")]
        public bool Ignore { get; set; }

        public BankDepositStructureEntity? BankDepositStructure { get; set; }
        public CompanyEntity? Company { get; set; }
    }
}