using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDR.Server.MeasurementProcessing;

namespace Atdi.SDR.Server.MeasurementProcessing.Measurement
{
    public class CalcFSFromLevel
    {
        public CalcFSFromLevel(FSemples[] F_Sem, SensorParameters sensorParameters = null)
        {

            Double ANT_VAL = 1.17;
            if (sensorParameters != null)
            {
                Double Rx = 0;
                Rx = sensorParameters.RxLoss;
                Double Gain = 3;
                Gain = sensorParameters.Gain; // Пока костыль, но мы его изменим
                ANT_VAL = Gain - Rx;
            }
            for (int i = 0; F_Sem.Length > i; i++)
            {
                F_Sem[i].LeveldBmkVm = (float)(77.2 + 20 * Math.Log10(F_Sem[i].Freq) + F_Sem[i].LeveldBm - ANT_VAL);
            }
        }
    }
}
