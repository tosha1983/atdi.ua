using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    [Serializable]
    public class ClientMeasTaskData : ClientData
    {
        public int SomeMeasType { get; set; }
    }
}
