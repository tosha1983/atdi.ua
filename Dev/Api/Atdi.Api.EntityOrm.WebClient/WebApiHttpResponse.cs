using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal sealed class WebApiHttpResponse
	{
		private readonly HttpResponseMessage _httpResponse;

		public WebApiHttpResponse(HttpResponseMessage httpResponse)
		{
			_httpResponse = httpResponse;
		}

		public TData Decode<TData>()
		{
			//var raw = _httpResponse.Content.ReadAsStringAsync()
			//	.GetAwaiter()
			//	.GetResult();

			var result = _httpResponse.Content.ReadAsAsync<TData>()
				.GetAwaiter()
				.GetResult();

			return result;
		}

		public object Decode(Type type)
		{
			var result = _httpResponse.Content.ReadAsAsync(type)
				.GetAwaiter()
				.GetResult();

			return result;
		}
	}
}
