using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public interface ISdrnMessageDispatcher : IDisposable
    {
        SdrnMessageDispatcherState State { get; }

        string Tag { get; }

        void Activate();

        void Deactivate();

        void RegistryHandler(Type handlerType);

    }
}
