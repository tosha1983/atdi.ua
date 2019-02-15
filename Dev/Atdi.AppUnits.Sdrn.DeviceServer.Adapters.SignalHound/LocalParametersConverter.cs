using System;
using PEN = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    public class LocalParametersConverter
    {
        public EN.Attenuator Attenuator(int ATTFromParameter)
        {
            EN.Attenuator res = EN.Attenuator.Atten_0;
            if (ATTFromParameter == -1)
            {
                res = EN.Attenuator.Atten_AUTO;
            }
            else if (ATTFromParameter == 0)
            {
                res = EN.Attenuator.Atten_0;
            }
            else if (ATTFromParameter == 10)
            {
                res = EN.Attenuator.Atten_10;
            }
            else if (ATTFromParameter == 20)
            {
                res = EN.Attenuator.Atten_20;
            }
            else if (ATTFromParameter == 30)
            {
                res = EN.Attenuator.Atten_30;
            }
            else
            {
                int delta = int.MaxValue;
                foreach (int t in Enum.GetValues(typeof(EN.Attenuator)))
                {
                    if (Math.Abs(ATTFromParameter - t) < delta)
                    {
                        delta = Math.Abs(ATTFromParameter - t);
                        res = (EN.Attenuator)t;
                    }
                }

            }
            return res;
        }
        public EN.Gain Gain(int GainFromParameter)
        {
            EN.Gain res = EN.Gain.Gain_0;
            if (GainFromParameter == -1)
            {
                res = EN.Gain.Gain_AUTO;
            }
            else if (GainFromParameter == 0)
            {
                res = EN.Gain.Gain_0;
            }
            else if (GainFromParameter == 1)
            {
                res = EN.Gain.Gain_1;
            }
            else if (GainFromParameter == 2)
            {
                res = EN.Gain.Gain_2;
            }
            else if (GainFromParameter == 3)
            {
                res = EN.Gain.Gain_3;
            }
            else
            {
                int delta = int.MaxValue;
                foreach (int t in Enum.GetValues(typeof(EN.Gain)))
                {
                    if (Math.Abs(GainFromParameter - t) < delta)
                    {
                        delta = Math.Abs(GainFromParameter - t);
                        res = (EN.Gain)t;
                    }
                }

            }
            return res;
        }
        public decimal FreqStart(Adapter SH, decimal FreqStartFromParameter)
        {
            decimal res = 0;
            if (FreqStartFromParameter < SH.FreqMin)
            {
                res = SH.FreqMin;
            }
            else if (FreqStartFromParameter > SH.FreqMax)
            {
                res = SH.FreqMax - 1000000;
            }
            else
            {
                res = FreqStartFromParameter;
            }
            return res;
        }
        public decimal FreqStop(Adapter SH, decimal FreqStopFromParameter)
        {
            decimal res = 0;
            if (FreqStopFromParameter > SH.FreqMax)
            {
                res = SH.FreqMax;
            }
            else if (FreqStopFromParameter < SH.FreqMin)
            {
                res = SH.FreqMin + 1000000;
            }
            else
            {
                res = FreqStopFromParameter;
            }
            return res;
        }
        public decimal RBW(Adapter SH, decimal RBWFromParameter)
        {
            decimal res = 0;
            if (RBWFromParameter > SH.RBWMax)
            {
                res = SH.RBWMax;
            }
            else if (RBWFromParameter < 1)
            {
                res = 1000;
            }
            else
            {
                res = RBWFromParameter;
            }
            return res;
        }
        public decimal VBW(Adapter SH, decimal VBWFromParameter)
        {
            decimal res = 0;
            if (VBWFromParameter > SH.VBWMax)
            {
                res = SH.VBWMax;
            }
            else if (VBWFromParameter < 1)
            {
                res = 1000;
            }
            else
            {
                res = VBWFromParameter;
            }
            return res;
        }
        public EN.TraceType TraceType(PEN.TraceType TraceTypeFromParameter)
        {
            EN.TraceType res = EN.TraceType.ClearWrite;
            if (TraceTypeFromParameter == PEN.TraceType.Auto)
            {
                //По результатам согласования принято такое решение
                res = EN.TraceType.ClearWrite;//OO
            }
            else if (TraceTypeFromParameter == PEN.TraceType.ClearWhrite)
            {
                res = EN.TraceType.ClearWrite;
            }
            else if (TraceTypeFromParameter == PEN.TraceType.Average)
            {
                res = EN.TraceType.Average;
            }
            else if (TraceTypeFromParameter == PEN.TraceType.MaxHold)
            {
                res = EN.TraceType.MaxHold;
            }
            else if (TraceTypeFromParameter == PEN.TraceType.MinHold)
            {
                res = EN.TraceType.MinHold;
            }
            return res;
        }
        public void DetectorType(Adapter SH, PEN.DetectorType DetectorTypeFromParameter)
        {
            SH.DetectorUse = EN.Detector.MaxOnly;
            SH.DetectorToSet = EN.Detector.MinAndMax;
            if (DetectorTypeFromParameter == PEN.DetectorType.Auto)
            {
                //По результатам согласования принято такое решение
                SH.DetectorUse = EN.Detector.MaxOnly;
                SH.DetectorToSet = EN.Detector.MinAndMax;
            }
            else if (DetectorTypeFromParameter == PEN.DetectorType.MaxPeak)
            {
                //По результатам согласования принято такое решение
                SH.DetectorUse = EN.Detector.MaxOnly;
                SH.DetectorToSet = EN.Detector.MinAndMax;
            }
            else if (DetectorTypeFromParameter == PEN.DetectorType.MinPeak)
            {
                //По результатам согласования принято такое решение
                SH.DetectorUse = EN.Detector.MinOnly;
                SH.DetectorToSet = EN.Detector.MinAndMax;
            }
            else if (DetectorTypeFromParameter == PEN.DetectorType.Average)
            {
                //По результатам согласования принято такое решение
                SH.DetectorUse = EN.Detector.Average;
                SH.DetectorToSet = EN.Detector.Average;
            }
            else if (DetectorTypeFromParameter == PEN.DetectorType.RMS)
            {
                //По результатам согласования принято такое решение
                SH.DetectorUse = EN.Detector.Average;
                SH.DetectorToSet = EN.Detector.Average;
            }
        }
        public MEN.LevelUnit LevelUnit(PEN.LevelUnit LevelUnitFromParameter)
        {
            MEN.LevelUnit res = MEN.LevelUnit.dBm;
            if (LevelUnitFromParameter == PEN.LevelUnit.dBm)
            {
                res = MEN.LevelUnit.dBm;
            }
            else if (LevelUnitFromParameter == PEN.LevelUnit.dBmkV)
            {
                res = MEN.LevelUnit.dBµV;
            }
            else if (LevelUnitFromParameter == PEN.LevelUnit.dBmkVm)
            {
                //По результатам согласования принято такое решение
                throw new Exception("LevelUnit must be set to dbm or dBmkVm");
                //res = MEN.LevelUnit.dBµVm;
            }
            else if (LevelUnitFromParameter == PEN.LevelUnit.mkV)
            {
                //По результатам согласования принято такое решение
                throw new Exception("LevelUnit must be set to dbm or dBmkVm");
                //res = MEN.LevelUnit.µV;
            }
            return res;
        }
    }
}
