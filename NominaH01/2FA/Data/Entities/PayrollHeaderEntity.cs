using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollHeaders")]
    public class PayrollHeaderEntity : BaseEntity
    {
        [Display(Name = "Grupo de Pago")]
        [Required]
        public int PaymentGroupId { get; set; }

        [Display(Name = "Inicia Hrs. Reg.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Fin de Hrs. Reg.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Inicio de Ausencias")]
        [DataType(DataType.Date)]
        public DateTime AbsensestDateStart { get; set; }

        [Display(Name = "Fin de Ausencias")]
        [DataType(DataType.Date)]
        public DateTime AbsensestDateEnd { get; set; }

        [Display(Name = "Inicio Sobretiempo")]
        [DataType(DataType.Date)]
        public DateTime ExtraTimeDateStart { get; set; }

        [Display(Name = "Fin Sobretiempo")]
        [DataType(DataType.Date)]
        public DateTime ExtraTimeDateEnd { get; set; }

        [Display(Name = "Estatus")]
        [StringLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Processed, Approved

        [Display(Name = "Notas")]
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        [Display(Name = "Es Nómina por Destajo")]
        public bool IsDestajo { get; set; } = false;

        [Display(Name = "Nombre de la Unidad")]
        [StringLength(50)]
        public string? UnitName { get; set; }

        [Display(Name = "Valor por Unidad")]
        [DataType(DataType.Currency)]
        public decimal? UnitValue { get; set; }

        public PaymentGroupEntity? PaymentGroup { get; set; }
        public CompanyEntity? Company { get; set; }
        public ICollection<PayrollTmpEmployeeEntity> PayrollTmpEmployees { get; set; } = [];

    }
}
