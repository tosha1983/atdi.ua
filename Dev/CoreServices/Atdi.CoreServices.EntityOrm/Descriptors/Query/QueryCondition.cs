using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryCondition
    {
        private readonly QueryRoot _root;
        private readonly Dictionary<string, FieldDescriptor> _fieldsCache;
        private readonly List<Condition> _conditions;

        public QueryCondition(QueryRoot root)
        {
            this._root = root;
            this._fieldsCache = new Dictionary<string, FieldDescriptor>();
            this._conditions = new List<Condition>();
        }

        public IReadOnlyList<Condition> Conditions => this._conditions;

        public IReadOnlyDictionary<string, FieldDescriptor> Cache => _fieldsCache;

        public void Append(Condition condition)
        {
            this._conditions.Add(condition);
            this.AppendFieldsFromCondition(condition);
        }

        public override string ToString()
        {
            return $"Conditions = {_conditions.Count}, Cached fields = {_fieldsCache.Count}";
        }

        private void AppendFieldsFromCondition(Condition condition)
        {
            if (condition.Type == ConditionType.Complex)
            {
                var complexCondition = condition as ComplexCondition;
                var conditions = complexCondition.Conditions;
                if (conditions != null && conditions.Length > 0)
                {
                    for (int i = 0; i < conditions.Length; i++)
                    {
                        AppendFieldsFromCondition(conditions[i]);
                    }
                }
                return;
            }
            else if (condition.Type == ConditionType.Expression)
            {
                var conditionExpression = condition as ConditionExpression;
                AppendFieldFromOperand(conditionExpression.LeftOperand);
                AppendFieldFromOperand(conditionExpression.RightOperand);
            }
        }

        private void AppendFieldFromOperand(Operand operand)
        {
            if (operand == null)
            {
                return;
            }
            if (operand.Type == OperandType.Column)
            {
                var columnOperand = operand as ColumnOperand;
                this.AppendConditionField(columnOperand.ColumnName);
            }
        }

        private void AppendConditionField(string path)
        {
            if (this._fieldsCache.ContainsKey(path))
            {
                return;
            }

            var field = this._root.EnsureField(path);
            this._fieldsCache.Add(path, field);
        }
    }
}
