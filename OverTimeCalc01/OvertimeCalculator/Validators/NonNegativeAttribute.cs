using System.ComponentModel.DataAnnotations;

namespace OvertimeCalculator.Validators
{
    /// <summary>
    /// Validates that a value is non-negative
    /// </summary>
    public class NonNegativeAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true; // Allow null

            if (value is not int intValue)
                return false;

            return intValue >= 0;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The field '{name}' must be non-negative.";
        }
    }
}
