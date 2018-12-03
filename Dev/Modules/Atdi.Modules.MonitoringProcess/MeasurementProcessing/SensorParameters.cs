using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.Modules.MonitoringProcess
{
    public class SensorParameters
    {
        public struct FreqGain
        {
            public double freq_MHz;
            public double GainLoss_dB;
        }
        public FreqGain[] freqGains;
        public double RxLoss { get; set; } // ослбаление в приемном фидере
        public double Gain { get; set; }  // Усиление антенны
        public double getGainLoss(double freq_MHz)
        { // НЕ ПРОВЕРЕННО
            if (freqGains == null)
            {
                return Gain;
            }
            else
            {
                if (freqGains[0].freq_MHz <= freq_MHz) { return freqGains[0].GainLoss_dB;}
                if (freqGains[freqGains.Length].freq_MHz >= freq_MHz) { return freqGains[freqGains.Length].GainLoss_dB; }
                for (int i = 0; i < freqGains.Length-1; i++)
                {
                    if ((freqGains[i].freq_MHz <= freq_MHz)&&(freqGains[i+1].freq_MHz >= freq_MHz))
                    {
                        double G = freqGains[i].GainLoss_dB + (freq_MHz - freqGains[i].freq_MHz) *(freqGains[i + 1].GainLoss_dB - freqGains[i].GainLoss_dB) / (freqGains[i + 1].freq_MHz - freqGains[i].freq_MHz);
                        return G;
                    }
                }

                return Gain;
            }
        }

    }
}
