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

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class StationMonitoringModelByIdExecutor : IReadQueryExecutor<StationMonitoringModelById, StationMonitoringModel>
    {
        private readonly AppComponentConfig _config;
        private readonly InfocenterDataLayer _dataLayer;

        public StationMonitoringModelByIdExecutor(AppComponentConfig config, InfocenterDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public StationMonitoringModel Read(StationMonitoringModelById criterion)
        {
            var query = _dataLayer.GetBuilder<IC_ES.IStationMonitoring>()
                .Read()
                .Select(
                    c => c.Id,
                    c => c.CreatedDate,
                    c => c.SensorName,
                    c => c.SensorTitle,
                    c => c.STATS.GsidCount,
                    c => c.STATS.MaxFreq_MHz,
                    c => c.STATS.MinFreq_MHz,
                    c => c.STATS.StandardStats,
                    c => c.StatusCode,
                    c => c.StatusName,
                    c => c.StatusNote
                    )
                .Filter(c => c.Id, criterion.Id);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return new StationMonitoringModel()
            {
                Id = reader.GetValue(c => c.Id),
                CountByStandard = reader.GetValueAs<DriveTestStandardStats>(c => c.STATS.StandardStats).Count,
                CountSID = reader.GetValue(c => c.STATS.GsidCount),
                Date = reader.GetValue(c => c.MeasTime),
                MaxFreq_MHz = reader.GetValue(c => c.STATS.MaxFreq_MHz),
                MinFreq_MHz = reader.GetValue(c => c.STATS.MinFreq_MHz),
                SensorName = reader.GetValue(c => c.SensorName),
                SensorTitle = reader.GetValue(c => c.SensorTitle),
                Standards = reader.GetValueAs<DriveTestStandardStats>(c => c.STATS.StandardStats).Standard,
            };
        }
    }
}
