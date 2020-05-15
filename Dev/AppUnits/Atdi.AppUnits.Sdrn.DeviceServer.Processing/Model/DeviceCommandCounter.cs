using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Вспомогательный класс для отслеживания управляющих команд по постановке тасков
    /// Если число итераций будет больше заданного, то  считаем, что для данной команды не найлен файл с параметрами таска и команду можно удалить
    /// </summary>
    public class DeviceCommandCounter
    { 
        /// <summary>
        /// Управляющая команда
        /// </summary>
        public DeviceCommand DeviceCommand { get; set; }

        /// <summary>
        /// Число итераций
        /// </summary>
        public long Counter { get; set; }
        
    }
}
