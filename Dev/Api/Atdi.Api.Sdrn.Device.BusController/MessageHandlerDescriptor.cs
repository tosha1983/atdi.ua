using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Atdi.Contracts.Api.Sdrn.MessageBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class MessageHandlerDescriptor
    {
        public static Type ReceivedMessageType = typeof(ReceivedMessage<>);
        public static Type HandlerBaseInterfaceType = typeof(IMessageHandler<>);

        public MessageHandlerDescriptor(object handlerInstance, string messageType)
        {
            this.Instance = handlerInstance;
            this.MessageType = messageType;

            var instanceType = handlerInstance.GetType();

            var handlerBaseInterface = instanceType.GetInterface(HandlerBaseInterfaceType.Name);
            if (handlerBaseInterface == null)
            {
                throw new InvalidOperationException("Handler instance don't support IMessageHandler<>");
            }

            this.MessageObjectType = handlerBaseInterface.GenericTypeArguments[0];

            //var tt = typeof(ReceivedMessage<>);
            //var aa = new Type[] { typeof(DM.MeasTask) };
            //var gt = tt.MakeGenericType(aa);

            this.ReceivedMessageGenericType = ReceivedMessageType.MakeGenericType(new Type[] { this.MessageObjectType });

            this.OnHandleMethod = instanceType.GetMethod("OnHandle");
        }

        public static Type MakeReceivedMessageGenericType<TObject>()
        {
            return ReceivedMessageType.MakeGenericType(new Type[] {typeof(TObject)});
        }

        public object Instance { get; }

        public string MessageType { get;  }

        public Type MessageObjectType { get; }

        public Type ReceivedMessageGenericType { get; }

        public MethodInfo OnHandleMethod { get; }
    }
}
