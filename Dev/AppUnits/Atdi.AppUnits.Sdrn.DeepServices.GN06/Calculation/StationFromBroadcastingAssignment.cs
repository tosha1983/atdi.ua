using System;
using Atdi.Platform;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.Gis;

namespace Atdi.AppUnits.Sdrn.DeepServices.GN06
{
    public static class StationFromBroadcastingAssignment
    {

        public static void Calc(BroadcastingAssignment inputBroadcastingAssignment, ref ContextStation outputContextStation)
        {
            var emission = inputBroadcastingAssignment.EmissionCharacteristics;
            var site = inputBroadcastingAssignment.SiteParameters;
            var antenna = inputBroadcastingAssignment.AntennaCharacteristics;
            outputContextStation.ClientContextStation = new ClientContextStation
            {
                Coordinate = new AtdiCoordinate()
            };
            outputContextStation.ClientContextStation.Antenna.Azimuth_deg = 0;
            outputContextStation.ClientContextStation.Antenna.Gain_dB = 0;
            outputContextStation.ClientContextStation.Antenna.Tilt_deg = 0;
            outputContextStation.ClientContextStation.Type = ClientContextStationType.A;
            outputContextStation.ClientContextStation.Transmitter = new StationTransmitter
            {
                Freq_MHz = emission.Freq_MHz
            };
            switch (emission.Polar)
            {
                case PolarType.H: outputContextStation.ClientContextStation.Transmitter.Polarization = PolarizationType.H;
                    outputContextStation.ClientContextStation.Transmitter.MaxPower_dBm = emission.ErpH_dBW + 30;
                    break;
                case PolarType.V: outputContextStation.ClientContextStation.Transmitter.Polarization = PolarizationType.V;
                    outputContextStation.ClientContextStation.Transmitter.MaxPower_dBm = emission.ErpV_dBW+30;
                    break;
                case PolarType.M: outputContextStation.ClientContextStation.Transmitter.Polarization = PolarizationType.M;
                    outputContextStation.ClientContextStation.Transmitter.MaxPower_dBm = (float)(30.0+ 10.0 * Math.Log10(Math.Pow(10, emission.ErpH_dBW / 10.0) + Math.Pow(10, emission.ErpV_dBW / 10.0)));
                    break;
            }

            outputContextStation.ClientContextStation.Site = new Wgs84Site
            {
                Altitude = antenna.AglHeight_m,
                Latitude = site.Lat_Dec,
                Longitude = site.Lon_Dec
            };
            outputContextStation.ClientContextStation.Antenna = new StationAntenna
            {
                HhPattern = GetantennaPattern(antenna, emission.Polar, outputContextStation.ClientContextStation.Transmitter.MaxPower_dBm - 30),
                HvPattern = GetantennaPattern(antenna, emission.Polar, outputContextStation.ClientContextStation.Transmitter.MaxPower_dBm - 30),
                VhPattern = new StationAntennaPattern() { Angle_deg = new double[2] { -90, 90 }, Loss_dB = new float[2] { 0, 0 } },
                VvPattern = new StationAntennaPattern() { Angle_deg = new double[2] { -90, 90 }, Loss_dB = new float[2] { 0, 0 } }
            };

            // хз что это и нафиг нужно
            //outputContextStation.ClientContextStation.ContextId
            // outputContextStation.ClientContextStation.CreatedDate
            // outputContextStation.ClientContextStation.Name
            // outputContextStation.ClientContextStation.Id
        }
        private static StationAntennaPattern GetantennaPattern(AntennaCharacteristics BroadcastAntennaCharacteristics, PolarType  polarType, float Pow_dBW)
        {

            var broadcastPatternH = BroadcastAntennaCharacteristics.DiagrH;
            var broadcastPatternV = BroadcastAntennaCharacteristics.DiagrV;
            StationAntennaPattern stationAntennaPattern = new StationAntennaPattern
            {
                Angle_deg = new double[36],
                Loss_dB = new float[36]
            };
            for (int i = 0; i < 36; i++)
            {
                stationAntennaPattern.Angle_deg[i] = i * 10.0;
                switch (polarType)
                {
                    case PolarType.H:
                        if (broadcastPatternH.Length > i) { stationAntennaPattern.Loss_dB[i] = Pow_dBW - broadcastPatternH[i]; }
                        else { stationAntennaPattern.Loss_dB[i] = 0; }
                        break;
                    case PolarType.V:
                        if (broadcastPatternV.Length > i) { stationAntennaPattern.Loss_dB[i] = Pow_dBW - broadcastPatternV[i]; }
                        else { stationAntennaPattern.Loss_dB[i] = 0; }
                        break;
                    case PolarType.M:
                        if ((broadcastPatternH.Length > i)&&(broadcastPatternV.Length > i))
                        { stationAntennaPattern.Loss_dB[i] = (float)(Pow_dBW - 10 * Math.Log10(Math.Pow(10, broadcastPatternV[i]/10.0)+ Math.Pow(10, broadcastPatternH[i] / 10.0))); }
                        else { stationAntennaPattern.Loss_dB[i] = 0; }
                        break;
                }
                if (stationAntennaPattern.Loss_dB[i] < 0) { stationAntennaPattern.Loss_dB[i] = 0; }
            }
            return stationAntennaPattern;
        }
    }
}
