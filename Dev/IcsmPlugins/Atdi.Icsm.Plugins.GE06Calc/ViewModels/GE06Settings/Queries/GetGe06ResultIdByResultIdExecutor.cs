using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings.Queries
{
    public class GetGe06ResultIdByResultIdExecutor : IReadQueryExecutor<GetGe06ResultIdByResultId, long?>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetGe06ResultIdByResultIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public long? Read(GetGe06ResultIdByResultId criterion)
        {
            var queryGN = _dataLayer.GetBuilder<CS_ES.IGn06Result>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.RESULT.Id, criterion.ResultId);

            var readerGN = _dataLayer.Executor.ExecuteReader(queryGN);
            if (!readerGN.Read())
            {
                return null;
            }

            return readerGN.GetValue(c => c.Id);
        }
    }
}
