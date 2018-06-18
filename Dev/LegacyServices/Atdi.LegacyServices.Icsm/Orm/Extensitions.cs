using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public static class IcsmOrmExtensitions
    {
        public static IFormatProvider CultureEnUs = new CultureInfo("en-US");

        public static double ParseDouble(this string s)
        {
            double result;
            if (s == null || !double.TryParse(s, NumberStyles.Float, CultureEnUs, out result))
            {
                result = 1E-99;
            }
            return result;
        }

        public static bool IsNull(this DateTime dat)
        {
            return dat.Ticks == 0L;
        }

        public static int ParseInt(this string s)
        {
            int result;
            if (s == null || !int.TryParse(s, out result))
            {
                result = 2147483647;
            }
            return result;
        }
    }
}
