using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Defines the extension methods to the QueryResult class. This is a static class.
    /// </summary>
    public static class QueryResultExtension
    {
        /// <summary>
        /// The method of obtaining a cell value
        /// </summary>
        /// <param name="column">The column index. Begins with 0</param>
        /// <param name="rowIndex">The row index. Begins with 0</param>
        /// <returns>The value of a cell</returns>
        public static object GetValue(this QueryResult data, uint column, uint rowIndex)
        {
            var type = data.ColumnTypeMap[column];
            var indexOfType = data.ColumnIndexMap[column];

            switch (type)
            {
                case CommonDataType.String:
                    return data.StringValues[indexOfType][rowIndex];
                case CommonDataType.Boolean:
                    return data.BooleanValues[indexOfType][rowIndex];
                case CommonDataType.Integer:
                    return data.IntegerValues[indexOfType][rowIndex];
                case CommonDataType.DateTime:
                    return data.DateTimeValues[indexOfType][rowIndex];
                case CommonDataType.Double:
                    return data.DoubleValues[indexOfType][rowIndex];
                case CommonDataType.Bytes:
                    return data.BytesValues[indexOfType][rowIndex];
                default:
                    throw new InvalidOperationException("Unsupported column type " + type.ToString());
            }
        }

        /// <summary>
        /// The method of obtaining a cell value as string
        /// </summary>
        /// <param name="column">The column index. Begins with 0</param>
        /// <param name="rowIndex">The row index. Begins with 0</param>
        /// <returns>The value of a cell</returns>
        public static string GetValueAsString(this QueryResult data, uint column, uint rowIndex)
        {
            var indexOfType = data.ColumnIndexMap[column];
            return data.StringValues[indexOfType][rowIndex];
        }

        /// <summary>
        /// The method of obtaining a cell value as boolean
        /// </summary>
        /// <param name="column">The column index. Begins with 0</param>
        /// <param name="rowIndex">The row index. Begins with 0</param>
        /// <returns>The value of a cell</returns>
        public static bool? GetValueAsBoolean(this QueryResult data, uint column, uint rowIndex)
        {
            var indexOfType = data.ColumnIndexMap[column];
            return data.BooleanValues[indexOfType][rowIndex];
        }

        /// <summary>
        /// The method of obtaining a cell value as integer
        /// </summary>
        /// <param name="column">The column index. Begins with 0</param>
        /// <param name="rowIndex">The row index. Begins with 0</param>
        /// <returns>The value of a cell</returns>
        public static int? GetValueAsInteger(this QueryResult data, uint column, uint rowIndex)
        {
            var indexOfType = data.ColumnIndexMap[column];
            return data.IntegerValues[indexOfType][rowIndex];
        }

        /// <summary>
        /// The method of obtaining a cell value as datetime
        /// </summary>
        /// <param name="column">The column index. Begins with 0</param>
        /// <param name="rowIndex">The row index. Begins with 0</param>
        /// <returns>The value of a cell</returns>
        public static DateTime? GetValueAsDateTime(this QueryResult data, uint column, uint rowIndex)
        {
            var indexOfType = data.ColumnIndexMap[column];
            return data.DateTimeValues[indexOfType][rowIndex];
        }


        /// <summary>
        /// The method of obtaining a cell value as double
        /// </summary>
        /// <param name="column">The column index. Begins with 0</param>
        /// <param name="rowIndex">The row index. Begins with 0</param>
        /// <returns>The value of a cell</returns>
        public static double? GetValueAsDouble(this QueryResult data, uint column, uint rowIndex)
        {
            var indexOfType = data.ColumnIndexMap[column];
            return data.DoubleValues[indexOfType][rowIndex];
        }

        /// <summary>
        /// The method of obtaining a cell value as bytes
        /// </summary>
        /// <param name="column">The column index. Begins with 0</param>
        /// <param name="rowIndex">The row index. Begins with 0</param>
        /// <returns>The value of a cell</returns>
        public static byte[] GetValueAsBytes(this QueryResult data, uint column, uint rowIndex)
        {
            var indexOfType = data.ColumnIndexMap[column];
            return data.BytesValues[indexOfType][rowIndex];
        }
    }
}
