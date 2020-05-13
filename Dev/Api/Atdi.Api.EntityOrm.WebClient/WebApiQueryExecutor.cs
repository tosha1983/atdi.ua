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
	internal sealed class WebApiQueryExecutor : IQueryExecutor
	{
		private readonly WebApiDataLayer _dataLayer;
		private readonly WebApiDataContext _dataContext;
		private readonly WebApiHttpClient _httpClient;

		public WebApiQueryExecutor(WebApiDataLayer dataLayer, WebApiEndpoint endpoint, WebApiDataContext dataContext)
		{
			_dataLayer = dataLayer;
			_dataContext = dataContext;
			_httpClient = new WebApiHttpClient(endpoint);
		}

		public WebApiHttpResponse PostQuery(IWebApiQuery webQuery, out IWebApiQueryHandler methodHandler)
		{
			methodHandler = _dataLayer.QueryHandlers[webQuery.QueryType];
			var request = methodHandler.CreateRequest(webQuery);
			request.Context = _dataContext.Name;
			var response = _httpClient.Post(methodHandler.WebQueryUrl, request);
			return response;
		}

		public long Execute(IWebApiQuery webQuery)
		{
			var response = this.PostQuery(webQuery, out var queryHandler);
			var result = queryHandler.Handle(response);
			return result;
		}

		public TResult Execute<TResult>(IWebApiQuery webQuery)
		{
			var response = this.PostQuery(webQuery, out var queryHandler);
			var result = queryHandler.Handle<TResult>(response);
			return result;
		}

		public TResult ExecuteAndFetch<TResult>(IWebApiQuery webQuery, Func<IDataReader, TResult> handler)
		{
			var response = this.PostQuery(webQuery, out var queryHandler);
			var result = queryHandler.Handle(response, handler);
			return result;
		}

		public TResult ExecuteAndFetch<TEntity, TResult>(IWebApiQuery<TEntity> webQuery, Func<IDataReader<TEntity>, TResult> handler)
		{
			var response = this.PostQuery(webQuery, out var queryHandler);
			var result = queryHandler.Handle(response, handler);
			return result;
		}

		public TRecord[] ExecuteAndRead<TEntity, TRecord>(IWebApiQuery<TEntity> webQuery, Func<IDataReader<TEntity>, TRecord> recordHandler)
		{
			var response = this.PostQuery(webQuery, out var queryHandler);
			var result = queryHandler.Handle(response, (IDataReader<TEntity> reader) =>
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

		public IDataReader ExecuteReader(IWebApiQuery webQuery)
		{
			var response = this.PostQuery(webQuery, out var queryHandler);
			return queryHandler.GetReader(response);
		}

		public IDataReader<TEntity> ExecuteReader<TEntity>(IWebApiQuery<TEntity> webQuery)
		{
			var response = this.PostQuery(webQuery, out var queryHandler);
			return queryHandler.GetReader<TEntity>(response);
		}
	}
}
