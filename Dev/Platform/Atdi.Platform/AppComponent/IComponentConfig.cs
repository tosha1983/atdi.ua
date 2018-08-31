using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.AppComponent
{
    public interface IComponentConfig
    {
        string Instance { get; }

        string Type { get;  }

        string Assembly { get; }

        object this[string paramName] { get; }
    }

    public static class ComponentConfigExtention
    {
        public static string GetParameterAsString(this IComponentConfig config, string paramName)
        {
            if (config == null)
            {
                return string.Empty;
            }

            var realValue = config[paramName];
            if (realValue == null)
            {
                return string.Empty;
            }

            var stringValue = Convert.ToString(realValue);
            if (stringValue == null)
            {
                return string.Empty;
            }

            return stringValue;
        }
    }
}
