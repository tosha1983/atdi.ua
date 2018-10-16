using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    

    public interface IBrokerEvent
    {
        Guid Id { get; }

        int Code { get; }

        DateTime Created { get; }

        BrokerEventLevel Level { get; }

        string Context { get; } 

        string Text { get; }

        int ManagedThread { get; }

        string Source { get; }

        Exception Exception { get;  }
    }
}
