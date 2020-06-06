using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Queries
{
    public class DriveTestPointsResultsModelByIdExecutor : IReadQueryExecutor<DriveTestPointsResultsModelById, DriveTestPoint[]>
    {
        private readonly AppComponentConfig _config;
        private readonly InfocenterDataLayer _dataLayer;

        public DriveTestPointsResultsModelByIdExecutor(AppComponentConfig config, InfocenterDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public DriveTestPoint[] Read(DriveTestPointsResultsModelById criterion)
        {
            var listStationCalibrationStaModel = new List<StationCalibrationStaModel>();
            var query = _dataLayer.GetBuilder<IC_ES.IDriveTestPoints>()
               .Read()
               .Select(
                 c => c.Id,
                 c => c.Points,
                 c => c.DRIVE_TEST.Id
             ).Filter(f => f.DRIVE_TEST.Id, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }
            return  reader.GetValueAs<DriveTestPoint[]>(c => c.Points);
        }
    }
}
