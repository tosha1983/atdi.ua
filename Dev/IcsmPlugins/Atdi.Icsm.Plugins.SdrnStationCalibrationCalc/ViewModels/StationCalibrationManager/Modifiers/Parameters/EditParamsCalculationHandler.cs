using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Modifiers
{
    public class EditParamsCalculationHandler : ICommandHandler<EditParamsCalculation>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public EditParamsCalculationHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }

        public void Handle(EditParamsCalculation command)
        {
            var query = _dataLayer.GetBuilder<IStationCalibrationArgs>()
                .Update()
                .SetValue(c => c.AltitudeStation, command.AltitudeStation)
                .SetValue(c => c.AzimuthStation, command.AzimuthStation)
                .SetValue(c => c.CascadeTuning, command.CascadeTuning)
                .SetValue(c => c.CoordinatesStation, command.CoordinatesStation)
                .SetValue(c => c.CorrelationDistance_m, command.CorrelationDistance_m)
                .SetValue(c => c.Delta_dB, command.Delta_dB)
                .SetValue(c => c.Detail, command.Detail)
                .SetValue(c => c.DetailOfCascade, command.DetailOfCascade)
                .SetValue(c => c.DistanceAroundContour_km, command.DistanceAroundContour_km)
                .SetValue(c => c.InfocMeasResults, command.InfocMeasResults)
                .SetValue(c => c.MaxAntennasPatternLoss_dB, command.MaxAntennasPatternLoss_dB)
                .SetValue(c => c.MaxDeviationAltitudeStation_m, command.MaxDeviationAltitudeStation_m)
                .SetValue(c => c.MaxDeviationAzimuthStation_deg, command.MaxDeviationAzimuthStation_deg)
                .SetValue(c => c.MaxDeviationCoordinatesStation_m, command.MaxDeviationCoordinatesStation_m)
                .SetValue(c => c.MaxDeviationTiltStation_deg, command.MaxDeviationTiltStation_deg)
                .SetValue(c => c.MaxRangeMeasurements_dBmkV, command.MaxRangeMeasurements_dBmkV)
                .SetValue(c => c.Method, command.Method)
                .SetValue(c => c.MinNumberPointForCorrelation, command.MinNumberPointForCorrelation)
                .SetValue(c => c.MinRangeMeasurements_dBmkV, command.MinRangeMeasurements_dBmkV)
                .SetValue(c => c.NumberCascade, command.NumberCascade)
                .SetValue(c => c.PowerStation, command.PowerStation)
                .SetValue(c => c.ShiftAltitudeStationMax_m, command.ShiftAltitudeStationMax_m)
                .SetValue(c => c.ShiftAltitudeStationMin_m, command.ShiftAltitudeStationMin_m)
                .SetValue(c => c.ShiftAltitudeStationStep_m, command.ShiftAltitudeStationStep_m)
                .SetValue(c => c.ShiftAzimuthStationMax_deg, command.ShiftAzimuthStationMax_deg)
                .SetValue(c => c.ShiftAzimuthStationMin_deg, command.ShiftAzimuthStationMin_deg)
                .SetValue(c => c.ShiftAzimuthStationStep_deg, command.ShiftAzimuthStationStep_deg)
                .SetValue(c => c.ShiftCoordinatesStationStep_m, command.ShiftCoordinatesStationStep_m)
                .SetValue(c => c.ShiftCoordinatesStation_m, command.ShiftCoordinatesStation_m)
                .SetValue(c => c.ShiftPowerStationMax_dB, command.ShiftPowerStationMax_dB)
                .SetValue(c => c.ShiftPowerStationMin_dB, command.ShiftPowerStationMin_dB)
                .SetValue(c => c.ShiftPowerStationStep_dB, command.ShiftPowerStationStep_dB)
                .SetValue(c => c.ShiftTiltStationMax_deg, command.ShiftTiltStationMax_deg)
                .SetValue(c => c.ShiftTiltStationMin_deg, command.ShiftTiltStationMin_deg)
                .SetValue(c => c.ShiftTiltStationStep_deg, command.ShiftTiltStationStep_deg)
                .SetValue(c => c.StationIds, command.StationIds)
                .SetValue(c => c.TiltStation, command.TiltStation)
                .SetValue(c => c.TrustOldResults, command.TrustOldResults)
                .SetValue(c => c.UseMeasurementSameGSID, command.UseMeasurementSameGSID)
                .SetValue(c => c.CorrelationThresholdHard, command.CorrelationThresholdHard)
                .SetValue(c => c.CorrelationThresholdWeak, command.CorrelationThresholdWeak)
                .Filter(c => c.TASK.Id, command.TaskId);
            

            _eventBus.Send(new OnEditParamsCalculation
            {
                 TaskId = command.TaskId
            });

        }
    }
}
