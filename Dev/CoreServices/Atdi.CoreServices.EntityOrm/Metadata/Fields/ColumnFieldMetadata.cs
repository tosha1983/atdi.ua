using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class ColumnFieldMetadata : FieldMetadata, IColumnFieldMetadata
    {
        public ColumnFieldMetadata(IEntityMetadata entityMetadata, string name)
            : base(entityMetadata, name, FieldSourceType.Column)
        {
        }

        
    }
}
