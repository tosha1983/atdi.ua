using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Adapters
{
    public sealed class StationCalibrationResultDataAdapter : EntityDataAdapter<CS_ES.IStationCalibrationResult, StationCalibrationResultModel>
    {
        public StationCalibrationResultDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long resultId;

        protected override void PrepareQuery(IReadQuery<CS_ES.IStationCalibrationResult> query)
        {
            query.Select(
                c => c.RESULT.Id,
                c => c.AreaName,
                c => c.CountMeasGSID,
                c => c.CountMeasGSID_IT,
                c => c.CountMeasGSID_LS,
                c => c.CountStation_CS,
                c => c.CountStation_IT,
                c => c.CountStation_NF,
                c => c.CountStation_NS,
                c => c.CountStation_UN,
                c => c.NumberStation,
                c => c.NumberStationInContour,
                c => c.PARAMETERS.TaskId,
                c => c.ResultId,
                c => c.Standard,
                c => c.TimeStart,
                c => c.PARAMETERS.AltitudeStation,
                c => c.PARAMETERS.AzimuthStation,
                c => c.PARAMETERS.CascadeTuning,
                c => c.PARAMETERS.CoordinatesStation,
                c => c.PARAMETERS.CorrelationDistance_m,
                c => c.PARAMETERS.CorrelationThresholdHard,
                c => c.PARAMETERS.CorrelationThresholdWeak,
                c => c.PARAMETERS.Delta_dB,
                c => c.PARAMETERS.Detail,
                c => c.PARAMETERS.DetailOfCascade,
                c => c.PARAMETERS.DistanceAroundContour_km,
                c => c.PARAMETERS.InfocMeasResults,
                c => c.PARAMETERS.MaxAntennasPatternLoss_dB,
                c => c.PARAMETERS.MaxDeviationAltitudeStation_m,
                c => c.PARAMETERS.MaxDeviationAzimuthStation_deg,
                c => c.PARAMETERS.MaxDeviationCoordinatesStation_m,
                c => c.PARAMETERS.MaxDeviationTiltStation_deg,
                c => c.PARAMETERS.MaxRangeMeasurements_dBmkV,
                c => c.PARAMETERS.Method,
                c => c.PARAMETERS.MinNumberPointForCorrelation,
                c => c.PARAMETERS.MinRangeMeasurements_dBmkV,
                c => c.PARAMETERS.NumberCascade,
                c => c.PARAMETERS.PowerStation,
                c => c.PARAMETERS.ShiftAltitudeStationMax_m,
                c => c.PARAMETERS.ShiftAltitudeStationMin_m,
                c => c.PARAMETERS.ShiftAltitudeStationStep_m,
                c => c.PARAMETERS.ShiftAzimuthStationMax_deg,
                c => c.PARAMETERS.ShiftAzimuthStationMin_deg,
                c => c.PARAMETERS.ShiftAzimuthStationStep_deg,
                c => c.PARAMETERS.ShiftCoordinatesStationStep_m,
                c => c.PARAMETERS.ShiftCoordinatesStation_m,
                c => c.PARAMETERS.ShiftPowerStationMax_dB,
                c => c.PARAMETERS.ShiftPowerStationMin_dB,
                c => c.PARAMETERS.ShiftPowerStationStep_dB,
                c => c.PARAMETERS.ShiftTiltStationMax_deg,
                c => c.PARAMETERS.ShiftTiltStationMin_deg,
                c => c.PARAMETERS.ShiftTiltStationStep_deg,
                c => c.PARAMETERS.TiltStation,
                c => c.PARAMETERS.TrustOldResults,
                c => c.PARAMETERS.UseMeasurementSameGSID

            ).Filter(f => f.RESULT.Id, DataModels.Api.EntityOrm.WebClient.FilterOperator.Equal, resultId);
        }
        protected override StationCalibrationResultModel ReadData(IDataReader<CS_ES.IStationCalibrationResult> reader, int index)
        {
            return new StationCalibrationResultModel
            {
                ResultId = reader.GetValue(c => c.RESULT.Id),
                AreaName = reader.GetValue(c => c.AreaName),
                CountMeasGSID = reader.GetValue(c => c.CountMeasGSID),
                CountMeasGSID_IT = reader.GetValue(c => c.CountMeasGSID_IT),
                CountMeasGSID_LS = reader.GetValue(c => c.CountMeasGSID),
                CountStation_CS = reader.GetValue(c => c.CountStation_CS),
                CountStation_IT = reader.GetValue(c => c.CountStation_IT),
                CountStation_NF = reader.GetValue(c => c.CountStation_NF),
                CountStation_NS = reader.GetValue(c => c.CountStation_NS),
                CountStation_UN = reader.GetValue(c => c.CountStation_UN),
                NumberStation = reader.GetValue(c => c.NumberStation),
                NumberStationInContour = reader.GetValue(c => c.NumberStationInContour),
                ParametersId = reader.GetValue(c => c.PARAMETERS.TaskId),
                Standard = reader.GetValue(c => c.Standard),
                TimeStart = reader.GetValue(c => c.TimeStart),

                AltitudeStation = reader.GetValue(c => c.PARAMETERS.AltitudeStation),
                 AzimuthStation = reader.GetValue(c => c.PARAMETERS.AzimuthStation),
                 CascadeTuning = reader.GetValue(c => c.PARAMETERS.CascadeTuning),
                  UseMeasurementSameGSID = reader.GetValue(c => c.PARAMETERS.UseMeasurementSameGSID),
                 TrustOldResults = reader.GetValue(c => c.PARAMETERS.TrustOldResults),
                 TiltStation = reader.GetValue(c => c.PARAMETERS.TiltStation),
                 ShiftTiltStationStep_deg = reader.GetValue(c => c.PARAMETERS.ShiftTiltStationStep_deg),
                 ShiftTiltStationMin_deg = reader.GetValue(c => c.PARAMETERS.ShiftTiltStationMin_deg),
                 ShiftTiltStationMax_deg = reader.GetValue(c => c.PARAMETERS.ShiftTiltStationMax_deg),
                 ShiftPowerStationStep_dB = reader.GetValue(c => c.PARAMETERS.ShiftPowerStationStep_dB),
                 CoordinatesStation = reader.GetValue(c => c.PARAMETERS.CoordinatesStation),
                 CorrelationDistance_m = reader.GetValue(c => c.PARAMETERS.CorrelationDistance_m),
                 CorrelationThresholdHard = reader.GetValue(c => c.PARAMETERS.CorrelationThresholdHard),
                CorrelationThresholdWeak  = reader.GetValue(c => c.PARAMETERS.CorrelationThresholdWeak),
                 Delta_dB = reader.GetValue(c => c.PARAMETERS.Delta_dB),
                 Detail = reader.GetValue(c => c.PARAMETERS.Detail),
                 DetailOfCascade = reader.GetValue(c => c.PARAMETERS.DetailOfCascade),
                 DistanceAroundContour_km = reader.GetValue(c => c.PARAMETERS.DistanceAroundContour_km),
                 MaxAntennasPatternLoss_dB = reader.GetValue(c => c.PARAMETERS.MaxAntennasPatternLoss_dB),
                 MaxDeviationAltitudeStation_m = reader.GetValue(c => c.PARAMETERS.MaxDeviationAltitudeStation_m),
                 MaxDeviationAzimuthStation_deg = reader.GetValue(c => c.PARAMETERS.MaxDeviationAzimuthStation_deg),
                 MaxDeviationCoordinatesStation_m = reader.GetValue(c => c.PARAMETERS.MaxDeviationCoordinatesStation_m),
                 MaxDeviationTiltStation_deg  = reader.GetValue(c => c.PARAMETERS.MaxDeviationTiltStation_deg),
                 MaxRangeMeasurements_dBmkV = reader.GetValue(c => c.PARAMETERS.MaxRangeMeasurements_dBmkV),
                 Method = reader.GetValue(c => c.PARAMETERS.Method),
                 MinNumberPointForCorrelation = reader.GetValue(c => c.PARAMETERS.MinNumberPointForCorrelation),
                 MinRangeMeasurements_dBmkV = reader.GetValue(c => c.PARAMETERS.MinRangeMeasurements_dBmkV),
                 NumberCascade = reader.GetValue(c => c.PARAMETERS.NumberCascade),
                 PowerStation = reader.GetValue(c => c.PARAMETERS.PowerStation),
                 ShiftAltitudeStationMax_m = reader.GetValue(c => c.PARAMETERS.ShiftAltitudeStationMax_m),
                 ShiftAltitudeStationMin_m = reader.GetValue(c => c.PARAMETERS.ShiftAltitudeStationMin_m),
                 ShiftAltitudeStationStep_m = reader.GetValue(c => c.PARAMETERS.ShiftAltitudeStationStep_m),
                 ShiftAzimuthStationMax_deg = reader.GetValue(c => c.PARAMETERS.ShiftAzimuthStationMax_deg),
                 ShiftAzimuthStationMin_deg = reader.GetValue(c => c.PARAMETERS.ShiftAzimuthStationMin_deg),
                 ShiftAzimuthStationStep_deg = reader.GetValue(c => c.PARAMETERS.ShiftAzimuthStationStep_deg),
                 ShiftCoordinatesStationStep_m = reader.GetValue(c => c.PARAMETERS.ShiftCoordinatesStationStep_m),
                 ShiftCoordinatesStation_m = reader.GetValue(c => c.PARAMETERS.ShiftCoordinatesStation_m),
                 ShiftPowerStationMax_dB = reader.GetValue(c => c.PARAMETERS.ShiftPowerStationMax_dB),
                 ShiftPowerStationMin_dB = reader.GetValue(c => c.PARAMETERS.ShiftPowerStationMin_dB),
            };
        }
    }

}
