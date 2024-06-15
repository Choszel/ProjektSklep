using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjektSklep
{
    public class PriceValidator : ValidationRule
    {
        public override ValidationResult Validate (object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value.ToString() == null || value.ToString() == "")
                return new ValidationResult(false, "Pole nie może być puste.");
            else
            {
                if(float.TryParse(value.ToString(), out float f))
                {
                    if(f< 0)
                    {
                        return new ValidationResult(false, "Wartość musi być dodatnia.");
                    }   
                }
                else
                {
                    return new ValidationResult(false, "Wartość musi być liczbą.");
                }
            }

            return ValidationResult.ValidResult;
        }
        public PriceValidator() { }
    }
}
