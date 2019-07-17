using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;

namespace Atdi.WebApiServices.EntityOrm.Helpers
{
    class FieldParser
    {
        // "field1 desc, @field asc"
        public static OrderExpression[] ParseOrderBy(string[] orderBy)
        {
            var expressions = new List<OrderExpression>();

            for (int i = 0; i < orderBy.Length; i++)
            {
                var expressionsRange = ParseOrderBy(orderBy[i]);
                if (expressionsRange.Length == 0)
                {
                    throw new InvalidOperationException($"Invalid fields sorting '{orderBy[i]}'");
                }
                expressions.AddRange(expressionsRange);
            }

            return expressions.ToArray();
        }
        public static OrderExpression[] ParseOrderBy(string orderBy)
        {
            var expressions = new List<OrderExpression>();
            var pathToken = new StringBuilder();
            var directionToken = new StringBuilder();
            var isPath = true;
            for (int i = 0; i < orderBy.Length; i++)
            {
                var c = orderBy[i];
                if (c == ',' && pathToken.Length > 0)
                {
                    var expression = BuildOrderExpression(pathToken, directionToken);
                    expressions.Add(expression);
                    pathToken = new StringBuilder();
                    directionToken = new StringBuilder();
                    isPath = true;
                }
                else if (c == '@' && pathToken.Length == 0)
                {
                    // просто игнорим - это признак начало поля
                }
                else if (c == ' ' && isPath && pathToken.Length > 0)
                {
                    isPath = false;
                }
                else if (c == ' ' || char.IsWhiteSpace(c))
                {
                    // игнорируем пробелы
                }
                else
                {
                    if (isPath)
                    {
                        pathToken.Append(c);
                    }
                    else
                    {
                        directionToken.Append(c);
                    }
                    
                }
            }

            expressions.Add(BuildOrderExpression(pathToken, directionToken));
            return expressions.ToArray();
        }

        private static OrderExpression BuildOrderExpression(StringBuilder pathToken, StringBuilder directionToken)
        {
            var expression = new OrderExpression { ColumnName = pathToken.ToString() };
            var type = directionToken.ToString();

            //  по умолчанию можно опустить указание сортировки от меньшего к большему
            if (directionToken.Length == 0)
            {
                expression.OrderType = OrderType.Ascending;
            }
            else if ("asc".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                expression.OrderType = OrderType.Ascending;
            }
            else if ("desc".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                expression.OrderType = OrderType.Descending;
            }
            else
            {
                throw new InvalidOperationException($"Invalid sorting direction '{type}'");
            }

            return expression;
        }
        public static string[] ParseSelect(string[] select)
        {
            var fields = new List<string>();

            for (int i = 0; i < select.Length; i++)
            {
                var fieldsRange = ParseSelect(select[i]);
                if (fieldsRange.Length == 0)
                {
                    throw new InvalidOperationException($"Invalid fields selection '{select[i]}'");
                }
                fields.AddRange(fieldsRange);
            }

            return fields.ToArray();
        }
        public static string[] ParseSelect(string select)
        {
            var fields = new List<string>();
            var path = new StringBuilder();
            for (int i = 0; i < select.Length; i++)
            {
                var c = select[i];
                if ((c == ',' || c == ' ' )&& path.Length > 0)
                {
                    fields.Add(path.ToString());
                    path = new StringBuilder();
                }
                else if (c == '@' && path.Length == 0)
                {
                    // просто игнорим - это признак начало поля
                }
                else if ( c == ' ' || char.IsWhiteSpace(c))
                {
                    // игнорируем пробелы
                }
                else
                {
                    path.Append(c);
                }
            }
            fields.Add(path.ToString());

            return fields.ToArray();
        }
    }
}
