﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Models.Views
{
    public class ResultsMeasurementsStationViewModel
    {
        public string StationSysInfo { get; set; }
        public string StationId { get; set; }
        public long? SectorId { get; set; }
        public string Status { get; set; }
        public string GlobalSID { get; set; }
        public string MeasGlobalSID { get; set; }
        public LevelMeasurementsCar[] LevelMeasurements { get; set; }
        public long Id { get; set; }
        public string Standard { get; set; }
        public double? CentralFrequencyMHz { get; set; }
        public double? CentralFrequencyMeas_MHz { get; set; }
        public MeasurementsParameterGeneral[] GeneralResults { get; set; }
        public int? CountSpectrums { get; set; }
        public int? CountLevelMeas { get; set; }
    }
}
