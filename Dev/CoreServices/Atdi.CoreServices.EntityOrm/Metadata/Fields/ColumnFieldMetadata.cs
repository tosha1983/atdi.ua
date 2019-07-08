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
        public ColumnFieldMetadata(IEntityMetadata entity, string name)
            : base(entity, entity, name, FieldSourceType.Column)
        {
        }

        public ColumnFieldMetadata(IEntityMetadata entity, IEntityMetadata baseEntity, string name)
            : base(entity, baseEntity, name, FieldSourceType.Column)
        {
        }

        public override FieldMetadata CopyTo(IEntityMetadata owner)
        {
            var cloneField = new ColumnFieldMetadata(owner, this.BaseEntity, this.Name)
            {
                DataType = this.DataType,
                Desc = this.Desc,
                Required = this.Required,
                SourceName = this.SourceName,
                Title = this.Title,
                Unit = this.Unit,
                Default = this.Default
            };

            return cloneField;
        }
    }
}
