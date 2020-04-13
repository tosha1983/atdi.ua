using Atdi.DataModels.Api.EntityOrm.WebClient;
using System;
using System.Linq.Expressions;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IFilteringQuery<out TQuery>
	{
		TQuery Filter(string condition);
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