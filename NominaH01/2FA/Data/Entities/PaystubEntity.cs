using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2FA.Data.Entities
{
    public class PaystubEntity : BaseEntity
    {
        [Required]
        public int EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; } = default!;

        [Required]
        public int PayrollHeaderId { get; set; }
        public PayrollHeaderEntity PayrollHeader { get; set; } = default!;

        [Required]
        public DateTime PeriodStart { get; set; }

        [Required]
        public DateTime PeriodEnd { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Gross { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Net { get; set; }

        [Required]
        [StringLength(512)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(128)]
        public string Hash { get; set; } = string.Empty;

        public bool IsLatest { get; set; }
    }
}
