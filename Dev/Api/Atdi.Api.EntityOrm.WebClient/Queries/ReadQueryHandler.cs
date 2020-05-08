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
	internal sealed class ReadQueryHandler : IWebApiQueryHandler
	{
		private readonly ProxyInstanceFactory _proxyInstanceFactory;

		public ReadQueryHandler(ProxyInstanceFactory proxyInstanceFactory)
		{
			_proxyInstanceFactory = proxyInstanceFactory;
		}

		public string WebQueryUrl => "/api/orm/data/$DataSet";

		public EntityQueryRequest CreateRequest(IWebApiQuery query)
		{
			var creator = (IWebApiRequestCreator)query;
			return creator.Create();
		}

		public TResult Handle<TResult>(WebApiHttpResponse response, Func<IDataReader, TResult> handler)
		{
			var result = response.Decode<ReadQueryResponse>();
			var reader = new WebApiDataReader(result);
			return handler(reader);
		}

		public TResult Handle<TEntity, TResult>(WebApiHttpResponse response, Func<IDataReader<TEntity>, TResult> handler)
		{
			var result = response.Decode<ReadQueryResponse>();
			var reader = new WebApiDataReader<TEntity>(new WebApiDataReader(result));
			return handler(reader);
		}

		public long Handle(WebApiHttpResponse response)
		{
			var result = response.Decode<ReadQueryResponse>();
			return result.Count;
		}

		public TResult Handle<TResult>(WebApiHttpResponse response)
		{
			throw new NotImplementedException();
		}
	}
}
