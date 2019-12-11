using System;
using System.Threading;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
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