using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("DestajoProductions")]
    public class DestajoProductionEntity : BaseEntity
    {
        [Display(Name = "Documento")]
        [Required]
        public int DocumentId { get; set; }

        [Display(Name = "Empleado")]
        [Required]
        public int EmployeeId { get; set; }

        [Display(Name = "Fecha de Producción")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime ProductionDate { get; set; }

        [Display(Name = "Unidades Producidas")]
        [Range(0, double.MaxValue, ErrorMessage = "Las unidades deben ser un valor positivo")]
        [Column(TypeName = "decimal(18,3)")]
        public decimal UnitsProduced { get; set; }

        [Display(Name = "Valor por Unidad")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "El valor por unidad debe ser un valor positivo")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitValue { get; set; }

        [Display(Name = "Monto Total")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Notas")]
        [StringLength(200)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey(nameof(EmployeeId))]
        public EmployeeEntity? Employee { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public DestajoDocumentEntity? Document { get; set; }
    }
}