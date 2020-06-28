using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeepServices.GN06
{
    public static class EtalonBroadcastingAssignmentFromAllotment
    {
        private static double GetERPGE06(double freq_MHz, RefNetworkConfigType refNetworkConfigType, RefNetworkType refNetworkType)
        {
            double etalonFreq_MHz; bool HBand;
            if (freq_MHz > 350) { etalonFreq_MHz = 650; HBand = true; } else { etalonFreq_MHz = 200; HBand = false; }
            double k = 0;
            switch (refNetworkConfigType)
            {
                case RefNetworkConfigType.RPC1:
                    k = 20 * Math.Log10(freq_MHz / etalonFreq_MHz);
                    break;
                case RefNetworkConfigType.RPC2:
                case RefNetworkConfigType.RPC3:
                case RefNetworkConfigType.RPC4:
                case RefNetworkConfigType.RPC5:
                    k = 30 * Math.Log10(freq_MHz / etalonFreq_MHz);
                    break;
            }
            double erp = 0;
            switch (refNetworkType)
            {
                case RefNetworkType.RN1:
                    switch (refNetworkConfigType)
                    {
                        case RefNetworkConfigType.RPC1:
                            if (HBand) { erp = 42.8; } else { erp = 34.1; } break;
                        case RefNetworkConfigType.RPC2:
                            if (HBand) { erp = 49.7; } else { erp = 36.2; } break;
                        case RefNetworkConfigType.RPC3:
                            if (HBand) { erp = 52.4; } else { erp = 40.0; } break;
                    }
                    break;
                case RefNetworkType.RN2:
                    switch (refNetworkConfigType)
                    {
                        case RefNetworkConfigType.RPC1:
                            if (HBand) { erp = 31.8; } else { erp = 24.1; }
                            break;
                        case RefNetworkConfigType.RPC2:
                            if (HBand) { erp = 39.0; } else { erp = 26.6; }
                            break;
                        case RefNetworkConfigType.RPC3:
                            if (HBand) { erp = 46.3; } else { erp = 34.1; }
                            break;
                    }
                    break;
                case RefNetworkType.RN3:
                    switch (refNetworkConfigType)
                    {
                        case RefNetworkConfigType.RPC1:
                            if (HBand) { erp = 31.8; } else { erp = 24.1; }
                            break;
                        case RefNetworkConfigType.RPC2:
                            if (HBand) { erp = 44.9; } else { erp = 32.5; }
                            break;
                        case RefNetworkConfigType.RPC3:
                            if (HBand) { erp = 52.2; } else { erp = 40.1; }
                            break;
                    }
                    break;
                case RefNetworkType.RN4:
                    switch (refNetworkConfigType)
                    {
                        case RefNetworkConfigType.RPC1:
                            if (HBand) { erp = 29.4; } else { erp = 22.0; }
                            break;
                        case RefNetworkConfigType.RPC2:
                            if (HBand) { erp = 37.2; } else { erp = 24.0; }
                            break;
                        case RefNetworkConfigType.RPC3:
                            if (HBand) { erp = 44.8; } else { erp = 32.5; }
                            break;
                    }
                    break;
                case RefNetworkType.RN5:
                    erp = 30;
                    break;
                case RefNetworkType.RN6:
                    erp = 39;
                    break;
            }
            erp = erp * k;
            return erp;

        }
        public static void Calc(BroadcastingAllotment inputBroadcastingAllotment, BroadcastingAssignment outputBroadcastingAssignment)
        {
            outputBroadcastingAssignment.AdmData = new AdministrativeData
            {
                Adm = inputBroadcastingAllotment.AdminData.Adm
            };
            outputBroadcastingAssignment.AntennaCharacteristics = new AntennaCharacteristics
            {
                AglHeight_m = 150,
                DiagrH = new float[36] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
                DiagrV = new float[36] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
                Direction = AntennaDirectionType.ND
            };
            outputBroadcastingAssignment.DigitalPlanEntryParameters = new DigitalPlanEntryParameters();
            var emmision = inputBroadcastingAllotment.EmissionCharacteristics;
            outputBroadcastingAssignment.EmissionCharacteristics = new BroadcastingAssignmentEmissionCharacteristics
            {
                Polar = emmision.Polar,
                Freq_MHz = emmision.Freq_MHz
            };
            double erp = GetERPGE06(emmision.Freq_MHz, emmision.RefNetworkConfig, emmision.RefNetwork);
            if (emmision.Polar == PolarType.H)
            {
                outputBroadcastingAssignment.EmissionCharacteristics.ErpH_dBW = (float)erp;
                outputBroadcastingAssignment.EmissionCharacteristics.ErpV_dBW = -9999;
            }
            else if (emmision.Polar == PolarType.V)
            {
                outputBroadcastingAssignment.EmissionCharacteristics.ErpH_dBW = -9999;
                outputBroadcastingAssignment.EmissionCharacteristics.ErpV_dBW = (float)erp;
            }
            else
            {
                outputBroadcastingAssignment.EmissionCharacteristics.ErpH_dBW = (float)erp - 3;
                outputBroadcastingAssignment.EmissionCharacteristics.ErpV_dBW = (float)erp - 3;
            }
            outputBroadcastingAssignment.SiteParameters = new SiteParameters();
            // по факту будет формироваться потом
        }
    }
}

