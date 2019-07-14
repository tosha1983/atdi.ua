using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IEntityRequest
    {
        string Context { get; set; }

        string Namespace { get; set; }

        string Entity { get; set; }
    }
}
