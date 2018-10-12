using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;

namespace Atdi.AppUnits.Sdrn.MessageController
{
    public class RegisterSensorFromDeviceHandler : SdrnPrimaryHandlerBase<Atdi.DataModels.Sdrns.Device.Sensor>
    {
  
        public RegisterSensorFromDeviceHandler() : base("RegisterSensor")
        {
         
        }

        public override void OnHandle(ISdrnReceivedMessage<Atdi.DataModels.Sdrns.Device.Sensor> message)
        {
            
        }
    }
}
