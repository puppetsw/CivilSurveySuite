using System.Globalization;
using System.Windows.Controls;

namespace CivilSurveySuite.UI.Validation
{
    public class NumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool canConvert = double.TryParse(value as string, out double _);
            return new ValidationResult(canConvert, "Not a valid double");
        }
    }
}