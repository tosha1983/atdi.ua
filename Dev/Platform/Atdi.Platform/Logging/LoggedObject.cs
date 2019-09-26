using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public abstract class LoggedObject : ILoggedObject
    {
        protected LoggedObject(ILogger logger)
        {
            this.Logger = logger;
        }

        public ILogger Logger { get; }
    }
}
