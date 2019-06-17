using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class ExtensionFieldMetadata : FieldMetadata, IExtensionFieldMetadata
    {
        public ExtensionFieldMetadata(IEntityMetadata entityMetadata, string name) 
            : base(entityMetadata, name, FieldSourceType.Extension)
        {
        }

        public IEntityMetadata ExtensionEntity { get; set; }

    }
}
