using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Atdi.Test.Api.Sdrn.CalcServer.Client.DTO;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client
{
	class Program
	{
		static HttpClient client = new HttpClient();

		static void Main(string[] args)
		{
			Console.WriteLine($"Press any key to start SDRN Calculation Server Client (AK) ...");
			Console.ReadLine();

			client.BaseAddress = new Uri("http://localhost:15070/");
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));

			var r = CreateProject();

		}

		static long CreateProject()
		{
			var request = new DTO.RecordCreateRequest
			{
				Context = "SDRN_CalcServer_DB",
				Namespace = "Atdi.DataModels.Sdrn.CalcServer.Entities",
				Entity = "Project",
				Fields = new string[]
				{
					"Name",
					"Note",
					"OwnerInstance",
					"OwnerProjectId",
					"StatusCode",
					"StatusName",
					"StatusNote"
				},
				Values = new object []
				{
					$"Project: {Guid.NewGuid()}",
					"Note",
					"Atdi.Test.Api.Sdrn.CalcServer.Client",
					Guid.NewGuid().ToString(),
					1,
					"Created",
					null
				}
			};

			var response = client.PostAsJsonAsync(
				"/appserver/v1/api/orm/data/$Record/create", request).GetAwaiter().GetResult();

			response.EnsureSuccessStatusCode();

			var result = response.Content.ReadAsAsync<SimplePkRecordCreateResult>().GetAwaiter().GetResult();
			// return project ID.
			return result.PrimaryKey.Id;
		}
	}
}
