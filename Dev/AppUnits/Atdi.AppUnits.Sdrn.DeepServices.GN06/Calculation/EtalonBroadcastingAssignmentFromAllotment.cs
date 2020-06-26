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
        private static double GetERPGE06(double Freq_MHz, RefNetworkConfigType refNetworkConfigType, RefNetworkType refNetworkType)
        {
            double EtalonFreq_MHz; bool HBand;
            if (Freq_MHz > 350) { EtalonFreq_MHz = 650; HBand = true; } else { EtalonFreq_MHz = 200; HBand = false; }
            double k = 0;
            switch (refNetworkConfigType)
            {
                case RefNetworkConfigType.RPC1:
                    k = 20 * Math.Log10(Freq_MHz / EtalonFreq_MHz);
                    break;
                case RefNetworkConfigType.RPC2:
                case RefNetworkConfigType.RPC3:
                case RefNetworkConfigType.RPC4:
                case RefNetworkConfigType.RPC5:
                    k = 30 * Math.Log10(Freq_MHz / EtalonFreq_MHz);
                    break;
            }
            double ERP = 0;
            switch (refNetworkType)
            {
                case RefNetworkType.RN1:
                    switch (refNetworkConfigType)
                    {
                        case RefNetworkConfigType.RPC1:
                            if (HBand) { ERP = 42.8; } else { ERP = 34.1; } break;
                        case RefNetworkConfigType.RPC2:
                            if (HBand) { ERP = 49.7; } else { ERP = 36.2; } break;
                        case RefNetworkConfigType.RPC3:
                            if (HBand) { ERP = 52.4; } else { ERP = 40.0; } break;
                    }
                    break;
                case RefNetworkType.RN2:
                    switch (refNetworkConfigType)
                    {
                        case RefNetworkConfigType.RPC1:
                            if (HBand) { ERP = 31.8; } else { ERP = 24.1; }
                            break;
                        case RefNetworkConfigType.RPC2:
                            if (HBand) { ERP = 39.0; } else { ERP = 26.6; }
                            break;
                        case RefNetworkConfigType.RPC3:
                            if (HBand) { ERP = 46.3; } else { ERP = 34.1; }
                            break;
                    }
                    break;
                case RefNetworkType.RN3:
                    switch (refNetworkConfigType)
                    {
                        case RefNetworkConfigType.RPC1:
                            if (HBand) { ERP = 31.8; } else { ERP = 24.1; }
                            break;
                        case RefNetworkConfigType.RPC2:
                            if (HBand) { ERP = 44.9; } else { ERP = 32.5; }
                            break;
                        case RefNetworkConfigType.RPC3:
                            if (HBand) { ERP = 52.2; } else { ERP = 40.1; }
                            break;
                    }
                    break;
                case RefNetworkType.RN4:
                    switch (refNetworkConfigType)
                    {
                        case RefNetworkConfigType.RPC1:
                            if (HBand) { ERP = 29.4; } else { ERP = 22.0; }
                            break;
                        case RefNetworkConfigType.RPC2:
                            if (HBand) { ERP = 37.2; } else { ERP = 24.0; }
                            break;
                        case RefNetworkConfigType.RPC3:
                            if (HBand) { ERP = 44.8; } else { ERP = 32.5; }
                            break;
                    }
                    break;
                case RefNetworkType.RN5:
                    ERP = 30;
                    break;
                case RefNetworkType.RN6:
                    ERP = 39;
                    break;
            }
            ERP = ERP * k;
            return ERP;

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
            var Emmision = inputBroadcastingAllotment.EmissionCharacteristics;
            outputBroadcastingAssignment.EmissionCharacteristics = new BroadcastingAssignmentEmissionCharacteristics
            {
                Polar = Emmision.Polar,
                Freq_MHz = Emmision.Freq_MHz
            };
            double ERP = GetERPGE06(Emmision.Freq_MHz, Emmision.RefNetworkConfig, Emmision.RefNetwork);
            if (Emmision.Polar == PolarType.H)
            {
                outputBroadcastingAssignment.EmissionCharacteristics.ErpH_dBW = (float)ERP;
                outputBroadcastingAssignment.EmissionCharacteristics.ErpV_dBW = -9999;
            }
            else if (Emmision.Polar == PolarType.V)
            {
                outputBroadcastingAssignment.EmissionCharacteristics.ErpH_dBW = -9999;
                outputBroadcastingAssignment.EmissionCharacteristics.ErpV_dBW = (float)ERP;
            }
            else
            {
                outputBroadcastingAssignment.EmissionCharacteristics.ErpH_dBW = (float)ERP-3;
                outputBroadcastingAssignment.EmissionCharacteristics.ErpV_dBW = (float)ERP-3;
            }
            outputBroadcastingAssignment.SiteParameters = new SiteParameters();
            // по факту будет формироваться потом
        }
    }
}

