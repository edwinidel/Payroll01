using System.ComponentModel.DataAnnotations;

namespace OvertimeCalculator.Validators
{
    /// <summary>
    /// Validates that a time string is in the format HH:mm and within valid range
    /// </summary>
    public class ValidTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success; // Allow null

            if (value is not string timeStr)
                return new ValidationResult("Time must be a string");

            if (!TimeSpan.TryParse(timeStr, out var time))
                return new ValidationResult($"The time '{timeStr}' is not in valid format HH:mm");

            if (time < TimeSpan.Zero || time >= TimeSpan.FromHours(24))
                return new ValidationResult("Time must be between 00:00 and 23:59");

            return ValidationResult.Success;
        }
    }
}
