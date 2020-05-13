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
	internal sealed class CreateQueryHandler : IWebApiQueryHandler
	{
		private readonly ProxyInstanceFactory _proxyInstanceFactory;

		public string WebQueryUrl => "/api/orm/data/$Record/create";

		public CreateQueryHandler(ProxyInstanceFactory proxyInstanceFactory)
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
			var result = response.Decode<CreateQueryResponse>();
			return result.Count;
		}

		public TResult Handle<TResult>(WebApiHttpResponse response)
		{
			var proxyType = _proxyInstanceFactory.GetProxyType<TResult>();
			var type = typeof(CreateQueryResponse<>);
			var responseType = type.MakeGenericType(proxyType);
			var result = response.Decode(responseType);
			var field = result.GetType().GetField("PrimaryKey");
			return (TResult)field.GetValue(result);
		}

		public IDataReader GetReader(WebApiHttpResponse response)
		{
			throw new NotImplementedException();
		}

		public IDataReader<TEntity> GetReader<TEntity>(WebApiHttpResponse response)
		{
			throw new NotImplementedException();
		}
	}
}
