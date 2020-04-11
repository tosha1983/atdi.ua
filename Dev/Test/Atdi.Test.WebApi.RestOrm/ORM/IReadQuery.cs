using System;
using System.Linq.Expressions;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface IReadQuery : IFilteringQuery<IReadQuery>, IWebApiQuery
	{
		IReadQuery Select(string path);

		IReadQuery OrderByAsc(string path);

		IReadQuery OrderByDesc(string path);

		IReadQuery OnTop(int count);
		
	}

	public interface IReadQuery<TEntity> : IFilteringQuery<TEntity, IReadQuery<TEntity>>, IWebApiQuery<TEntity>
	{
		IReadQuery<TEntity> Select(params Expression<Func<TEntity, object>>[] pathExpressions);

		IReadQuery<TEntity> OrderByAsc(params Expression<Func<TEntity, object>>[] pathExpressions);

		IReadQuery<TEntity> OrderByDesc(params Expression<Func<TEntity, object>>[] pathExpressions);

		IReadQuery<TEntity> OnTop(int count);

	}
}