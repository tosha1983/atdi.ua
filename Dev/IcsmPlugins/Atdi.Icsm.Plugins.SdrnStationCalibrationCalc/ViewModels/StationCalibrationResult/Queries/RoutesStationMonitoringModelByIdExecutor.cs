using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Queries
{
    public class RoutesStationMonitoringModelByIdExecutor : IReadQueryExecutor<RoutesStationMonitoringModelById, RoutesStationMonitoringModel[]>
    {
        private readonly AppComponentConfig _config;
        private readonly InfocenterDataLayer _dataLayer;

        public RoutesStationMonitoringModelByIdExecutor(AppComponentConfig config, InfocenterDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public RoutesStationMonitoringModel[] Read(RoutesStationMonitoringModelById criterion)
        {
            var listRoutes = new List<RoutesStationMonitoringModel>();
            var query = _dataLayer.GetBuilder<IC_ES.IDriveRoute>()
                .Read()
                .Select(
                    c => c.Id,
                    c => c.Altitude,
                    c => c.Latitude,
                    c => c.Longitude
                    )
                .Filter(c => c.RESULT.Id, criterion.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            while (reader.Read())
            {
                listRoutes.Add(new RoutesStationMonitoringModel()
                {
                    Longitude = reader.GetValue(c => c.Longitude),
                    Latitude = reader.GetValue(c => c.Latitude),
                    Altitude_m = reader.GetValue(c => c.Altitude),
                    Id = reader.GetValue(c => c.Id)
                });
            }
            return listRoutes.ToArray();
        }
    }
}
