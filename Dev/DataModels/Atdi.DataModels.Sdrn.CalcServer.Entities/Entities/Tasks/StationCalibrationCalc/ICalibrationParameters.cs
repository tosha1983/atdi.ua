using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface ICalibrationParameters_PK
    {
		long TaskId { get; set; }
	}

	[Entity]
	public interface ICalibrationParameters : ICalibrationParameters_PK
    {
        ICalcTask TASK { get; set; }
        float СorrelationThresholdHard { get; set; }
        float СorrelationThresholdWeak { get; set; }
        float TrustOldResults { get; set; }
        bool UseMeasurementSameGSID { get; set; }
        int DistanceAroundContour_km { get; set; }
        int MinNumberPointForCorrelation { get; set; }
        bool AltitudeStation { get; set; }
        int ShiftAltitudeStationMin_m { get; set; }
        int ShiftAltitudeStationMax_m { get; set; }
        int ShiftAltitudeStationStep_m { get; set; }
        int MaxDeviationAltitudeStation_m { get; set; }
        bool TiltStation { get; set; }
        float ShiftTiltStationMin_Deg { get; set; }
        float ShiftTiltStationMax_Deg { get; set; }
        float ShiftTiltStationStep_Deg { get; set; }
        float MaxDeviationTiltStationDeg { get; set; }
        bool AzimuthStation { get; set; }
        float ShiftAzimuthStationMin_deg { get; set; }
        float ShiftAzimuthStationMax_deg { get; set; }
        float ShiftAzimuthStationStep_deg { get; set; }
        float MaxDeviationAzimuthStation_deg { get; set; }
        bool CoordinatesStation { get; set; }
        int ShiftCoordinatesStation_m { get; set; }
        int ShiftCoordinatesStationStep_m { get; set; }
        int MaxDeviationCoordinatesStation_m { get; set; }
        bool PowerStation { get; set; }
        float ShiftPowerStationMin_dB { get; set; }
        float ShiftPowerStationMax_dB { get; set; }
        float ShiftPowerStationStep_dB { get; set; }
        bool CascadeTuning { get; set; }
        int NumberCascade { get; set; }
        int DetailOfCascade { get; set; }
        byte Method { get; set; }
        float MinRangeMeasurements_dBmkV { get; set; }
        float MaxRangeMeasurements_dBmkV { get; set; }
        int CorrelationDistance_m { get; set; }
        float Delta_dB { get; set; }
        float MaxAntennasPatternLoss_dB { get; set; }
        bool Detail { get; set; }
    }
	
}
