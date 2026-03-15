using System.ComponentModel.DataAnnotations;

namespace OvertimeCalculator.Models
{
    public class DayFactor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string DayType { get; set; } = string.Empty;

        [Required]
        public double Factor { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
