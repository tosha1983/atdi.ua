using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IDataSourceMetadata
    {
        string Name { get; set; }

        string Schema { get; set; }

        DataSourceType Type { get; set; }

        DataSourceObject Object { get; set; }
    }
}
