using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IEntityOrmConfig
    {
        string Name { get; }

        string Version { get; }

        string Namespace { get; }
    }
}
