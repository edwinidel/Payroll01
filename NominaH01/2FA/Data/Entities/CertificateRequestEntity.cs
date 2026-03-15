using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class CertificateRequestEntity : BaseEntity
    {
        [Required]
        public int EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; } = default!;

        [Required]
        [StringLength(30)]
        public string Type { get; set; } = string.Empty; // e.g. work | income

        public int SalaryHistoryMonths { get; set; }

        [Required]
        [StringLength(512)]
        public string FilePath { get; set; } = string.Empty;
    }
}
