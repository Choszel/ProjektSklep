using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjektSklep
{
    public class NotEmptyValidator : ValidationRule
    {
        public override ValidationResult Validate (object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value.ToString() == null || value.ToString() == "")
                return new ValidationResult(false, "Pole nie może być puste.");


            return ValidationResult.ValidResult;
        }
        public NotEmptyValidator () { }
    }
}
