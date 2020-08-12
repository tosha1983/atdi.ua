using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries
{
    public class GetResultStatusByIdExecutor : IReadQueryExecutor<GetResultStatusById, byte?>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetResultStatusByIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public byte? Read(GetResultStatusById criterion)
        {
            var query = _dataLayer.GetBuilder<ICalcResult>()
                .Read()
                .Select(c => c.Id)
                .Select(c => c.StatusCode)
                .Filter(c => c.Id, criterion.ResultId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return reader.GetValue(c => c.StatusCode);
        }
    }
}
