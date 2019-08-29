using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    public class OnlineMeasMessage
    {
        public OnlineMeasMessageKind Kind { get; set; }

        public object Container { get; set; }
    }
}
