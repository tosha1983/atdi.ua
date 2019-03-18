using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class CommandDescriptor : ICommandDescriptor
    {
        private CommandBase _command;
        private ITaskContext _taskContext;

        public ICommand Command
        {
            get => this._command;
            set
            {
                this._command = value as CommandBase;
                this.CommandType = value.GetType();
            }
        }

        public Type CommandType { get; private set; }


        public Type ResultType { get; set; }

        public ITaskContext TaskContext
        {
            get => this._taskContext;
            set
            {
                this._taskContext = value;
                this.ProcessType = value.Descriptor.ProcessType;
                this.TaskType = value.Descriptor.TaskType;
            }
        }

        public Type TaskType { get; private set; }

        public Type ProcessType { get; private set; }

        public CancellationToken CancellationToken { get; set; }

        public ControllerFailureAction FailureAction { get; set; }

        public IDevice Device { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseFailureAction(CommandFailureReason reason, Exception exception)
        {
            Task.Run(() =>
            {
                this.FailureAction(this.TaskContext, this.Command, reason, exception);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reject(CommandFailureReason reason, Exception exception = null)
        {
            this._command.State = CommandState.Rejected;
            this.RaiseFailureAction(reason, exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort(CommandFailureReason reason, Exception exception)
        {
            this._command.State = CommandState.Aborted;
            this.RaiseFailureAction(reason, exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cancel()
        {
            this._command.State = CommandState.Cancelled;
            this.RaiseFailureAction(CommandFailureReason.CanceledBeforeExecution, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Done()
        {
            this._command.State = CommandState.Done;
            if ((this._command.Options & CommandOption.RaiseExecutionCompleted) == CommandOption.RaiseExecutionCompleted)
            {
                this.RaiseFailureAction(CommandFailureReason.ExecutionCompleted, null);
            }
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Process()
        {
            this._command.State = CommandState.Processing;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToPending()
        {
            this._command.State = CommandState.Pending;
        }
    }
}
