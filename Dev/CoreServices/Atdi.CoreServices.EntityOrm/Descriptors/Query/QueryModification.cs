using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryModification
    {
        private readonly QueryRoot _root;
        private readonly Dictionary<string, FieldValueDescriptor> _fields;

        public QueryModification(QueryRoot root)
        {
            this._root = root;
            this._fields = new Dictionary<string, FieldValueDescriptor>();
        }

        public IReadOnlyDictionary<string, FieldValueDescriptor> Fields => _fields;

        public void Append(ColumnValue value)
        {
            if (this._fields.ContainsKey(value.Name))
            {
                throw new InvalidOperationException($"A field with path '{value.Name}' has been specified more than once in the modification.");
            }

            var descriptor = this._root.EnsureField(value.Name);

            if (descriptor.Field.DataType.CodeVarType != value.DataType)
            {
                throw new InvalidOperationException($"Data type mismatch for the field with name '{descriptor.Field.Name}({descriptor.Field.Title}) in the entity '{descriptor.Field.Entity.Name}({descriptor.Field.Entity.Title})'. The expected type is '{descriptor.Field.DataType.CodeVarType.ToString()}' but the incoming type is '{value.DataType.ToString()}'");
            }

            var fieldValue = new FieldValueDescriptor
            {
                Descriptor = descriptor,
                Store = value
            };
            this._fields.Add(value.Name, fieldValue);
        }

        public override string ToString()
        {
            return $"Fields = {_fields.Count}";
        }
    }
}
