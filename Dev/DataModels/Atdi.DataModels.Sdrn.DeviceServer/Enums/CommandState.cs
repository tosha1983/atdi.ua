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
        Created,
        /// <summary>
        /// Команда принята контролером но еще не передана адаптеру на исполнение
        /// </summary>
        InLine,
        /// <summary>
        /// Команда в процессе выполнения адаптером
        /// </summary>
        Processing,
        /// <summary>
        /// Команда выполнена
        /// </summary>
        Done,
        /// <summary>
        /// Команда отменена
        /// </summary>
        Cancelled,
        /// <summary>
        ///  Комманда отклонена контролером
        /// </summary>
        Rejected,
        /// <summary>
        ///  Комманда неожидано прекратила свое выполнение
        /// </summary>
        Aborted
    }
}
