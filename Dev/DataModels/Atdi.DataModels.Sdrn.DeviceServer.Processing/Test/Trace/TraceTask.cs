using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing.Test
{
    public class TraceTask : TaskBase
    {
        /// <summary>
        /// Кол-вл трейсов, которые должна обработать задача
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Кол-вл точек в одном трейсе
        /// </summary>
        public int BlockSize { get; set; }

        /// <summary>
        /// Индекс задачи
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Таймер для измерения времени команды и получения от нее результата через контроллер
        /// </summary>
        public System.Diagnostics.Stopwatch Timer { get; set; }
    }
}
