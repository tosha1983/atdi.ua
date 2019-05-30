using Atdi.DataModels.Api.DataBus;
using Atdi.Contracts.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq.Expressions;

namespace Atdi.Api.DataBus
{

    internal sealed class HandlerDescriptor
    {
        public delegate void HandlerInvoker(object instance, IIncomingEnvelope envelope, IHandlingResult result);

        public HandlerDescriptor(Type handlerType)
        {
            this.HandlerType = handlerType;
            var instanceInterface = handlerType.GetInterface(typeof(IMessageHandler<,>).Name);

            if (instanceInterface == null || (instanceInterface.GenericTypeArguments.Length != 2))
            {
                throw new InvalidOperationException("Invalid result message handler definition");
            }
            this.MessageType = instanceInterface.GenericTypeArguments[0];
            this.DeliveryObjectType = instanceInterface.GenericTypeArguments[1];
            this.MessageTypeInstance = (IMessageType)Activator.CreateInstance(this.MessageType);
            this.Invoker = CreateInvoker(handlerType.GetMethod("Handle"));
        }

        public Type HandlerType { get; }

        public Type MessageType { get; }

        public Type DeliveryObjectType { get; }

        public IMessageType MessageTypeInstance { get; }

        public string Key
        {
            get
            {
                return BuildKey(this.MessageType, this.DeliveryObjectType);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static string BuildKey(Type messageType, Type deliveryObjectType)
        {
            return $"MT:[{messageType.FullName}];DOT:[{deliveryObjectType.FullName}]";
        }

        public HandlerInvoker Invoker { get; }

        static HandlerInvoker CreateInvoker(MethodInfo method)
        {
            var targetArg = Expression.Parameter(typeof(object));
            var envelopeParam = Expression.Parameter(typeof(IIncomingEnvelope));
            var resultPartParam = Expression.Parameter(typeof(IHandlingResult));

            var instance = Expression.Convert(targetArg, method.DeclaringType);

            var methodParams = method.GetParameters();
            if (methodParams.Length != 2)
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }

            var envelopeArg = Expression.Convert(envelopeParam, methodParams[0].ParameterType);
            var resultPartArg = Expression.Convert(resultPartParam, methodParams[1].ParameterType);

            Expression body = Expression.Call(instance, method, envelopeArg, resultPartArg);

            if (body.Type != typeof(void))
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }

            var block = Expression.Block(body, Expression.Constant(null));


            var lambda = Expression.Lambda<HandlerInvoker>(block, targetArg, envelopeParam, resultPartParam);

            return lambda.Compile();
        }
    }
}
