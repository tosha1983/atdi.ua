using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.MonitoringProcess
{
    public class SDRTraceParameters
    {
        /// <summary>
        /// Шаг с которым идут отсчеты
        /// </summary>
        public Double StepFreq_Hz;
        /// <summary>
        /// Начальная частота отсчетов
        /// </summary>
        public Double StartFreq_Hz;
        /// <summary>
        /// Размер масива с отсчетами
        /// </summary>
        public uint TraceSize;
        /// <summary>
        /// Ширина фрейма при режиме RealTime
        /// </summary>
        public int FrameWidth;
        /// <summary>
        /// Высота фрейма при режиме RealTime
        /// </summary>
        public int FrameHeight;
        /// <summary>
        /// Количество агрегированных трейсов (свипирований)
        /// </summary>
        public int SwNumber;
    }
}
