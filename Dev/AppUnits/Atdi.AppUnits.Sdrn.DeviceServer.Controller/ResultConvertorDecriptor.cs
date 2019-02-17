using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class ResultConvertorDecriptor
    {
        public delegate ICommandResultPart HandlerInvoker(object instance, ICommandResultPart resultPart, ICommand command, IProcessingContext context);

        public ResultConvertorDecriptor(Type instanceType)
        {
            var instanceInterface = instanceType.GetInterface(typeof(IResultConvertor<,>).Name);

            if (instanceInterface == null || instanceInterface.GenericTypeArguments.Length != 2)
            {
                throw new InvalidOperationException("Invalid convertor definition");
            }

            this.FromType = instanceInterface.GenericTypeArguments[0];
            this.ResultType = instanceInterface.GenericTypeArguments[1];
            this.InstanceType = instanceType;
            this.Invoker = CreateInvoker(instanceType.GetMethod("Convert"));
        }

        public Type FromType { get; private set; }

        public Type ResultType { get; private set; }

        public Type InstanceType { get; private set; }

        public string Key { get => BuildKey(FromType, ResultType);  }

        public HandlerInvoker Invoker { get; private set; }

        static HandlerInvoker CreateInvoker(MethodInfo method)
        {
            var targetArg = Expression.Parameter(typeof(object));
            var resultPartParam = Expression.Parameter(typeof(ICommandResultPart));
            var commandParam = Expression.Parameter(typeof(ICommand));
            var contextParam = Expression.Parameter(typeof(IProcessingContext));

            var instance = Expression.Convert(targetArg, method.DeclaringType);

            var methodParams = method.GetParameters();
            if (methodParams.Length != 3)
            {
                throw new InvalidOperationException("Invalid convertor definition");
            }
            if (methodParams[1].ParameterType != typeof(ICommand))
            {
                throw new InvalidOperationException("Invalid convertor definition");
            }
            if (methodParams[2].ParameterType != typeof(IProcessingContext))
            {
                throw new InvalidOperationException("Invalid convertor definition");
            }
            
            var resultPartArg = Expression.Convert(resultPartParam, methodParams[0].ParameterType);
            var commandArg = Expression.Convert(commandParam, methodParams[1].ParameterType);
            var contextArg = Expression.Convert(contextParam, methodParams[2].ParameterType);

            Expression body = Expression.Call(instance, method, resultPartArg, commandArg, contextArg);

            if (body.Type == typeof(void))
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }

            //var block = Expression.Block(body, Expression.Constant(null));
            var block = Expression.Convert(body, typeof(ICommandResultPart));


            var lambda = Expression.Lambda<HandlerInvoker>(block, targetArg, resultPartParam, commandParam, contextParam);

            return lambda.Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string BuildKey(Type fromType, Type resultType)
        {
            return $"{fromType.FullName}.{resultType.FullName}";
        }
    }
}
