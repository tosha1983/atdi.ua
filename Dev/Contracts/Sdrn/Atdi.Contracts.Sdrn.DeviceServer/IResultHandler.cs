using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IResultHandler
    {
        void Handle(ICommand command, ICommandResultPart result, IProcessingContext context);
    }

    public interface IResultHandler<TCommand, TResult> 
        where TCommand : ICommand
        where TResult : ICommandResultPart
    {
        void Handle(TCommand command, TResult result, IProcessingContext context);
    }

    public interface IResultHandler<TCommand, TResult, TContext>
        where TCommand : ICommand
        where TResult : ICommandResultPart
        where TContext : IProcessingContext
    {
        void Handle(TCommand command, TResult result, TContext context);
    }
}
