using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public interface IConfigParameter
    {
        string Name { get; }

        object Value { get; }
    }

    interface IConfigParameter<TValue>
    {
        string Name { get; }

        TValue Value { get; }
    }
}
