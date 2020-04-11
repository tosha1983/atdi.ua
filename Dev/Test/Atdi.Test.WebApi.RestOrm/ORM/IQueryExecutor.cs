using System;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface IQueryExecutor
	{
		long Execute(IWebApiQuery webQuery);

		TResult Execute<TResult>(IWebApiQuery webQuery);

		TResult ExecuteAndFetch<TResult>(IWebApiQuery webQuery, Func<IDataReader, TResult> handler);

		TResult ExecuteAndFetch<TEntity, TResult>(IWebApiQuery<TEntity> webQuery, Func<IDataReader<TEntity>, TResult> handler);
	}
}