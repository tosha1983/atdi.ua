using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Common
{
    public static class DateTimeExtensions
    {
        public static string ConvertToISO8601DateTimeString(this DateTime value)
        {
            return value.ToString("o");
            /*
            var result = new StringBuilder();
                result.Append(value.Year.ToString("0000"));
                result.Append("-");
                result.Append(value.Month.ToString("00"));
                result.Append("-");
                result.Append(value.Day.ToString("00"));
                result.Append("T");
                result.Append(value.Hour.ToString("00"));
                result.Append(":");
                result.Append(value.Minute.ToString("00"));
                result.Append(":");
                result.Append(value.Second.ToString("00"));
                result.Append("Z");
            return result.ToString();
            */
        }
    }
}
