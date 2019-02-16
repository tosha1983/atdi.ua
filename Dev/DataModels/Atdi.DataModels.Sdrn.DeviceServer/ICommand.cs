using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface ICommand
    {
        Guid Id { get; }

        CommandOption Options { get; set; }

        CommandState State { get; }

        CommandType Type { get; }
         
        long Delay { get; set; }

        long Timeout { get; set; }

        long StartTimeStamp { get; set; }

        Type ParameterType { get;  }
    }

    public interface ICommand<TParameter> : ICommand
    {
        TParameter Parameter { get; }
    }

    public abstract class CommandBase : ICommand
    {
        volatile private CommandState _state;

        public CommandBase(CommandType type)
        {
            this.Id = Guid.NewGuid();
            this._state = CommandState.Created;
            this.Type = type;
            this.Options = CommandOption.PutInQueue;
        }

        public CommandState State { get => _state; set => _state = value; }

        public CommandType Type { get; private set; }

        public Type ParameterType { get; set; }

        public Guid Id { get; private set; }

        public CommandOption Options { get; set; }

        public long Delay { get; set; }

        public long Timeout { get; set; }

        public long StartTimeStamp { get; set; }
    }

    public abstract class CommandBase<TParameter> : CommandBase, ICommand<TParameter>
    {
        public CommandBase(CommandType type, TParameter parameter) : base(type)
        {
            this.Parameter = parameter;
            this.ParameterType = typeof(TParameter);
        }

        public TParameter Parameter { get; private set; }
    }
}
