using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns
{
    public abstract class SdrnBusMessageType
    {
        public SdrnBusMessageType(string name, Type deliveryObjectType)
        {
            this.Name = name;
            this.DeliveryObjectType = deliveryObjectType;
        }

        public string Name { get; }

        public Type DeliveryObjectType { get; }

        public static SdrnBusMessageType Instance<TMessage>()
            where TMessage : SdrnBusMessageType, new()
        {
            return new TMessage();
        }
    }
    public abstract class SdrnBusMessageType<TDeliveryObject> : SdrnBusMessageType
    {
        public SdrnBusMessageType(string name) : base(name, typeof(TDeliveryObject))
        {
        }        
    }
}
