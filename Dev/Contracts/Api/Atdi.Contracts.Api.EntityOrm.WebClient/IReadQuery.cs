using System;
using System.Linq.Expressions;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IReadQuery : IFilteringQuery<IReadQuery>, IWebApiQuery
	{
		IReadQuery Select(string path);

		IPagingReadQuery OrderByAsc(string path);

		IPagingReadQuery OrderByDesc(string path);

		IReadQuery OnTop(int count);

		IReadQuery FetchRows(long count);

		IReadQuery Distinct();
	}

	public interface IPagingReadQuery: IReadQuery
	{
		IReadQuery OffsetRows(long count);

		IReadQuery Paginate(long offsetRows, long fetchRows);
	}

	public interface IReadQuery<TEntity> : IFilteringQuery<TEntity, IReadQuery<TEntity>>, IWebApiQuery<TEntity>
	{
		IReadQuery<TEntity> Select(params Expression<Func<TEntity, object>>[] pathExpressions);

		IPagingReadQuery<TEntity> OrderByAsc(params Expression<Func<TEntity, object>>[] pathExpressions);

		IPagingReadQuery<TEntity> OrderByDesc(params Expression<Func<TEntity, object>>[] pathExpressions);

		IReadQuery<TEntity> OnTop(int count);

		IReadQuery<TEntity> FetchRows(long count);

		IReadQuery<TEntity> Distinct();
	}

	public interface IPagingReadQuery<TEntity> : IReadQuery<TEntity>
	{
		IReadQuery<TEntity> OffsetRows(long count);

		IReadQuery<TEntity> Paginate(long offsetRows, long fetchRows);
	}
}