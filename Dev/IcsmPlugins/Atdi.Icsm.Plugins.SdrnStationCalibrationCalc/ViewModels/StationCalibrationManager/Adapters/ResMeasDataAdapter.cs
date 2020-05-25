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


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Adapters
{
    public sealed class ResMeasDataAdapter : EntityDataAdapter<IC_ES.IStationMonitoring, ResMeasModel>
    {
        public ResMeasDataAdapter(InfocenterDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
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
            ).Filter(f => f.MeasTime, DataModels.Api.EntityOrm.WebClient.FilterOperator.Between, StartDateTime, StopDateTime);
        }
        protected override ResMeasModel ReadData(IDataReader<IC_ES.IStationMonitoring> reader, int index)
        {
            return new ResMeasModel
            {
                Id = reader.GetValue(c => c.Id),
                Date  = reader.GetValue(c => c.MeasTime),
                //CountByStandard =  (DriveTestStandardStats)reader.GetValue(c => c.STATS.StandardStats), ?????????????????????????????
                CountSID = reader.GetValue(c => c.STATS.GsidCount),
                MaxFreq_MHz = reader.GetValue(c => c.STATS.MaxFreq_MHz),
                MinFreq_MHz = reader.GetValue(c => c.STATS.MinFreq_MHz),
                SensorName = reader.GetValue(c => c.SensorName),
                SensorTitle = reader.GetValue(c => c.SensorTitle),
                //Standards = (DriveTestStandardStats)reader.GetValue(c => c.STATS.StandardStats), ?????????????????????????????
            };
        }
    }

}
