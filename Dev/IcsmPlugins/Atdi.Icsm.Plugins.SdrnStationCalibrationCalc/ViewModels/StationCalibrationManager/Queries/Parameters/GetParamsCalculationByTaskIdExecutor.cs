using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class GetParamsCalculationByTaskIdExecutor : IReadQueryExecutor<GetParamsCalculationByTaskId, ParamsCalculationModel>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;

        public GetParamsCalculationByTaskIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
        {
            _config = config;
            _dataLayer = dataLayer;
        }
        public ParamsCalculationModel Read(GetParamsCalculationByTaskId criterion)
        {
            var query = _dataLayer.GetBuilder<CS_ES.Tasks.IStationCalibrationArgs>()
                .Read()
                .Select(
                c=>c.Standard,
                c => c.TASK.Id,
                c => c.AltitudeStation,
                c => c.AzimuthStation,
                c => c.CascadeTuning,
                c => c.CoordinatesStation,
                c => c.CorrelationDistance_m,
                c => c.Delta_dB,
                c => c.Detail,
                c => c.DetailOfCascade,
                c => c.DistanceAroundContour_km,
                c => c.MaxAntennasPatternLoss_dB,
                c => c.MaxDeviationAltitudeStation_m,
                c => c.MaxDeviationAzimuthStation_deg,
                c => c.MaxDeviationCoordinatesStation_m,
                c => c.MaxDeviationTiltStation_deg,
                c => c.MaxRangeMeasurements_dBmkV,
                c => c.Method,
                c => c.MinNumberPointForCorrelation,
                c => c.MinRangeMeasurements_dBmkV,
                c => c.NumberCascade,
                c => c.PowerStation,
                c => c.ShiftAltitudeStationMax_m,
                c => c.ShiftAltitudeStationMin_m,
                c => c.ShiftAltitudeStationStep_m,
                c => c.ShiftAzimuthStationMax_deg,
                c => c.ShiftAzimuthStationMin_deg,
                c => c.ShiftAzimuthStationStep_deg,
                c => c.ShiftCoordinatesStationStep_m,
                c => c.ShiftCoordinatesStation_m,
                c => c.ShiftPowerStationMax_dB,
                c => c.ShiftPowerStationMin_dB,
                c => c.ShiftPowerStationStep_dB,
                c => c.ShiftTiltStationMax_deg,
                c => c.ShiftTiltStationMin_deg,
                c => c.ShiftTiltStationStep_deg,
                c => c.TiltStation,
                c => c.TrustOldResults,
                c => c.UseMeasurementSameGSID,
                c => c.CorrelationThresholdHard,
                c => c.CorrelationThresholdWeak,
                c => c.InfocMeasResults,
                c => c.StationIds,
                c => c.Contours
                    )
                .Filter(c => c.TASK.Id, criterion.TaskId);

            var reader = _dataLayer.Executor.ExecuteReader(query);
            if (!reader.Read())
            {
                return null;
            }

            return new ParamsCalculationModel()
            {
                TaskId  = reader.GetValue(c => c.TASK.Id),
                AltitudeStation = reader.GetValue(c => c.AltitudeStation),
                AzimuthStation = reader.GetValue(c => c.AzimuthStation),
                CascadeTuning = reader.GetValue(c => c.CascadeTuning),
                CoordinatesStation = reader.GetValue(c => c.CoordinatesStation),
                CorrelationDistance_m = reader.GetValue(c => c.CorrelationDistance_m),
                Delta_dB = reader.GetValue(c => c.Delta_dB),
                Detail = reader.GetValue(c => c.Detail),
                DetailOfCascade = reader.GetValue(c => c.DetailOfCascade),
                DistanceAroundContour_km = reader.GetValue(c => c.DistanceAroundContour_km),
                MaxAntennasPatternLoss_dB = reader.GetValue(c => c.MaxAntennasPatternLoss_dB),
                MaxDeviationAltitudeStation_m = reader.GetValue(c => c.MaxDeviationAltitudeStation_m),
                MaxDeviationAzimuthStation_deg = reader.GetValue(c => c.MaxDeviationAzimuthStation_deg),
                MaxDeviationCoordinatesStation_m  = reader.GetValue(c => c.MaxDeviationCoordinatesStation_m),
                MaxDeviationTiltStation_deg = reader.GetValue(c => c.MaxDeviationTiltStation_deg),
                MaxRangeMeasurements_dBmkV = reader.GetValue(c => c.MaxRangeMeasurements_dBmkV),
                Method = reader.GetValue(c => c.Method),
                MinNumberPointForCorrelation = reader.GetValue(c => c.MinNumberPointForCorrelation),
                MinRangeMeasurements_dBmkV = reader.GetValue(c => c.MinRangeMeasurements_dBmkV),
                NumberCascade = reader.GetValue(c => c.NumberCascade),
                PowerStation = reader.GetValue(c => c.PowerStation),
                ShiftAltitudeStationMax_m = reader.GetValue(c => c.ShiftAltitudeStationMax_m),
                ShiftAltitudeStationMin_m = reader.GetValue(c => c.ShiftAltitudeStationMin_m),
                ShiftAltitudeStationStep_m = reader.GetValue(c => c.ShiftAltitudeStationStep_m),
                ShiftAzimuthStationMax_deg = reader.GetValue(c => c.ShiftAzimuthStationMax_deg),
                ShiftAzimuthStationMin_deg = reader.GetValue(c => c.ShiftAzimuthStationMin_deg),
                ShiftAzimuthStationStep_deg = reader.GetValue(c => c.ShiftAzimuthStationStep_deg),
                ShiftCoordinatesStationStep_m = reader.GetValue(c => c.ShiftCoordinatesStationStep_m),
                ShiftCoordinatesStation_m = reader.GetValue(c => c.ShiftCoordinatesStation_m),
                ShiftPowerStationMax_dB = reader.GetValue(c => c.ShiftPowerStationMax_dB),
                ShiftPowerStationMin_dB = reader.GetValue(c => c.ShiftPowerStationMin_dB),
                ShiftPowerStationStep_dB = reader.GetValue(c => c.ShiftPowerStationStep_dB),
                ShiftTiltStationMax_deg = reader.GetValue(c => c.ShiftTiltStationMax_deg),
                ShiftTiltStationMin_deg = reader.GetValue(c => c.ShiftTiltStationMin_deg),
                ShiftTiltStationStep_deg = reader.GetValue(c => c.ShiftTiltStationStep_deg),
                TiltStation = reader.GetValue(c => c.TiltStation),
                TrustOldResults = reader.GetValue(c => c.TrustOldResults),
                UseMeasurementSameGSID = reader.GetValue(c => c.UseMeasurementSameGSID),
                CorrelationThresholdHard = reader.GetValue(c => c.CorrelationThresholdHard),
                CorrelationThresholdWeak = reader.GetValue(c => c.CorrelationThresholdWeak),
                Standard = reader.GetValue(c => c.Standard),
                Contours = reader.GetValue(c => c.Contours)
                //InfocMeasResults = reader.GetValue(c => c.InfocMeasResults),
                //StationIds = reader.GetValue(c => c.StationIds)
            };
        }
    }
}
