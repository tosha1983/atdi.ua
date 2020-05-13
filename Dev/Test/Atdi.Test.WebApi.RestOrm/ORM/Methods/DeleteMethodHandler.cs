using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Test.WebApi.RestOrm.ORM.DTO;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	internal class DeleteMethodHandler : IWebApiMethodHandler
	{
		private readonly ProxyInstanceFactory _proxyInstanceFactory;

		public string WebMethodUrl => "/api/orm/data/$DataSet/delete";

		public DeleteMethodHandler(ProxyInstanceFactory proxyInstanceFactory)
		{
			_proxyInstanceFactory = proxyInstanceFactory;
		}
		public EntityRequest CreateRequest(IWebApiQuery query)
		{
			var creator = (IWebApiRequestCreator) query;
			return creator.Create();
		}

		public TResult Handle<TResult>(HttpResponseMessage response, Func<IDataReader, TResult> handler)
		{
			throw new NotImplementedException();
		}

		public TResult Handle<TEntity, TResult>(HttpResponseMessage response, Func<IDataReader<TEntity>, TResult> handler)
		{
			throw new NotImplementedException();
		}

		public long Handle(HttpResponseMessage response)
		{
			var result = response.Content.ReadAsAsync<RecordDeleteResponse>()
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
