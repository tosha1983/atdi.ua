using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Adapters
{
    public sealed class ProjectDataAdapter : EntityDataAdapter<CS_ES.IProject, ProjectModel>
    {
        public ProjectDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        protected override void PrepareQuery(IReadQuery<CS_ES.IProject> query)
        {
            query.Select(
                c => c.Id,
                c => c.Name,
                c => c.Note,
                c => c.CreatedDate,
                c => c.Projection,
                c => c.StatusName,
                c => c.OwnerInstance)
                .Filter(f => f.StatusCode, 2)
                .OrderByDesc(o => o.Id);
        }
        protected override ProjectModel ReadData(IDataReader<CS_ES.IProject> reader, int index)
        {
            return new ProjectModel
            {
                Id = reader.GetValue(c => c.Id),
                Name = reader.GetValue(c => c.Name),
                Note = reader.GetValue(c => c.Note),
                CreatedDate = reader.GetValue(c => c.CreatedDate),
                Projection = reader.GetValue(c => c.Projection),
                StatusName = reader.GetValue(c => c.StatusName),
                OwnerInstance = reader.GetValue(c => c.OwnerInstance)
            };
        }
    }
}
