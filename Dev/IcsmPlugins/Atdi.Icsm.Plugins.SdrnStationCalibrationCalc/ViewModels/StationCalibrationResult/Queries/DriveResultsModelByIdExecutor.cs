using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Queries
{
    public class DriveResultsModelByIdExecutor : IReadQueryExecutor<DriveResultsModelById, long?>
    {
        private readonly AppComponentConfig _config;
        private readonly InfocenterDataLayer _dataLayer;

        public DriveResultsModelByIdExecutor(AppComponentConfig config, InfocenterDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public long? Read(DriveResultsModelById criterion)
        {
            var listStationCalibrationStaModel = new List<StationCalibrationStaModel>();
            var query = _dataLayer.GetBuilder<IC_ES.IDriveTest>()
               .Read()
               .Select(
                 c => c.Id,
                 c => c.RESULT.Id
             ).Filter(f => f.Id, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }
            return reader.GetValue(c => c.RESULT.Id);
        }
    }
}
