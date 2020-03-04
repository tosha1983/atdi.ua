using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Environment.Wpf;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class DataSynchronizationProcessProtocolsViewModel
    {
        public long? Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public Status Status { get; set; }

        public string SensorName { get; set; }
        public string Administration { get; set; }
        public string NetworkId { get; set; }
        public string Remark { get; set; }
        public DateTime? BiuseDate { get; set; }
        public DateTime? EouseDate { get; set; }
        public Double? Azimuth { get; set; }
        public Double? Elevation { get; set; }
        public Double? AGL { get; set; }
        public string TypeSensor { get; set; }
        public Double? StepMeasTime { get; set; }
        public Double? RxLoss { get; set; }
        public Double? OpHHFr { get; set; }
        public Double? OpHHTo { get; set; }
        public string OpDays { get; set; }
        public SDR.SensorLocation[] Locations { get; set; }


        public string Standard { get; set; }
        public string StandardName { get; set; }
        public string OwnerName { get; set; }
        public string PermissionNumber { get; set; }
        public DateTime? PermissionStart { get; set; }
        public DateTime? PermissionStop { get; set; }
        public string Address { get; set; }
        public DataLocation Location { get; set; }
        public double? BandWidth { get; set; }
        public string DesigEmission { get; set; }
        public string Province { get; set; }

        public long SensorId { get; set; }
        public string GlobalSID { get; set; }
        public double Freq_MHz { get; set; }
        public double Level_dBm { get; set; }
        public double? DispersionLow { get; set; }
        public double? DispersionUp { get; set; }
        public double? Percent { get; set; }
        public DateTime DateMeas { get; set; }

        public double? RadioControlMeasFreq_MHz { get; set; }
        public double? RadioControlBandWidth { get; set; }

        public SDR.ProtocolsWithEmittings ProtocolsLinkedWithEmittings { get; set; }
    }
}
