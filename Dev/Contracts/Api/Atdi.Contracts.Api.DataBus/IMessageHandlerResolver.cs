using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.DataBus
{
    public interface IMessageHandlerResolver
    {
        IMessageHandler Resolve(Type type);
    }
}
