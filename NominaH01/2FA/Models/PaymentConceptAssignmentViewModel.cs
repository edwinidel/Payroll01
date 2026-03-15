using _2FA.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _2FA.Models
{
    public class PaymentConceptAssignmentViewModel
    {
        public int PaymentConceptId { get; set; }
        public string PaymentConceptName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
        public bool RegularHours { get; set; }
        public bool ExtraHours { get; set; }
        public decimal PayFactor { get; set; }
        public bool RecurrentPayment { get; set; }
        public CountryEntity? Country { get; set; }

        // Para el select
        public List<SelectListItem> AvailableDeductions { get; set; } = new();

        // Lista actual asignada
        public List<LegalDeductionAssigned> AssignedDeductions { get; set; } = new();
    }

    
    public class LegalDeductionAssigned
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal EmployeeDiscount { get; set; }
        public decimal EmployerDiscount { get; set; }

    }
}
