using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public class PrimaryKeyMappingMetadata : IPrimaryKeyMappingMetadata
    {
        public IReadOnlyDictionary<string, IPrimaryKeyFieldMappedMetadata> Fields { get; set; }
    }
}
