using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    [Serializable]
    public class DeviceServerCancellationData : DeviceServerData
    {
        public string Message { get; set; }
        public FailureReason FailureCode  { get; set; }
    }
}
