using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp.Validations
{
    public class NumberInfoValidationRules : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int minValue = 1;
            int maxValue = 65535;
            //если числовое значение введено
            if (int.TryParse(value.ToString(), out int intValue))
            {
                if (!((intValue >= minValue) && (maxValue >= intValue)))
                { 
                    return new ValidationResult(false, "Числовое значение не входит в разрешенный диапазон");
                }
                else
                    return ValidationResult.ValidResult;
            }
            else if (value.ToString() == "")
            {
                return new ValidationResult(false, "Пустое поле");
            }
            return new ValidationResult(false, "Некорректное значение");
        }
    }

}
