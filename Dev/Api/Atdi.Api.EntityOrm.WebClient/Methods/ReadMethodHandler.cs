using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient.DTO;
using Atdi.Contracts.Api.EntityOrm.WebClient;

namespace Atdi.Api.EntityOrm.WebClient
{
	class ReadMethodHandler : IWebApiMethodHandler
	{
		private readonly ProxyInstanceFactory _proxyInstanceFactory;

		public ReadMethodHandler(ProxyInstanceFactory proxyInstanceFactory)
		{
			_proxyInstanceFactory = proxyInstanceFactory;
		}

		public string WebMethodUrl => "/api/orm/data/$DataSet";

		public EntityRequest CreateRequest(IWebApiQuery query)
		{
			var creator = (IWebApiRequestCreator)query;
			return creator.Create();
		}

		public TResult Handle<TResult>(HttpResponseMessage response, Func<IDataReader, TResult> handler)
		{
			var result = response.Content.ReadAsAsync<RecordReadResponse>()
				.GetAwaiter()
				.GetResult();

			var reader = new WebApiDataReader(result);

			return handler(reader);
		}

		public TResult Handle<TEntity, TResult>(HttpResponseMessage response, Func<IDataReader<TEntity>, TResult> handler)
		{
			var result = response.Content.ReadAsAsync<RecordReadResponse>()
				.GetAwaiter()
				.GetResult();

			var reader = new WebApiDataReader<TEntity>(new WebApiDataReader(result));

			return handler(reader);
		}

		public long Handle(HttpResponseMessage response)
		{
			var result = response.Content.ReadAsAsync<RecordReadResponse>()
				.GetAwaiter()
				.GetResult();

			return result.Count;
		}

		public TResult Handle<TResult>(HttpResponseMessage response)
		{
			throw new NotImplementedException();
		}
	}
}
