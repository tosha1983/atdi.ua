using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public interface IExeption
    {
        string Type { get; }

        string Source { get; }

        string StackTrace { get; }

        string TargetSite { get; }

        IExeption Inner { get; }

        string Message { get; }
    }
}
