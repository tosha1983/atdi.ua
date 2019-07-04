using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Common
{
    public static class TypeExtensions
    {
        public static PropertyInfo[] GetPropertiesWithInherited(this Type type)
        {
            if (!type.IsInterface)
                return type.GetProperties();

            return (new Type[] { type })
                   .Concat(type.GetInterfaces())
                   .SelectMany(i => i.GetProperties()).ToArray();
        }
    }
}
