using System;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IQueryExecutor
	{
		long Execute(IWebApiQuery webQuery);

		TResult Execute<TResult>(IWebApiQuery webQuery);

		TResult ExecuteAndFetch<TResult>(IWebApiQuery webQuery, Func<IDataReader, TResult> handler);

		TResult ExecuteAndFetch<TEntity, TResult>(IWebApiQuery<TEntity> webQuery, Func<IDataReader<TEntity>, TResult> handler);

		TRecord[] ExecuteAndRead<TEntity, TRecord>(IWebApiQuery<TEntity> webQuery, Func<IDataReader<TEntity>, TRecord> recordHandler);

		IDataReader ExecuteReader(IWebApiQuery webQuery);

		IDataReader<TEntity> ExecuteReader<TEntity>(IWebApiQuery<TEntity> webQuery);
	}
}