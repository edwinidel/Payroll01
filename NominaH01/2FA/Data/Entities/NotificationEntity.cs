using System;
using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class NotificationEntity : BaseEntity
    {
        [Required]
        public int EmployeeId { get; set; }
        public EmployeeEntity Employee { get; set; } = default!;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Body { get; set; } = string.Empty;

        [StringLength(10)]
        public string Severity { get; set; } = "info"; // info | warning | critical

        public DateTime? ReadAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [StringLength(20)]
        public string Channel { get; set; } = "in-app"; // in-app | email
    }
}
