using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebApiServices.EntityOrm.Helpers
{
    class ValueParser
    {
        private static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-us");
        /// <summary>
        /// Парсим строку и автоматически определяем тип значения следуя следующему соглашению
        /// bool - true|false
        /// целое число (byte, short, int, long в зависимости от значения без буквы в конце)
        ///   примеры 123,  1B, 1S, 1I, 1L, 
        /// число с точокой в конце числа буква N - decimel, F - float, D - double, без буквы decimal, затем 
        /// Guid -жотский размре, значение заключены в фигурные скобки {09972164-5C36-4C84-8931-ED72465030BB} - 40 символов
        /// дата согласно формату ISO8061 - 2019-06-29T14:13:52.7778341+03
        /// строка в одинарных кавычках, одинарная кавычка внтури экранированы двумя символами
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ParseValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var firstChar = value[0];
            switch (firstChar)
            {
                case 't':
                case 'T':
                case 'f':
                case 'F':
                    return ParseBoolValue(value);
                case '{':
                    return ParseGuidValue(value);
                case '\'':
                    return ParseStringValue(value);
                case '-':
                case '+':
                case '.':
                    return ParseNumberOrDateTimeValue(value);
                default:
                    if (char.IsNumber(firstChar) || char.IsDigit(firstChar))
                    {
                        return ParseNumberOrDateTimeValue(value);
                    }
                    break;
            }

            throw new InvalidOperationException($"Unknown value format '{value}'");
        }

        public static bool ParseBoolValue(string value)
        {
            if ("true".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if ("false".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            throw new InvalidCastException($"Incorrect Boolean value '{value}'");
        }

        public static Guid ParseGuidValue(string value)
        {
            if (Guid.TryParse(value, out Guid result))
            {
                return result;
            }
            throw new InvalidCastException($"Incorrect GUID value '{value}'");
        }

        public static string ParseStringValue(string value)
        {
            if (value[value.Length - 1] == '\'')
            {
                var result = value.Substring(1, value.Length - 2);
                if (result.Length == 0)
                {
                    return result;
                }
                return result.Replace("''", "'");
            }
            throw new InvalidCastException($"Incorrect String value '{value}'");
        }

        public static object ParseNumberOrDateTimeValue(string value)
        {
            // признак даты
            // 2019-06-29T14:13:52.7778341+03
            if (value.Length >= 10 && value[4] == '-' && value[7] == '-')
            {
                if (value.Length > 27 && (value[27] == '-' || value[27] == '+'))
                {
                    if (value.Length > 30 && DateTimeOffset.TryParseExact(value, "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset dateTimeOffsetValue))
                    {
                        return dateTimeOffsetValue;
                    }
                    if (value.Length == 30 && DateTimeOffset.TryParseExact(value + ":00", "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffsetValue))
                    {
                        return dateTimeOffsetValue;
                    }

                    throw new InvalidCastException($"Incorrect Date Time Offset value '{value}'");
                }

                if (DateTime.TryParseExact(value, "O", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeValue))
                {
                    return dateTimeValue;
                }

                throw new InvalidCastException($"Incorrect Date Time value '{value}'");
            }

            // TimeSpan = 28.15:54:36.3463746
            if (value.IndexOf(':') > 0)
            {
                if (TimeSpan.TryParse(value, out TimeSpan timeSpan))
                {
                    return timeSpan;
                }

                throw new InvalidCastException($"Incorrect Time Span value '{value}'");
            }

            switch (value[value.Length - 1])
            {
                case 'I':
                case 'i':
                    if (int.TryParse(value.Substring(0, value.Length - 1), out int valInt))
                    {
                        return valInt;
                    }
                    throw new InvalidCastException($"Incorrect Integer value '{value}'");
                case 'L':
                case 'l':
                    if (long.TryParse(value.Substring(0, value.Length - 1), out long valLong))
                    {
                        return valLong;
                    }
                    throw new InvalidCastException($"Incorrect Long value '{value}'");
                case 'S':
                case 's':
                    if (short.TryParse(value.Substring(0, value.Length - 1), out short valShort))
                    {
                        return valShort;
                    }
                    throw new InvalidCastException($"Incorrect Short value '{value}'");
                case 'B':
                case 'b':
                    if (byte.TryParse(value.Substring(0, value.Length - 1), out byte valByte))
                    {
                        return valByte;
                    }
                    throw new InvalidCastException($"Incorrect Byte value '{value}'");
                case 'N':
                case 'n':
                    if (decimal.TryParse(value.Substring(0, value.Length - 1), NumberStyles.Number, DefaultCulture, out decimal valDecimal))
                    {
                        return valDecimal;
                    }
                    throw new InvalidCastException($"Incorrect Decimal value '{value}'");
                case 'F':
                case 'f':
                    if (float.TryParse(value.Substring(0, value.Length - 1), NumberStyles.Float, DefaultCulture, out float valFloat))
                    {
                        return valFloat;
                    }
                    throw new InvalidCastException($"Incorrect Float value '{value}'");
                case 'D':
                case 'd':
                    if (double.TryParse(value.Substring(0, value.Length - 1), NumberStyles.Float, DefaultCulture, out double valDouble))
                    {
                        return valDouble;
                    }
                    throw new InvalidCastException($"Incorrect Double value '{value}'");
                default:
                    break;
            }

            if (int.TryParse(value, out int valAsInt))
            {
                return valAsInt;
            }
            if (long.TryParse(value, out long valAsLong))
            {
                return valAsLong;
            }

            if (float.TryParse(value, NumberStyles.Float, DefaultCulture, out float valAsFloat))
            {
                return valAsFloat;
            }
            if (double.TryParse(value, NumberStyles.Float, DefaultCulture, out double valAsDouble))
            {
                return valAsDouble;
            }
            if (decimal.TryParse(value, NumberStyles.Number, DefaultCulture, out decimal valAsDecimal))
            {
                return valAsDecimal;
            }

            throw new InvalidCastException($"Incorrect Number value '{value}'");
        }

        public static object[] ParseValues(string values)
        {
            var result = new List<string>();
            var currentValue = new StringBuilder();
            var isString = false;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == ',' && isString == false)
                {
                    var value = currentValue.ToString();
                    result.Add(value);
                    currentValue = new StringBuilder();
                }
                else if (values[i] == '\'' && isString == false)
                {
                    isString = true;
                    currentValue.Append(values[i]);
                }
                else if (values[i] == '\'' && isString == true && (values.Length - 1 == i || (values[i + 1] != '\'')))
                {
                    isString = false;
                    currentValue.Append(values[i]);
                    //if (values.Length - 1 != i)
                    //{
                    //    currentValue.Append(values[++i]);
                    //}
                }
                else if ((values[i] == ' ' || char.IsWhiteSpace(values[i])) && isString == false)
                {
                    // подавляем символ
                }
                else
                {
                    currentValue.Append(values[i]);
                }
            }
            result.Add(currentValue.ToString());
            return result.Select(v => ParseValue(v)).ToArray();
        }


    }
}
