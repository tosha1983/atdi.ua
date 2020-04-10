using System;
using System.Net.Http;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface IWebApiMethodHandler
	{
		string WebMethodUrl { get; }

		DTO.EntityRequest CreateRequest(IWebApiQuery query);

		int Handle(HttpResponseMessage response);

		TResult Handle<TResult>(HttpResponseMessage response);

		TResult ExecuteAndFetch<TResult>(HttpResponseMessage response, Func<IDataReader, TResult> handler);

		TResult ExecuteAndFetch<TEntity, TResult>(HttpResponseMessage response, Func<IDataReader<TEntity>, TResult> handler);
	}

	public interface IWebApiRequestCreator
	{
		DTO.EntityRequest Create();
	}
}