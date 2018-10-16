using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IDataSourceMetadata
    {
        string Name { get; }

        string Schema { get; }

        DataSourceType Type { get; }

        DataSourceObject Object { get;  }
    }
}
