using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing.Test
{
    public class TraceProcess : ProcessBase
    {
        public TraceProcess()
            : base("Measurement Trace Process")
        {
        }

        /// <summary>
        /// Кол-во обработанных комманд адаптера 
        /// </summary>
        public int CommandCount;

        // <summary>
        /// Кол-во завершенных тасков
        /// </summary>
        public int TaskCount;

        public int IterationCount;
    }
}
