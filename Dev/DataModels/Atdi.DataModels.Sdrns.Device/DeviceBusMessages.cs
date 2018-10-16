using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.BusMessages
{
    namespace Device
    {
        public class RegisterSensorMessage : SdrnBusMessageType<Sensor>
        {
            public RegisterSensorMessage()
                : base("RegisterSensor")
            {
            }
        }

        public class UpdateSensorMessage : SdrnBusMessageType<Sensor>
        {
            public UpdateSensorMessage()
                : base("UpdateSensor")
            {
            }
        }

        public class SendCommandResultMessage : SdrnBusMessageType<DeviceCommandResult>
        {
            public SendCommandResultMessage()
                : base("SendCommandResult")
            {
            }
        }

        public class SendMeasResultsMessage : SdrnBusMessageType<MeasResults>
        {
            public SendMeasResultsMessage()
                : base("SendMeasResults")
            {
            }
        }

        public class SendEntityMessage : SdrnBusMessageType<Entity>
        {
            public SendEntityMessage()
                : base("SendEntity")
            {
            }
        }

        public class SendEntityPartMessage : SdrnBusMessageType<EntityPart>
        {
            public SendEntityPartMessage()
                : base("SendEntityPart")
            {
            }
        }
    }
}
