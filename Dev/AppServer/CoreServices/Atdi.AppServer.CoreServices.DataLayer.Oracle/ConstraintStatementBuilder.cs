using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Atdi.AppServer.Common;

namespace Atdi.AppServer.CoreServices.DataLayer.Oracle
{
    public class ConstraintStatementBuilder : IConstraintStatementBuilder
    {
        private readonly IFormatProvider _formatProvider = System.Globalization.CultureInfo.InvariantCulture;

        string IConstraintStatementBuilder.LikeAnyChar => "%";

        string IConstraintStatementBuilder.OpeningBracket => "(";

        string IConstraintStatementBuilder.ClosingBracket => ")";

        string IConstraintStatementBuilder.EncodeFieldName(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));

            return $"[{name}]";
        }

        string IConstraintStatementBuilder.EncodeFieldName(string alias, string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(alias));
            Debug.Assert(!string.IsNullOrEmpty(name));

            return $"[{alias}].[{name}]";
        }

        string IConstraintStatementBuilder.EncodeValue(string value)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(value));

            return string.Concat("'", value.Replace("'", "''"), "'");
        }

        string IConstraintStatementBuilder.EncodeValue(int value)
        {
            return value.ToString(_formatProvider);
        }

        string IConstraintStatementBuilder.EncodeValue(bool value)
        {
            return (value ? 1 : 0).ToString(_formatProvider);
        }

        string IConstraintStatementBuilder.EncodeValue(DateTime value)
        {
            // cast('YYYYMMDD HH:MM:SS.mmm' as datetime)
            return $"CAST('{value.ToString("yyyymmdd HH:MM:ss.fff")}' as datetime)";
        }

        string IConstraintStatementBuilder.EncodeValue(double value)
        {
            return value.ToString(_formatProvider);
        }

        string IConstraintStatementBuilder.EncodeValue(decimal value)
        {
            return value.ToString(_formatProvider);
        }

        string IConstraintStatementBuilder.MakeAndStatement(string[] expressions)
        {
            Debug.Assert(!(expressions == null || expressions.Length == 0));

            return string.Join(" AND ", expressions);
        }

        string IConstraintStatementBuilder.MakeBetweenStatement(string testExpression, string beginExpression, string endExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(testExpression));
            Debug.Assert(!string.IsNullOrEmpty(beginExpression));
            Debug.Assert(!string.IsNullOrEmpty(endExpression));

            return $"{testExpression} BETWEEN {beginExpression} AND {endExpression}";
        }

        string IConstraintStatementBuilder.MakeEqualToStatement(string leftExpression, string rightExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(leftExpression));
            Debug.Assert(!string.IsNullOrEmpty(rightExpression));

            return $"{leftExpression} = {rightExpression}";
        }

        string IConstraintStatementBuilder.MakeGreaterThanOrEqualToStatement(string leftExpression, string rightExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(leftExpression));
            Debug.Assert(!string.IsNullOrEmpty(rightExpression));

            return $"{leftExpression} >= {rightExpression}";
        }

        string IConstraintStatementBuilder.MakeGreaterThanStatement(string leftExpression, string rightExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(leftExpression));
            Debug.Assert(!string.IsNullOrEmpty(rightExpression));

            return $"{leftExpression} > {rightExpression}";
        }

        string IConstraintStatementBuilder.MakeInStatement(string testExpression, string[] expressions)
        {
            Debug.Assert(!string.IsNullOrEmpty(testExpression));
            Debug.Assert(!(expressions == null || expressions.Length == 0));

            return $"{testExpression} IN ({string.Join(", ", expressions)})";
        }

        string IConstraintStatementBuilder.MakeLessThanOrEqualToStatement(string leftExpression, string rightExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(leftExpression));
            Debug.Assert(!string.IsNullOrEmpty(rightExpression));

            return $"{leftExpression} <= {rightExpression}";
        }

        string IConstraintStatementBuilder.MakeLessThanStatement(string leftExpression, string rightExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(leftExpression));
            Debug.Assert(!string.IsNullOrEmpty(rightExpression));

            return $"{leftExpression} < {rightExpression}";
        }

        string IConstraintStatementBuilder.MakeLikeStatement(string matchExpression, string paternExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(matchExpression));
            Debug.Assert(!string.IsNullOrEmpty(paternExpression));

            return $"{matchExpression} LIKE {paternExpression}";
        }

        string IConstraintStatementBuilder.MakeNotBetweenStatement(string testExpression, string beginExpression, string endExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(testExpression));
            Debug.Assert(!string.IsNullOrEmpty(beginExpression));
            Debug.Assert(!string.IsNullOrEmpty(endExpression));

            return $"{testExpression} NOT BETWEEN {beginExpression} AND {endExpression}";
        }

        string IConstraintStatementBuilder.MakeNotEqualToStatement(string leftExpression, string rightExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(leftExpression));
            Debug.Assert(!string.IsNullOrEmpty(rightExpression));

            return $"{leftExpression} <> {rightExpression}";
        }

        string IConstraintStatementBuilder.MakeNotInStatement(string testExpression, string[] expressions)
        {
            Debug.Assert(string.IsNullOrEmpty(testExpression));
            Debug.Assert(expressions == null || expressions.Length == 0);

            return $"{testExpression} NOT IN ({string.Join(", ", expressions)})";
        }

        string IConstraintStatementBuilder.MakeNotLikeStatement(string matchExpression, string paternExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(matchExpression));
            Debug.Assert(!string.IsNullOrEmpty(paternExpression));

            return $"{matchExpression} NOT LIKE {paternExpression}";
        }

        string IConstraintStatementBuilder.MakeNotNullStatement(string testExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(testExpression));

            return $"{testExpression} IS NOT NULL";
        }

        string IConstraintStatementBuilder.MakeNullStatement(string testExpression)
        {
            Debug.Assert(!string.IsNullOrEmpty(testExpression));

            return $"{testExpression} IS NULL";
        }

        string IConstraintStatementBuilder.MakeOrStatement(string[] expressions)
        {
            Debug.Assert(!(expressions == null || expressions.Length == 0));

            return string.Join(" OR ", expressions);
        }
    }
}
