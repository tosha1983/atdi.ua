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
        public static int? TryToInt(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return null;
        }
        public static double? TryToDouble(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return null;
        }

        public static string Wrap(this string text, int margin, string separator)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            int start = 0, end;
            var lines = new List<string>(text.Length / margin + 1);
            //text = Regex.Replace(text, @"\s", " ").Trim();

            while ((end = start + margin) < text.Length)
            {
                while (text[end] != ' ' && end > start)
                    end -= 1;

                if (end == start)
                    end = start + margin;

                lines.Add(text.Substring(start, end - start));
                start = end + 1;
            }

            if (start < text.Length)
                lines.Add(text.Substring(start));

            return string.Join(separator, lines.ToArray());
        }
    }
}
