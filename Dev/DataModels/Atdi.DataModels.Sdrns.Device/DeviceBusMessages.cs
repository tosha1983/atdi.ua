using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    public static class DeviceBusMessages
    {
        public static class RegisterSensorMessage
        {
            public readonly static string Name = "RegisterSensor";
            public readonly static Type SendObjectType = typeof(Sensor);
        }

        public static class UpdateSensorMessage
        {
            public readonly static string Name = "UpdateSensor";
            public readonly static Type SendObjectType = typeof(Sensor);
        }

        public static class SendCommandResultMessage
        {
            public readonly static string Name = "SendCommandResult";
            public readonly static Type SendObjectType = typeof(DeviceCommandResult);
        }

        public static class SendMeasResultsMessage
        {
            public readonly static string Name = "SendMeasResults";
            public readonly static Type SendObjectType = typeof(MeasResults);
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
