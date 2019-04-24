using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    static class CommonCalcPowFromTrace
    { 
        public static double GetPow_dBm(BWResult BWResult)
        {// НЕ ТЕСТИРОВАННО
            if (BWResult.СorrectnessEstimations)
            {
                return GetPow_dBm(BWResult, BWResult.T1, BWResult.T2);
            }
            {
                return GetPow_dBm(BWResult, 0, BWResult.Levels_dBm.Length-1);
            }
        }
        public static double GetPow_dBm(BWResult BWResult, int T1, int T2)
        {// НЕ ТЕСТИРОВАННО
            if (T1 > T2) { int A1 = T1; T1 = T2; T2 = T1;}
            if (T1 < 0) { return -999;}
            if (T2 > BWResult.Levels_dBm.Length-1) { return -999; }
            double Pow_mW = 0;
            for (int i = T1; i<=T2; i++)
            {
                Pow_mW = Pow_mW + Math.Pow(10, BWResult.Levels_dBm[i] / 10);
            }
            return 10*Math.Log10(Pow_mW);
        }
        public static double GetPow_dBm(float []Levels_dBm, double StartLevelFreq_Hz, double StepLevelFreq_Hz, double StartSignalFreq_Hz, double StopSignalFreq_Hz)
        { // НЕ ТЕСТИРОВАННО
            if (StartSignalFreq_Hz > StopSignalFreq_Hz) { double A1 = StartSignalFreq_Hz; StartSignalFreq_Hz = StopSignalFreq_Hz; StopSignalFreq_Hz = StartSignalFreq_Hz;}
            if (StartSignalFreq_Hz < StartLevelFreq_Hz) { return -999; }
            if (StopSignalFreq_Hz > StartLevelFreq_Hz + StepLevelFreq_Hz * (Levels_dBm.Length - 1)) { return -999; }
            int T1 = (int) Math.Ceiling((StartSignalFreq_Hz - StartLevelFreq_Hz) / StepLevelFreq_Hz);
            int T2 = (int)Math.Floor((StartSignalFreq_Hz - StartLevelFreq_Hz) / StepLevelFreq_Hz);
            if (T1 < 0) { T1 = 0;}
            if (T2 > Levels_dBm.Length - 1) { T2 = Levels_dBm.Length - 1; }
            if (T1 >= T2) { return Levels_dBm[T1] + 10 * Math.Log10((-StartSignalFreq_Hz + StopSignalFreq_Hz)/StepLevelFreq_Hz);}
            double Pow_mW = 0;
            Pow_mW = Levels_dBm[T1] + 10 * Math.Log10((StartLevelFreq_Hz + StepLevelFreq_Hz*T1 - StartSignalFreq_Hz) / StepLevelFreq_Hz);
            for (int i = T1; i <= T2; i++)
            {
                Pow_mW = Pow_mW + Math.Pow(10, Levels_dBm[i] / 10);
            }
            Pow_mW = Levels_dBm[T2] + 10 * Math.Log10((StopSignalFreq_Hz - StartLevelFreq_Hz - StepLevelFreq_Hz * T2) / StepLevelFreq_Hz);
            return 10 * Math.Log10(Pow_mW);
        }
    }
}
