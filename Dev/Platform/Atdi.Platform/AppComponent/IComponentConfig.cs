using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.AppComponent
{
    public interface IComponentConfig
    {
        string Instance { get; }

        string Type { get;  }

        string Assembly { get; }

        object this[string paramName] { get; }
    }
}
