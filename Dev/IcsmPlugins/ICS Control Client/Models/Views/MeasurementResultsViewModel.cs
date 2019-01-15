using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.AppServer.Contracts.Sdrns;

namespace XICSM.ICSControlClient.Models.Views
{
    public class MeasurementResultsViewModel
    {
        public int MeasSdrResultsId { get; set; }

        public int MeasTaskId { get; set; }

        public int SubMeasTaskId { get; set; }

        public int SubMeasTaskStationId { get; set; }

        public double? AntVal { get; set; }

        public DateTime TimeMeas { get; set; }

        public int? DataRank { get; set; }

        public int? N { get; set; }

        public string Status { get; set; }

        public MeasurementType TypeMeasurements { get; set; }

        public int StationMeasurementsStationId { get; set; }

        public LocationSensorMeasurement[] LocationSensorMeasurement { get; set; }

        public FrequencyMeasurement[] FrequenciesMeasurements { get; set; }

        public MeasurementResult[] MeasurementsResults { get; set; }

        public ResultsMeasurementsStation[] ResultsMeasStation { get; set; }

        public double? LowFreq { get; set; }
        public double? UpFreq { get; set; }

        public int? MeasDeviceId { get; set; }
        public int? StationsNumber { get; set; }
        public int? PointsNumber { get; set; }
        public string SensorName { get; set; }
        public string SensorTechId { get; set; }
        public int? CountStationMeasurements { get; set; }
        public int? CountUnknownStationMeasurements { get; set; }
    }
}
