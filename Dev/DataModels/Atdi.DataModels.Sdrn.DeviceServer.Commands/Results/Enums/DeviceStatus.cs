using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.Enums
{
    public enum DeviceStatus : int
    {
        Normal = 0, //Все хорошо и работает
        StartReset = 1, //запушен перзупуск устройства
        FinishReset = 2 //перезапуск устройства завершен
    }
}
