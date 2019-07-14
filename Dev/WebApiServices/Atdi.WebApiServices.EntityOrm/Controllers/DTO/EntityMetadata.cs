using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = Atdi.DataModels.EntityOrm.Api;

namespace Atdi.WebApiServices.EntityOrm.Controllers.DTO
{
    public class EntityMetadata : API.IEntityMetadata
    {
        public EntityMetadata(IEntityMetadata entityMetadata)
        {
            if (entityMetadata == null)
            {
                throw new ArgumentNullException(nameof(entityMetadata));
            }

            this.Name = entityMetadata.Name;
            this.QualifiedName = entityMetadata.QualifiedName;
            this.Namespace = entityMetadata.Namespace;
            this.Title = entityMetadata.Title;
            this.Desc = entityMetadata.Desc;
            this.TypeCode = (int)entityMetadata.Type;
            this.TypeName = entityMetadata.Type.ToString();
            if (entityMetadata.BaseEntity != null)
            {
                this.BaseEntity = new EntityMetadata(entityMetadata.BaseEntity);
            }
            var fields = entityMetadata.DefineFieldsWithInherited();
            this.Fields = new EntityFieldMetadata[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                switch (field.SourceType)
                {
                    case FieldSourceType.Column:
                        this.Fields[i] = new ColumnEntityFieldMetadata((IColumnFieldMetadata)fields[i]);
                        break;
                    case FieldSourceType.Reference:
                        this.Fields[i] = new ReferenceEntityFieldMetadata((IReferenceFieldMetadata)fields[i]);
                        break;
                    case FieldSourceType.Extension:
                        this.Fields[i] = new ExtensionEntityFieldMetadata((IExtensionFieldMetadata)fields[i]);
                        break;
                    case FieldSourceType.Relation:
                        this.Fields[i] = new RelationEntityFieldMetadata((IRelationFieldMetadata)fields[i]);
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported source type {field.SourceType}");
                }
                
            }

            var pk = entityMetadata.DefinePrimaryKey();
            if (pk != null && pk.FieldRefs != null && pk.FieldRefs.Count > 0)
            {
                var pkFields = pk.FieldRefs.Values.ToArray();
                this.PrimaryKey = new string[pkFields.Length];
                for (int i = 0; i < pkFields.Length; i++)
                {
                    this.PrimaryKey[i] = pkFields[i].Field.Name;
                }
            }
            
        }

        /// <summary>
        /// The qualified name
        /// </summary>
        public string Name { get; }

        public string QualifiedName { get; }

        public string Namespace { get; }

        public string Title { get; }

        public string Desc { get; }

        public int TypeCode { get; }

        public string TypeName { get; }

        public API.IEntityMetadata BaseEntity { get; }

        public API.IEntityFieldMetadata[] Fields { get; }

        public string[] PrimaryKey { get; }
    }
}
