using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Adapters
{
    public sealed class ProjectMapDataAdapter : EntityDataAdapter<CS_ES.IProjectMap, ProjectMapModel>
    {
        public ProjectMapDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long ProjectId;
        protected override void PrepareQuery(IReadQuery<CS_ES.IProjectMap> query)
        {
            query.Select(
                c => c.Id,
                c => c.MapName,
                c => c.MapNote,
                c => c.CreatedDate
            ).Filter(f => f.PROJECT.Id, ProjectId);
        }
        protected override ProjectMapModel ReadData(IDataReader<CS_ES.IProjectMap> reader, int index)
        {
            return new ProjectMapModel
            {
                Id = reader.GetValue(c => c.Id),
                MapName = reader.GetValue(c => c.MapName),
                MapNote = reader.GetValue(c => c.MapNote),
                CreatedDate = reader.GetValue(c => c.CreatedDate)
            };
        }
    }

}
