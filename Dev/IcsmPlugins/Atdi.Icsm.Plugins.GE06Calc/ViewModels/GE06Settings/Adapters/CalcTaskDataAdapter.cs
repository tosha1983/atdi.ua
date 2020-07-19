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
    public sealed class CalcTaskDataAdapter : EntityDataAdapter<CS_ES.Tasks.IGn06Args, CalcTaskModel>
    {
        public CalcTaskDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long ContextId;
        protected override void PrepareQuery(IReadQuery<CS_ES.Tasks.IGn06Args> query)
        {
            query.Select(
                c => c.TASK.Id,
                c => c.TASK.TypeName,
                c => c.TASK.StatusName,
                c => c.TASK.StatusNote,
                c => c.TASK.MapName,
                c => c.TASK.CreatedDate,
                c => c.TASK.OwnerInstance,
                c => c.CalculationTypeName)
            .Filter(f => f.TASK.CONTEXT.Id, ContextId)
            .OrderByDesc(o => o.TASK.Id);
        }
        protected override CalcTaskModel ReadData(IDataReader<CS_ES.Tasks.IGn06Args> reader, int index)
        {
            return new CalcTaskModel
            {
                Id = reader.GetValue(c => c.TASK.Id),
                TypeName = reader.GetValue(c => c.TASK.TypeName),
                StatusName = reader.GetValue(c => c.TASK.StatusName),
                StatusNote = reader.GetValue(c => c.TASK.StatusNote),
                MapName = reader.GetValue(c => c.TASK.MapName),
                CreatedDate = reader.GetValue(c => c.TASK.CreatedDate),
                OwnerInstance = reader.GetValue(c => c.TASK.OwnerInstance),
                TypeOfCalculation = reader.GetValue(c => c.CalculationTypeName)
            };
        }
    }
}
