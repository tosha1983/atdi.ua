using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm.Metadata
{
    public class PrimaryKeyMetadata : IPrimaryKeyMetadata
    {
        public bool? Clustered { get; set; }

        public IReadOnlyDictionary<string, IPrimaryKeyFieldRefMetadata> FieldRefs { get; set; }
    }
}
