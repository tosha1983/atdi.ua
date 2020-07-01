using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;

namespace Atdi.Icsm.Plugins.Core
{
    public static class OthesExtentions
    {
        public static int TryToInt(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return IM.NullI;
            }
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return IM.NullI;
        }
        public static double? ToNull(this double value)
        {
            if (value == IM.NullD)
            {
                return null;
            }

            return value;
        }
        public static double? ToNull(this double? value)
        {
            if (value == null || value == IM.NullD)
            {
                return null;
            }

            return value;
        }
        public static int? ToNull(this int value)
        {
            if (value == IM.NullI)
            {
                return null;
            }

            return value;
        }
        public static int? ToNull(this int? value)
        {
            if (value == null || value == IM.NullI)
            {
                return null;
            }

            return value;
        }
        public static long? ToNull(this long value)
        {
            if (value == IM.NullI)
            {
                return null;
            }

            return value;
        }
        public static long? ToNull(this long? value)
        {
            if (value == null || value == IM.NullI)
            {
                return null;
            }

            return value;
        }
        public static DateTime? ToNull(this DateTime value)
        {
            if (value == IM.NullT)
            {
                return null;
            }

            return value;
        }
        public static DateTime? ToNull(this DateTime? value)
        {
            if (value == null || value == IM.NullT)
            {
                return null;
            }

            return value;
        }
        public static bool LessOrEqual(this double value, double target)
        {
            if (value <= target)
            {
                return true;
            }

            var delate = Math.Abs(value - target);

            return delate <= 0.0000001;
        }
        public static bool ExistsItems<T>(this List<T> list)
        {
            return list != null && list.Count > 0;
        }

        public static bool IsNull(this DateTime value)
        {
            return value == IM.NullT;
        }
    }
}
