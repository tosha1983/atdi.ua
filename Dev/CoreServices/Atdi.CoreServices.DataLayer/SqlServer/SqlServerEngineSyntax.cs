using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System.Data;
using System.Data.SqlClient;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;

namespace Atdi.CoreServices.DataLayer
{
    internal sealed class SqlServerEngineSyntax : IEngineSyntax, IConstraintEngineSyntax
    {
        private const string IDENT = "    ";
        private readonly IFormatProvider _formatProvider = System.Globalization.CultureInfo.InvariantCulture;

        public IConstraintEngineSyntax Constraint => this;

        string IConstraintEngineSyntax.LikeAnyChar => "%";

        public int MaxAliasLenth => throw new NotImplementedException();

        public string SortedColumn(string expression, SortDirection direction)
        {
            switch (direction)
            {
                case SortDirection.Ascending:
                    return expression + " ASC";
                case SortDirection.Descending:
                    return expression + " DESC";
                default:
                    throw new InvalidOperationException(Exceptions.SortDirectionNotSupported.With(direction));
            }
        }

        public string ColumnExpression(string expression, string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return expression;
            }

            return $"{expression} AS [{alias}]";
        }

        public string EncodeFieldName(string name)
        {
            return $"[{name}]";
        }

        public string EncodeFieldNameExpression(string name)
        {
            return $"{name}"; 
        }

        public string EncodeFieldName(string source, string name)
        {
            return $"[{source}].[{name}]";
        }

        public string EncodeParameterName(string name)
        {
            return "@" + name;
        }

        public string EncodeValue(string value)
        {
            return string.Concat("'", value.Replace("'", "''"), "'");
        }

        public string EncodeValue(int value)
        {
            return value.ToString(_formatProvider);
        }

        public string EncodeValue(bool value)
        {
            return (value ? 1 : 0).ToString(_formatProvider);
        }

        public string EncodeValue(DateTime value)
        {
            return $"CAST('{value.ToString("yyyymmdd HH:MM:ss.fff")}' as datetime)";
        }

        public string EncodeValue(float value)
        {
            return value.ToString(_formatProvider);
        }

        public string EncodeValue(double value)
        {
            return value.ToString(_formatProvider);
        }

        public string EncodeValue(decimal value)
        {
            return value.ToString(_formatProvider);
        }

        public string SelectExpression(string[] selectColumns, string fromExpression, string whereExpression = null, string[] orderByColumns = null, DataLimit limit = null, string[] groupByColumns = null)
        {
            var statement = new StringBuilder();

            if (limit == null || limit.Value < 0)
            {
                statement.AppendLine("SELECT");
            }
            else
            {
                if (limit.Type == LimitValueType.Records)
                {
                    statement.AppendLine($"SELECT TOP({limit.Value})");
                }
                else if(limit.Type == LimitValueType.Percent)
                {
                    statement.AppendLine($"SELECT TOP({limit.Value}) PERCENT");
                }
                else
                {
                    throw new InvalidOperationException(Exceptions.DataLimitTypeNotSupported.With(limit.Type));
                }
            }
            if (selectColumns != null && selectColumns.Length > 0)
            {
               statement.AppendLine(IDENT + string.Join("," + Environment.NewLine + IDENT, selectColumns));
            }
            else
            {
                statement.AppendLine(IDENT + "*");
            }

            if (!string.IsNullOrEmpty(fromExpression))
            {
                statement.AppendLine("FROM");
                statement.AppendLine(IDENT+ fromExpression.Replace(Environment.NewLine, Environment.NewLine + IDENT));
            }
            if (!string.IsNullOrEmpty(whereExpression))
            {
                statement.AppendLine("WHERE");
                statement.AppendLine(IDENT + whereExpression.Replace(Environment.NewLine, Environment.NewLine + IDENT));
            }
            if (groupByColumns != null && groupByColumns.Length > 0)
            {
                statement.AppendLine("GROUP BY");
                statement.AppendLine(IDENT + string.Join("," + Environment.NewLine + IDENT, groupByColumns));
            }
            if (orderByColumns != null && orderByColumns.Length > 0)
            {
                statement.AppendLine("ORDER BY");
                statement.AppendLine(IDENT + string.Join("," + Environment.NewLine + IDENT, orderByColumns));
            }
            return statement.ToString();
        }

        public string FromExpression(string expression, string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return expression;
            }

            var statment = new StringBuilder();
            statment.AppendLine("(");
            statment.AppendLine(IDENT + expression.Replace(Environment.NewLine, Environment.NewLine + IDENT));
            statment.AppendLine($") AS [{alias}]");
            return statment.ToString();
        }

        string IConstraintEngineSyntax.JoinExpressions(LogicalOperator operation, string[] expressions)
        {
            if (expressions == null || expressions.Length == 0)
            {
                return string.Empty;
            }
            if (expressions.Length == 1)
            {
                return expressions[0];
            }
            if (operation == LogicalOperator.And)
            {
                return "(" + string.Join(" AND ", expressions) + ")";
            }
            if (operation == LogicalOperator.Or)
            {
                return "(" + string.Join(" OR ", expressions) + ")";
            }

            throw new InvalidOperationException(Exceptions.LogicalOperatorNotSupported.With(operation));
        }

        string IConstraintEngineSyntax.IsNull(string testExpression)
        {
            return $"({testExpression} IS NULL)";
        }

        string IConstraintEngineSyntax.IsNotNull(string testExpression)
        {
            return $"({testExpression} IS NOT NULL)";
        }

        string IConstraintEngineSyntax.In(string testExpression, string[] expressions)
        {
            return $"({testExpression} IN ({string.Join(", ", expressions)}))";
        }

        string IConstraintEngineSyntax.NotIn(string testExpression, string[] expressions)
        {
            return $"({testExpression} NOT IN ({string.Join(", ", expressions)}))";
        }

        string IConstraintEngineSyntax.Between(string testExpression, string beginExpression, string endExpression)
        {
            return $"({testExpression} BETWEEN {beginExpression} AND {endExpression})";
        }

        string IConstraintEngineSyntax.NotBetween(string testExpression, string beginExpression, string endExpression)
        {
            return $"({testExpression} NOT BETWEEN {beginExpression} AND {endExpression})";
        }

        string IConstraintEngineSyntax.Like(string matchExpression, string paternExpression)
        {
            return $"({matchExpression} LIKE {paternExpression})";
        }

        string IConstraintEngineSyntax.NotLike(string matchExpression, string paternExpression)
        {
            return $"({matchExpression} NOT LIKE {paternExpression})";
        }

        string IConstraintEngineSyntax.Equal(string leftExpression, string rightExpression)
        {
            return $"({leftExpression} = {rightExpression})";
        }

        string IConstraintEngineSyntax.GreaterThan(string leftExpression, string rightExpression)
        {
            return $"({leftExpression} > {rightExpression})";
        }

        string IConstraintEngineSyntax.LessThan(string leftExpression, string rightExpression)
        {
            return $"({leftExpression} < {rightExpression})";
        }

        string IConstraintEngineSyntax.GreaterEqual(string leftExpression, string rightExpression)
        {
            return $"({leftExpression} >= {rightExpression})";
        }

        string IConstraintEngineSyntax.LessEqual(string leftExpression, string rightExpression)
        {
            return $"({leftExpression} <= {rightExpression})";
        }

        string IConstraintEngineSyntax.NotEqual(string leftExpression, string rightExpression)
        {
            return $"({leftExpression} <> {rightExpression})";
        }

        public string EncodeTableName(string name)
        {
            return $"[{name}]";
        }

        public string EncodeTableName(string schema, string name)
        {
            return $"[{schema}].[{name}]";
        }

        public string SourceExpression(string sourceExpression, string alias)
        {
            return $"{sourceExpression} AS [{alias}]";
        }

        public string DeleteExpression(string sourceExpression, string fromExpression = null, string whereExpression = null)
        {
            var statement = new StringBuilder();

            statement.AppendLine($"DELETE {sourceExpression}");
            
            if (!string.IsNullOrEmpty(fromExpression))
            {
                statement.AppendLine("FROM");
                statement.AppendLine(IDENT + fromExpression.Replace(Environment.NewLine, Environment.NewLine + IDENT));
            }
            if (!string.IsNullOrEmpty(whereExpression))
            {
                statement.AppendLine("WHERE");
                statement.AppendLine(IDENT + whereExpression.Replace(Environment.NewLine, Environment.NewLine + IDENT));
            }
            
            return statement.ToString();
        }

        public string InsertExpression(string sourceExpression, string columnsExpression, string valuesExpression)
        {
            var statement = new StringBuilder();

            statement.AppendLine($"INSERT INTO {sourceExpression} ({columnsExpression})");
            statement.AppendLine($"SELECT {valuesExpression}");

            return statement.ToString();
        }

        public string UpdateExpression(string sourceExpression, string valuesExpression, string fromExpression = null, string whereExpression = null)
        {
            var statement = new StringBuilder();

            statement.AppendLine($"UPDATE {sourceExpression} SET");
            statement.AppendLine(IDENT + valuesExpression.Replace(Environment.NewLine, Environment.NewLine + IDENT));
            if (!string.IsNullOrEmpty(fromExpression))
            {
                statement.AppendLine("FROM");
                statement.AppendLine(IDENT + fromExpression.Replace(Environment.NewLine, Environment.NewLine + IDENT));
            }
            if (!string.IsNullOrEmpty(whereExpression))
            {
                statement.AppendLine("WHERE");
                statement.AppendLine(IDENT + whereExpression.Replace(Environment.NewLine, Environment.NewLine + IDENT));
            }

            return statement.ToString();
        }

        public string SetColumnValueExpression(string columnExpression, string valueExpression)
        {
            return $"{columnExpression} = {valueExpression}";
        }
    }
}
