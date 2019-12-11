using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IEventWaiter
    {
        bool Wait<TEvent>(out TEvent @event, int millisecondsTimeout = System.Threading.Timeout.Infinite);

        bool Wait<TEvent>(int millisecondsTimeout = System.Threading.Timeout.Infinite);

        bool Wait<TEvent>(IEventToken token, out TEvent @event, int millisecondsTimeout = System.Threading.Timeout.Infinite);

        bool Wait<TEvent>(IEventToken token, int millisecondsTimeout = System.Threading.Timeout.Infinite);

        void Emit<TEvent>(TEvent @event);

        void Emit<TEvent>();

        void Emit<TEvent>(IEventToken token, TEvent @event);

        void Emit<TEvent>(IEventToken token);

        IEventToken GetToken();

        void ReleaseToken(ref IEventToken token);
    }
}
