using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Atdi.Common
{
    public static class DoubleExtensions
    {
        public static double? ConvertStringToDouble(this string value)
        {
            var aStr = value.Trim(new char[] { '(', ')', ' ' });
            double resValue = 0;
            var format = new System.Globalization.NumberFormatInfo();
            if (!double.TryParse(aStr, NumberStyles.Float, format, out resValue))
            {
                if (format.NumberDecimalSeparator == ",")
                    format.NumberDecimalSeparator = ".";
                else if (format.NumberDecimalSeparator == ".")
                    format.NumberDecimalSeparator = ",";
                if (!Double.TryParse(aStr, NumberStyles.Float, format, out resValue))
                {
                    return null;
                }
            }
            return resValue;
        }
    }
}
