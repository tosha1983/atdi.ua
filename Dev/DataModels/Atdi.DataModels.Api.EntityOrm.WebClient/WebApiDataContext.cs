﻿namespace Atdi.DataModels.Api.EntityOrm.WebClient
{
	public class WebApiDataContext
	{
		public readonly string Name;

		public WebApiDataContext(string contextName)
		{
			this.Name = contextName;
		}
	}
}