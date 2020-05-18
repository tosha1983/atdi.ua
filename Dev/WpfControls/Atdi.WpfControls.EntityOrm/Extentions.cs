using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WpfControls.EntityOrm
{
    public static class Extentions
    {
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
    }
}
