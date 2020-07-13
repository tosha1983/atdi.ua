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
    public sealed class AllotmentOrAssignmentDataAdapter : EntityDataAdapter<CS_ES.IGn06AllotmentOrAssignmentResult, AllotmentOrAssignmentModel>
    {
        public AllotmentOrAssignmentDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long ResultId;
        protected override void PrepareQuery(IReadQuery<CS_ES.IGn06AllotmentOrAssignmentResult> query)
        {
            query.Select(
                c => c.Id,
                c => c.Gn06ResultId,
                c => c.Adm,
                c => c.TypeTable,
                c => c.Name,
                c => c.Freq_MHz,
                c => c.Longitude_DEC,
                c => c.Latitude_DEC,
                c => c.MaxEffHeight_m,
                c => c.Polar,
                c => c.ErpH_dbW,
                c => c.ErpV_dbW,
                c => c.AntennaDirectional,
                c => c.AdmRefId,
                c => c.CountoursPoints)
            .Filter(f => f.Gn06ResultId, ResultId);
        }
        protected override AllotmentOrAssignmentModel ReadData(IDataReader<CS_ES.IGn06AllotmentOrAssignmentResult> reader, int index)
        {
            return new AllotmentOrAssignmentModel
            {
                Id = reader.GetValue(c => c.Id),
                Gn06ResultId = reader.GetValue(c => c.Gn06ResultId),
                Adm = reader.GetValue(c => c.Adm),
                TypeTable = reader.GetValue(c => c.TypeTable),
                Name = reader.GetValue(c => c.Name),
                Freq_MHz = reader.GetValue(c => c.Freq_MHz),
                Longitude_DEC = reader.GetValue(c => c.Longitude_DEC),
                Latitude_DEC = reader.GetValue(c => c.Latitude_DEC),
                MaxEffHeight_m = reader.GetValue(c => c.MaxEffHeight_m),
                Polar = reader.GetValue(c => c.Polar),
                ErpH_dbW = reader.GetValue(c => c.ErpH_dbW),
                ErpV_dbW = reader.GetValue(c => c.ErpV_dbW),
                AntennaDirectional = reader.GetValue(c => c.AntennaDirectional),
                AdmRefId = reader.GetValue(c => c.AdmRefId),
                CountoursPoints = string.IsNullOrEmpty(reader.GetValue(c => c.CountoursPoints)) ? null : reader.GetValueAs<CountoursPoint[]>(c => c.CountoursPoints)
            };
        }
    }
}