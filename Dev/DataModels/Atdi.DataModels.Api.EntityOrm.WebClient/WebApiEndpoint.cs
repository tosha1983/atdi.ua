using System;

namespace Atdi.DataModels.Api.EntityOrm.WebClient
{
	public class WebApiEndpoint
	{
		public Uri BaseAddress;

		public string ApiUri;

		public WebApiEndpoint(Uri baseAddress, string apiUri)
		{
			this.BaseAddress = baseAddress;
			this.ApiUri = apiUri;

		}

	}
}