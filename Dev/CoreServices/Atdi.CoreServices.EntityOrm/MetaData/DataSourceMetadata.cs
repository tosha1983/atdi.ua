using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class DataSourceMetadata : IDataSourceMetadata
    {
        public string Name { get; set; }

        public string Schema { get; set; }

        public DataSourceType Type { get; set; }

        public DataSourceObject Object { get; set; }
    }
}
