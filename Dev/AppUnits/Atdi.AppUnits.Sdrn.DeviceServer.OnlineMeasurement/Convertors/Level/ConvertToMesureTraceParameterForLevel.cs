using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks
{
    public static class ConvertToMesureTraceParameterForLevel
    {
        public static MesureTraceParameter ConvertForLevel(this ClientMeasTaskData deviceServerParametersDataLevel)
        {
            MesureTraceParameter mesureTraceParameter = new MesureTraceParameter();

            mesureTraceParameter.FreqStart_Hz = (decimal)(deviceServerParametersDataLevel.FreqStart_MHz * 1000000);
            mesureTraceParameter.FreqStop_Hz = (decimal)(deviceServerParametersDataLevel.FreqStop_MHz * 1000000);
            mesureTraceParameter.SweepTime_s = deviceServerParametersDataLevel.SweepTime_s;

            mesureTraceParameter.RefLevel_dBm = deviceServerParametersDataLevel.RefLevel_dBm;
            mesureTraceParameter.Att_dB = deviceServerParametersDataLevel.Att_dB;    
            mesureTraceParameter.PreAmp_dB = deviceServerParametersDataLevel.PreAmp_dB;
            mesureTraceParameter.RBW_Hz = deviceServerParametersDataLevel.RBW_kHz * 1000;
            switch (deviceServerParametersDataLevel.DetectorType)
            {
                case DataModels.Sdrns.Device.OnlineMeasurement.DetectorType.Average:
                    mesureTraceParameter.DetectorType = DataModels.Sdrn.DeviceServer.Commands.Parameters.DetectorType.Average;
                    break;
                case DataModels.Sdrns.Device.OnlineMeasurement.DetectorType.MaxPeak:
                    mesureTraceParameter.DetectorType = DataModels.Sdrn.DeviceServer.Commands.Parameters.DetectorType.MaxPeak;
                    break;
                case DataModels.Sdrns.Device.OnlineMeasurement.DetectorType.MinPeak:
                    mesureTraceParameter.DetectorType = DataModels.Sdrn.DeviceServer.Commands.Parameters.DetectorType.MinPeak;
                    break;
                case DataModels.Sdrns.Device.OnlineMeasurement.DetectorType.RMS:
                    mesureTraceParameter.DetectorType = DataModels.Sdrn.DeviceServer.Commands.Parameters.DetectorType.RMS;
                    break;
                case DataModels.Sdrns.Device.OnlineMeasurement.DetectorType.Auto:
                    mesureTraceParameter.DetectorType = DataModels.Sdrn.DeviceServer.Commands.Parameters.DetectorType.Auto;
                    break;
                default:
                    mesureTraceParameter.DetectorType = DataModels.Sdrn.DeviceServer.Commands.Parameters.DetectorType.MaxPeak; 
                    break;
            }


            mesureTraceParameter.VBW_Hz = -1; // константа для Level
            mesureTraceParameter.TraceCount = deviceServerParametersDataLevel.TraceCount; 
            mesureTraceParameter.TracePoint = -1; // константа для Level

            switch (deviceServerParametersDataLevel.TraceType)
            {
                case DataModels.Sdrns.Device.OnlineMeasurement.TraceType.Auto:
                    mesureTraceParameter.TraceType = DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.Auto;
                    break;
                case DataModels.Sdrns.Device.OnlineMeasurement.TraceType.Average:
                    mesureTraceParameter.TraceType = DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.Average;
                    break;
                case DataModels.Sdrns.Device.OnlineMeasurement.TraceType.ClearWhrite:
                    mesureTraceParameter.TraceType = DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.ClearWhrite;
                    break;
                case DataModels.Sdrns.Device.OnlineMeasurement.TraceType.MaxHold:
                    mesureTraceParameter.TraceType = DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.MaxHold;
                    break;
                case DataModels.Sdrns.Device.OnlineMeasurement.TraceType.MinHold:
                    mesureTraceParameter.TraceType = DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.MinHold;
                    break;
                case DataModels.Sdrns.Device.OnlineMeasurement.TraceType.Unknown:
                    mesureTraceParameter.TraceType = DataModels.Sdrn.DeviceServer.Commands.Parameters.TraceType.Unknown;
                    break;
            }
            
            mesureTraceParameter.LevelUnit = DataModels.Sdrn.DeviceServer.Commands.Parameters.LevelUnit.dBm; // константа для Level
            return mesureTraceParameter;
        }
    }
}
