using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    [Table("PayrollTmpEmployees")]
    public class PayrollTmpEmployeeEntity : BaseEntity
    {
        private const string Requ = "El campo {0} es requerido";

        [Display(Name = "No. de Planilla")]
        [Required(ErrorMessage = Requ)]
        public int PayrollTmpHeaderId { get; set; }

        [Display(Name = "Horas Regulares")]
        public decimal RegularHours { get; set; }

        [Display(Name = "Sal. por Hora")]
        public decimal HourlySalary { get; set; }

        [Display(Name = "Empleado")]
        [Required(ErrorMessage = Requ)]
        public int EmployeeId { get; set; }

        [Display(Name = "Salario Pactado")]
        public decimal AgreeSalary { get; set; }

        [Display(Name = "Salario Regular")]
        public decimal RegularSalary { get; set; }

        [Display(Name = "Sobretiempo")]
        public decimal OverTimeSalary { get; set; }

        [Display(Name = "Otros Pagos")]
        public decimal OtherPayment { get; set; }

        [Display(Name = "Salario Bruto")]
        [DataType(DataType.Currency)]
        public decimal TotalEarnings { get; set; }

        [Display(Name = "Deducciones Legales")]
        public decimal TotalLegalDiscount { get; set; }

        [Display(Name = "Deducciones Legales")]
        public decimal TotalOtherDiscount { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalDeductions { get; set; }

        [DataType(DataType.Currency)]
        public decimal NetPay { get; set; }

        // Destajo fields
        [Display(Name = "Unidades Producidas")]
        public decimal UnitsProduced { get; set; }

        [Display(Name = "Valor por Unidad")]
        [DataType(DataType.Currency)]
        public decimal UnitValue { get; set; }

        [Display(Name = "Monto por Destajo")]
        [DataType(DataType.Currency)]
        public decimal DestajoAmount { get; set; }

        public EmployeeEntity? Employee { get; set; }
        public PayrollTmpHeaderEntity? PayrollTmpHeader { get; set; } 
        public ICollection<PayrollTmpConceptEntity> PayrollTmpConcepts { get; set; } = [];
        public ICollection<PayrollTmpLegalDeductionEntity> PayrollTmpLegalDeductions { get; set; } = [];
        public ICollection<HistoricTmpLiabilityEntity> HistoricTmpLiabilities { get; set; } = [];
        public ICollection<PayrollTmpOvertimeEntity> PayrollTmpOvertime { get; set; } = [];
        
    }
}