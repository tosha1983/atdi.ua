using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class StandartBW
    {
        // константы
        public static readonly double[] BW_kHz = {2.8, 6.25, 12.5, 25, 30, 100, 200, 271, 1250, 1750, 2000, 3500, 5000, 7000, 7600, 8000, 10000, 15000, 20000, 28000, 56000, 100000, 110000, 140000, 200000};
        private static double AllowableExcessRealBW = 0.05;
        //конец константам
        public static double GetStandartBW_kHz(double Real_BW_kHz)
        {
            for (var i = 0; i < BW_kHz.Length; i++)
            {
                if (Real_BW_kHz < BW_kHz[i]* (AllowableExcessRealBW+1))
                {
                    return BW_kHz[i];
                }
            }
            return -1;
        }
    }
}
