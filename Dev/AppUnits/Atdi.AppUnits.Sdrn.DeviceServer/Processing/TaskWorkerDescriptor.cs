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

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    class TaskWorkerDescriptor
    {
        public delegate void HandlerInvokerSync(object instance, ITaskContext taskContext);
        public delegate Task HandlerInvokerAsync(object instance, ITaskContext taskContext);

        public TaskWorkerDescriptor(Type instanceType)
        {
            this.InstanceType = instanceType;
            var instanceInterface = instanceType.GetInterface(typeof(ITaskWorker<,,>).Name);
            
            if (instanceInterface == null)
            {
                instanceInterface = instanceType.GetInterface(typeof(ITaskWorkerAsync<,,>).Name);

                if (instanceInterface == null || (instanceInterface.GenericTypeArguments.Length != 3))
                {
                    throw new InvalidOperationException("Invalid result handler definition");
                }
                this.IsAsync = true;
            }
            if (instanceInterface.GenericTypeArguments.Length != 3)
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }

            this.TaskType = instanceInterface.GenericTypeArguments[0];
            this.ProccesType = instanceInterface.GenericTypeArguments[1];
            this.LifetimeType = instanceInterface.GenericTypeArguments[1];

            if (this.IsAsync)
            {
                this.InvokerAsync = CreateInvokerAsync(instanceType.GetMethod("RunAsync"));
            }
            else
            {
                this.InvokerSync = CreateInvokerSync(instanceType.GetMethod("Run"));
            }

            var taskContextType = typeof(TaskContext<,>);
            this.TaskContextType = taskContextType.MakeGenericType(this.TaskType, this.ProccesType);

            this.IsAutoTask = this.TaskType.GetInterface(typeof(IAutoTask).Name) != null;
        }

        public bool IsAutoTask { get; private set; }

        public bool IsAsync { get; private set; }

        public Type TaskType { get; private set; }
        public Type ProccesType { get; private set; }

        public Type LifetimeType { get; private set; }
        public Type InstanceType { get; private set; }

        public string Key { get => BuildKey(TaskType, ProccesType); }

        public HandlerInvokerSync InvokerSync  { get; private set; }
        public HandlerInvokerAsync InvokerAsync { get; private set; }

        public Type TaskContextType { get; private set; }

        public ITaskContext CreateTaskContext(ITaskDescriptor descriptor)
        {
            var result = Activator.CreateInstance(this.TaskContextType, descriptor);
            return result as ITaskContext;
        }

        static HandlerInvokerAsync CreateInvokerAsync(MethodInfo method)
        {
            var targetArg = Expression.Parameter(typeof(object));
            var taskContextParam = Expression.Parameter(typeof(ITaskContext));
            
            var instance = Expression.Convert(targetArg, method.DeclaringType);

            var methodParams = method.GetParameters();
            if (methodParams.Length != 1)
            {
                throw new InvalidOperationException("Invalid convertor definition");
            }

            var taskContextArg = Expression.Convert(taskContextParam, methodParams[0].ParameterType);

            Expression body = Expression.Call(instance, method, taskContextArg);
            if (body.Type != typeof(Task))
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }

            var block = Expression.Convert(body, typeof(ICommandResultPart));
            var lambda = Expression.Lambda<HandlerInvokerAsync>(block, targetArg, taskContextParam);

            return lambda.Compile();
        }

        static HandlerInvokerSync CreateInvokerSync(MethodInfo method)
        {
            var targetArg = Expression.Parameter(typeof(object));
            var taskContextParam = Expression.Parameter(typeof(ITaskContext));

            var instance = Expression.Convert(targetArg, method.DeclaringType);

            var methodParams = method.GetParameters();
            if (methodParams.Length != 1)
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }

            var taskContextArg = Expression.Convert(taskContextParam, methodParams[0].ParameterType);

            Expression body = Expression.Call(instance, method, taskContextArg);
            if (body.Type != typeof(void))
            {
                throw new InvalidOperationException("Invalid result handler definition");
            }

            var block = Expression.Block(body, Expression.Constant(null));
            var lambda = Expression.Lambda<HandlerInvokerSync>(block, targetArg, taskContextParam);

            return lambda.Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string BuildKey(Type taskType, Type processType)
        {
            return $"{taskType.FullName}.{processType.FullName}";
        }
    }

}

