using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Queries
{
    public class GetProjectByIdExecutor : IReadQueryExecutor<GetProjectById, ProjectModel>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetProjectByIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public ProjectModel Read(GetProjectById criterion)
        {
            var query = _dataLayer.GetBuilder<IProject>()
                .Read()
                .Select(
                    c => c.Id,
                    c => c.Name,
                    c => c.Note,
                    c => c.Projection)
                .Filter(c => c.Id, criterion.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return new ProjectModel()
            {
                Id = reader.GetValue(c => c.Id),
                Name = reader.GetValue(c => c.Name),
                Note = reader.GetValue(c => c.Note),
                Projection = reader.GetValue(c => c.Projection)
            };
        }
    }
}
