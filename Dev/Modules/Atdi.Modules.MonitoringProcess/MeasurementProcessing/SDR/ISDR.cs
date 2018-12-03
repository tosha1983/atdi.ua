using System.Collections.Generic;
//using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.Modules.MonitoringProcess
{
    /// <summary>
    /// Represent function any type measurements
    /// </summary>
    public interface ISDR
    {
        bool Initiation();
        bool Calibration();
        bool ResetDevice();
        void Close();
        SDRLoc GetSDRLocation();

        bool SetConfiguration(SDRParameters sDRParameters);

        bool ChangeSpan(double MinFrequency_MHz, double MaxFrequency_MHz);
        bool ChangeSweepCoupling(double RBW_Hz, double VBW_Hz);
        bool ChangeSweepGain(int Gain);
        bool ChangeSweepAtt(double Att);
        bool ChangeSweepRefLevel(double RefLevel_dBm);


        float[] GetTrace(int TraceCount = 1);
        bool GetIQStream(ref float[] iqSample, ref int[] trigger);

        SDRState GetSDRState();
        int GetLastTaskId();
        SDRTraceParameters GetSDRTraceParameters();
    }
}