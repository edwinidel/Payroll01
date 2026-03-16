using System;

namespace _2FA.Models
{
    public class InitialEmployeeImportRowViewModel
    {
        public int RowNumber { get; set; }
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
        public string CodOfClock { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string IdDocument { get; set; } = string.Empty;
        public string Dv { get; set; } = string.Empty;
        public string Genere { get; set; } = string.Empty;
        public string CivilStatus { get; set; } = string.Empty;
        public string SocSecNum { get; set; } = string.Empty;
        public string IsrGroup { get; set; } = "A0";
        public string BloodType { get; set; } = "N/A";
        public string? CDIR1 { get; set; } = string.Empty;
        public string? CDIR2 { get; set; } = string.Empty;
        public string CEMAIL { get; set; } = string.Empty;
        public string? FixedPhone { get; set; } = string.Empty;
        public string? CellPhone { get; set; } = string.Empty;
        public string Status { get; set; } = "Activo";
        public string SalaryType { get; set; } = "Mensual";
        public decimal RegularHours { get; set; }
        public decimal AgreeSalary { get; set; }
        public decimal HourSalary { get; set; }
        public decimal UnitValue { get; set; }
        public string UnitType { get; set; } = string.Empty;
        public bool RetainISR { get; set; }
        public string IsrMethod { get; set; } = "P";
        public decimal IsrFixed { get; set; }
        public decimal AdditionalIsr { get; set; }
        public bool DeclareIsr { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool Unionized { get; set; }
        public string PayAccountType { get; set; } = "Ahorro";
        public string PayAccount { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? ApiOtcUsername { get; set; } = string.Empty;
        public string? ApiOtcPassword { get; set; } = string.Empty;
        public DateTime HiringDate { get; set; }
        public DateTime LiquidationDate { get; set; }
        public DateTime EndOfContract { get; set; }
        public DateTime LastSalaryIncrease { get; set; }
        public DateTime? DateOfBird { get; set; }
        public bool IsContractor { get; set; }

        public int ScheduleId { get; set; }
        public int BranchId { get; set; }
        public int DepartmentId { get; set; }
        public int CostCenterId { get; set; }
        public int SectionId { get; set; }
        public int DivisionId { get; set; }
        public int ProjectId { get; set; }
        public int PhaseId { get; set; }
        public int? ActivityId { get; set; }
        public int PositionId { get; set; }
        public int EmployeeTypeId { get; set; }
        public int OriginCountryId { get; set; }
        public int TypeOfWorkerId { get; set; }
        public int BankId { get; set; }
        public int? PayingBankId { get; set; }
        public int IdentityDocumentTypeId { get; set; }
        public int PaymentGroupId { get; set; }
        public int EmployeeObservationId { get; set; }
        public int? PieceworkUnitTypeId { get; set; }
    }
}