using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.MessageBus
{
    public interface IMessageObjectTypeResolver
    {
        Type Resolve(string messageType);
    }
}
