using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public enum DeviceState
    {
        /// <summary>
        /// Начальное состояние объекта устройства
        /// </summary>
        Created = 0,

        /// <summary>
        /// Устройство в ожидании команды
        /// </summary>
        Available = 1,

        /// <summary>
        /// Устройство занято
        /// </summary>
        Basy = 2
    }
}
