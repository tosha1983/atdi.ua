using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QuerySelection
    {
        private readonly QueryRoot _root;
        private readonly Dictionary<string, FieldDescriptor> _fields;

        public QuerySelection(QueryRoot root)
        {
            this._root = root;
            this._fields = new Dictionary<string, FieldDescriptor>();
        }

        public IReadOnlyDictionary<string, FieldDescriptor> Fields => _fields;

        public void Append(string path)
        {
            if (this._fields.ContainsKey(path))
            {
                throw new InvalidOperationException($"A field with path '{path}' has been specified more than once in the selection.");
            }

            var field = this._root.EnsureField(path);
            this._fields.Add(path, field);
        }
        public override string ToString()
        {
            return $"Fields = {_fields.Count}";
        }
    }
}
