using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    

    public interface IController : IDisposable
    {
        void SendCommand<TResult>(ITaskContext taskContext, ICommand command, CancellationToken cancellationToken, ControllerFailureAction onFailureAction);

        TProperties[] EnsureDevicesProperties<TProperties>(CommandType commandType)
            where TProperties : IDeviceProperties, new();

        IReadOnlyDictionary<CommandType, IDeviceProperties[]> GetDevicesProperties();
            
    }

    public static class ControllerExtensions
    {
        public static void SendCommand<TResult>(this IController controller, ITaskContext taskContext, ICommand command)
        {
            controller.SendCommand<TResult>(taskContext, command, CancellationToken.None, FailureDefaultAction);
        }

        public static void SendCommand<TResult>(this IController controller, ITaskContext taskContext, ICommand command, CancellationToken cancellationToken)
        {
            controller.SendCommand<TResult>(taskContext, command, cancellationToken, FailureDefaultAction);
        }

        public static void SendCommand<TResult>(this IController controller, ITaskContext taskContext, ICommand command, ControllerFailureAction onFailureAction)
        {
            controller.SendCommand<TResult>(taskContext, command, CancellationToken.None, onFailureAction);
        }

        private static void FailureDefaultAction(ITaskContext taskContext, ICommand command, CommandFailureReason failureReason, Exception exception)
        {
            return;
        }
    }
}
