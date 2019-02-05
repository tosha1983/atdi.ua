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
         
        TimeSpan Delay { get; set; }

        DateTime SpecificTime { get; set; }

        Type ParameterType { get;  }
    }

    public interface ICommand<TParameter> : ICommand
    {
        TParameter Parameter { get; }
    }

    public abstract class CommandBase : ICommand
    {
        public CommandBase(CommandType type)
        {
            this.Id = Guid.NewGuid();
            this.State = CommandState.Created;
            this.Type = type;
            this.Options = CommandOption.StartImmediately;
        }

        public CommandState State { get; set; }

        public CommandType Type { get; private set; }


        public Type ParameterType { get; set; }

        public Guid Id { get; private set; }
        public CommandOption Options { get; set; }
        public TimeSpan Delay { get; set; }
        public DateTime SpecificTime { get; set; }
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
