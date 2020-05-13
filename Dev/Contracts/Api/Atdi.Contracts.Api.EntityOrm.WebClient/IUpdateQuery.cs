using System;
using System.Linq.Expressions;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IUpdateQuery: IFilteringQuery<IUpdateQuery>, IWebApiQuery
	{
		IUpdateQuery SetValue<TValue>(string path, TValue value);

	}

	public interface IUpdateQuery<TEntity> : IFilteringQuery<TEntity, IUpdateQuery<TEntity>>, IWebApiQuery<TEntity>
	{
		IUpdateQuery<TEntity> SetValue<TValue>(Expression<Func<TEntity, TValue>> pathExpression, TValue value);

	}
}