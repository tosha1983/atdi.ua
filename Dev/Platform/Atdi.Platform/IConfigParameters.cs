using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public interface IConfigParameters
    {
        bool Has(string name);

        int Count { get;  }

        object this[string name] { get;  }

        IConfigParameter this[int index] { get; }

        IConfigParameter GetByName(string name);
    }

}
