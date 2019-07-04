using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class UpdateQueryDescriptor
    {
        private readonly QueryRoot _root;
        private readonly QueryModification _modification;
        private readonly QueryCondition _condition;

        public UpdateQueryDescriptor(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._root = new QueryRoot(entityOrm, entityMetadata);
            this._modification = new QueryModification(_root);
            this._condition = new QueryCondition(_root);
        }

        public IEntityMetadata Entity => this._root.Entity;

        public IReadOnlyList<Condition> Conditions => this._condition.Conditions;

        public IReadOnlyDictionary<string, FieldValueDescriptor> Fields => this._modification.Fields;

        public IReadOnlyDictionary<string, FieldDescriptor> Cache => this._root.Cache;

        public FieldValueDescriptor[] GetValues()
        {
            return this._modification.Fields.Values.ToArray();
        }

        public void AppendCondition(Condition condition)
        {
            this._condition.Append(condition);
        }

        public void AppendValue(ColumnValue value)
        {
            this._modification.Append(value);
        }

        public override string ToString()
        {
            return $"Root = [{this._root}], Modification = [{_modification}], Condition = [{_condition}]";
        }
    }
}
