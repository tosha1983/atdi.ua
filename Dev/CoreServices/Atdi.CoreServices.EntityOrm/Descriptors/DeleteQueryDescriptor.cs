﻿using Atdi.Contracts.CoreServices.EntityOrm;
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
    internal sealed class DeleteQueryDescriptor
    {
        private readonly QueryRoot _root;
        private readonly QueryCondition _condition;

        public DeleteQueryDescriptor(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._root = new QueryRoot(entityOrm, entityMetadata);
            this._condition = new QueryCondition(_root);
        }

        public IEntityMetadata Entity => this._root.Entity;

        public IReadOnlyList<Condition> Conditions => this._condition.Conditions;

        public IReadOnlyDictionary<string, FieldDescriptor> Cache => this._root.Cache;

        public void AppendCondition(Condition condition)
        {
            this._condition.Append(condition);
        }

        public override string ToString()
        {
            return $"Root = [{this._root}], Condition = [{_condition}]";
        }
    }
}
