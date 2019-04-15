using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.DataBus
{
    [Flags]
    public enum BusEventLevel
    {
        None = 0,
        Verbouse = 1,
        Info = 2,
        Warning = 4,
        Trace = 8,
        Debug = 16,
        Error = 32,
        Exception = 64,
        Critical = 128
    }

    public interface IBusEvent
    {
        Guid Id { get; }

        int Code { get; }

        DateTime Created { get; }

        BusEventLevel Level { get; }

        string Context { get; }

        string Text { get; }

        int ManagedThread { get; }

        string Source { get; }

        Exception Exception { get; }
    }
}
