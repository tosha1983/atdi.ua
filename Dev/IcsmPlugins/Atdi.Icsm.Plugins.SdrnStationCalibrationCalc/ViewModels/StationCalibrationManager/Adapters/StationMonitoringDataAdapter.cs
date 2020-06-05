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
            )
            .Filter(f => f.MeasTime, DataModels.Api.EntityOrm.WebClient.FilterOperator.GreaterEqual, new DateTime(StartDateTime.Date.Year, StartDateTime.Date.Month, StartDateTime.Date.Day, 0, 0, 0, 1))
            .Filter(f => f.MeasTime, DataModels.Api.EntityOrm.WebClient.FilterOperator.LessEqual, new DateTime(StopDateTime.Date.Year, StopDateTime.Date.Month, StopDateTime.Date.Day, 23, 59, 59, 999));
        }
        protected override StationMonitoringModel ReadData(IDataReader<IC_ES.IStationMonitoring> reader, int index)
        {
            string standardStatistatics = "";
            var standardStats = reader.GetValueAs<DriveTestStandardStats[]>(c => c.STATS.StandardStats);
            if (standardStats!=null)
            {
                for (int i=0; i< standardStats.Length; i++)
                {
                    standardStatistatics += $"{standardStats[i].Standard}-{standardStats[i].Count};"; 
                }
                
            }
            return new StationMonitoringModel()
            {
                Id = reader.GetValue(c => c.Id),
                StandardStats = standardStatistatics,
                CountSID = reader.GetValue(c => c.STATS.GsidCount),
                Date = reader.GetValue(c => c.MeasTime),
                MaxFreq_MHz = reader.GetValue(c => c.STATS.MaxFreq_MHz),
                MinFreq_MHz = reader.GetValue(c => c.STATS.MinFreq_MHz),
                SensorName = reader.GetValue(c => c.SensorName),
                SensorTitle = reader.GetValue(c => c.SensorTitle),
                DriveTestStandardStats = standardStats
            };
        }
    }

}
