using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer
{
    public abstract class LoggedObject : ILoggedObject
    {
        private readonly ILogger _logger;

        public LoggedObject(ILogger logger)
        {
            this._logger = logger;
        }

        public ILogger Logger => this._logger;
    }
}
