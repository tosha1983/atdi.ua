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
    public static class ConvertTaskParametersToMesureTraceParameterForSignaling
    {
        public static MesureTraceParameter ConvertForSignaling(this TaskParameters taskParameters)
        {

            MesureTraceParameter mesureTraceParameter = new MesureTraceParameter();

            mesureTraceParameter.FreqStart_Hz = (decimal)(taskParameters.MinFreq_MHz * 1000000 - taskParameters.StepSO_kHz * 500);
            mesureTraceParameter.FreqStop_Hz = (decimal)(taskParameters.MaxFreq_MHz * 1000000 + taskParameters.StepSO_kHz * 500);
            mesureTraceParameter.SweepTime_s = taskParameters.SweepTime_s;
            mesureTraceParameter.TracePoint = (int)Math.Ceiling((double)((mesureTraceParameter.FreqStop_Hz - mesureTraceParameter.FreqStart_Hz)) / (1000 * (taskParameters.StepSO_kHz / taskParameters.NChenal)));

            mesureTraceParameter.TraceCount = 1; // константа для Signaling
            mesureTraceParameter.TraceType = TraceType.MaxHold; // константа для Signaling
            mesureTraceParameter.LevelUnit = LevelUnit.dBm; // константа для Signaling
            mesureTraceParameter.VBW_Hz = -1; // константа для Signaling
            mesureTraceParameter.RBW_Hz = -2; // константа для Signaling


            // Авто 
            mesureTraceParameter.RefLevel_dBm = (int)taskParameters.RefLevel_dBm.Value;
            mesureTraceParameter.Att_dB = taskParameters.RfAttenuation_dB.Value;    // константа для Signaling
            mesureTraceParameter.PreAmp_dB = taskParameters.Preamplification_dB.Value; // константа для Signaling
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
                    mesureTraceParameter.DetectorType = DetectorType.MaxPeak; // константа для Signaling
                    break;
            }

            return mesureTraceParameter;
        }
    }
}
