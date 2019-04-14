using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.DataBus
{
    public enum DispatcherState
    {
        Activated = 0,
        Deactivated = 1
    }

    public interface IDispatcher : IDisposable
    {
        DispatcherState State { get; }

        void Activate();

        void Deactivate();

        void RegistryHandler(Type handlerType);
    }
}
