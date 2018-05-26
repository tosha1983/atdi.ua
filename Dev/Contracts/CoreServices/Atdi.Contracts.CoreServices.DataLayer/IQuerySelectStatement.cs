using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IQuerySelectStatement
    {
        IQuerySelectStatement Select(params string[] columns);

        IQuerySelectStatement Where(Condition condition);

        IQuerySelectStatement OrderByAsc(params string[] columns);

        IQuerySelectStatement OrderByDesc(params string[] columns);

        IQuerySelectStatement OnTop(int count);

        
    }

    public static class QuerySelectStatementExtensions
    {
        public static IQuerySelectStatement Where(this IQuerySelectStatement query, string column, string value)
        {
            return query.Where(
                new ConditionExpression
                {
                    LeftOperand = new ColumnOperand { ColumnName = column },
                    Operator = ConditionOperator.Equal,
                    RightOperand = new StringValueOperand { Value = value }
                });
        }
        public static IQuerySelectStatement Where(this IQuerySelectStatement query, string column, int? value)
        {
            return query.Where(
                new ConditionExpression
                {
                    LeftOperand = new ColumnOperand { ColumnName = column },
                    Operator = ConditionOperator.Equal,
                    RightOperand = new IntegerValueOperand { Value = value }
                });
        }

        public static IQuerySelectStatement Where(this IQuerySelectStatement query, string column, bool? value)
        {
            return query.Where(
                new ConditionExpression
                {
                    LeftOperand = new ColumnOperand { ColumnName = column },
                    Operator = ConditionOperator.Equal,
                    RightOperand = new BooleanValueOperand { Value = value }
                });
        }
    }
}
