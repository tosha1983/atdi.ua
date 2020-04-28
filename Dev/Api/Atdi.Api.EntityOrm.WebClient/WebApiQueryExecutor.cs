using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Api.EntityOrm.WebClient
{
	internal class WebApiQueryExecutor : IQueryExecutor
	{
		private readonly WebApiDataLayer _dataLayer;
		private readonly WebApiEndpoint _endpoint;
		private readonly WebApiDataContext _dataContext;
		private readonly HttpClient _httpClient;

		public WebApiQueryExecutor(WebApiDataLayer dataLayer, WebApiEndpoint endpoint, WebApiDataContext dataContext)
		{
			_dataLayer = dataLayer;
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

		public long Execute(IWebApiQuery webQuery)
		{
			var handler = _dataLayer.MethodHandlers[webQuery.QueryType];
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
			var handler = _dataLayer.MethodHandlers[webQuery.QueryType];
			var request = handler.CreateRequest(webQuery);
			request.Context = _dataContext.Name;

			var response = _httpClient.PostAsJsonAsync(CombineUrl(_endpoint.ApiUrl, handler.WebMethodUrl), request)
				.GetAwaiter()
				.GetResult();
			response.EnsureSuccessStatusCode();
			var result = handler.Handle<TResult>(response);

			return result;
		}

		public TResult ExecuteAndFetch<TResult>(IWebApiQuery webQuery, Func<IDataReader, TResult> handler)
		{
			var methodHandler = _dataLayer.MethodHandlers[webQuery.QueryType];
			var request = methodHandler.CreateRequest(webQuery);
			request.Context = _dataContext.Name;

			var response = _httpClient.PostAsJsonAsync(CombineUrl(_endpoint.ApiUrl, methodHandler.WebMethodUrl), request)
				.GetAwaiter()
				.GetResult();
			response.EnsureSuccessStatusCode();
			var result = methodHandler.Handle(response, handler);

			return result;
		}

		public TResult ExecuteAndFetch<TEntity, TResult>(IWebApiQuery<TEntity> webQuery, Func<IDataReader<TEntity>, TResult> handler)
		{
			var methodHandler = _dataLayer.MethodHandlers[webQuery.QueryType];
			var request = methodHandler.CreateRequest(webQuery);
			request.Context = _dataContext.Name;

			var response = _httpClient.PostAsJsonAsync(CombineUrl(_endpoint.ApiUrl, methodHandler.WebMethodUrl), request)
				.GetAwaiter()
				.GetResult();
			response.EnsureSuccessStatusCode();
			var result = methodHandler.Handle(response, handler);

			return result;
		}

		public TRecord[] ExecuteAndRead<TEntity, TRecord>(IWebApiQuery<TEntity> webQuery, Func<IDataReader<TEntity>, TRecord> recordHandler)
		{
			var methodHandler = _dataLayer.MethodHandlers[webQuery.QueryType];
			var request = methodHandler.CreateRequest(webQuery);
			request.Context = _dataContext.Name;

			var response = _httpClient.PostAsJsonAsync(CombineUrl(_endpoint.ApiUrl, methodHandler.WebMethodUrl), request)
				.GetAwaiter()
				.GetResult();
			response.EnsureSuccessStatusCode();
			var result = methodHandler.Handle(response, (IDataReader<TEntity> reader) =>
			{
				var count = reader.Count;
				if (count == 0)
				{
					return new TRecord[] { };
				}
				var records = new TRecord[count];
				for (int i = 0; i < count; i++)
				{
					if (!reader.Read())
					{
						throw new InvalidOperationException($"The data set ended unexpectedly: count={count}, index={i}");
					}

					records[i] = recordHandler(reader);
				}
				return records;
			});

			return result;
		}
	}
}
