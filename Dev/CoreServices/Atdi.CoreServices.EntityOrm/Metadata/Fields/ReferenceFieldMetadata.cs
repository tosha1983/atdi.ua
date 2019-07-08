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
        public ReferenceFieldMetadata(IEntityMetadata entity, string name)
            : base(entity, entity, name, FieldSourceType.Reference)
        {
        }

        public ReferenceFieldMetadata(IEntityMetadata entity, IEntityMetadata baseEntity, string name)
            : base(entity, baseEntity, name, FieldSourceType.Reference)
        {
        }

        public IEntityMetadata RefEntity { get; set; }

        public IPrimaryKeyMappingMetadata Mapping { get; set; }

        public override FieldMetadata CopyTo(IEntityMetadata owner)
        {

            var cloneMapping = default(PrimaryKeyMappingMetadata);

            if (this.Mapping != null)
            {
                var fields = new Dictionary<string, IPrimaryKeyFieldMappedMetadata>();
                foreach (var item in this.Mapping.Fields.Values)
                {
                    switch (item.MatchWith)
                    {
                        case PrimaryKeyMappedMatchWith.Value:
                            var itemAsValue = item as ValuePrimaryKeyFieldMappedMetadata;
                            var pValue = new ValuePrimaryKeyFieldMappedMetadata(item.KeyField)
                            {
                                Value = itemAsValue.Value
                            };
                            fields.Add(pValue.KeyField.Name, pValue);
                            break;
                        case PrimaryKeyMappedMatchWith.Field:
                            var itemAsField = item as FieldPrimaryKeyFieldMappedMetadata;
                            var pField = new FieldPrimaryKeyFieldMappedMetadata(item.KeyField)
                            {
                                EntityField = itemAsField.EntityField
                            };
                            fields.Add(pField.KeyField.Name, pField);
                            break;
                        case PrimaryKeyMappedMatchWith.SourceName:
                            var itemAsSourceName = item as SourceNamePrimaryKeyFieldMappedMetadata;
                            var pSourceName = new SourceNamePrimaryKeyFieldMappedMetadata(item.KeyField)
                            {
                                SourceName = itemAsSourceName.SourceName 
                            };
                            fields.Add(pSourceName.KeyField.Name, pSourceName);
                            break;
                        default:
                            break;
                    }
                }
                cloneMapping = new PrimaryKeyMappingMetadata()
                {
                    Fields = fields
                };
            }
            var cloneField = new ReferenceFieldMetadata(owner, this.BaseEntity, this.Name)
            {
                DataType = this.DataType,
                Desc = this.Desc,
                Required = this.Required,
                SourceName = this.SourceName,
                Title = this.Title,
                Unit = this.Unit,
                Default = this.Default,
                RefEntity = this.RefEntity,
                Mapping = cloneMapping
            };

            return cloneField;
        }
    }
}
