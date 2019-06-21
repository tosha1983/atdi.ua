using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class ReferenceFieldMetadata : FieldMetadata, IReferenceFieldMetadata
    {
        public ReferenceFieldMetadata(IEntityMetadata entityMetadata, string name)
            : base(entityMetadata, name, FieldSourceType.Reference)
        {
        }

        public IEntityMetadata RefEntity { get; set; }

        public IPrimaryKeyMappingMetadata Mapping { get; set; }

        
    }
}
