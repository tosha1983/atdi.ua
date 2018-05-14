using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.AppServer
{
    /// <summary>
    /// Represent the context of the application server
    /// </summary>
    public interface IServerContext
    {
        IServerConfig Config { get;  }
    }
}
