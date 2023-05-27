// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

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