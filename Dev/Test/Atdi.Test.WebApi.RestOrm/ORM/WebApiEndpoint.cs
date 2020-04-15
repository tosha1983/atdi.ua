using System;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public class WebApiEndpoint
	{
		public Uri BaseAddress;

		public string ApiUrl;

		public WebApiEndpoint(Uri baseAddress, string apiUrl)
		{
			this.BaseAddress = baseAddress;
			this.ApiUrl = apiUrl;

		}

	}
}