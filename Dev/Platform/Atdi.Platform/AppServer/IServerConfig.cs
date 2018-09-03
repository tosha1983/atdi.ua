using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.Platform.AppServer
{
    public interface IServerConfig
    {
        string Instance { get; }

        object this[string propertyName] { get; }

        IComponentConfig[] Components { get; }
    }
}
