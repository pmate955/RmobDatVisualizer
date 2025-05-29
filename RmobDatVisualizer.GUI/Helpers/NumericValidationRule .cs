using System.Globalization;
using System.Windows.Controls;

namespace RmobDatVisualizer.GUI
{
    public class NumericValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? input = value as string;
            if (!int.TryParse(input, out int res))
                return new ValidationResult(false, "Only numbers");

            if (res < 0)
                return new ValidationResult(false, "At least 1");

            return ValidationResult.ValidResult;
        }
    }
}
