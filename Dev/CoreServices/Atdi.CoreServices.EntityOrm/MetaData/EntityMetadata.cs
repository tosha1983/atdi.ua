using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class EntityMetadata : IEntityMetadata
    {
        private readonly Dictionary<string, IFieldMetadata> _localFields;

        public EntityMetadata()
        {
            this._localFields = new Dictionary<string, IFieldMetadata>();
        }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public EntityType Type { get; set; }

        public IEntityMetadata BaseEntity { get; set; }

        public IDataSourceMetadata DataSource { get; set; }

        public IPrimaryKeyMetadata PrimaryKey { get; set; }

        public IReadOnlyDictionary<string, IFieldMetadata> Fields => _localFields;

        public string QualifiedName { get; set; }

        public override string ToString()
        {
            return $"{this.Name}: Type = '{Type}', Base = '{BaseEntity?.QualifiedName}', Count fields = {Fields.Count}";
        }

        public void AppendField(IFieldMetadata field)
        {
            if (_localFields.ContainsKey(field.Name))
            {
                throw new InvalidOperationException($"A local field named {field.Name} already exists in the local field set");
            }
            this._localFields.Add(field.Name, field);
        }

        public void CopyField(IFieldMetadata field)
        {
            if (_localFields.ContainsKey(field.Name))
            {
                throw new InvalidOperationException($"A local field named {field.Name} already exists in the local field set");
            }
            var cloneField = ((FieldMetadata)field).CopyTo(this);
            this._localFields.Add(cloneField.Name, cloneField);
        }
    }
}
