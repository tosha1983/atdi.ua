using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Test.WebApi.RestOrm.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.WebApi.RestOrm
{
	
	class Program
	{
		static void Main(string[] args)
		{
			Console.ReadLine();

			var endpoint = new WebApiEndpoint(new Uri("http://localhost:15070/"), "/appserver/v1");
			var dataContext = new WebApiDataContext("SDRN_CalcServer_DB");

			var dataLayer = new WebApiDataLayer();
			var webQuery = dataLayer.GetBuilder<IProject>()
				.Create()
				.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
				.SetValue(c => c.Projection, "4UTN35")
				.SetValue(c => c.StatusCode, (byte) ProjectStatusCode.Created)
				.SetValue(c => c.StatusName, ProjectStatusCode.Created.ToString())
				.SetValue(c => c.OwnerInstance, "Atdi.Test.WebApi.RestOrm")
				.SetValue(c => c.OwnerProjectId, Guid.NewGuid())
				.SetValue(c => c.Name, "Web API ORM Test project")
				.SetValue(c => c.Note, "The Web API ORM Test project");


			var executor = dataLayer.GetExecutor(endpoint, dataContext);
			//var count = executor.Execute(webQuery);

			var projectPk = executor.Execute<IProject_PK>(webQuery);
		}


		static void Example1()
		{
			var endpoint = new WebApiEndpoint(new Uri("http://localhost:15070/"), "");
			var dataContext = new WebApiDataContext("DB_CalcServer");

			var dataLayer = new WebApiDataLayer();
			var webQuery = dataLayer.GetBuilder<IProject>()
				.Read()
				.Select(
					c => c.Id,
					c => c.Name
				)
				.Filter(c => c.Id, c => c.Id)
				.BeginFilter()
					.Begin()
						.Condition(c => c.Id, 1)
						.Condition(c => c.Name, "Test")
					.End()
					.Or()
					.Begin()
						.Begin()
							.Condition(c => c.Id, 2)
							.Condition(c => c.Name, "Test2")
						.End()
						.Condition(c => c.Name, "Test3")
						.Condition(c => c.CreatedDate, FilterOperator.Between, DateTimeOffset.MinValue, DateTimeOffset.MaxValue)
						.Or()
						.Condition(c => c.StatusCode, FilterOperator.In, 1, 2, 3, 4)
					.End()
				.EndFilter()
				.OrderByAsc(c => c.CreatedDate)
				.OrderByDesc(c => c.StatusCode);
			var executor = dataLayer.GetExecutor(endpoint, dataContext);

			var data = executor.ExecuteAndFetch(webQuery, reader =>
			{
				var result = new List<object>();
				while (reader.Read())
				{
					var record = new
					{
						Id = reader.GetValue(c => c.Id),
						Name = reader.GetValue(c => c.Name)
					};
					result.Add(record);
				}
				return result;
			});
		}
	}
}
