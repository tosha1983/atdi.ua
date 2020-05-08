using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient.DTO;
using Atdi.Contracts.Api.EntityOrm.WebClient;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal sealed class ApplyQueryHandler : IWebApiQueryHandler
	{
		private readonly ProxyInstanceFactory _proxyInstanceFactory;

		public string WebQueryUrl => "/api/orm/data/$Record/apply";

		public ApplyQueryHandler(ProxyInstanceFactory proxyInstanceFactory)
		{
			_proxyInstanceFactory = proxyInstanceFactory;
		}
		public EntityQueryRequest CreateRequest(IWebApiQuery query)
		{
			var creator = (IWebApiRequestCreator) query;
			return creator.Create();
		}

		public TResult Handle<TResult>(WebApiHttpResponse response, Func<IDataReader, TResult> handler)
		{
			throw new NotImplementedException();
		}

		public TResult Handle<TEntity, TResult>(WebApiHttpResponse response, Func<IDataReader<TEntity>, TResult> handler)
		{
			throw new NotImplementedException();
		}

		public long Handle(WebApiHttpResponse response)
		{
			var result = response.Decode<ApplyQueryResponse>();
			return result.Count;
		}

		public TResult Handle<TResult>(WebApiHttpResponse response)
		{
			throw new NotImplementedException();
		}
	}
}
