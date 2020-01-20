using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class CalcFSFromLevel
    {
        public CalcFSFromLevel(SemplFreq[] F_Sem, SensorParameters sensorParameters = null)
        {

            double ANT_VAL = 1.17;
            if (sensorParameters != null)
            {
                double Rx = 0;
                Rx = sensorParameters.RxLoss;
                double Gain = 3;
                Gain = sensorParameters.Gain; // Пока костыль, но мы его изменим
                ANT_VAL = Gain - Rx;
            }
            for (var i = 0; F_Sem.Length > i; i++)
            {
                F_Sem[i].LeveldBmkVm = (float)(77.2 + 20 * Math.Log10(F_Sem[i].Freq) + F_Sem[i].LeveldBm - ANT_VAL);
            }
        }
    }
}
