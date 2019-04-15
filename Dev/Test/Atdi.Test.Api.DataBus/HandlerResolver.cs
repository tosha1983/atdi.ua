using Atdi.Contracts.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.DataBus
{
    class HandlerResolver : IMessageHandlerResolver
    {
        public IMessageHandler Resolve(Type type)
        {
            return (IMessageHandler)Activator.CreateInstance(type);
        }
    }
}
