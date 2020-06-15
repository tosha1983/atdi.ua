using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Queries
{
    public class CalcResultsModelByIdExecutor : IReadQueryExecutor<CalcResultsModelById, long?>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public CalcResultsModelByIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public long? Read(CalcResultsModelById criterion)
        {
            var query = _dataLayer.GetBuilder<CS_ES.ICalcResult>()
                .Read()
                .Select(c => c.Id)
                    .Select(c => c.TASK.Id)
                    .Filter(c => c.Id, criterion.ResultId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }
            return  reader.GetValue(c => c.TASK.Id);
        }
    }
}
