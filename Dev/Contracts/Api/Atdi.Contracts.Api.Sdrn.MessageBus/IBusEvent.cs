using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    

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
    }
}
