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
        public ExtensionFieldMetadata(IEntityMetadata entity, string name)
            : base(entity, entity, name, FieldSourceType.Extension)
        {
        }
        public ExtensionFieldMetadata(IEntityMetadata entity, IEntityMetadata baseEntity, string name) 
            : base(entity, baseEntity, name, FieldSourceType.Extension)
        {
        }

        public IEntityMetadata ExtensionEntity { get; set; }

        public override FieldMetadata CopyTo(IEntityMetadata owner)
        {
            var cloneField = new ExtensionFieldMetadata(owner, this.BaseEntity, this.Name)
            {
                DataType = this.DataType,
                Desc = this.Desc,
                Required = this.Required,
                SourceName = this.SourceName,
                Title = this.Title,
                Unit = this.Unit,
                Default = this.Default,
                ExtensionEntity = this.ExtensionEntity
            };

            return cloneField;
        }
    }
}
