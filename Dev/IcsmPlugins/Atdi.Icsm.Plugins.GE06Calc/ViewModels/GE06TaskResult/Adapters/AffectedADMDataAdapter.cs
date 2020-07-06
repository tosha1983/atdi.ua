using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06TaskResult.Adapters
{
    public sealed class AffectedADMDataAdapter : EntityDataAdapter<CS_ES.IGn06AffectedADMResult, AffectedADMModel>
    {
        public AffectedADMDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long ResultId;
        protected override void PrepareQuery(IReadQuery<CS_ES.IGn06AffectedADMResult> query)
        {
            query.Select(
                c => c.Id,
                c => c.Gn06ResultId,
                c => c.Adm,
                c => c.TypeAffected,
                c => c.AffectedServices)
            .Filter(f => f.Gn06ResultId, ResultId);
        }
        protected override AffectedADMModel ReadData(IDataReader<CS_ES.IGn06AffectedADMResult> reader, int index)
        {
            return new AffectedADMModel
            {
                Id = reader.GetValue(c => c.Id),
                Gn06ResultId = reader.GetValue(c => c.Gn06ResultId),
                Adm = reader.GetValue(c => c.Adm),
                TypeAffected = reader.GetValue(c => c.TypeAffected),
                AffectedServices = reader.GetValue(c => c.AffectedServices)
            };
        }
    }
}