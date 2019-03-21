using System;
using ENP = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer
{
    //public class LocalParametersConverter
    //{
    //    public decimal RefLevel(LocalSpectrumAnalyzerInfo uniqueData, int RefLevelFromParameter)
    //    {
    //        decimal res = 0;
    //        if (RefLevelFromParameter > uniqueData.RefLevelMax) res = uniqueData.RefLevelMax;
    //        else if (RefLevelFromParameter < uniqueData.RefLevelMin) res = uniqueData.RefLevelMin;
    //        else res = RefLevelFromParameter;
    //        return res;
    //    }
    //    public decimal Attenuator(LocalSpectrumAnalyzerInfo uniqueData, int ATTFromParameter)
    //    {
    //        decimal res = 0;
    //        decimal delta = decimal.MaxValue;
    //        for (decimal i = 0; i < uniqueData.AttMax; i += uniqueData.AttStep)
    //        {
    //            if (Math.Abs(ATTFromParameter - i) < delta)
    //            {
    //                delta = Math.Abs(ATTFromParameter - i);
    //                res = i;
    //            }
    //        }
    //        return res;
    //    }
    //    public bool PreAmp(int PreAmpFromParameter)
    //    {
    //        bool res = false;
    //        if (PreAmpFromParameter == -1)
    //        {
    //            res = false;//потому что нет альтернативы
    //        }
    //        else if (PreAmpFromParameter == 0)
    //        {
    //            res = false;
    //        }
    //        else if (PreAmpFromParameter == 1)
    //        {
    //            res = true;
    //        }
    //        else if (PreAmpFromParameter > 1 || PreAmpFromParameter < -1)
    //        {
    //            throw new Exception("PreAmp must be set to within limits.");
    //        }
            
    //        return res;
    //    }
    //    public decimal FreqStart(Adapter AN, decimal FreqStartFromParameter)
    //    {
    //        decimal res = 0;
    //        if (FreqStartFromParameter < AN.FreqMin) res = AN.FreqMin;
    //        else if (FreqStartFromParameter > AN.FreqMax) res = AN.FreqMax - 1000000;
    //        else res = FreqStartFromParameter;
    //        return res;
    //    }
    //    public decimal FreqStop(Adapter AN, decimal FreqStopFromParameter)
    //    {
    //        decimal res = 0;
    //        if (FreqStopFromParameter > AN.FreqMax) res = AN.FreqMax;
    //        else if (FreqStopFromParameter < AN.FreqMin) res = AN.FreqMin + 1000000;
    //        else res = FreqStopFromParameter;
    //        return res;
    //    }
    //    public decimal RBW(LocalSpectrumAnalyzerInfo uniqueData, decimal RBWFromParameter)
    //    {
    //        int res = 0;
    //        decimal delta = decimal.MaxValue;
    //        for (int i = 0; i < uniqueData.RBWArr.Length; i++)
    //        {
    //            if (Math.Abs(RBWFromParameter - uniqueData.RBWArr[i]) < delta)
    //            {
    //                delta = Math.Abs(RBWFromParameter - uniqueData.RBWArr[i]);
    //                res = i;
    //            }
    //        }
    //        return uniqueData.RBWArr[res];
    //    }
    //    public decimal VBW(LocalSpectrumAnalyzerInfo uniqueData, decimal VBWFromParameter)
    //    {
    //        int res = 0;
    //        decimal delta = decimal.MaxValue;
    //        for (int i = 0; i < uniqueData.VBWArr.Length; i++)
    //        {
    //            if (Math.Abs(VBWFromParameter - uniqueData.VBWArr[i]) < delta)
    //            {
    //                delta = Math.Abs(VBWFromParameter - uniqueData.VBWArr[i]);
    //                res = i;
    //            }
    //        }
    //        return uniqueData.VBWArr[res];
    //    }
    //    public EN.TraceType TraceType(LocalSpectrumAnalyzerInfo uniqueData, ENP.TraceType TraceTypeFromParameter)
    //    {
    //        EN.TraceType res = EN.TraceType.ClearWrite;
    //        for (int i = 0; i < uniqueData.TraceType.Count; i++)
    //        {
    //            if (TraceTypeFromParameter == ENP.TraceType.ClearWhrite && uniqueData.TraceType[i].Id == (int)EN.TraceType.ClearWrite)
    //            {
    //                res = EN.TraceType.ClearWrite;
    //            }
    //            else if (TraceTypeFromParameter == ENP.TraceType.Average && uniqueData.TraceType[i].Id == (int)EN.TraceType.Average)
    //            {
    //                res = EN.TraceType.Average;
    //            }
    //            else if (TraceTypeFromParameter == ENP.TraceType.MaxHold && uniqueData.TraceType[i].Id == (int)EN.TraceType.MaxHold)
    //            {
    //                res = EN.TraceType.MaxHold;
    //            }
    //            else if (TraceTypeFromParameter == ENP.TraceType.MinHold && uniqueData.TraceType[i].Id == (int)EN.TraceType.MinHold)
    //            {
    //                res = EN.TraceType.MinHold;
    //            }
    //            else if (TraceTypeFromParameter == ENP.TraceType.Auto && uniqueData.TraceType[i].Id == (int)EN.TraceType.ClearWrite)
    //            {
    //                //По результатам согласования принято такое решение
    //                res = EN.TraceType.ClearWrite;
    //            }
    //        }
    //        //EN.TraceType res = EN.TraceType.ClearWrite;
    //        //if (TraceTypeFromParameter == ENP.TraceType.Auto) res = EN.TraceType.ClearWrite;//OO
    //        //else if (TraceTypeFromParameter == ENP.TraceType.ClearWhrite) res = EN.TraceType.ClearWrite;
    //        //else if (TraceTypeFromParameter == ENP.TraceType.Average) res = EN.TraceType.Average;
    //        //else if (TraceTypeFromParameter == ENP.TraceType.MaxHold) res = EN.TraceType.MaxHold;
    //        //else if (TraceTypeFromParameter == ENP.TraceType.MinHold) res = EN.TraceType.MinHold;
    //        return res;
    //    }
    //    public EN.Detector DetectorType(ENP.DetectorType DetectorTypeFromParameter)
    //    {
    //        EN.Detector res = EN.Detector.MaxOnly;
    //        if (DetectorTypeFromParameter == ENP.DetectorType.Auto) res = EN.Detector.MaxOnly;//OO
    //        else if (DetectorTypeFromParameter == ENP.DetectorType.MaxPeak) res = EN.Detector.MaxOnly;
    //        else if (DetectorTypeFromParameter == ENP.DetectorType.MinPeak) res = EN.Detector.MinOnly;
    //        else if (DetectorTypeFromParameter == ENP.DetectorType.Average) res = EN.Detector.Average;
    //        else if (DetectorTypeFromParameter == ENP.DetectorType.RMS) res = EN.Detector.Average;
    //        return res;
    //    }
    //    public MEN.LevelUnit LevelUnit(ENP.LevelUnit LevelUnitFromParameter)
    //    {
    //        MEN.LevelUnit res = MEN.LevelUnit.dBm;
    //        if (LevelUnitFromParameter == ENP.LevelUnit.dBm) res = MEN.LevelUnit.dBm;
    //        else if (LevelUnitFromParameter == ENP.LevelUnit.dBmkV) res = MEN.LevelUnit.dBµV;
    //        else if (LevelUnitFromParameter == ENP.LevelUnit.dBmkVm) res = MEN.LevelUnit.dBµVm;
    //        else if (LevelUnitFromParameter == ENP.LevelUnit.mkV) res = MEN.LevelUnit.µV;
    //        return res;
    //    }
    //}
}
