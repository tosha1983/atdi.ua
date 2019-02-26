using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor
{
    public static class ConvertSensorToSensorParameters
    {
        public static SensorParameters Convert(this Sensor sensor)
        {
            SensorParameters sensorParameters = new SensorParameters();
            if (sensor != null)
            {
                if (sensor.RxLoss != null) { sensorParameters.RxLoss = sensor.RxLoss.Value; } else { sensorParameters.RxLoss = 1; }
                if (sensor.Antenna != null)
                {
                    sensorParameters.Gain = sensor.Antenna.GainMax;
                    if (sensor.Antenna.Patterns != null)
                    {
                        if (sensor.Antenna.Patterns.Length > 0)
                        {
                            sensorParameters.freqGains = new SensorParameters.FreqGain[sensor.Antenna.Patterns.Length];
                            for (int i = 0; i < sensor.Antenna.Patterns.Length; i++)
                            {// сразу с сортировкой
                                sensorParameters.freqGains[i].freq_MHz = sensor.Antenna.Patterns[i].Freq_MHz;
                                sensorParameters.freqGains[i].GainLoss_dB = sensor.Antenna.Patterns[i].Gain;
                            }
                            // сортировка массива
                            IComparer<SensorParameters.FreqGain> compareGain = new CompareGain();
                            Array.Sort(sensorParameters.freqGains, compareGain);
                        }
                    }
                }
                else { sensorParameters.Gain = 2.17; }
            }
            return (sensorParameters);
        }


        class CompareGain : IComparer<SensorParameters.FreqGain>
        {
            public int Compare(SensorParameters.FreqGain x, SensorParameters.FreqGain y)
            {
                if (x.freq_MHz == y.freq_MHz) { return 0; }
                else
                {
                    if (x.freq_MHz > y.freq_MHz) { return -1; } else { return 1; }
                }
            }
        }

    }
}
