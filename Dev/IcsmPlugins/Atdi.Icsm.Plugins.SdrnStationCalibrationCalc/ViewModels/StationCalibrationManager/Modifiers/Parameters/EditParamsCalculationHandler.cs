using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
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

        public void Handle(EditParamsCalculation commandParameters)
        {
            var query = _dataLayer.GetBuilder<IStationCalibrationArgs>()
                .Update()
                  .SetValue(c => c.Standard, commandParameters.Standard)
                  .SetValue(c => c.AltitudeStation, commandParameters.AltitudeStation)
                  .SetValue(c => c.AzimuthStation, commandParameters.AzimuthStation)
                  .SetValue(c => c.CascadeTuning, commandParameters.CascadeTuning)
                  .SetValue(c => c.CoordinatesStation, commandParameters.CoordinatesStation)
                  .SetValue(c => c.CorrelationDistance_m, commandParameters.CorrelationDistance_m)
                  .SetValue(c => c.Delta_dB, commandParameters.Delta_dB)
                  .SetValue(c => c.Detail, commandParameters.Detail)
                  .SetValue(c => c.DetailOfCascade, commandParameters.DetailOfCascade)
                  .SetValue(c => c.DistanceAroundContour_km, commandParameters.DistanceAroundContour_km)
                  .SetValue(c => c.MaxAntennasPatternLoss_dB, commandParameters.MaxAntennasPatternLoss_dB)
                  .SetValue(c => c.MaxDeviationAltitudeStation_m, commandParameters.MaxDeviationAltitudeStation_m)
                  .SetValue(c => c.MaxDeviationAzimuthStation_deg, commandParameters.MaxDeviationAzimuthStation_deg)
                  .SetValue(c => c.MaxDeviationCoordinatesStation_m, commandParameters.MaxDeviationCoordinatesStation_m)
                  .SetValue(c => c.MaxDeviationTiltStation_deg, commandParameters.MaxDeviationTiltStation_deg)
                  .SetValue(c => c.MaxRangeMeasurements_dBmkV, commandParameters.MaxRangeMeasurements_dBmkV)
                  .SetValue(c => c.Method, commandParameters.Method)
                  .SetValue(c => c.MinNumberPointForCorrelation, commandParameters.MinNumberPointForCorrelation)
                  .SetValue(c => c.MinRangeMeasurements_dBmkV, commandParameters.MinRangeMeasurements_dBmkV)
                  .SetValue(c => c.NumberCascade, commandParameters.NumberCascade)
                  .SetValue(c => c.PowerStation, commandParameters.PowerStation)
                  .SetValue(c => c.ShiftAltitudeStationMax_m, commandParameters.ShiftAltitudeStationMax_m)
                  .SetValue(c => c.ShiftAltitudeStationMin_m, commandParameters.ShiftAltitudeStationMin_m)
                  .SetValue(c => c.ShiftAltitudeStationStep_m, commandParameters.ShiftAltitudeStationStep_m)
                  .SetValue(c => c.ShiftAzimuthStationMax_deg, commandParameters.ShiftAzimuthStationMax_deg)
                  .SetValue(c => c.ShiftAzimuthStationMin_deg, commandParameters.ShiftAzimuthStationMin_deg)
                  .SetValue(c => c.ShiftAzimuthStationStep_deg, commandParameters.ShiftAzimuthStationStep_deg)
                  .SetValue(c => c.ShiftCoordinatesStationStep_m, commandParameters.ShiftCoordinatesStationStep_m)
                  .SetValue(c => c.ShiftCoordinatesStation_m, commandParameters.ShiftCoordinatesStation_m)
                  .SetValue(c => c.ShiftPowerStationMax_dB, commandParameters.ShiftPowerStationMax_dB)
                  .SetValue(c => c.ShiftPowerStationMin_dB, commandParameters.ShiftPowerStationMin_dB)
                  .SetValue(c => c.ShiftPowerStationStep_dB, commandParameters.ShiftPowerStationStep_dB)
                  .SetValue(c => c.ShiftTiltStationMax_deg, commandParameters.ShiftTiltStationMax_deg)
                  .SetValue(c => c.ShiftTiltStationMin_deg, commandParameters.ShiftTiltStationMin_deg)
                  .SetValue(c => c.ShiftTiltStationStep_deg, commandParameters.ShiftTiltStationStep_deg);
                  if (commandParameters.StationIds != null)
                  {
                     query.SetValue(c => c.StationIds, commandParameters.StationIds);
                  }
                 if (commandParameters.InfocMeasResults != null)
                 {
                     query.SetValue(c => c.InfocMeasResults, commandParameters.InfocMeasResults);
                 }
                 query.SetValue(c => c.TiltStation, commandParameters.TiltStation)
                .SetValue(c => c.TrustOldResults, commandParameters.TrustOldResults)
                .SetValue(c => c.UseMeasurementSameGSID, commandParameters.UseMeasurementSameGSID)
                .SetValue(c => c.CorrelationThresholdHard, commandParameters.CorrelationThresholdHard)
                .SetValue(c => c.CorrelationThresholdWeak, commandParameters.CorrelationThresholdWeak)
                .Filter(c => c.TASK.Id, commandParameters.TaskId)
                ;
                _dataLayer.Executor.Execute(query);

                _eventBus.Send(new OnEditParamsCalculation
                {
                    ClientContextId = commandParameters.ClientContextId,
                    IsSuccessUpdateParameters = true
                });
        }
    }
}
