using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries
{
    public class GetProjectMapByClientContextIdExecutor : IReadQueryExecutor<GetProjectMapByClientContextId, ProjectMapsModel>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetProjectMapByClientContextIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public ProjectMapsModel Read(GetProjectMapByClientContextId criterion)
        {
            var queryProject = _dataLayer.GetBuilder<IClientContext>()
                .Read()
                .Select(
                    c => c.Id,
                    c => c.PROJECT.Id)
                .Filter(c => c.Id, criterion.ContextId);

            var readerProject = _dataLayer.Executor.ExecuteReader(queryProject);
            if (!readerProject.Read())
            {
                return null;
            }

            var projectId = readerProject.GetValue(c => c.PROJECT.Id);

            var query = _dataLayer.GetBuilder<IProjectMap>()
                .Read()
                .Select(
                    c => c.Id,
                    c => c.PROJECT.Id,
                    c => c.MapName)
                .Filter(c => c.PROJECT.Id, projectId)
                .OnTop(1);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return new ProjectMapsModel()
            {
                ProjectId = reader.GetValue(c => c.PROJECT.Id),
                MapName = reader.GetValue(c => c.MapName)
            };
        }
    }
}
