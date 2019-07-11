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
    public static class CalcNeedResearchSysInfo
    {

        /// <summary>
        /// Функция которая определяет логику по постановке задачь на детальное сканирование 
        /// </summary>
        /// <param name="EmittingsSummury"></param>
        /// <param name="taskParameters"></param>
        /// <returns></returns>
        public static bool NeedGetSysInfo(int CounterCallSignaling)
        {
            if (CounterCallSignaling==0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
