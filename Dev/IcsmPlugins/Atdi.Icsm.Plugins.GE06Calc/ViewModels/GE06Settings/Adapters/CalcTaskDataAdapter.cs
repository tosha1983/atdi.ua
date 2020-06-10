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

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings.Adapters
{
    public sealed class CalcTaskDataAdapter : EntityDataAdapter<CS_ES.ICalcTask, CalcTaskModel>
    {
        public CalcTaskDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long ContextId;
        protected override void PrepareQuery(IReadQuery<CS_ES.ICalcTask> query)
        {
            query.Select(
                c => c.Id,
                c => c.TypeName,
                c => c.StatusName,
                c => c.StatusNote,
                c => c.MapName,
                c => c.CreatedDate,
                c => c.OwnerInstance)
            .Filter(f => f.CONTEXT.Id, ContextId);
        }
        protected override CalcTaskModel ReadData(IDataReader<CS_ES.ICalcTask> reader, int index)
        {
            return new CalcTaskModel
            {
                Id = reader.GetValue(c => c.Id),
                TypeName = reader.GetValue(c => c.TypeName),
                StatusName = reader.GetValue(c => c.StatusName),
                StatusNote = reader.GetValue(c => c.StatusNote),
                MapName = reader.GetValue(c => c.MapName),
                CreatedDate = reader.GetValue(c => c.CreatedDate),
                OwnerInstance = reader.GetValue(c => c.OwnerInstance)
            };
        }
    }
}
