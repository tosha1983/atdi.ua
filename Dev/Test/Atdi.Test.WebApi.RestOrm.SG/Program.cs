using Atdi.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrns.Server.Entities;

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
            var executor = dataLayer.GetExecutor(endpoint, dataContext);

            //TestReadMethod(executor, dataLayer);

        }

        /*
        static void TestReadMethod(IQueryExecutor executor, WebApiDataLayer dataLayer)
        {
            var webQuery = dataLayer.GetBuilder<IMeasTask>()
                .Read()
                .Select(
                   c => c.Id,
                   c => c.Name
                )
                .OrderByAsc(c => c.Id)
                .BeginFilter()
                    .Condition(c => c.Id, FilterOperator.ween, DateTimeOffset.MinValue, DateTimeOffset.MaxValue)
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
        */
    }
}
