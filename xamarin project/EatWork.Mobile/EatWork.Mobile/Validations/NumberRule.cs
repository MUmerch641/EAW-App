using System;
using System.Collections.Generic;
using System.Text;

namespace EatWork.Mobile.Validations
{
    public class NumberRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            /*
            var str = value.ToString();
            var val = string.Empty;
            decimal newVal;

            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsDigit(str[i]))
                    val += str[i];
            }

            */
            var newVal = decimal.Parse(value.ToString());

            var isValid = (newVal > 0);

            return isValid;
        }
    }
}
