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
        public PrimaryKeyMetadata(IEntityMetadata entity)
        {
            this.Entity = entity;
        }
        public bool? Clustered { get; set; }

        public IReadOnlyDictionary<string, IPrimaryKeyFieldRefMetadata> FieldRefs { get; set; }

        public IEntityMetadata Entity { get; }
    }
}
