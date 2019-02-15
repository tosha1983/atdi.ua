using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public enum CommandState
    {
        /// <summary>
        /// Начальное состояние команды
        /// </summary>
        Created = 0,
        /// <summary>
        /// Команда принята контролером но еще не передана адаптеру на исполнение, в ожидании обработки
        /// </summary>
        Pending = 1,
        /// <summary>
        /// Команда в процессе выполнения адаптером
        /// </summary>
        Processing = 2,
        /// <summary>
        /// Команда выполнена
        /// </summary>
        Done = 3,
        /// <summary>
        /// Команда отменена
        /// </summary>
        Cancelled = 4,
        /// <summary>
        ///  Комманда отклонена контролером
        /// </summary>
        Rejected = 5,
        /// <summary>
        ///  Комманда неожидано прекратила свое выполнение
        /// </summary>
        Aborted = 6
    }
}
