using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Atdi.Test.WebApi.RestOrm.ORM.DTO;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	internal class CreationMethodHandler : IWebApiMethodHandler
	{
		private readonly ProxyInstanceFactory _proxyInstanceFactory;

		public string WebMethodUrl => "/api/orm/data/$Record/create";

		public CreationMethodHandler(ProxyInstanceFactory proxyInstanceFactory)
		{
			_proxyInstanceFactory = proxyInstanceFactory;
		}
		public EntityRequest CreateRequest(IWebApiQuery query)
		{
			var creator = (IWebApiRequestCreator) query;
			return creator.Create();
		}

		public TResult ExecuteAndFetch<TResult>(HttpResponseMessage response, Func<IDataReader, TResult> handler)
		{
			throw new NotImplementedException();
		}

		public TResult ExecuteAndFetch<TEntity, TResult>(HttpResponseMessage response, Func<IDataReader<TEntity>, TResult> handler)
		{
			throw new NotImplementedException();
		}

		public int Handle(HttpResponseMessage response)
		{
			var result = response.Content.ReadAsAsync<RecordCreationResult>()
				.GetAwaiter()
				.GetResult();

			return result.Count;
		}

		public TResult Handle<TResult>(HttpResponseMessage response)
		{
			var proxyType = _proxyInstanceFactory.GetProxyType<TResult>();
			var result = response.Content.ReadAsAsync(proxyType)
				.GetAwaiter()
				.GetResult();

			return (TResult) result;
		}
	}
}
