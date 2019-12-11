using System;
using System.Collections.Generic;
using System.Linq;
using ENP = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL
{
    public class LocalParametersConverter
    {       
        public decimal FreqCentrIQ(LocalSpectrumAnalyzerInfo uniqueData, decimal FreqStartFromParameter, decimal FreqStopFromParameter)
        {
            if (FreqStopFromParameter < uniqueData.FreqMin || FreqStopFromParameter > uniqueData.FreqMax)
            {
                throw new Exception("The stop frequency must be set to the available range of the instrument.");
            }
            if (FreqStartFromParameter < uniqueData.FreqMin || FreqStartFromParameter > uniqueData.FreqMax)
            {
                throw new Exception("The start frequency must be set to the available range of the instrument.");
            }
            return (FreqStartFromParameter + FreqStopFromParameter) / 2;
        }
        public decimal FreqSpanIQ(LocalSpectrumAnalyzerInfo uniqueData, decimal FreqStartFromParameter, decimal FreqStopFromParameter)
        {
            if (FreqStopFromParameter < uniqueData.FreqMin || FreqStopFromParameter > uniqueData.FreqMax)
            {
                throw new Exception("The stop frequency must be set to the available range of the instrument.");
            }
            if (FreqStartFromParameter < uniqueData.FreqMin || FreqStartFromParameter > uniqueData.FreqMax)
            {
                throw new Exception("The start frequency must be set to the available range of the instrument.");
            }
            decimal res = FreqStopFromParameter - FreqStartFromParameter;
            if (uniqueData.InstrManufacture == 1)
            {
                if (res > uniqueData.IQMaxSampleSpeed * 0.8m)//т.к. такой прибамбас на всех анализаторах от R&S
                {
                    throw new Exception("The IQ band must be set to the available instrument range.");
                }
            }
            else if (uniqueData.InstrManufacture == 2)
            {

            }
            else if (uniqueData.InstrManufacture == 3)
            {

            }

            return res;
        }
        public decimal RBW(LocalSpectrumAnalyzerInfo uniqueData, decimal RBWFromParameter)
        {
            decimal res = 0;
            decimal delta = decimal.MaxValue;
            int index = 0;
            for (int i = 0; i < uniqueData.RBWArr.Length; i++)
            {
                if (Math.Abs(RBWFromParameter - uniqueData.RBWArr[i]) < delta)
                {
                    delta = Math.Abs(RBWFromParameter - uniqueData.RBWArr[i]);
                    index = i;
                }
            }
            if (index > 1 && uniqueData.RBWArr[index] > RBWFromParameter)
            {
                res = uniqueData.RBWArr[index - 1];
            }
            else
            {
                res = uniqueData.RBWArr[index];
            }
            return res;
        }
        public decimal VBW(LocalSpectrumAnalyzerInfo uniqueData, decimal VBWFromParameter)
        {
            decimal res = 0;
            decimal delta = decimal.MaxValue;
            int index = 0;
            for (int i = 0; i < uniqueData.VBWArr.Length; i++)
            {
                if (Math.Abs(VBWFromParameter - uniqueData.VBWArr[i]) < delta)
                {
                    delta = Math.Abs(VBWFromParameter - uniqueData.VBWArr[i]);
                    index = i;
                }
            }
            if (index > 1 && uniqueData.VBWArr[index] > VBWFromParameter)
            {
                res = uniqueData.VBWArr[index - 1];
            }
            else
            {
                res = uniqueData.VBWArr[index];
            }
            return res;
        }
        public (decimal, bool) SweepTime(LocalSpectrumAnalyzerInfo uniqueData, decimal SweepTimeFromParameter)
        {
            decimal res = 0;
            bool auto = false;
            if (SweepTimeFromParameter != -1)
            {
                if (SweepTimeFromParameter < uniqueData.SWTMin || SweepTimeFromParameter > uniqueData.SWTMax)
                {
                    throw new Exception("The SweepTime must be set to the available range of the instrument.");
                }
                else
                {
                    res = SweepTimeFromParameter;
                }
            }
            else
            {
                auto = true;
            }
            return (res, auto);
        }

        public int SweepPoints(LocalSpectrumAnalyzerInfo uniqueData, int SweepPointsFromParameter)
        {
            int res = 0;
            decimal delta = decimal.MaxValue;
            int index = 0;
            for (int i = 0; i < uniqueData.SweepPointArr.Length; i++)
            {
                if (Math.Abs(SweepPointsFromParameter - uniqueData.SweepPointArr[i]) < delta)
                {
                    delta = Math.Abs(SweepPointsFromParameter - uniqueData.SweepPointArr[i]);
                    index = i;
                }
            }
            if (index < uniqueData.SweepPointArr.Length - 2 && uniqueData.SweepPointArr[index] < SweepPointsFromParameter)
            {
                res = uniqueData.SweepPointArr[index + 1];
            }
            else
            {
                res = uniqueData.SweepPointArr[index];
            }

            return res;
        }

        public (ParamWithId, EN.TraceType) TraceType(LocalSpectrumAnalyzerInfo uniqueData, ENP.TraceType TraceTypeFromParameter)
        {
            ParamWithId id = new ParamWithId { Id = 0, Parameter = "BLAN" };
            EN.TraceType type = EN.TraceType.ClearWrite;
            for (int i = 0; i < uniqueData.TraceType.Count; i++)
            {
                //Всегда исходный ClearWrite
                if (uniqueData.TraceType[i].Id == (int)EN.TraceType.ClearWrite)
                {
                    id = uniqueData.TraceType[i];
                }
                //Результирующий по наявности
                if (TraceTypeFromParameter == ENP.TraceType.ClearWhrite && uniqueData.TraceType[i].Id == (int)EN.TraceType.ClearWrite)
                {
                    type = EN.TraceType.ClearWrite;
                }
                else if (TraceTypeFromParameter == ENP.TraceType.Average && uniqueData.TraceType[i].Id == (int)EN.TraceType.Average)
                {
                    type = EN.TraceType.Average;
                }
                else if (TraceTypeFromParameter == ENP.TraceType.MaxHold && uniqueData.TraceType[i].Id == (int)EN.TraceType.MaxHold)
                {
                    type = EN.TraceType.MaxHold;
                }
                else if (TraceTypeFromParameter == ENP.TraceType.MinHold && uniqueData.TraceType[i].Id == (int)EN.TraceType.MinHold)
                {
                    type = EN.TraceType.MinHold;
                }
                else if (TraceTypeFromParameter == ENP.TraceType.Auto && uniqueData.TraceType[i].Id == (int)EN.TraceType.ClearWrite)
                {
                    //По результатам согласования принято такое решение
                    type = EN.TraceType.ClearWrite;
                }
            }
            return (id, type);
        }
        public ParamWithId DetectorType(LocalSpectrumAnalyzerInfo uniqueData, ENP.DetectorType DetectorTypeFromParameter)
        {
            ParamWithId res = new ParamWithId { Id = 0, Parameter = "BLAN" };
            for (int i = 0; i < uniqueData.TraceDetector.Count; i++)
            {
                if (DetectorTypeFromParameter == ENP.DetectorType.Auto && uniqueData.TraceDetector[i].Id == (int)EN.TraceDetector.AutoPeak)
                {
                    res = uniqueData.TraceDetector[i];
                }
                else if (DetectorTypeFromParameter == ENP.DetectorType.Average && uniqueData.TraceDetector[i].Id == (int)EN.TraceDetector.Average)
                {
                    res = uniqueData.TraceDetector[i];
                }
                else if (DetectorTypeFromParameter == ENP.DetectorType.MaxPeak && uniqueData.TraceDetector[i].Id == (int)EN.TraceDetector.MaxPeak)
                {
                    res = uniqueData.TraceDetector[i];
                }
                else if (DetectorTypeFromParameter == ENP.DetectorType.MinPeak && uniqueData.TraceDetector[i].Id == (int)EN.TraceDetector.MinPeak)
                {
                    res = uniqueData.TraceDetector[i];
                }
                else if (DetectorTypeFromParameter == ENP.DetectorType.RMS && uniqueData.TraceDetector[i].Id == (int)EN.TraceDetector.RMS)
                {
                    res = uniqueData.TraceDetector[i];
                }
            }
            if (res.Parameter == "BLAN")
            {
                throw new Exception("The TraceDetector must be set to the available instrument range.");
            }
            return res;
        }
        public (ParamWithId, MEN.LevelUnit) LevelUnit(LocalSpectrumAnalyzerInfo uniqueData, ENP.LevelUnit LevelUnitFromParameter)
        {
            MEN.LevelUnit res = MEN.LevelUnit.NotSet;
            ParamWithId tupe = new ParamWithId { Id = 0, Parameter = "BLAN" };
            for (int i = 0; i < uniqueData.LevelUnits.Count; i++)
            {
                if (LevelUnitFromParameter == ENP.LevelUnit.dBm && uniqueData.LevelUnits[i].Id == (int)MEN.LevelUnit.dBm)
                {
                    res = MEN.LevelUnit.dBm;
                    tupe = uniqueData.LevelUnits[i];
                }
                else if (LevelUnitFromParameter == ENP.LevelUnit.dBmkV && uniqueData.TraceDetector[i].Id == (int)MEN.LevelUnit.dBµV)
                {
                    res = MEN.LevelUnit.dBµV;
                    tupe = uniqueData.LevelUnits[i];
                }
            }
            if (res == MEN.LevelUnit.NotSet)
            {
                throw new Exception("The LevelUnits must be set to the available instrument range.");
            }
            return (tupe, res);
        }
        public decimal SampleSpeed(LocalSpectrumAnalyzerInfo uniqueData, decimal SampleSpeedFromParameter)
        {
            decimal res = 10000;
            if (SampleSpeedFromParameter < uniqueData.IQMinSampleSpeed || SampleSpeedFromParameter > uniqueData.IQMaxSampleSpeed)
            {
                throw new Exception("The SampleSpeed must be set to the available range of the instrument.");
            }
            else
            {
                res = SampleSpeedFromParameter;
            }

            if (uniqueData.InstrManufacture == 1)
            {

            }
            else if (uniqueData.InstrManufacture == 2)
            {

            }
            else if (uniqueData.InstrManufacture == 3)
            {

            }

            return res;
        }
    }
}
