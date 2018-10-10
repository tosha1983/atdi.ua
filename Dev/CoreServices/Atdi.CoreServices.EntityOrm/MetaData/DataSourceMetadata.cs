using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm.Metadata
{
    public class DataSourceMetadata : IDataSourceMetadata
    {
        public string Name { get; set; }

        public string Schema { get; set; }

        public DataSourceType Type { get; set; }

        public DataSourceObject Object { get; set; }
    }
}
