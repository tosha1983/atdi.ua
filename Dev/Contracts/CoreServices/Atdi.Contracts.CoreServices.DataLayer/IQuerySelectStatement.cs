﻿using Atdi.DataModels.DataConstraint;
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
        IQuerySelectStatement Select(params string[] paths);

        IQuerySelectStatement Where(Condition condition);

        IQueryPagingStatement OrderByAsc(params string[] paths);

        IQueryPagingStatement OrderByDesc(params string[] paths);

        IQuerySelectStatement OnTop(int count);

        IQuerySelectStatement OnPercentTop(int percent);

        IQuerySelectStatement Distinct();

        IQuerySelectStatement FetchRows(long count);

	}

    public interface IQueryPagingStatement : IQuerySelectStatement
    {
	    IQuerySelectStatement OffsetRows(long count);

	    IQuerySelectStatement Paginate(long offsetRows, long fetchRows);
    }


	public interface IQuerySelectStatement<TModel> : IQueryStatement<TModel>
    {
        IQuerySelectStatement<TModel> Select(params Expression<Func<TModel, object>>[] columnsExpressions);

        IQuerySelectStatement<TModel> Where(Expression<Func<TModel, bool>> expression);

        IQuerySelectStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression);

        IQuerySelectStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values);

        IQueryPagingStatement<TModel> OrderByAsc(params Expression<Func<TModel, object>>[] columnsExpressions);

        IQueryPagingStatement<TModel> OrderByDesc(params Expression<Func<TModel, object>>[] columnsExpressions);

        IQuerySelectStatement<TModel> OnTop(int count);

        IQuerySelectStatement<TModel> OnPercentTop(int percent);

        IQuerySelectStatement<TModel> Distinct();

        IQuerySelectStatement<TModel> FetchRows(long count);

	}

	public interface IQueryPagingStatement<TModel> : IQuerySelectStatement<TModel>
	{
		IQuerySelectStatement<TModel> OffsetRows(long count);

		IQuerySelectStatement<TModel> Paginate(long offsetRows, long fetchRows);
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
                        query.FetchRows(limit.Value);
                        break;
                    case DataModels.DataConstraint.LimitValueType.Percent:
                        query.OnPercentTop((int)limit.Value);
                        break;
                    default:
                        break;
                }
            }
            return query;
        }

        public static IQueryPagingStatement OrderBy(this IQuerySelectStatement query, DataModels.DataConstraint.OrderExpression[] orderExpressions)
        {
            if (orderExpressions != null)
            {
                for (int i = 0; i < orderExpressions.Length; i++)
                {
                    var orderExpression = orderExpressions[i];
                    if (orderExpression.OrderType == DataModels.DataConstraint.OrderType.Ascending)
                    {
                        query.OrderByAsc(orderExpression.ColumnName);
                    }
                    else if (orderExpression.OrderType == DataModels.DataConstraint.OrderType.Descending)
                    {
                        query.OrderByDesc(orderExpression.ColumnName);
                    }
                }
            }
            return (IQueryPagingStatement)query;
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

        public static IQuerySelectStatement Where(this IQuerySelectStatement query, string column, long? value)
        {
            return query.Where(
                new ConditionExpression
                {
                    LeftOperand = new ColumnOperand { ColumnName = column },
                    Operator = ConditionOperator.Equal,
                    RightOperand = new LongValueOperand { Value = value }
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
