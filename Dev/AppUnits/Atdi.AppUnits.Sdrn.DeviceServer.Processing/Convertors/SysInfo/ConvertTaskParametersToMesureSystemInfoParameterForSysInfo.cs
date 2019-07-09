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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public static class ConvertTaskParametersToMesureSystemInfoParameterForSysInfo
    {
        /// <summary>
        /// Конвертор из TaskParameters в MesureSystemInfoParameter(объект на основе которого выполняется отправка команды в адаптер)
        /// </summary>
        /// <param name="taskParameters"></param>
        /// <returns></returns>
        public static MesureSystemInfoParameter ConvertForMesureSystemInfoParameter(this TaskParameters taskParameters)
        {

            MesureSystemInfoParameter mesureSystemInfoParameter = new MesureSystemInfoParameter();


            return mesureSystemInfoParameter;
        }
    }
}
