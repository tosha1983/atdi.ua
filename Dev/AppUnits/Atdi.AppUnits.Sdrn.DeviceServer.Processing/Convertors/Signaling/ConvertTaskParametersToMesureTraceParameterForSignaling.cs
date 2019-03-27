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

            mesureTraceParameter.FreqStart_Hz = (decimal)(taskParameters.MinFreq_MHz*1000000 - taskParameters.StepSO_kHz*500);
            mesureTraceParameter.FreqStop_Hz = (decimal)(taskParameters.MaxFreq_MHz*1000000 + taskParameters.StepSO_kHz * 500);
            mesureTraceParameter.SweepTime_s = taskParameters.SweepTime_s;
            mesureTraceParameter.TracePoint = (int)Math.Ceiling((double) ((mesureTraceParameter.FreqStop_Hz - mesureTraceParameter.FreqStart_Hz)) / (1000*(taskParameters.StepSO_kHz / taskParameters.NChenal)));

            mesureTraceParameter.RefLevel_dBm = -1; // константа для Signaling
            mesureTraceParameter.TraceType = TraceType.MaxHold; // константа для Signaling
            mesureTraceParameter.TraceCount = 10; // константа для Signaling
            mesureTraceParameter.Att_dB = -1;    // константа для Signaling
            mesureTraceParameter.PreAmp_dB = -1; // константа для Signaling
            mesureTraceParameter.DetectorType = DetectorType.MaxPeak; // константа для Signaling
            mesureTraceParameter.LevelUnit = LevelUnit.dBm; // константа для Signaling
            mesureTraceParameter.VBW_Hz = -1; // константа для Signaling
            mesureTraceParameter.RBW_Hz = -1; // константа для Signaling
            return mesureTraceParameter;
        }
    }
}
