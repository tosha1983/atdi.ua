using System;
using PEN = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using System.Diagnostics;

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
        public int Attenuator(EN.Attenuator ATTFromDevice)
        {
            int res = 0;
            if (ATTFromDevice == EN.Attenuator.Atten_AUTO)
            {
                res = -1;
            }
            else if (ATTFromDevice == EN.Attenuator.Atten_0)
            {
                res = 0;
            }
            else if (ATTFromDevice == EN.Attenuator.Atten_10)
            {
                res = 10;
            }
            else if (ATTFromDevice == EN.Attenuator.Atten_20)
            {
                res = 20;
            }
            else if (ATTFromDevice == EN.Attenuator.Atten_30)
            {
                res = 30;
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
            else if (GainFromParameter == 10)
            {
                res = EN.Gain.Gain_1;
            }
            else if (GainFromParameter == 20)
            {
                res = EN.Gain.Gain_2;
            }
            else if (GainFromParameter == 30)
            {
                res = EN.Gain.Gain_3;
            }
            else
            {
                int delta = int.MaxValue;
                foreach (int t in Enum.GetValues(typeof(EN.Gain)))
                {
                    if (Math.Abs(GainFromParameter - t * 10) < delta)
                    {
                        delta = Math.Abs(GainFromParameter - t * 10);
                        res = (EN.Gain)t;
                    }
                }

            }
            return res;
        }
        public int Gain(EN.Gain GainFromDevice)
        {
            int res = 0;
            if (GainFromDevice == EN.Gain.Gain_AUTO)
            {
                res = -1;
            }
            else if (GainFromDevice == EN.Gain.Gain_0)
            {
                res = 0;
            }
            else if (GainFromDevice == EN.Gain.Gain_1)
            {
                res = 10;
            }
            else if (GainFromDevice == EN.Gain.Gain_2)
            {
                res = 20;
            }
            else if (GainFromDevice == EN.Gain.Gain_3)
            {
                res = 30;
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


        public (decimal FreqStart, decimal FreqStop) IQFreqStartStop(Adapter SH, decimal FreqStartFromParameter, decimal FreqStopFromParameter)
        {
            decimal fstart = 0, fstop = 0;
            if (FreqStopFromParameter - FreqStartFromParameter <= 20000000)
            {
                if (FreqStartFromParameter < FreqStopFromParameter)
                {
                    if (FreqStartFromParameter >= SH.FreqMin)
                    {
                        fstart = FreqStartFromParameter;
                    }
                    else
                    {
                        throw new Exception("FreqStart < FreqMin");
                    }

                    if (FreqStopFromParameter <= SH.FreqMax)
                    {
                        fstop = FreqStopFromParameter;
                    }
                    else
                    {
                        throw new Exception("FreqStop > FreqMax");
                    }
                }
                else
                {
                    throw new Exception("FreqStart > FreqStop");
                }
            }
            else
            {
                throw new Exception("Span > 20 MHz");
            }

            return (fstart, fstop);
        }
        public int IQDownsampleFactor(double BitRate_MBsFromParameter, decimal Span)
        {
            int res = 1;
            double BitRate = BitRate_MBsFromParameter * 1000000;
            if (Span <= 20000000)
            {
                if (BitRate < 0)
                {
                    BitRate = 40000000;
                }

                if ((decimal)BitRate / Span >= 2)
                {
                    double df = 40000000 / BitRate;
                    if (df < 2) res = 1;
                    else if (df >= 2 && df < 4) res = 2;
                    else if (df >= 4 && df < 8) res = 4;
                    else if (df >= 8 && df < 16) res = 8;
                    else if (df >= 16 && df < 32) res = 16;
                    else if (df >= 32 && df < 64) res = 32;
                    else if (df >= 64 && df < 128) res = 64;
                    else if (df >= 128 && df < 256) res = 128;
                    else if (df >= 256 && df < 512) res = 256;
                    else if (df >= 512 && df < 1024) res = 512;
                    else if (df >= 1024 && df < 2048) res = 1024;
                    else if (df >= 2048 && df < 4096) res = 2048;
                    else if (df >= 4096 && df < 8192) res = 4096;
                    else if (df >= 8192) res = 8192;
                }
                else
                {
                    throw new Exception("BitRate / Span < 2");
                }

            }
            else
            {
                throw new Exception("Span > 20 MHz");
            }
            return res;
        }

        public (double BlockDuration, double ReceiveTime) IQTimeParameters(double BlockDuration, double ReceiveTime)
        {
            double blockduration = 1, receivetime = 1;
            if (BlockDuration > 0 && ReceiveTime > 0)
            {
                if (BlockDuration <= ReceiveTime)
                {
                    blockduration = BlockDuration;
                    receivetime = ReceiveTime;
                }
                else
                {
                    throw new Exception("BlockDuration > ReceiveTime");
                }
            }
            else
            {
                if (BlockDuration <= 0)
                    throw new Exception("BlockDuration cannot be less than or equal to zero.");

                if (ReceiveTime <= 0)
                    throw new Exception("ReceiveTime cannot be less than or equal to zero.");
            }
            return (blockduration, receivetime);
        }
    }
}
