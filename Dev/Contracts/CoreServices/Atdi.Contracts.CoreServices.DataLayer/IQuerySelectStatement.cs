using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IQuerySelectStatement : IQueryStatement
    {
        IQuerySelectStatement Select(params string[] columns);

        IQuerySelectStatement Where(Condition condition);

        IQuerySelectStatement OrderByAsc(params string[] columns);

        IQuerySelectStatement OrderByDesc(params string[] columns);

        IQuerySelectStatement OnTop(int count);

        IQuerySelectStatement OnPercentTop(int percent);

        IQuerySelectStatement Distinct();

    }

    public interface IQuerySelectStatement<TModel>
    {
        IQuerySelectStatement<TModel> Select(params Expression<Func<TModel, object>>[] columnsExpressions);

        IQuerySelectStatement<TModel> Where(Expression<Func<TModel, bool>> expression);

        IQuerySelectStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression);

        IQuerySelectStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values);

        IQuerySelectStatement<TModel> OrderByAsc(params Expression<Func<TModel, object>>[] columnsExpressions);

        IQuerySelectStatement<TModel> OrderByDesc(params Expression<Func<TModel, object>>[] columnsExpressions);

        IQuerySelectStatement<TModel> OnTop(int count);

        IQuerySelectStatement<TModel> OnPercentTop(int percent);

        IQuerySelectStatement<TModel> Distinct();

    }
    public interface IQuerySelectStatementOperation
    {
        bool Between<TValue>(TValue testValue, TValue arg1, TValue arg2);

        bool NotBetween<TValue>(TValue testValue, TValue arg1, TValue arg2);

        bool Like(string testValue, string arg);
        bool NotLike(string testValue, string arg);

        bool In<TValue>(TValue testValue, params TValue[] args);
        bool NotIn<TValue>(TValue testValue, params TValue[] args);
    }

    public static class QuerySelectStatementExtensions
    {
        public static IQuerySelectStatement SetLimit(this IQuerySelectStatement query, DataLimit limit)
        {
            if (limit != null)
            {
                switch (limit.Type)
                {
                    case DataModels.DataConstraint.LimitValueType.Records:
                        query.OnTop(limit.Value);
                        break;
                    case DataModels.DataConstraint.LimitValueType.Percent:
                        query.OnPercentTop(limit.Value);
                        break;
                    default:
                        break;
                }
            }
            return query;
        }

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

        public static IQuerySelectStatement Where(this IQuerySelectStatement query, string column, Guid? value)
        {
            return query.Where(
                new ConditionExpression
                {
                    LeftOperand = new ColumnOperand { ColumnName = column },
                    Operator = ConditionOperator.Equal,
                    RightOperand = new GuidValueOperand { Value = value }
                });
        }

        public static IQuerySelectStatement Where(this IQuerySelectStatement query, Condition[] conditions)
        {
            if (conditions == null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }
            for (int i = 0; i < conditions.Length; i++)
            {
                query.Where(conditions[i]);
            }
            return query;
        }
    }
}
