using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Adapters
{
    public sealed class BaseClientContextDataAdapter : EntityDataAdapter<CS_ES.IClientContext, ClientContextModel>
    {
        public BaseClientContextDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long ProjectId;
        protected override void PrepareQuery(IReadQuery<CS_ES.IClientContext> query)
        {
            query.Select(
                c => c.Id,
                c => c.Name,
                c => c.Note,
                c => c.CreatedDate,
                c => c.TypeName,
                c => c.StatusName,
                c => c.StatusNote,
                c => c.OwnerInstance)
            .Filter(f => f.PROJECT.Id, ProjectId)
            .Filter(f => f.TypeCode, 1);
        }
        protected override ClientContextModel ReadData(IDataReader<CS_ES.IClientContext> reader, int index)
        {
            return new ClientContextModel
            {
                Id = reader.GetValue(c => c.Id),
                Name = reader.GetValue(c => c.Name),
                Note = reader.GetValue(c => c.Note),
                CreatedDate = reader.GetValue(c => c.CreatedDate),
                TypeName = reader.GetValue(c => c.TypeName),
                StatusName = reader.GetValue(c => c.StatusName),
                StatusNote = reader.GetValue(c => c.StatusNote),
                OwnerInstance = reader.GetValue(c => c.OwnerInstance)
            };
        }
    }
}
