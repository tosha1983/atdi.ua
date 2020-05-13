using Atdi.DataModels.Api.EntityOrm.WebClient;
using System;
using System.Linq.Expressions;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	

	public interface IQueryFilter<TEntity, out TQuery>
	{
		IQueryFilter<TEntity, TQuery> Condition<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression,
			TValue value);

		IQueryFilter<TEntity, TQuery> Condition<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression,
			FilterOperator filterOperator, params TValue[] values);

		IQueryFilter<TEntity, TQuery> Condition<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression,
			Expression<Func<TEntity, TValue>> rightOperandPathExpression);

		IQueryFilter<TEntity, TQuery> Condition<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression,
			FilterOperator filterOperator, params Expression<Func<TEntity, TValue>>[] rightOperandPathExpressions);

		IQueryFilter<TEntity, TQuery> And();

		IQueryFilter<TEntity, TQuery> Or();

		IQueryFilter<TEntity, TQuery> Begin();

		IQueryFilter<TEntity, TQuery> End();

		TQuery EndFilter();
	}
}