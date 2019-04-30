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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class ConvertTaskParametersToMesureTraceParameterForBandWidth
    {
        public static MesureTraceParameter ConvertForBW(this TaskParameters taskParameters)
        {
            
            MesureTraceParameter mesureTraceParameter = new MesureTraceParameter();

            mesureTraceParameter.FreqStart_Hz = (decimal)(taskParameters.MinFreq_MHz*1000000);
            mesureTraceParameter.FreqStop_Hz = (decimal)(taskParameters.MaxFreq_MHz*1000000);
            mesureTraceParameter.SweepTime_s = taskParameters.SweepTime_s;
            mesureTraceParameter.TraceCount = taskParameters.NCount;
            if (mesureTraceParameter.TraceCount == 0) { mesureTraceParameter.TraceCount = 1; }
            mesureTraceParameter.TracePoint = 600; // константа для BandWidth
            mesureTraceParameter.RefLevel_dBm = 1000000000; // константа для BandWidth
            mesureTraceParameter.TraceType = TraceType.MaxHold; // константа для BandWidth
            mesureTraceParameter.Att_dB = -1;    // константа для BandWidth
            mesureTraceParameter.PreAmp_dB = -1; // константа для BandWidth
            mesureTraceParameter.DetectorType = DetectorType.MaxPeak; // константа для BandWidth
            mesureTraceParameter.LevelUnit = LevelUnit.dBm; // константа для BandWidth
            mesureTraceParameter.VBW_Hz = -1; // константа для BandWidth
            mesureTraceParameter.RBW_Hz = -1; // константа для BandWidth
            return mesureTraceParameter;
        }
    }
}
