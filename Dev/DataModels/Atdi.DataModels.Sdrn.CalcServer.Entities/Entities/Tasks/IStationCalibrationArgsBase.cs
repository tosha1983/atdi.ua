using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{

    [Entity]
    public interface IStationCalibrationArgsBase
    {
        long[] InfocMeasResults { get; set; }
        long[] StationIds { get; set; }
        float? CorrelationThresholdHard { get; set; }
        float? CorrelationThresholdWeak { get; set; }
        float? TrustOldResults { get; set; }
        bool UseMeasurementSameGSID { get; set; }
        int? DistanceAroundContour_km { get; set; }
        int? MinNumberPointForCorrelation { get; set; }
        float? MinRangeMeasurements_dBmkV { get; set; }
        float? MaxRangeMeasurements_dBmkV { get; set; }
        int? CorrelationDistance_m { get; set; }
        float? Delta_dB { get; set; }
        float? MaxAntennasPatternLoss_dB { get; set; }
        bool Detail { get; set; }
        bool AltitudeStation { get; set; }
        int? ShiftAltitudeStationMin_m { get; set; }
        int? ShiftAltitudeStationMax_m { get; set; }
        int? ShiftAltitudeStationStep_m { get; set; }
        int? MaxDeviationAltitudeStation_m { get; set; }
        bool TiltStation { get; set; }
        float? ShiftTiltStationMin_deg { get; set; }
        float? ShiftTiltStationMax_deg { get; set; }
        float? ShiftTiltStationStep_deg { get; set; }
        float? MaxDeviationTiltStation_deg { get; set; }
        bool AzimuthStation { get; set; }
        float? ShiftAzimuthStationMin_deg { get; set; }
        float? ShiftAzimuthStationMax_deg { get; set; }
        float? ShiftAzimuthStationStep_deg { get; set; }
        float? MaxDeviationAzimuthStation_deg { get; set; }
        bool CoordinatesStation { get; set; }
        int? ShiftCoordinatesStation_m { get; set; }
        int? ShiftCoordinatesStationStep_m { get; set; }
        int? MaxDeviationCoordinatesStation_m { get; set; }
        bool PowerStation { get; set; }
        float? ShiftPowerStationMin_dB { get; set; }
        float? ShiftPowerStationMax_dB { get; set; }
        float? ShiftPowerStationStep_dB { get; set; }
        bool CascadeTuning { get; set; }
        int? NumberCascade { get; set; }
        int? DetailOfCascade { get; set; }
        byte? Method { get; set; }
    }
    
}
