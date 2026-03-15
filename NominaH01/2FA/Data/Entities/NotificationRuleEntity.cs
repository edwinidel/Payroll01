using System.ComponentModel.DataAnnotations;

namespace _2FA.Data.Entities
{
    public class NotificationRuleEntity : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        public int DaysBefore { get; set; }

        public bool Enabled { get; set; } = true;

        [StringLength(20)]
        public string Channel { get; set; } = "in-app"; // in-app | email
    }
}
