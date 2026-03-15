using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("Banks")]
    public class BankEntity : BaseEntity
    {
        private const string StrLength = "El campo {0} no puede exceder los {1} caracteres.";

        [Display(Name = "Banco")]
        [StringLength(100, ErrorMessage = StrLength)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Ruta y Tránsito")]
        public int TransitBankId { get; set; }

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        public TransitBankEntity? TransitBank { get; set; }
        
        [InverseProperty("Bank")]
        public ICollection<EmployeeEntity>? Employees { get; set; }
        
        [InverseProperty("PayingBank")]
        public ICollection<EmployeeEntity>? PayingEmployees { get; set; }
    }
}
