using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class PrimaryKeyMetadata : IPrimaryKeyMetadata
    {
        public bool? Clustered { get; set; }

        public IReadOnlyDictionary<string, IPrimaryKeyFieldRefMetadata> FieldRefs { get; set; }
    }
}
