﻿using Atdi.Contracts.Sdrn.DeviceServer;
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
    class ResultHandlerDecriptor
    {
        public delegate void HandlerInvoker(object instance, ICommand command, ICommandResultPart resultPart, ITaskContext taskContext);

        public ResultHandlerDecriptor(Type instanceType)
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
            this.Invoker = CreateInvoker(instanceType.GetMethod("Handle"));
        }

        public Type CommandType { get; private set; }

        public Type ResultType { get; private set; }

        public Type TaskType { get; private set; }

        public Type ProcessType { get; private set; }

        public Type InstanceType { get; private set; }

        public string Key
        {
            get => BuildKey(CommandType, ResultType, TaskType, ProcessType);
        }

        public HandlerInvoker Invoker { get; private set; }

        static HandlerInvoker CreateInvoker(MethodInfo method)
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
        public static string BuildKey(Type commandType, Type resultType, Type taskType, Type processType)
        {
            return $"{commandType.FullName}.{resultType.FullName}.{taskType.FullName}.{processType.FullName}";
        }
    }
}