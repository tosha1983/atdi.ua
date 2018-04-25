using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer
{
    /// <summary>
    /// Represent the context of the application server
    /// </summary>
    public interface IAppServerContext
    {
        ISecurityContext SecurityContext { get; }

        string HostName { get;  }
    }
}
