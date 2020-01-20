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
    internal class ResultHandlerDescriptor
    {
        public delegate void HandlerInvoker(object instance, ICommand command, ICommandResultPart resultPart, ITaskContext taskContext);

        public ResultHandlerDescriptor(Type instanceType)
        {
            this.InstanceType = instanceType;
            var instanceInterface = instanceType.GetInterface(typeof(IResultHandler<,,,>).Name);

            if (instanceInterface == null || (instanceInterface.GenericTypeArguments.Length != 4))
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }
            this.CommandType = instanceInterface.GenericTypeArguments[0];
            this.ResultType = instanceInterface.GenericTypeArguments[1];
            this.TaskType = instanceInterface.GenericTypeArguments[2];
            this.ProcessType = instanceInterface.GenericTypeArguments[3];
            this.Key = BuildKey(CommandType, ResultType, TaskType, ProcessType);
            this.Invoker = CreateInvoker(instanceType.GetMethod("Handle"));
        }

        public Type CommandType { get; }

        public Type ResultType { get; }

        public Type TaskType { get; }

        public Type ProcessType { get; }

        public Type InstanceType { get; }

        public ValueTuple<Type, Type, Type, Type> Key { get; }

        public HandlerInvoker Invoker { get; private set; }

        private static HandlerInvoker CreateInvoker(MethodInfo method)
        {
            var targetArg = Expression.Parameter(typeof(object));
            var commandParam = Expression.Parameter(typeof(ICommand));
            var resultPartParam = Expression.Parameter(typeof(ICommandResultPart));
            var contextParam = Expression.Parameter(typeof(ITaskContext));

            var instance = Expression.Convert(targetArg, method.DeclaringType);

            var methodParams = method.GetParameters();
            if (methodParams.Length != 3)
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }
            //if (methodParams[2].ParameterType != typeof(IProcessingContext))
            //{
            //    throw new InvalidOperationException("Invalid result handler definition");
            //}
            var commandArg = Expression.Convert(commandParam, methodParams[0].ParameterType);
            var resultPartArg = Expression.Convert(resultPartParam, methodParams[1].ParameterType);
            var contextArg = Expression.Convert(contextParam, methodParams[2].ParameterType);

            Expression body = Expression.Call(instance, method, commandArg, resultPartArg, contextArg);

            if (body.Type != typeof(void))
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }

            var block = Expression.Block(body, Expression.Constant(null));


            var lambda = Expression.Lambda<HandlerInvoker>(block, targetArg, commandParam, resultPartParam, contextParam);

            return lambda.Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTuple<Type, Type, Type, Type> BuildKey(Type commandType, Type resultType, Type taskType, Type processType)
        {
            return new ValueTuple<Type, Type, Type, Type>(commandType, resultType, taskType, processType);
            //return $"{commandType.FullName}.{resultType.FullName}.{taskType.FullName}.{processType.FullName}";
        }
    }
}
