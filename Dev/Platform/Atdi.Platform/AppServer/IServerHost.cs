using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.AppServer
{
    public interface IServerHost : IDisposable
    {
        HostState State { get; }

        void Start();

        void Stop();
    }
}
