using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Adapters
{
    public sealed class StationMonitoringDataAdapter : EntityDataAdapter<IC_ES.IStationMonitoring, StationMonitoringModel>
    {
        public StationMonitoringDataAdapter(InfocenterDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public DateTime StartDateTime;
        public DateTime StopDateTime;
        protected override void PrepareQuery(IReadQuery<IC_ES.IStationMonitoring> query)
        {
            query.Select(
                c => c.Id,
                c => c.CreatedDate,
                c => c.MeasTime,
                c => c.SensorName,
                c => c.SensorTitle,
                c => c.STATS.MaxFreq_MHz,
                c => c.STATS.MinFreq_MHz,
                c => c.STATS.StandardStats,
                c => c.STATS.GsidCount,
                c => c.StatusCode,
                c => c.StatusName,
                c => c.StatusNote
            ).Filter(f => f.MeasTime, DataModels.Api.EntityOrm.WebClient.FilterOperator.GreaterEqual, StartDateTime.Date)
            .Filter(f => f.MeasTime, DataModels.Api.EntityOrm.WebClient.FilterOperator.LessEqual, StopDateTime.Date);
        }
        protected override StationMonitoringModel ReadData(IDataReader<IC_ES.IStationMonitoring> reader, int index)
        {
            return new StationMonitoringModel
            {
                Id = reader.GetValue(c => c.Id),
                Date  = reader.GetValue(c => c.MeasTime),
                CountByStandard = reader.GetValueAs<DriveTestStandardStats>(c => c.STATS.StandardStats).Count,
                CountSID = reader.GetValue(c => c.STATS.GsidCount),
                MaxFreq_MHz = reader.GetValue(c => c.STATS.MaxFreq_MHz),
                MinFreq_MHz = reader.GetValue(c => c.STATS.MinFreq_MHz),
                SensorName = reader.GetValue(c => c.SensorName),
                SensorTitle = reader.GetValue(c => c.SensorTitle),
                Standards = reader.GetValueAs<DriveTestStandardStats>(c => c.STATS.StandardStats).Standard,
            };
        }
    }

}
