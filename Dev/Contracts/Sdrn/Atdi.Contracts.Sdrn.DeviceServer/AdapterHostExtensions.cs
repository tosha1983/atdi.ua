using System;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public static class AdapterHostExtensions
    {
        public static void RegisterHandler<TCommand, TResult>(this IAdapterHost host,
            Action<TCommand, IExecutionContext> commandHandler)
            where TCommand : new()
        {
            host.RegisterHandler<TCommand, TResult>(commandHandler, null, null);
        }
        public static void RegisterHandler<TCommand, TResult>(this IAdapterHost host,
            Action<TCommand, IExecutionContext> commandHandler, IDeviceProperties deviceProperties)
            where TCommand : new()
        {
            host.RegisterHandler<TCommand, TResult>(commandHandler, null, deviceProperties);
        }

        public static void RegisterHandler<TCommand, TResult>(this IAdapterHost host,
            Action<TCommand, IExecutionContext> commandHandler, IResultPoolDescriptor<TResult>[] poolDescriptors)
            where TCommand : new()
        {
            host.RegisterHandler<TCommand, TResult>(commandHandler, poolDescriptors, null);
        }
    }
}