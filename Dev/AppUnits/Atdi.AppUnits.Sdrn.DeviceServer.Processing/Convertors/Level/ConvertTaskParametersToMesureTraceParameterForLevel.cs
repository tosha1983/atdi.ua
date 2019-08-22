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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public static class ConvertTaskParametersToMesureTraceParameterForLevel
    {
        public static MesureTraceParameter ConvertForLevel(this TaskParameters taskParameters)
        {

            MesureTraceParameter mesureTraceParameter = new MesureTraceParameter();

            mesureTraceParameter.FreqStart_Hz = (decimal)(taskParameters.MinFreq_MHz * 1000000);
            mesureTraceParameter.FreqStop_Hz = (decimal)(taskParameters.MaxFreq_MHz * 1000000);
            mesureTraceParameter.SweepTime_s = taskParameters.SweepTime_s;

            mesureTraceParameter.RefLevel_dBm = (int)taskParameters.RefLevel_dBm.Value;
            mesureTraceParameter.Att_dB = taskParameters.RfAttenuation_dB.Value;    
            mesureTraceParameter.PreAmp_dB = taskParameters.Preamplification_dB.Value;
            mesureTraceParameter.RBW_Hz = taskParameters.RBW_Hz.Value;
            switch (taskParameters.DetectType)
            {
                case DataModels.Sdrns.DetectingType.Average:
                    mesureTraceParameter.DetectorType = DetectorType.Average;
                    break;
                case DataModels.Sdrns.DetectingType.MaxPeak:
                    mesureTraceParameter.DetectorType = DetectorType.MaxPeak;
                    break;
                case DataModels.Sdrns.DetectingType.MinPeak:
                    mesureTraceParameter.DetectorType = DetectorType.MinPeak;
                    break;
                case DataModels.Sdrns.DetectingType.Peak:
                    mesureTraceParameter.DetectorType = DetectorType.MaxPeak;
                    break;
                case DataModels.Sdrns.DetectingType.RMS:
                    mesureTraceParameter.DetectorType = DetectorType.RMS;
                    break;
                case DataModels.Sdrns.DetectingType.Auto:
                    mesureTraceParameter.DetectorType = DetectorType.Auto;
                    break;
                default:
                    mesureTraceParameter.DetectorType = DetectorType.MaxPeak; // константа для Level
                    break;
            }


            mesureTraceParameter.VBW_Hz = -1; // константа для Level
            mesureTraceParameter.TraceCount = 1; // константа для Level
            mesureTraceParameter.TracePoint = -1; // константа для Level
            mesureTraceParameter.TraceType = TraceType.ClearWhrite; // константа для Level
            mesureTraceParameter.LevelUnit = LevelUnit.dBm; // константа для Level


            return mesureTraceParameter;
        }
    }
}
