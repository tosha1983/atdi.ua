using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;

namespace XICSM.ICSControlClient.Models.Views
{
    public class DataSynchronizationProcessProtocolsViewModel
    {
        public long? Id { get; set; }
        public DateTime? DateMeas { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string StandardName { get; set; }
        public string GlobalSID { get; set; }
        public string PermissionGlobalSID { get; set; }
        public string StationTxFreq { get; set; }
        public string StationTxChannel { get; set; }
        public string StationRxChannel { get; set; }
        public string StatusMeas { get; set; }
        public string StatusMeasFull { get; set; }
        public string SensorName { get; set; }
        public string DesigEmission { get; set; }
        public double? Freq_MHz { get; set; }
        public double? BandWidth { get; set; }
        public double? RadioControlMeasFreq_MHz { get; set; }
        public double? RadioControlBandWidth_KHz { get; set; }
        public double? RadioControlDeviationFreq_MHz { get; set; }
        public double? Level_dBm { get; set; }
        public double? FieldStrength { get; set; }
        public DateTime? DateMeas_OnlyDate { get; set; }
        public TimeSpan? DateMeas_OnlyTime { get; set; }
        public TimeSpan? DurationMeasurement { get; set; }

        public double? SensorLongitude { get; set; }
        public double? SensorLatitude { get; set; }
        public string SensorCoordinates { get; set; }
        public string SensorCoordinatesLat { get; set; }
        public string SensorCoordinatesLon { get; set; }

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string Coordinates { get; set; }
        public string CoordinatesLat { get; set; }
        public string CoordinatesLon { get; set; }

        public string OwnerName { get; set; }
        public string Standard { get; set; }
        public string Address { get; set; }
        public string PermissionNumber { get; set; }
        public DateTime? PermissionStart { get; set; }
        public DateTime? PermissionStop { get; set; }
        public string TitleSensor { get; set; }
        public string CurentStatusStation { get; set; }
        public ProtocolsWithEmittings ProtocolsLinkedWithEmittings { get; set; }
    }
}
