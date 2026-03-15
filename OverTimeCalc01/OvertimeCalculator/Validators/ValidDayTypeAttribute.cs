using System.ComponentModel.DataAnnotations;
using OvertimeCalculator.Data;

namespace OvertimeCalculator.Validators
{
    public class ValidDayTypeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dayType = value?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dayType))
            {
                return ValidationResult.Success; // Required validation is handled by [Required]
            }

            var context = validationContext.GetService(typeof(OvertimeDbContext)) as OvertimeDbContext;
            if (context == null)
            {
                // If context is not available, skip validation
                return ValidationResult.Success;
            }

            // Check if the day type exists in DayFactors table and is active
            var exists = context.DayFactors.Any(df => df.DayType == dayType && df.IsActive);

            if (!exists)
            {
                var validDayTypes = context.DayFactors
                    .Where(df => df.IsActive)
                    .Select(df => df.DayType)
                    .OrderBy(dt => dt)
                    .ToList();

                var validTypesString = string.Join(", ", validDayTypes);
                return new ValidationResult(
                    $"Day type '{dayType}' is not valid. Valid options are: {validTypesString}",
                    new[] { validationContext.MemberName ?? string.Empty }
                );
            }

            return ValidationResult.Success;
        }
    }
}
