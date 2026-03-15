using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollTmpHeaders")]
    public class PayrollTmpHeaderEntity : BaseEntity
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

        [Display(Name = "Tipo de Planilla")]
        public int PayrollTypeId { get; set; }

        [Display(Name = "Compañía")]
        public int CompanyId { get; set; }

        [NotMapped]
        public string ShortNotes
        {
            get
            {
                if (string.IsNullOrEmpty(Notes))
                    return string.Empty;

                return Notes.Length <= 100 ? Notes : Notes[..100];
            }
        }

        public PayrollTypeEntity? PayrollType { get; set; }
        public PaymentGroupEntity? PaymentGroup { get; set; }
        public CompanyEntity? Company { get; set; }
        public ICollection<PayrollTmpEmployeeEntity> Employees { get; set; } = [];
        public ICollection<HistoricTmpLiabilityEntity> HistoricTmpLiabilities { get; set; } = [];
        public ICollection<PayrollTmpLegalDeductionEntity> PayrollTmpLegalDeductions { get; set; } = [];
        public ICollection<PayrollTmpConceptEntity> PayrollTmpConcepts { get; set; } = [];
    }
}
