using _2FA.Data.Entities;

namespace _2FA.Models
{
    public class PaymentTmpConceptsViewModel
    {
    public IEnumerable<PayrollTmpConceptEntity> Concepts { get; set; } = [];
    public IEnumerable<PayrollTmpLegalDeductionEntity> LegalDeductions { get; set; } = [];
    public IEnumerable<HistoricTmpLiabilityEntity> Liabilities { get; set; } = [];
    public IEnumerable<PayrollTmpOvertimeEntity> Overtime { get; set; } = [];
    }
}
