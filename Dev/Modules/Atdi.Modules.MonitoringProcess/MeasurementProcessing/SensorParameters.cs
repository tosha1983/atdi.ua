using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDR.Server.MeasurementProcessing
{
    public class SensorParameters
    {
        struct FreqGain
        {
            public double freq_MHz;
            public double GainLoss_dB;
        }
        FreqGain[] freqGains;
        public double RxLoss { get; protected set; } // ослбаление в приемном фидере
        public double Gain { get; protected set; }  // Усиление антенны
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
        public SensorParameters(object sensor)
        {
            if (sensor is Sensor)
            {
                FillingSensorParameters_v1(sensor as Sensor);
            }
            if (sensor is null)
            {
                RxLoss = 1;
                Gain = 2.17;
            }

        }
        class CompareGain : IComparer<FreqGain>
        {
            public int Compare(FreqGain x, FreqGain y)
            {
                if (x.freq_MHz == y.freq_MHz) { return 0; }
                else
                {
                    if (x.freq_MHz > y.freq_MHz) { return -1; } else { return 1; }
                }
            }
        }
        void FillingSensorParameters_v1(Sensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.RxLoss != null) { RxLoss = sensor.RxLoss.Value; } else { RxLoss = 1; }
                if (sensor.Antenna != null)
                {
                    Gain = sensor.Antenna.GainMax;
                    if (sensor.Antenna.AntennaPatterns != null)
                    { // НЕ ТЕСТИРОВАННО
                        if (sensor.Antenna.AntennaPatterns.Length > 0)
                        {
                            freqGains = new FreqGain[sensor.Antenna.AntennaPatterns.Length];
                            for (int i = 0; i < sensor.Antenna.AntennaPatterns.Length; i++)
                            {// сразу с сортировкой
                                freqGains[i].freq_MHz = sensor.Antenna.AntennaPatterns[i].Freq;
                                freqGains[i].GainLoss_dB = sensor.Antenna.AntennaPatterns[i].Gain;
                            }
                            // сортировка массива
                            IComparer<FreqGain> compareGain = new CompareGain ();
                            Array.Sort(freqGains, compareGain);
                        }
                    }
                } else { Gain = 2.17; }
            }

        }
    }
}
