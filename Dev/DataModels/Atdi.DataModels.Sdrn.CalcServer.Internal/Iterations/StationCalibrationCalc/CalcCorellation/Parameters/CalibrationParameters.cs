using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct CalibrationParameters
    {
        public bool AltitudeStation;
        public int ShiftAltitudeStationMin_m;
        public int ShiftAltitudeStationMax_m;
        public int ShiftAltitudeStationStep_m;
        public int MaxDeviationAltitudeStation_m;
        public bool TiltStation;
        public float ShiftTiltStationMin_Deg;
        public float ShiftTiltStationMax_Deg;
        public float ShiftTiltStationStep_Deg;
        public float MaxDeviationTiltStationDeg;
        public bool AzimuthStation;
        public float ShiftAzimuthStationMin_deg;
        public float ShiftAzimuthStationMax_deg;
        public float ShiftAzimuthStationStep_deg;
        public float MaxDeviationAzimuthStation_deg;
        public bool CoordinatesStation;
        public int ShiftCoordinatesStation_m;
        public int ShiftCoordinatesStationStep_m;
        public int MaxDeviationCoordinatesStation_m;
        public float PowerStation;
        public float ShiftPowerStationMin_dB;
        public float ShiftPowerStationMax_dB;
        public float ShiftPowerStationStep_dB;
        public bool CascadeTuning;
        public int NumberCascade;
        public int DetailOfCascade;
        public Method Method;

    }

    public enum Method
    {
        ExhaustiveSearch=0,
        QuickDescent=1
    }
}

