using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryRoot
    {
        private readonly IEntityOrm _entityOrm;
        private readonly IEntityMetadata _entityMetadata;
        private readonly Dictionary<string, FieldDescriptor> _fieldsCache;

        public QueryRoot(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._entityOrm = entityOrm;
            this._entityMetadata = entityMetadata;
            this._fieldsCache = new Dictionary<string, FieldDescriptor>();
        }

        public IEntityMetadata Entity => this._entityMetadata;

        public IReadOnlyDictionary<string, FieldDescriptor> Cache => _fieldsCache;

        public FieldDescriptor EnsureField(string path)
        {
            if (this._fieldsCache.TryGetValue(path, out FieldDescriptor field))
            {
                return field;
            }

            field = this.BuidlFileDecriptorByPath(path);
            this._fieldsCache.Add(path, field);

            return field;
        }
        public override string ToString()
        {
            return $"Entity = '{this._entityMetadata.QualifiedName}', Cached fields = {_fieldsCache.Count}";
        }

        private FieldDescriptor BuidlFileDecriptorByPath(string path)
        {
            var nameParts = path.Split('.');
            if (nameParts.Length == 0)
            {
                throw new InvalidOperationException($"Incorrect field path {path}");
            }

            var fieldName = nameParts[0];
            if (!this._entityMetadata.TryGetLocalField(fieldName, out IFieldMetadata fieldMetadata))
            {
                throw new InvalidOperationException($"Not found a field with name '{fieldName}' from the entity {this._entityMetadata} by path '{path}'");
            }

            var field = new FieldDescriptor(this._entityMetadata, fieldMetadata)
            {
                Path = path
            };

            // is it no reference field?
            if (nameParts.Length == 1)
            {
                field.Entity = this._entityMetadata;
                field.Field = fieldMetadata;
                return field;
            }

            IEntityMetadata entityMetadata = this._entityMetadata;
            var fieldReference = new FieldReferenceDescriptor
            {
                Path = fieldName,
                Entity = entityMetadata,
                Field = fieldMetadata
            };
            field.Reference = fieldReference;
            
            for (int i = 1; i < nameParts.Length; i++)
            {
                ++field.RefDepth;
                if (fieldMetadata.SourceType != FieldSourceType.Reference
                    && fieldMetadata.SourceType != FieldSourceType.Extension
                    && fieldMetadata.SourceType != FieldSourceType.Relation)
                {
                    throw new InvalidOperationException($"Incorrect the field '{fieldMetadata}' metadata for reference definition");
                }
                var nextEntityName = fieldMetadata.SourceName;
                // SourceName can be: EntityName, Namespace.EntityName, Namescpae.Folder1.Folder2...FolderN.EntityName
                entityMetadata = _entityOrm.GetEntityMetadata(nextEntityName);
                fieldName = nameParts[i];
                if (!entityMetadata.TryGetLocalField(fieldName, out fieldMetadata))
                {
                    throw new InvalidOperationException($"Not found a field with name '{fieldName}' from the entity {entityMetadata} by path '{path}'");
                }

                fieldReference.Next = new FieldReferenceDescriptor
                {
                    Path = $"{fieldReference.Path}.{fieldName}",
                    Entity = entityMetadata,
                    Field = fieldMetadata,
                    Prev = fieldReference
                };
                fieldReference = fieldReference.Next;
            }
            field.Entity = entityMetadata;
            field.Field = fieldMetadata;
            return field;
        }

        
    }
}
