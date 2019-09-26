using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class MeasurementResultsViewModel 
    {
        public long MeasSdrResultsId { get; set; }
        public long MeasTaskId { get; set; }
        public long SubMeasTaskId { get; set; }
        public long SubMeasTaskStationId { get; set; }
        public double? AntVal { get; set; }
        public DateTime TimeMeas { get; set; }
        public int? DataRank { get; set; }
        public int? N { get; set; }
        public string Status { get; set; }
        public MeasurementType TypeMeasurements { get; set; }
        public long StationMeasurementsStationId { get; set; }
        public LocationSensorMeasurement[] LocationSensorMeasurement { get; set; }
        public FrequencyMeasurement[] FrequenciesMeasurements { get; set; }
        public MeasurementResult[] MeasurementsResults { get; set; }
        public ResultsMeasurementsStation[] ResultsMeasStation { get; set; }
        public double? LowFreq { get; set; }
        public double? UpFreq { get; set; }
        public long? MeasDeviceId { get; set; }
        public int? StationsNumber { get; set; }
        public int? PointsNumber { get; set; }
        public string SensorName { get; set; }
        public string SensorTechId { get; set; }
        public int? CountStationMeasurements { get; set; }
        public int? CountUnknownStationMeasurements { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public int? ScansNumber { get; set; }

    }
}
