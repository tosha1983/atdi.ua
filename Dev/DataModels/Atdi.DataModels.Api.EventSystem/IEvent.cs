using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.EventSystem
{
    public interface IEvent
    {
        Guid Id { get;  }

        string Name { get; }

        string Source { get; }

        DateTimeOffset Created { get; }
    }
}
