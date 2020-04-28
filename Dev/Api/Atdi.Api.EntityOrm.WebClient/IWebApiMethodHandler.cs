using Atdi.Contracts.Api.EntityOrm.WebClient;
using System;
using System.Net.Http;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal interface IWebApiMethodHandler
	{
		string WebMethodUrl { get; }

		DTO.EntityRequest CreateRequest(IWebApiQuery query);

		long Handle(HttpResponseMessage response);

		TResult Handle<TResult>(HttpResponseMessage response);

		TResult Handle<TResult>(HttpResponseMessage response, Func<IDataReader, TResult> handler);

		TResult Handle<TEntity, TResult>(HttpResponseMessage response, Func<IDataReader<TEntity>, TResult> handler);
	}

	internal interface IWebApiRequestCreator
	{
		DTO.EntityRequest Create();
	}
}