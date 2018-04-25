using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.TechServices
{
    public interface ITechService
    {
    }

    public interface ITechService<TContract> : ITechService
    {
    }
}
