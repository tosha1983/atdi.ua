using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public interface IEventsConsumer
    {
        void Push(IEvent[] events);
    }
}
