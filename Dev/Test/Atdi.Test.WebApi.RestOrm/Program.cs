using Atdi.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.WebApi.RestOrm
{
	class Project : IProject
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Note { get; set; }
		public string OwnerInstance { get; set; }
		public Guid OwnerProjectId { get; set; }
		public DateTimeOffset CreatedDate { get; set; }
		public byte StatusCode { get; set; }
		public string StatusName { get; set; }
		public string StatusNote { get; set; }
		public string Projection { get; set; }
	}
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
			var projectPk = executor.Execute<IProject_PK>(webQuery);

			TestReadMethod(executor, dataLayer);
			TestUpdateMethod(executor, dataLayer);
			TestDeleteMethod(executor, dataLayer);
		}

		static void TestUpdateMethod(IQueryExecutor executor, WebApiDataLayer dataLayer)
		{
			var webQuery = dataLayer.GetBuilder<IProject>()
				.Update()
				.SetValue(c => c.StatusCode, (byte) ProjectStatusCode.Available)
				.SetValue(c => c.StatusName, ProjectStatusCode.Available.ToString())
				.Filter(c => c.Id, 28);
			var count = executor.Execute(webQuery);
		}

		static void TestDeleteMethod(IQueryExecutor executor, WebApiDataLayer dataLayer)
		{
			var webQuery = dataLayer.GetBuilder<IProject>()
				.Delete()
				.Filter(c => c.Id, 29);
			var count = executor.Execute(webQuery);
		}

		static void TestReadMethod(IQueryExecutor executor, WebApiDataLayer dataLayer)
		{
			var webQuery = dataLayer.GetBuilder<IProject>()
				.Read()
				.Select(
					c => c.Id,
					c => c.Name,
					c => c.StatusName,
					c => c.StatusCode,
					c => c.Projection,
					c => c.CreatedDate,
					c => c.Note,
					c => c.OwnerInstance,
					c => c.OwnerProjectId,
					c => c.StatusNote,
					c => c.Note
				)
				.OrderByDesc(c => c.Projection, c => c.StatusCode)
				.OrderByAsc(c => c.Id, c => c.CreatedDate)
				.Filter(c => c.Projection, "4UTN35")
				.Filter(c => c.StatusCode, FilterOperator.In, 
					(byte) ProjectStatusCode.Created,
					(byte) ProjectStatusCode.Available)
				.BeginFilter()
					.Begin()
						.Condition(c=>c.OwnerInstance, "Atdi.Test.WebApi.RestOrm")
						.And()
						.Condition(c => c.Id, FilterOperator.GreaterThan, -100)
					.End()
					.Or()
					.Begin()
						.Condition(c => c.OwnerInstance, "Atdi.Test.WebApi.RestOrm")
						.And()
						.Condition(c => c.Id, FilterOperator.GreaterThan, -100)
					.End()
					.Or()
					.Condition(c => c.CreatedDate, FilterOperator.LessEqual, DateTimeOffset.Now)
					.Or()
					.Condition(c => c.CreatedDate, FilterOperator.Between, DateTimeOffset.MinValue, DateTimeOffset.MaxValue)
				.EndFilter()
				.OnTop(100);

			var count = executor.Execute(webQuery);
			var records = executor.ExecuteAndFetch(webQuery, reader =>
			{
				var data = new IProject[reader.Count];
				var index = 0;
				while (reader.Read())
				{
					var project = new Project();
					project.Id = reader.GetValue(c => c.Id);
					project.Name = reader.GetValue(c => c.Name);
					project.Projection = reader.GetValue(c => c.Projection);
					project.CreatedDate = reader.GetValue(c => c.CreatedDate);
					project.StatusCode = reader.GetValue(c => c.StatusCode);
					project.Note = reader.GetValue(c => c.Note);
					project.StatusNote = reader.GetValue(c => c.StatusNote);
					project.OwnerProjectId = reader.GetValue(c => c.OwnerProjectId);
					project.StatusName = reader.GetValue(c => c.StatusName);
					project.OwnerInstance = reader.GetValue(c => c.OwnerInstance);
					data[index++] = project;
				}
				return data;
			});
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
