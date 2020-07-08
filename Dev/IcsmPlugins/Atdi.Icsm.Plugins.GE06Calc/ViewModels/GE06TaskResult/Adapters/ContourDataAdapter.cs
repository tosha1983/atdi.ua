using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06TaskResult.Adapters
{
    public sealed class ContourDataAdapter : EntityDataAdapter<CS_ES.IGn06ContoursResult, ContourModel>
    {
        public ContourDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long ResultId;
        protected override void PrepareQuery(IReadQuery<CS_ES.IGn06ContoursResult> query)
        {
            query.Select(
                c => c.Id,
                c => c.Gn06ResultId,
                c => c.ContourType,
                c => c.Distance,
                c => c.FS,
                c => c.AffectedADM,
                c => c.PointsCount,
                c => c.CountoursPoints)
            .Filter(f => f.Gn06ResultId, ResultId);
        }
        protected override ContourModel ReadData(IDataReader<CS_ES.IGn06ContoursResult> reader, int index)
        {
            return new ContourModel
            {
                Id = reader.GetValue(c => c.Id),
                Gn06ResultId = reader.GetValue(c => c.Gn06ResultId),
                ContourType = reader.GetValue(c => c.ContourType),
                Distance = reader.GetValue(c => c.Distance),
                FS = reader.GetValue(c => c.FS),
                AffectedADM = reader.GetValue(c => c.AffectedADM),
                PointsCount = reader.GetValue(c => c.PointsCount),
                CountoursPoints = reader.GetValueAs<CountoursPoint[]>(c => c.CountoursPoints)
            };
        }
    }
}