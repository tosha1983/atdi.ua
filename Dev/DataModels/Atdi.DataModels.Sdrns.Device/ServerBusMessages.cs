using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    public static class ServerBusMessages
    {
        public static class SendRegistrationResultMessage
        {
            public readonly static string Name = "SendRegistrationResult";
            public readonly static Type SendObjectType = typeof(SensorRegistrationResult);
        }

        public static class SendSensorUpdatingResultMessage
        {
            public readonly static string Name = "SendSensorUpdatingResult";
            public readonly static Type SendObjectType = typeof(SensorUpdatingResult);
        }

        public static class SendCommandMessage
        {
            public readonly static string Name = "SendCommand";
            public readonly static Type SendObjectType = typeof(DeviceCommand);
        }

        public static class SendMeasTaskMessage
        {
            public readonly static string Name = "SendMeasTask";
            public readonly static Type SendObjectType = typeof(MeasTask);
        }

        public static class SendEntityMessage
        {
            public readonly static string Name = "SendEntity";
            public readonly static Type SendObjectType = typeof(Entity);
        }

        public static class SendEntityPartMessage
        {
            public readonly static string Name = "SendEntityPart";
            public readonly static Type SendObjectType = typeof(EntityPart);
        }
    }
}
