using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{

    public interface IEvent
    {
        Guid Id { get; }

        EventLevel Level { get; }

        DateTime Time { get; }

        int OSThread { get; }

        int ManagedThread { get; }

        EventContext Context { get; }

        EventCategory? Category { get; }

        EventText? Text { get; }
    }
    
}
