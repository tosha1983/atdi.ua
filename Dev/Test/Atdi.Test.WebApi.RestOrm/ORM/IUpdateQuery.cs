using System;
using System.Linq.Expressions;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface IUpdateQuery: IFilteringQuery, IWebApiQuery
	{
		IUpdateQuery SetValue<TValue>(string path, TValue value);

	}

	public interface IUpdateQuery<TEntity> : IFilteringQuery<TEntity, IUpdateQuery<TEntity>>, IWebApiQuery<TEntity>
	{
		IUpdateQuery<TEntity> SetValue<TValue>(Expression<Func<TEntity, TValue>> pathExpression, TValue value);

	}
}