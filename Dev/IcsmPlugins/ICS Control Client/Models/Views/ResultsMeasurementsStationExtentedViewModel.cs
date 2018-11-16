using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atdi.AppServer.Contracts.Sdrns;

namespace XICSM.ICSControlClient.Models.Views
{
    public class ResultsMeasurementsStationExtentedViewModel
    {
        public int Id { get; set; }
        public string StationId { get; set; }
        public int? SectorId { get; set; }
        public string Status { get; set; }
        public string GlobalSID { get; set; }
        public string MeasGlobalSID { get; set; }
        public string Standard { get; set; }
        public double? CentralFrequencyMHz { get; set; }
        public double? CentralFrequencyMeas_MHz { get; set; }
        public string StationSysInfo { get; set; }
    }
}
