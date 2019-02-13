using System;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    public class LocalParametersConverter
    {
        public EN.Attenuator Attenuator(int ATTFromParameter)
        {
            EN.Attenuator res = EN.Attenuator.Atten_0;
            if (ATTFromParameter == -1) res = EN.Attenuator.Atten_AUTO;
            else if (ATTFromParameter == 0) res = EN.Attenuator.Atten_0;
            else if (ATTFromParameter == 10) res = EN.Attenuator.Atten_10;
            else if (ATTFromParameter == 20) res = EN.Attenuator.Atten_20;
            else if (ATTFromParameter == 30) res = EN.Attenuator.Atten_30;
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
            if (GainFromParameter == -1) res = EN.Gain.Gain_AUTO;
            else if (GainFromParameter == 0) res = EN.Gain.Gain_0;
            else if (GainFromParameter == 1) res = EN.Gain.Gain_1;
            else if (GainFromParameter == 2) res = EN.Gain.Gain_2;
            else if (GainFromParameter == 3) res = EN.Gain.Gain_3;
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
            if (FreqStartFromParameter < SH.FreqMin) res = SH.FreqMin;
            else if (FreqStartFromParameter > SH.FreqMax) res = SH.FreqMax - 1000000;
            else res = FreqStartFromParameter;
            return res;
        }
        public decimal FreqStop(Adapter SH, decimal FreqStopFromParameter)
        {
            decimal res = 0;
            if (FreqStopFromParameter > SH.FreqMax) res = SH.FreqMax;
            else if(FreqStopFromParameter < SH.FreqMin) res = SH.FreqMin + 1000000;
            else res = FreqStopFromParameter;
            return res;
        }
        public decimal RBW(Adapter SH, decimal RBWFromParameter)
        {
            decimal res = 0;
            if (RBWFromParameter > SH.RBWMax) res = SH.RBWMax;
            else if (RBWFromParameter < 1) res = 1000;
            else res = RBWFromParameter;
            return res;
        }
        public decimal VBW(Adapter SH, decimal VBWFromParameter)
        {
            decimal res = 0;
            if (VBWFromParameter > SH.VBWMax) res = SH.VBWMax;
            else if (VBWFromParameter < 1) res = 1000;
            else res = VBWFromParameter;
            return res;
        }
    }
}
