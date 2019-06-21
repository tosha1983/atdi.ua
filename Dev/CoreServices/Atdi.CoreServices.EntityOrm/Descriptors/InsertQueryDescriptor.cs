using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class InsertQueryDescriptor
    {
        private readonly QueryRoot _root;
        private readonly QueryModification _modification;

        public InsertQueryDescriptor(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._root = new QueryRoot(entityOrm, entityMetadata);
            this._modification = new QueryModification(_root);
        }

        public IEntityMetadata Entity => this._root.Entity;

        public IReadOnlyDictionary<string, FieldValueDescriptor> Fields => this._modification.Fields;

        public FieldValueDescriptor[] GetValues()
        {
            return this._modification.Fields.Values.ToArray();
        }

        public void AppendValue(ColumnValue value)
        {
            this._modification.Append(value);
        }

        public override string ToString()
        {
            return $"Root = [{this._root}], Modification = [{_modification}]";
        }
    }
}
