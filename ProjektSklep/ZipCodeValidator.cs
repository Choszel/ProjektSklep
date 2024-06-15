using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjektSklep
{
    public class ZipCodeValidator : ValidationRule
    {
        public override ValidationResult Validate (object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value.ToString() == null || value.ToString() == "")
                return new ValidationResult(false, "Pole nie może być puste.");
            else
            {
                if (!Regex.IsMatch(value.ToString(), @"^\d{2}-\d{3}$"))
                {
                    return new ValidationResult(false, "Pole musi być podane w formacie 00-000");
                }
            }

            return ValidationResult.ValidResult;
        }
        public ZipCodeValidator() { }
    }
}
