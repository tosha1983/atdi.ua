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
    public static class ConvertTaskParametersToMesureTraceParameterForSO
    {
        public static MesureTraceParameter Convert(this TaskParameters taskParameters)
        {
            
            MesureTraceParameter mesureTraceParameter = new MesureTraceParameter();

            mesureTraceParameter.FreqStart_Hz = (decimal)(taskParameters.MinFreq_MHz*1000000 - taskParameters.StepSO_kHz*500);
            mesureTraceParameter.FreqStop_Hz = (decimal)(taskParameters.MaxFreq_MHz*1000000 + taskParameters.StepSO_kHz * 500);
            mesureTraceParameter.SweepTime_s = taskParameters.SweepTime_s;
            mesureTraceParameter.TracePoint = (int)Math.Ceiling((double) ((mesureTraceParameter.FreqStop_Hz - mesureTraceParameter.FreqStart_Hz)) / (1000*(taskParameters.StepSO_kHz / taskParameters.NChenal)));

            mesureTraceParameter.RefLevel_dBm = -1; // константа для SO
            mesureTraceParameter.TraceType = TraceType.ClearWhrite; // константа для SO
            mesureTraceParameter.TraceCount = 1; // константа для SO
            mesureTraceParameter.Att_dB = -1;    // константа для SO
            mesureTraceParameter.PreAmp_dB = -1; // константа для SO
            mesureTraceParameter.DetectorType = DetectorType.MaxPeak; // константа для SO
            mesureTraceParameter.LevelUnit = LevelUnit.dBm; // константа для SO
            mesureTraceParameter.VBW_Hz = -1; // константа для SO
            mesureTraceParameter.RBW_Hz = -1; // константа для SO
            return mesureTraceParameter;
        }
    }
}
