using Atdi.Contracts.Api.EntityOrm.WebClient;
using System;
using System.Net.Http;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal interface IWebApiQueryHandler
	{
		string WebQueryUrl { get; }

		DTO.EntityQueryRequest CreateRequest(IWebApiQuery query);

		long Handle(WebApiHttpResponse response);

		TResult Handle<TResult>(WebApiHttpResponse response);

		TResult Handle<TResult>(WebApiHttpResponse response, Func<IDataReader, TResult> handler);

		TResult Handle<TEntity, TResult>(WebApiHttpResponse response, Func<IDataReader<TEntity>, TResult> handler);
	}

	internal interface IWebApiRequestCreator
	{
		DTO.EntityQueryRequest Create();
	}
}