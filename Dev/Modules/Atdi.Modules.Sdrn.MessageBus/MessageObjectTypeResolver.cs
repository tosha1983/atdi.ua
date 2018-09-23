using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.Modules.Sdrn.MessageBus
{
    public class MessageObjectTypeResolver : IMessageObjectTypeResolver
    {
        private readonly IDictionary<string, Type> _types;

        public MessageObjectTypeResolver()
        {
            this._types = new Dictionary<string, Type>();
        }
        public Type Resolve(string messageType)
        {
            return _types[messageType];
        }

        public void Register(string messageType, Type type)
        {
            this._types[messageType] = type;
        }

        public void Register<TObject>(string messageType)
        {
            this._types[messageType] = typeof(TObject);
        }

        public static MessageObjectTypeResolver CreateForApi20()
        {
            var typeResolver = new MessageObjectTypeResolver();
            typeResolver.Register<DM.Sensor>("RegisterSensor");
            typeResolver.Register<DM.DeviceCommandResult>("SendCommandResult");
            typeResolver.Register<DM.MeasResults>("SendMeasResults");
            typeResolver.Register<DM.Entity>("SendEntity");
            typeResolver.Register<DM.EntityPart>("SendEntityPart");
            typeResolver.Register<DM.Sensor>("UpdateSensor");

            typeResolver.Register<DM.SensorRegistrationResult>("SendRegistrationResult");
            typeResolver.Register<DM.SensorUpdatingResult>("SendSensorUpdatingResult");
            typeResolver.Register<DM.DeviceCommand>("SendCommand");
            typeResolver.Register<DM.MeasTask>("SendMeasTask");

            return typeResolver;
        }
    }
}
