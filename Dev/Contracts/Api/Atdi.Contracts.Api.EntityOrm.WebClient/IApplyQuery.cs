using System;
using System.Linq.Expressions;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IApplyQuery : IFilteringQuery<IApplyQuery>, IWebApiQuery
	{
		IApplyQuery SetValue<TValue>(string path, TValue value);
		IApplyQuery UpdateIfExists();
		IApplyQuery CreateIfNotExists();
	}

	public interface IApplyQuery<TEntity> : IFilteringQuery<TEntity, IApplyQuery<TEntity>>, IWebApiQuery<TEntity>
	{
		IApplyQuery<TEntity> SetValue<TValue>(Expression<Func<TEntity, TValue>> pathExpression, TValue value);

		IApplyQuery<TEntity> SetValueAsJson<TValue>(Expression<Func<TEntity, string>> pathExpression, TValue value);

		IApplyQuery<TEntity> UpdateIfExists();

		IApplyQuery<TEntity> CreateIfNotExists();
	}
}