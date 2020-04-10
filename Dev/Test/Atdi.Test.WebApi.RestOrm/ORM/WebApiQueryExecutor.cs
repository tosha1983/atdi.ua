using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	internal class WebApiQueryExecutor : IQueryExecutor
	{
		private static readonly ProxyInstanceFactory ProxyInstanceFactory;
		private static readonly Dictionary<WebApiQueryType, IWebApiMethodHandler> MethodHandlers;

		static WebApiQueryExecutor()
		{
			ProxyInstanceFactory = new ProxyInstanceFactory();
			MethodHandlers = new Dictionary<WebApiQueryType, IWebApiMethodHandler>
			{
				[WebApiQueryType.Create] = new CreationMethodHandler(ProxyInstanceFactory)
			};

		}

		private readonly WebApiEndpoint _endpoint;
		private readonly WebApiDataContext _dataContext;
		private readonly HttpClient _httpClient;

		public WebApiQueryExecutor(WebApiEndpoint endpoint, WebApiDataContext dataContext)
		{
			_endpoint = endpoint;
			_dataContext = dataContext;
			_httpClient = new HttpClient
			{
				BaseAddress = endpoint.BaseAddress
			};
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public static string CombineUrl(string uri1, string uri2)
		{
			uri1 = uri1.TrimEnd('/');
			uri2 = uri2.TrimStart('/');
			return $"{uri1}/{uri2}";
		}

		public int Execute(IWebApiQuery webQuery)
		{
			var handler = MethodHandlers[webQuery.QueryType];
			var request = handler.CreateRequest(webQuery);
			request.Context = _dataContext.Name;

			var response = _httpClient.PostAsJsonAsync(CombineUrl(_endpoint.ApiUrl, handler.WebMethodUrl), request)
				.GetAwaiter()
				.GetResult();
			response.EnsureSuccessStatusCode();
			var result = handler.Handle(response);

			return result;
		}

		public TResult Execute<TResult>(IWebApiQuery webQuery)
		{
			throw new NotImplementedException();
		}

		public TResult ExecuteAndFetch<TResult>(IWebApiQuery webQuery, Func<IDataReader, TResult> handler)
		{
			throw new NotImplementedException();
		}

		public TResult ExecuteAndFetch<TEntity, TResult>(IWebApiQuery<TEntity> webQuery, Func<IDataReader<TEntity>, TResult> handler)
		{
			throw new NotImplementedException();
		}
	}
}
