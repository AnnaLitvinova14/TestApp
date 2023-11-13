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
    public class IPInfoValidationRules : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            //если введено строковое значение
            if (IPAddress.TryParse(value.ToString(), out IPAddress IPAddr))
            {
                return ValidationResult.ValidResult;
            }
            else if (value.ToString() == "")
            {
                return new ValidationResult(false, "Пустое поле");
            }
            return new ValidationResult(false, "Некорректное значение IP-адреса");
        }
    }
}
