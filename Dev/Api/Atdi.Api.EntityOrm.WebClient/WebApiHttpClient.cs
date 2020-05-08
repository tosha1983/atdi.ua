using Atdi.DataModels.Api.EntityOrm.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal sealed class WebApiHttpClient
	{
		private readonly WebApiEndpoint _endpoint;
		private readonly HttpClient _httpClient;

		public WebApiHttpClient(WebApiEndpoint endpoint)
		{
			_endpoint = endpoint;
			_httpClient = new HttpClient
			{
				BaseAddress = endpoint.BaseAddress
			};
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
		}

		

		public WebApiHttpResponse Post<TData>(string requestUri, TData data)
		{
			var response = _httpClient.PostAsJsonAsync(WebApiUtility.CombineUri(_endpoint.ApiUri, requestUri), data)
				.GetAwaiter()
				.GetResult();

			if (!response.IsSuccessStatusCode)
			{
				var exceptionResponse = response.Content.ReadAsAsync<WebApiServerException>()
					.GetAwaiter()
					.GetResult();

				throw new EntityOrmWebApiException(
					response.StatusCode,
					response.RequestMessage.RequestUri.ToString(),
					"An error occurred while executing a POST request.",
					exceptionResponse);
			}

			return new WebApiHttpResponse(response);
		}

		public WebApiHttpResponse Get(string requestUri)
		{
			var response = _httpClient.GetAsync(WebApiUtility.CombineUri(_endpoint.ApiUri, requestUri))
				.GetAwaiter()
				.GetResult();

			if (!response.IsSuccessStatusCode)
			{
				var exceptionResponse = response.Content.ReadAsAsync<WebApiServerException>()
					.GetAwaiter()
					.GetResult();

				throw new EntityOrmWebApiException(
					response.StatusCode,
					response.RequestMessage.RequestUri.ToString(),
					"An error occurred while executing a POST request.",
					exceptionResponse);
			}

			return new WebApiHttpResponse(response);
		}
	}
}
