using System;
using PEN = Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums;
using MEN = Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums;
using System.Diagnostics;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    public class LocalParametersConverter
    {
        private readonly ILogger logger;

        public LocalParametersConverter(ILogger logger)
        {
            this.logger = logger;
        }

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
        public double RefLevel(double refLevelFromParameter)
        {
            double res = 0;
            if (refLevelFromParameter == 1000000000.0)//предусмотренно договаренностью
            {
                res = -40;
            }
            else if (refLevelFromParameter > 20.0)
            {
                res = 20.0;
                logger.Warning(Contexts.ThisComponent, "RefLevel is set correct and is equal to " + res + "dBm");
            }
            else if (refLevelFromParameter < -130.0)
            {
                res = -130.0;
                logger.Warning(Contexts.ThisComponent, "RefLevel is set correct and is equal to " + res + "dBm");
            }
            else
            {
                res = refLevelFromParameter;
            }
            return res;
        }
        public EN.Gain Gain(int gainFromParameter)
        {
            EN.Gain res = EN.Gain.Gain_0;
            if (gainFromParameter == -1)
            {
                res = EN.Gain.Gain_AUTO;
            }
            else if (gainFromParameter == 0)
            {
                res = EN.Gain.Gain_0;
            }
            else if (gainFromParameter == 10)
            {
                res = EN.Gain.Gain_1;
            }
            else if (gainFromParameter == 20)
            {
                res = EN.Gain.Gain_2;
            }
            else if (gainFromParameter == 30)
            {
                res = EN.Gain.Gain_3;
            }
            else
            {
                int delta = int.MaxValue;
                foreach (int t in Enum.GetValues(typeof(EN.Gain)))
                {
                    if (Math.Abs(gainFromParameter - t * 10) < delta)
                    {
                        delta = Math.Abs(gainFromParameter - t * 10);
                        res = (EN.Gain)t;
                    }
                }

            }
            return res;
        }
        public int Gain(EN.Gain gainFromDevice)
        {
            int res = 0;
            if (gainFromDevice == EN.Gain.Gain_AUTO)
            {
                res = -1;
            }
            else if (gainFromDevice == EN.Gain.Gain_0)
            {
                res = 0;
            }
            else if (gainFromDevice == EN.Gain.Gain_1)
            {
                res = 10;
            }
            else if (gainFromDevice == EN.Gain.Gain_2)
            {
                res = 20;
            }
            else if (gainFromDevice == EN.Gain.Gain_3)
            {
                res = 30;
            }

            return res;
        }
        public double SweepTime(Adapter SH, double sweepTimeFromParameter)
        {
            double res = 0;
            if (sweepTimeFromParameter < SH.SweepTimeMin)
            {
                res = SH.SweepTimeMin;
                logger.Warning(Contexts.ThisComponent, "SweepTime is set correct and is equal to " + res.ToString() + "s");
            }
            else if (sweepTimeFromParameter > SH.SweepTimeMax)
            {
                res = SH.SweepTimeMax;
                logger.Warning(Contexts.ThisComponent, "SweepTime is set correct and is equal to " + res.ToString() + "s");
            }
            else
            {
                res = sweepTimeFromParameter;
            }
            return res;
        }
        public decimal FreqStart(Adapter SH, decimal freqStartFromParameter)
        {
            decimal res = 0;
            if (freqStartFromParameter < SH.FreqMin)
            {
                res = SH.FreqMin;
                logger.Warning(Contexts.ThisComponent, "FreqStart changed to FreqMin and equals " + SH.FreqMin / 1000000 + "MHz");
            }
            else if (freqStartFromParameter > SH.FreqMax)
            {
                res = SH.FreqMax - 1000000;
                logger.Warning(Contexts.ThisComponent, "FreqStart changed to FreqMax and equals " + (SH.FreqMax - 1000000) / 1000000 + "MHz");
            }
            else
            {
                res = freqStartFromParameter;
            }
            return res;
        }
        public decimal FreqStop(Adapter SH, decimal freqStopFromParameter)
        {
            decimal res = 0;
            if (freqStopFromParameter > SH.FreqMax)
            {
                res = SH.FreqMax;
            }
            else if (freqStopFromParameter < SH.FreqMin)
            {
                res = SH.FreqMin + 1000000;
            }
            else
            {
                res = freqStopFromParameter;
            }
            return res;
        }
        public double RBW(Adapter SH, double RBWFromParameter)
        {
            double res = 0;
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
        public double VBW(Adapter SH, double VBWFromParameter)
        {
            double res = 0;
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
        public EN.TraceType TraceType(PEN.TraceType traceTypeFromParameter)
        {
            EN.TraceType res = EN.TraceType.ClearWrite;
            if (traceTypeFromParameter == PEN.TraceType.Auto)
            {
                //По результатам согласования принято такое решение
                res = EN.TraceType.ClearWrite;//OO
            }
            else if (traceTypeFromParameter == PEN.TraceType.ClearWhrite)
            {
                res = EN.TraceType.ClearWrite;
            }
            else if (traceTypeFromParameter == PEN.TraceType.Average)
            {
                res = EN.TraceType.Average;
            }
            else if (traceTypeFromParameter == PEN.TraceType.MaxHold)
            {
                res = EN.TraceType.MaxHold;
            }
            else if (traceTypeFromParameter == PEN.TraceType.MinHold)
            {
                res = EN.TraceType.MinHold;
            }
            return res;
        }
        public (EN.Detector, EN.Detector) DetectorType(PEN.DetectorType detectorTypeFromParameter)
        {
            EN.Detector DetectorUse = EN.Detector.MaxOnly;
            EN.Detector DetectorToSet = EN.Detector.MinAndMax;
            if (detectorTypeFromParameter == PEN.DetectorType.Auto)
            {
                //По результатам согласования принято такое решение
                DetectorUse = EN.Detector.MaxOnly;
                DetectorToSet = EN.Detector.MinAndMax;
            }
            else if (detectorTypeFromParameter == PEN.DetectorType.MaxPeak)
            {
                //По результатам согласования принято такое решение
                DetectorUse = EN.Detector.MaxOnly;
                DetectorToSet = EN.Detector.MinAndMax;
            }
            else if (detectorTypeFromParameter == PEN.DetectorType.MinPeak)
            {
                //По результатам согласования принято такое решение
                DetectorUse = EN.Detector.MinOnly;
                DetectorToSet = EN.Detector.MinAndMax;
            }
            else if (detectorTypeFromParameter == PEN.DetectorType.Average)
            {
                //По результатам согласования принято такое решение
                DetectorUse = EN.Detector.Average;
                DetectorToSet = EN.Detector.Average;
            }
            else if (detectorTypeFromParameter == PEN.DetectorType.RMS)
            {
                //По результатам согласования принято такое решение
                DetectorUse = EN.Detector.Average;
                DetectorToSet = EN.Detector.Average;
            }
            return (DetectorUse, DetectorToSet);
        }
        public MEN.LevelUnit LevelUnit(PEN.LevelUnit levelUnitFromParameter)
        {
            MEN.LevelUnit res = MEN.LevelUnit.dBm;
            if (levelUnitFromParameter == PEN.LevelUnit.dBm)
            {
                res = MEN.LevelUnit.dBm;
            }
            else if (levelUnitFromParameter == PEN.LevelUnit.dBmkV)
            {
                res = MEN.LevelUnit.dBµV;
            }
            else if (levelUnitFromParameter == PEN.LevelUnit.dBmkVm)
            {
                //По результатам согласования принято такое решение
                throw new Exception("LevelUnit must be set to dbm or dBmkV");
                //res = MEN.LevelUnit.dBµVm;
            }
            else if (levelUnitFromParameter == PEN.LevelUnit.mkV)
            {
                //По результатам согласования принято такое решение
                throw new Exception("LevelUnit must be set to dbm or dBmkV");
                //res = MEN.LevelUnit.µV;
            }
            return res;
        }


        public (decimal freqStart, decimal freqStop) IQFreqStartStop(Adapter SH, decimal freqStartFromParameter, decimal freqStopFromParameter)
        {
            decimal fstart = 0, fstop = 0;
            if (freqStopFromParameter - freqStartFromParameter <= 20000000)
            {
                if (freqStartFromParameter < freqStopFromParameter)
                {
                    if (freqStartFromParameter >= SH.FreqMin)
                    {
                        fstart = freqStartFromParameter;
                    }
                    else
                    {
                        throw new Exception("FreqStart < FreqMin");
                    }

                    if (freqStopFromParameter <= SH.FreqMax)
                    {
                        fstop = freqStopFromParameter;
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
        public int IQDownsampleFactor(double bitRate_MBsFromParameter, decimal span)
        {
            int res = 1;
            double BitRate = bitRate_MBsFromParameter * 1000000;
            if (span <= 20000000)
            {
                if (BitRate < 0)
                {
                    BitRate = 40000000;
                }

                if ((decimal)BitRate / span >= 2)
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

        public (double blockDuration, double receiveTime) IQTimeParameters(double blockDuration, double receiveTime)
        {
            double blockduration = 1, receivetime = 1;
            if (blockDuration > 0 && receiveTime > 0)
            {
                if (blockDuration <= receiveTime)
                {
                    blockduration = blockDuration;
                    receivetime = receiveTime;
                }
                else
                {
                    throw new Exception("BlockDuration > ReceiveTime");
                }
            }
            else
            {
                if (blockDuration <= 0)
                    throw new Exception("BlockDuration cannot be less than or equal to zero.");

                if (receiveTime <= 0)
                    throw new Exception("ReceiveTime cannot be less than or equal to zero.");
            }
            return (blockduration, receivetime);
        }
    }
}
