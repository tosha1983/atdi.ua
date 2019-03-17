using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Common
{
    public static class StringExtensions
    {
        public static DateTime ConvertISO8601ToDateTime(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new System.ArgumentNullException("value");
            }
            return DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
            //return new System.DateTime(System.Convert.ToInt32(value.Substring(0, 4)), System.Convert.ToInt32(value.Substring(5, 2)), System.Convert.ToInt32(value.Substring(8, 2)), System.Convert.ToInt32(value.Substring(11, 2)), System.Convert.ToInt32(value.Substring(14, 2)), System.Convert.ToInt32(value.Substring(17, 2)), new System.Globalization.GregorianCalendar());
        }

        public static string With(this string format, params object[] args)
        {
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }

        public static string With(this string format, IFormatProvider formatProvider, params object[] args)
        {
            try
            {
                return string.Format(formatProvider, format, args);
            }
            catch
            {
                return format;
            }
        }
        public static string SubString(this string value, int length)
        {
            if (!string.IsNullOrEmpty(value) && length > 0 && value.Length > length)
            {
                return value.Substring(0, length);
            }
            return value;
        }
    }
}
