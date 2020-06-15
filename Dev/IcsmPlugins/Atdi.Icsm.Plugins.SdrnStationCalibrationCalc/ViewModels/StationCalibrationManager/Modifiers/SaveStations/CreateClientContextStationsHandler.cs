using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Events;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Modifiers
{
    public class CreateClientContextStationsHandler : ICommandHandler<CreateClientContextStations>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;
        private readonly IObjectReader _objectReader;

        public CreateClientContextStationsHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IObjectReader objectReader, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
            _objectReader = objectReader;
        }

        public void Handle(CreateClientContextStations commandParameters)
        {
            var clientContextStations = commandParameters.IcsmMobStation;
            var stations = new long[clientContextStations.Length];
            for (int i = 0; i < clientContextStations.Length; i++)
            {
                var command = clientContextStations[i];
                var contextStationId = _objectReader.Read<long?>().By(new ClientContextStationModelByParams() {  ClientContextId = commandParameters.ClientContextId, ExternalCode = command.ExternalCode, ExternalSource = command.ExternalSource, Standard = command.Standard });
                if (contextStationId!=null)
                {
                    stations[i] = contextStationId.Value;
                    continue;
                }
                var enumStateCode = Enum.GetValues(typeof(StationStateCode)).Cast<StationStateCode>().ToList();
                var insQuery = _dataLayer.GetBuilder<IContextStation>()
                    .Create()
                    .SetValue(c => c.CreatedDate, command.CreatedDate)
                    .SetValue(c => c.CONTEXT.Id, commandParameters.ClientContextId)
                    .SetValue(c => c.Name, command.Name)
                    .SetValue(c => c.StateCode, (byte)enumStateCode.Find(x => x.ToString() == command.StateName))
                    .SetValue(c => c.StateName, command.StateName)
                    .SetValue(c => c.CallSign, command.CallSign)
                    .SetValue(c => c.Standard, command.Standard)
                    .SetValue(c => c.RealGsid, command.RealGsid)
                    .SetValue(c => c.LicenseGsid, command.LicenseGsid)
                    .SetValue(c => c.RegionCode, command.RegionCode)
                    .SetValue(c => c.ModifiedDate, command.ModifiedDate)
                    .SetValue(c => c.ExternalSource, command.ExternalSource)
                    .SetValue(c => c.ExternalCode, command.ExternalCode)
                    .SetValue(c => c.SITE.Longitude_DEC, command.SITE.Longitude_DEC)
                    .SetValue(c => c.SITE.Latitude_DEC, command.SITE.Latitude_DEC)
                    .SetValue(c => c.SITE.Altitude_m, command.SITE.Altitude_m)
                    .SetValue(c => c.ANTENNA.ItuPatternCode, command.ANTENNA.ItuPatternCode)
                    .SetValue(c => c.ANTENNA.ItuPatternName, command.ANTENNA.ItuPatternName)
                    .SetValue(c => c.ANTENNA.XPD_dB, command.ANTENNA.XPD_dB)
                    .SetValue(c => c.ANTENNA.Gain_dB, command.ANTENNA.Gain_dB)
                    .SetValue(c => c.ANTENNA.Tilt_deg, command.ANTENNA.Tilt_deg)
                    .SetValue(c => c.ANTENNA.Azimuth_deg, command.ANTENNA.Azimuth_deg);

                var stationPk = _dataLayer.Executor.Execute<IContextStation_PK>(insQuery);
                var stationId = stationPk.Id;
                stations[i] = stationId;

                // создаем запись о трансмитере
                var transQuery = _dataLayer.GetBuilder<IContextStationTransmitter>()
                    .Create()
                    .SetValue(c => c.StationId, stationId)
                    .SetValue(c => c.PolarizationCode, command.TRANSMITTER.PolarizationCode)
                    .SetValue(c => c.PolarizationName, ((PolarizationCode)(command.TRANSMITTER.PolarizationCode)).ToString())
                    .SetValue(c => c.Loss_dB, command.TRANSMITTER.Loss_dB)
                    .SetValue(c => c.Freq_MHz, command.TRANSMITTER.Freq_MHz)
                    .SetValue(c => c.Freqs_MHz, command.TRANSMITTER.Freqs_MHz)
                    .SetValue(c => c.BW_kHz, command.TRANSMITTER.BW_kHz)
                    .SetValue(c => c.MaxPower_dBm, command.TRANSMITTER.MaxPower_dBm);
                _dataLayer.Executor.Execute(transQuery);


                // создаем запись о приемнике
                var receiveQuery = _dataLayer.GetBuilder<IContextStationReceiver>()
                    .Create()
                    .SetValue(c => c.StationId, stationId)
                    .SetValue(c => c.PolarizationCode, command.RECEIVER.PolarizationCode)
                    .SetValue(c => c.PolarizationName, ((PolarizationCode)(command.RECEIVER.PolarizationCode)).ToString())
                    .SetValue(c => c.Loss_dB, command.RECEIVER.Loss_dB)
                    .SetValue(c => c.Freq_MHz, command.RECEIVER.Freq_MHz)
                    .SetValue(c => c.BW_kHz, command.RECEIVER.BW_kHz)
                    .SetValue(c => c.KTBF_dBm, command.RECEIVER.KTBF_dBm)
                    .SetValue(c => c.Freqs_MHz, command.RECEIVER.Freqs_MHz)
                    .SetValue(c => c.Threshold_dBm, command.RECEIVER.Threshold_dBm)
                    ;
                _dataLayer.Executor.Execute(receiveQuery);


                //  создаем патерн антенны
                var paternQuery = _dataLayer.GetBuilder<IContextStationPattern>()
                    .Create()
                    .SetValue(c => c.StationId, stationId)
                    .SetValue(c => c.AntennaPlane, "H")
                    .SetValue(c => c.WavePlane, "H")
                    .SetValue(c => c.Angle_deg, command.ANTENNA.HH_PATTERN.Angle_deg)
                    .SetValue(c => c.Loss_dB, command.ANTENNA.HH_PATTERN.Loss_dB);
                _dataLayer.Executor.Execute(paternQuery);

                paternQuery = _dataLayer.GetBuilder<IContextStationPattern>()
                    .Create()
                    .SetValue(c => c.StationId, stationId)
                    .SetValue(c => c.AntennaPlane, "H")
                    .SetValue(c => c.WavePlane, "V")
                    .SetValue(c => c.Angle_deg, command.ANTENNA.HV_PATTERN.Angle_deg)
                    .SetValue(c => c.Loss_dB, command.ANTENNA.HV_PATTERN.Loss_dB);

                _dataLayer.Executor.Execute(paternQuery);

                paternQuery = _dataLayer.GetBuilder<IContextStationPattern>()
                    .Create()
                    .SetValue(c => c.StationId, stationId)
                    .SetValue(c => c.AntennaPlane, "V")
                    .SetValue(c => c.WavePlane, "H")
                    .SetValue(c => c.Angle_deg, command.ANTENNA.VH_PATTERN.Angle_deg)
                    .SetValue(c => c.Loss_dB, command.ANTENNA.VH_PATTERN.Loss_dB);
                _dataLayer.Executor.Execute(paternQuery);

                paternQuery = _dataLayer.GetBuilder<IContextStationPattern>()
                    .Create()
                    .SetValue(c => c.StationId, stationId)
                    .SetValue(c => c.AntennaPlane, "V")
                    .SetValue(c => c.WavePlane, "V")
                    .SetValue(c => c.Angle_deg, command.ANTENNA.VV_PATTERN.Angle_deg)
                    .SetValue(c => c.Loss_dB, command.ANTENNA.VV_PATTERN.Loss_dB);
                _dataLayer.Executor.Execute(paternQuery);
            }
            _eventBus.Send(new OnSavedStations
            {
                ClientContextId = commandParameters.ClientContextId,
                ContextStationIds = stations
            });
        }
    }
}
