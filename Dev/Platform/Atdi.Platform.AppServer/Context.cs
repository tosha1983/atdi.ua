using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.AppServer
{
    class Context : IContext
    {
        private IServerConfig _serverConfig;

        public Context(IServerConfig config)
        {
            this._serverConfig = config;
        }

        public IServerConfig Config { get => this._serverConfig; }
    }
}
