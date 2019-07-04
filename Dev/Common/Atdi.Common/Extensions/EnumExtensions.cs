using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Common
{
    public static class EnumExtensions
    {
        public static TEnum ToEnumValue<TEnum>(this string value)
            where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }

        public static TEnum CopyTo<TEnum>(this Enum value)
            where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value.ToString());
        }
    }
}
