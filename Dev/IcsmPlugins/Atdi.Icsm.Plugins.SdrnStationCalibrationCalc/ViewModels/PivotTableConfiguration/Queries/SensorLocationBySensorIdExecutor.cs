using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using IS_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using MP = Atdi.WpfControls.EntityOrm.Controls;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Queries
{
    public class SensorLocationBySensorIdExecutor : IReadQueryExecutor<SensorLocationBySensorId, MP.Location>
    {
        private readonly AppComponentConfig _config;
        private readonly InfocenterDataLayer _dataLayer;

        public SensorLocationBySensorIdExecutor(AppComponentConfig config, InfocenterDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public MP.Location Read(SensorLocationBySensorId criterion)
        {
            var listIds = new List<long>();
            var query = _dataLayer.GetBuilder<IS_ES.ISensorLocation>()
               .Read()
               .Select( c => c.Id,
                        c => c.Lat,
                        c => c.Lon)
               .Filter(f => f.SENSOR.Id, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, criterion.SensorId)
               .Filter(f => f.SENSOR.Status, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, "A")
               .OrderByDesc(o => o.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            var lat = reader.GetValue(c => c.Lat);
            var lon = reader.GetValue(c => c.Lon);

            if (lat.HasValue && lon.HasValue)
                return new MP.Location() { Lat = lat.Value, Lon = lon.Value };
            else
                return null;
        }
    }
}
