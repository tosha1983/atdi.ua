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
        void SendCommand<TResult>(IProcessingContext context, ICommand command, CancellationToken cancellationToken, Action<CommandFailureReason, Exception> onFailureAction);
    }

    public static class ControllerExtentions
    {
        public static void SendCommand<TResult>(this IController controller, IProcessingContext context, ICommand command)
        {
            controller.SendCommand<TResult>(context, command, CancellationToken.None, onFailureDefaultAction);
        }

        public static void SendCommand<TResult>(this IController controller, IProcessingContext context, ICommand command, CancellationToken cancellationToken)
        {
            controller.SendCommand<TResult>(context, command, cancellationToken, onFailureDefaultAction);
        }

        public static void SendCommand<TResult>(this IController controller, IProcessingContext context, ICommand command, Action<CommandFailureReason, Exception> onFailureAction)
        {
            controller.SendCommand<TResult>(context, command, CancellationToken.None, onFailureAction);
        }

        private static void onFailureDefaultAction(CommandFailureReason failureReason, Exception exception)
        {
            return;
        }
    }
}
