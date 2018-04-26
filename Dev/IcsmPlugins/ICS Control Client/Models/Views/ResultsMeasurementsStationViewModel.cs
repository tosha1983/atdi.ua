using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atdi.AppServer.Contracts.Sdrns;

namespace XICSM.ICSControlClient.Models.Views
{
    public class ResultsMeasurementsStationViewModel
    {
        public int? StationId { get; set; }

        public int? SectorId { get; set; }

        public string Status { get; set; }

        public string GlobalSID { get; set; }

        public string MeasGlobalSID { get; set; }

        public LevelMeasurementsCar[] LevelMeasurements { get; set; }

        public int? LevelMeasurementsLength { get; set; }


        public double? GeneralResultCentralFrequency { get; set; }

        public double? GeneralResultCentralFrequencyMeas { get; set; }

        public double? GeneralResultOffsetFrequency { get; set; }

        public decimal? GeneralResultSpecrumStartFreq { get; set; }

        public decimal? GeneralResultSpecrumSteps { get; set; }

        public float[] GeneralResultLevelsSpecrum { get; set; }

        public MaskElements[] GeneralResultMaskBW { get; set; }

        public int? GeneralResultT1 { get; set; }

        public int? GeneralResultT2 { get; set; }

        public int? GeneralResultMarkerIndex { get; set; }

        public double? GeneralResultDurationMeas { get; set; }

        public DateTime? GeneralResultTimeStartMeas { get; set; }

        public DateTime? GeneralResultTimeFinishMeas { get; set; }


        public int? NumberPointsOfSpectrum { get; set; }
        public double? T1 { get; set; }
        public double? T2 { get; set; }
        public double? Marker { get; set; }
    }
}
