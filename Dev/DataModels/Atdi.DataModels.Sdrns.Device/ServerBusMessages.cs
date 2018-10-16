using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.BusMessages
{
    namespace Server
    {
        public class SendRegistrationResultMessage : SdrnBusMessageType<SensorRegistrationResult>
        {
            public SendRegistrationResultMessage() 
                : base("SendRegistrationResult")
            {
            }
        }

        public class SendSensorUpdatingResultMessage : SdrnBusMessageType<SensorUpdatingResult>
        {
            public SendSensorUpdatingResultMessage()
                : base("SendSensorUpdatingResult")
            {
            }
        }

        public class SendCommandMessage : SdrnBusMessageType<DeviceCommand>
        {
            public SendCommandMessage()
                : base("SendCommand")
            {
            }
        }

        public class SendMeasTaskMessage : SdrnBusMessageType<MeasTask>
        {
            public SendMeasTaskMessage()
                : base("SendMeasTask")
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
