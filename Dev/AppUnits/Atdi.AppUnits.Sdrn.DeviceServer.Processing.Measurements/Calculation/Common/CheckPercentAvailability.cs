using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.Device;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    static class CheckPercentAvailability
    {
        /// <summary>
        /// Корректировка PersentAvailability
        /// </summary>
        /// <param name="emittings"></param>
        public static void CheckPercent(ref Emitting[] emittings)
        {
            for (int i=0; i< emittings.Length; i++)
            {
                var valueEmit = emittings[i];
                if (valueEmit!=null)
                {
                    if (valueEmit.WorkTimes!=null)
                    {
                        for (int j=0; j< valueEmit.WorkTimes.Length; j++)
                        {
                            var workTime = valueEmit.WorkTimes[j];
                            if (workTime!=null)
                            {
                                if (workTime.PersentAvailability>100)
                                {
                                    workTime.HitCount = workTime.ScanCount;
                                    workTime.PersentAvailability = 100;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
