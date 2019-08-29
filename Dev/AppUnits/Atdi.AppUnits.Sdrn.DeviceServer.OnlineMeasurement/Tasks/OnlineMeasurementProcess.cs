using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks
{
    public class OnlineMeasurementProcess : ProcessBase
    {
        public OnlineMeasurementProcess()
            : base("Online Measurement")
        {
        }

        public OnlineMeasurementProcess(IProcess parentProcess)
            : base("Online Measurement", parentProcess)
        {
        }

        public ClientRegistrationData RegistrationData { get; set; }

        public DeviceServerParametersData Parameters { get; set; }

        public WebSocketPublisher Publisher { get; set; }
    }
}
