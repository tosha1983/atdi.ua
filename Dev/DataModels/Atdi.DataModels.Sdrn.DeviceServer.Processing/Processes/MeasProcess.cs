using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class MeasProcess : ProcessBase
    {
        public MeasTask MeasTask; // задача на измерение и ее параметры
        public MeasResults MeasResults; // результаты измерений
        public MeasProcess() : base("Meas process")
        {
        }
    }
}
