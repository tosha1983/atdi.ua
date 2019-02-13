using System;
using ENP = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer
{
    public class LocalParametersConverter
    {
        //public EN.Attenuator Attenuator(int ATTFromParameter)
        //{
        //    EN.Attenuator res = EN.Attenuator.Atten_0;
        //    if (ATTFromParameter == -1) res = EN.Attenuator.Atten_AUTO;
        //    else if (ATTFromParameter == 0) res = EN.Attenuator.Atten_0;
        //    else if (ATTFromParameter == 10) res = EN.Attenuator.Atten_10;
        //    else if (ATTFromParameter == 20) res = EN.Attenuator.Atten_20;
        //    else if (ATTFromParameter == 30) res = EN.Attenuator.Atten_30;
        //    else
        //    {
        //        int delta = int.MaxValue;
        //        foreach (int t in Enum.GetValues(typeof(EN.Attenuator)))
        //        {
        //            if (Math.Abs(ATTFromParameter - t) < delta)
        //            {
        //                delta = Math.Abs(ATTFromParameter - t);
        //                res = (EN.Attenuator)t;
        //            }
        //        }
                
        //    }
        //    return res;
        //}
        //public EN.Gain Gain(int GainFromParameter)
        //{
        //    EN.Gain res = EN.Gain.Gain_0;
        //    if (GainFromParameter == -1) res = EN.Gain.Gain_AUTO;
        //    else if (GainFromParameter == 0) res = EN.Gain.Gain_0;
        //    else if (GainFromParameter == 1) res = EN.Gain.Gain_1;
        //    else if (GainFromParameter == 2) res = EN.Gain.Gain_2;
        //    else if (GainFromParameter == 3) res = EN.Gain.Gain_3;
        //    else
        //    {
        //        int delta = int.MaxValue;
        //        foreach (int t in Enum.GetValues(typeof(EN.Gain)))
        //        {
        //            if (Math.Abs(GainFromParameter - t) < delta)
        //            {
        //                delta = Math.Abs(GainFromParameter - t);
        //                res = (EN.Gain)t;
        //            }
        //        }

        //    }
        //    return res;
        //}
        //public decimal FreqStart(Adapter SH, decimal FreqStartFromParameter)
        //{
        //    decimal res = 0;
        //    if (FreqStartFromParameter < SH.FreqMin) res = SH.FreqMin;
        //    else if (FreqStartFromParameter > SH.FreqMax) res = SH.FreqMax - 1000000;
        //    else res = FreqStartFromParameter;
        //    return res;
        //}
        //public decimal FreqStop(Adapter SH, decimal FreqStopFromParameter)
        //{
        //    decimal res = 0;
        //    if (FreqStopFromParameter > SH.FreqMax) res = SH.FreqMax;
        //    else if(FreqStopFromParameter < SH.FreqMin) res = SH.FreqMin + 1000000;
        //    else res = FreqStopFromParameter;
        //    return res;
        //}
        //public decimal RBW(Adapter SH, decimal RBWFromParameter)
        //{
        //    decimal res = 0;
        //    if (RBWFromParameter > SH.RBWMax) res = SH.RBWMax;
        //    else if (RBWFromParameter < 1) res = 1000;
        //    else res = RBWFromParameter;
        //    return res;
        //}
        //public decimal VBW(Adapter SH, decimal VBWFromParameter)
        //{
        //    decimal res = 0;
        //    if (VBWFromParameter > SH.VBWMax) res = SH.VBWMax;
        //    else if (VBWFromParameter < 1) res = 1000;
        //    else res = VBWFromParameter;
        //    return res;
        //}
        //public EN.TraceType TraceType(ENP.TraceType TraceTypeFromParameter)
        //{
        //    EN.TraceType res = EN.TraceType.ClearWrite;
        //    if (TraceTypeFromParameter == ENP.TraceType.Auto) res = EN.TraceType.ClearWrite;//OO
        //    else if (TraceTypeFromParameter == ENP.TraceType.ClearWhrite) res = EN.TraceType.ClearWrite;
        //    else if (TraceTypeFromParameter == ENP.TraceType.Average) res = EN.TraceType.Average;
        //    else if (TraceTypeFromParameter == ENP.TraceType.MaxHold) res = EN.TraceType.MaxHold;
        //    else if (TraceTypeFromParameter == ENP.TraceType.MinHold) res = EN.TraceType.MinHold;           
        //    return res;
        //}
        //public EN.Detector DetectorType(ENP.DetectorType DetectorTypeFromParameter)
        //{
        //    EN.Detector res = EN.Detector.MaxOnly;
        //    if (DetectorTypeFromParameter == ENP.DetectorType.Auto) res = EN.Detector.MaxOnly;//OO
        //    else if (DetectorTypeFromParameter == ENP.DetectorType.MaxPeak) res = EN.Detector.MaxOnly;
        //    else if (DetectorTypeFromParameter == ENP.DetectorType.MinPeak) res = EN.Detector.MinOnly;
        //    else if (DetectorTypeFromParameter == ENP.DetectorType.Average) res = EN.Detector.Average;
        //    else if (DetectorTypeFromParameter == ENP.DetectorType.RMS) res = EN.Detector.Average;
        //    return res;
        //}
        //public MEN.LevelUnit LevelUnit(ENP.LevelUnit LevelUnitFromParameter)
        //{
        //    MEN.LevelUnit res = MEN.LevelUnit.dBm;
        //    if (LevelUnitFromParameter == ENP.LevelUnit.dBm) res = MEN.LevelUnit.dBm;
        //    else if (LevelUnitFromParameter == ENP.LevelUnit.dBmkV) res = MEN.LevelUnit.dBµV;
        //    else if (LevelUnitFromParameter == ENP.LevelUnit.dBmkVm) res = MEN.LevelUnit.dBµVm;
        //    else if (LevelUnitFromParameter == ENP.LevelUnit.mkV) res = MEN.LevelUnit.µV;
        //    return res;
        //}
    }
}
