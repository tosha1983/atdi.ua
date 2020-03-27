using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client.Tasks
{
	class PointFieldStrengthCalcTask
	{
		private readonly HttpClient _httpClient;

		public PointFieldStrengthCalcTask(HttpClient httpClient, int projectId)
		{
			this._httpClient = httpClient;
		}
	}
}
