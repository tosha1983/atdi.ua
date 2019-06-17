using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public sealed class SelectQueryDescriptor
    {
        private readonly QueryRoot _root;
        private readonly QuerySelection _selection;
        private readonly QueryCondition _condition;
        private readonly QuerySorting _sorting;

        private DataLimit _limit;
        private bool _isDistinct;

        public SelectQueryDescriptor(IEntityOrm entityOrm, IEntityMetadata entityMetadata)
        {
            this._root = new QueryRoot(entityOrm, entityMetadata);
            this._selection = new QuerySelection(_root);
            this._condition = new QueryCondition(_root);
            this._sorting = new QuerySorting(_root);
        }

        public IEntityMetadata Entity => this._root.Entity;

        public bool IsDistinct => this._isDistinct;

        public DataLimit Limit => this._limit;

        public IReadOnlyDictionary<string, FieldDescriptor> Cache => this._root.Cache;

        public IReadOnlyDictionary<string, FieldDescriptor> SelectableFields => this._selection.Fields;

        public IReadOnlyList<Condition> Conditions => this._condition.Conditions;

        public IReadOnlyList<SortableFieldDescriptor> SortableFields => this._sorting.Fields;

        public void AppendSelectableField(string path)
        {
            this._selection.Append(path);
        }

        public void AppendSelectableFields(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                this.AppendSelectableField(paths[i]);
            }
        }

        public void AppendSortableField(string path, SortDirection direction)
        {
            this._sorting.Append(path, direction);
        }

        public void AppendCondition(Condition condition)
        {
            this._condition.Append(condition);
        }

        public void Distinct()
        {
            this._isDistinct = true;
        }

        public void SetLimit(int limitValue, LimitValueType limitType)
        {
            if (this._limit == null)
            {
                this._limit = new DataLimit();
            }
            this._limit.Type = limitType;
            this._limit.Value = limitValue;
        }

        public override string ToString()
        {
            return $"Root = [{this._root}], Selection = [{_selection}], Condition = [{_condition}], Sorting = [{_sorting}]";
        }
    }
}
