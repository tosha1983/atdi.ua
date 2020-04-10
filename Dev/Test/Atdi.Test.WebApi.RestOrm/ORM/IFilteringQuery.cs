using System;
using System.Linq.Expressions;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface IFilteringQuery
	{
		IReadQuery Filter(string condition);
	}

	public interface IFilteringQuery<TEntity, out TQuery>
	{
		IQueryFilter<TEntity, TQuery> BeginFilter();

		TQuery Filter<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression,
			TValue value);

		TQuery Filter<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression,
			FilterOperator filterOperator, params TValue[] values);

		TQuery Filter<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression,
			Expression<Func<TEntity, TValue>> rightOperandPathExpression);

		TQuery Filter<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression,
			FilterOperator filterOperator, params Expression<Func<TEntity, TValue>>[] rightOperandPathExpressions);
	}
}