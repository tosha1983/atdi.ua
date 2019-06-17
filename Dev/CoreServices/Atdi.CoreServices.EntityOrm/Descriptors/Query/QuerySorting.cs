using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QuerySorting
    {
        private readonly QueryRoot _root;
        private readonly Dictionary<string, FieldDescriptor> _cache;
        private readonly List<SortableFieldDescriptor> _fields;

        public QuerySorting(QueryRoot root)
        {
            this._root = root;
            this._cache = new Dictionary<string, FieldDescriptor>();
            this._fields = new List<SortableFieldDescriptor>();
        }

        public IReadOnlyDictionary<string, FieldDescriptor> Cache => _cache;

        public IReadOnlyList<SortableFieldDescriptor> Fields => _fields;

        public void Append(string path, SortDirection direction)
        {
            if (this._cache.ContainsKey(path))
            {
                throw new InvalidOperationException($"A field with path '{path}' has been specified more than once in the order by list.");
            }

            var field = this._root.EnsureField(path);
            this._cache.Add(path, field);

            var descriptor = new SortableFieldDescriptor
            {
                Field = field,
                Direction = direction
            };
            this._fields.Add(descriptor);
        }
        public override string ToString()
        {
            return $"Fields = {_fields.Count}";
        }
    }
}
