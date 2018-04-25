using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer
{
    public interface ILoggedObject
    {
        ILogger Logger { get; }
    }
}
