﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult
{
    public class StationCalibrationResultModel
    {
        public long Id { get; set; }
        public long ResultId { get; set; }

        public long ParametersId { get; set; }
        public DateTimeOffset TimeStart { get; set; }
        public string Standard { get; set; }
        public string AreaName { get; set; }
        public int NumberStation { get; set; }
        public int NumberStationInContour { get; set; }
        public int CountStation_CS { get; set; }
        public int CountStation_NS { get; set; }
        public int CountStation_IT { get; set; }
        public int CountStation_NF { get; set; }
        public int CountStation_UN { get; set; }
        public int CountMeasGSID { get; set; }
        public int CountMeasGSID_LS { get; set; }
        public int CountMeasGSID_IT { get; set; }
        public int PercentComplete { get; set; }
        public float? CorrelationThresholdHard { get; set; }
        public float? CorrelationThresholdWeak { get; set; }
        public bool TrustOldResults { get; set; }
        public bool UseMeasurementSameGSID { get; set; }
        public int? DistanceAroundContour_km { get; set; }
        public int? MinNumberPointForCorrelation { get; set; }
        public float? MinRangeMeasurements_dBmkV { get; set; }
        public float? MaxRangeMeasurements_dBmkV { get; set; }
        public int? CorrelationDistance_m { get; set; }
        public float? Delta_dB { get; set; }
        public float? MaxAntennasPatternLoss_dB { get; set; }
        public bool Detail { get; set; }
        public bool AltitudeStation { get; set; }
        public int? ShiftAltitudeStationMin_m { get; set; }
        public int? ShiftAltitudeStationMax_m { get; set; }
        public int? ShiftAltitudeStationStep_m { get; set; }
        public int? MaxDeviationAltitudeStation_m { get; set; }
        public bool TiltStation { get; set; }
        public float? ShiftTiltStationMin_deg { get; set; }
        public float? ShiftTiltStationMax_deg { get; set; }
        public float? ShiftTiltStationStep_deg { get; set; }
        public float? MaxDeviationTiltStation_deg { get; set; }
        public bool AzimuthStation { get; set; }
        public float? ShiftAzimuthStationMin_deg { get; set; }
        public float? ShiftAzimuthStationMax_deg { get; set; }
        public float? ShiftAzimuthStationStep_deg { get; set; }
        public float? MaxDeviationAzimuthStation_deg { get; set; }
        public bool CoordinatesStation { get; set; }
        public int? ShiftCoordinatesStation_m { get; set; }
        public int? ShiftCoordinatesStationStep_m { get; set; }
        public int? MaxDeviationCoordinatesStation_m { get; set; }
        public bool PowerStation { get; set; }
        public float? ShiftPowerStationMin_dB { get; set; }
        public float? ShiftPowerStationMax_dB { get; set; }
        public float? ShiftPowerStationStep_dB { get; set; }
        public bool CascadeTuning { get; set; }
        public int? NumberCascade { get; set; }
        public int? DetailOfCascade { get; set; }
        public byte? Method { get; set; }
    }
}
