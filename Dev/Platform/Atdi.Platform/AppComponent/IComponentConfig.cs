using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Atdi.Platform.AppComponent
{
    public interface IComponentConfig
    {
        string Instance { get; }

        string Type { get;  }

        string Assembly { get; }

        object this[string paramName] { get; }
    }

    public static class ComponentConfigExtensions
    {
        public static TConfig Extract<TConfig>(this IComponentConfig config)
            where TConfig : class, new()
        {
            var result = new TConfig();
            var type = typeof(TConfig);
            var properies = type.GetProperties(BindingFlags.Public);
            for (int i = 0; i < properies.Length; i++)
            {
                var propertyInfo = properies[i];
                
            }
            return result;
        }
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

        public static string GetParameterAsDecodeString(this IComponentConfig config, string paramName, string sharedSecret)
        {
            var encodeValue = config.GetParameterAsString(paramName);
            if (string.IsNullOrEmpty(encodeValue))
            {
                return string.Empty;
            }

            return Cryptography.Encryptor.DecryptStringAES(encodeValue, sharedSecret);

        }

        public static bool GetParameterAsBoolean(this IComponentConfig config, string paramName)
        {
            if (config == null)
            {
                return false;
            }

            var realValue = config[paramName];
            if (realValue == null)
            {
                return false;
            }

            var stringValue = Convert.ToString(realValue);
            if (stringValue == null)
            {
                return false;
            }

            return "true".Equals(stringValue, StringComparison.OrdinalIgnoreCase);
        }

        public static int? GetParameterAsInteger(this IComponentConfig config, string paramName)
        {
            if (config == null)
            {
                return null;
            }

            var realValue = config[paramName];
            if (realValue == null)
            {
                return null;
            }

            var stringValue = Convert.ToString(realValue);
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }

            if (int.TryParse(stringValue, out int result))
            {
                return result;
            }

            return null;
        }
    }
}
