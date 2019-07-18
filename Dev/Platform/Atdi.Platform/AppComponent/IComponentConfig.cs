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

        IComponentConfigParameter[] Parameters { get; }
    }

    public interface IComponentConfigParameter
    {
        string Name { get; }
        string Value { get; }
    }

    public static class ComponentConfigExtensions
    {
        public static TConfig Extract<TConfig>(this IComponentConfig config)
            where TConfig : class, new()
        {
            var result = new TConfig();
            var type = typeof(TConfig);
            var properies = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < properies.Length; i++)
            {
                var propertyInfo = properies[i];

                if (!propertyInfo.CanWrite)
                {
                    continue;
                }

                var propertyName = propertyInfo.Name;

                var propertyAttribute = propertyInfo.GetCustomAttribute<ComponentConfigPropertyAttribute>();
                if (propertyAttribute != null)
                {
                    propertyName = propertyAttribute.Name;
                }

                if (propertyInfo.PropertyType == typeof(string) && propertyAttribute != null && !string.IsNullOrEmpty(propertyAttribute.SharedSecret))
                {
                    propertyInfo.SetValue(result, config.GetParameterAsDecodeString(propertyName, propertyAttribute.SharedSecret));
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(result, config.GetParameterAsString(propertyName));
                }
                else if (propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
                {
                    propertyInfo.SetValue(result, config.GetParameterAsBoolean(propertyName));
                }
                else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                {
                    propertyInfo.SetValue(result, config.GetParameterAsInteger(propertyName));
                }
                else if (propertyInfo.PropertyType == typeof(float) || propertyInfo.PropertyType == typeof(float?))
                {
                    propertyInfo.SetValue(result, config.GetParameterAsFloat(propertyName));
                }
                else if (propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(double?))
                {
                    propertyInfo.SetValue(result, config.GetParameterAsDouble(propertyName));
                }
                else if (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?))
                {
                    propertyInfo.SetValue(result, config.GetParameterAsDecimal(propertyName));
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported config property type: {propertyInfo.PropertyType}");
                }
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

            throw new InvalidOperationException($"Incorrect parameter value as int: Name = '{paramName}', Value = '{stringValue}'");
        }

        public static float? GetParameterAsFloat(this IComponentConfig config, string paramName)
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

            if (float.TryParse(stringValue, out float result))
            {
                return result;
            }

            throw new InvalidOperationException($"Incorrect parameter value as float: Name = '{paramName}', Value = '{stringValue}'");
        }

        public static double? GetParameterAsDouble(this IComponentConfig config, string paramName)
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

            if (double.TryParse(stringValue, out double result))
            {
                return result;
            }

            throw new InvalidOperationException($"Incorrect parameter value as double: Name = '{paramName}', Value = '{stringValue}'");
        }

        public static decimal? GetParameterAsDecimal(this IComponentConfig config, string paramName)
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

            if (decimal.TryParse(stringValue, out decimal result))
            {
                return result;
            }

            throw new InvalidOperationException($"Incorrect parameter value as decimal: Name = '{paramName}', Value = '{stringValue}'");
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ComponentConfigPropertyAttribute : Attribute
    {
        private readonly string _name;

        public ComponentConfigPropertyAttribute(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get => this._name;
        }

        public string SharedSecret { get; set; }
    }
}
