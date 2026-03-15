using _2FA.Data.Entities;

namespace _2FA.Models
{
    public class ConceptFormViewModel 
    {
        public int EmployeeId { get; set; }
        public IEnumerable<PaymentConceptEntity> ConceptList { get; set; } = [];
        public IEnumerable<PayrollTmpConceptEntity> Concepts { get; set; } = new List<PayrollTmpConceptEntity>();
        public IEnumerable<PayrollTmpLegalDeductionEntity> LegalDeductions { get; set; } = new List<PayrollTmpLegalDeductionEntity>();
        public IEnumerable<HistoricTmpLiabilityEntity> Liabilities { get; set; } = new List<HistoricTmpLiabilityEntity>();

    }
}
