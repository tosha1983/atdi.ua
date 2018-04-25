using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts;

namespace Atdi.AppServer.Common
{
    public abstract class StatementConstrainParserBase<TModel, TParser> : IConstraintParser<string>
        where TParser : StatementConstrainParserBase<TModel, TParser>
    {
        private class MappingEntry
        {
            public string SourceName;
            public string TargetName;
        }

        private readonly int _nullForInt = 0;
        private readonly double _nullForDouble = 0;
        private readonly DateTime _nullForDateTime = new DateTime();

        private readonly IConstraintStatementBuilder _builder;
        private readonly Type _modelType;
        private readonly string _tableName;
        private readonly Dictionary<string, MappingEntry> _mappingEntries;

        public StatementConstrainParserBase(IConstraintStatementBuilder builder, string tableName)
        {
            this._builder = builder;
            this._modelType = typeof(TModel);
            this._tableName = tableName;
            this._mappingEntries = new Dictionary<string, MappingEntry>();
            this.OnSetupMapping();
        }

        protected abstract void OnSetupMapping();

        protected void RegistryFieldMapping<TValue>(string sourceName, string targetName)
        {
            var mappingEntry = new MappingEntry
            {
                TargetName = targetName,
                SourceName = sourceName
            };

            if (this._mappingEntries.ContainsKey(mappingEntry.SourceName))
            {
                throw new InvalidOperationException("Duplicate name of the source");
            }

            this._mappingEntries.Add(mappingEntry.SourceName, mappingEntry);
        }

        protected void RegistryFieldMapping<TValue>(Expression<Func<TModel, TValue>> member, string targetName)
        {
            var mappingEntry = new MappingEntry
            {
                TargetName = targetName,
                SourceName = this.GetMemberName(member)
            };

            if (this._mappingEntries.ContainsKey(mappingEntry.SourceName))
            {
                throw new InvalidOperationException("Duplicate name of the source");
            }

            this._mappingEntries.Add(mappingEntry.SourceName, mappingEntry);
        }

        private string GetMemberName<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var memberName = string.Empty;

            var currentExpression = expression.Body;
            while (currentExpression.NodeType == ExpressionType.MemberAccess)
            {
                var currentMember = currentExpression as MemberExpression;
                if (!string.IsNullOrEmpty(memberName))
                {
                    memberName = currentMember.Member.Name + "." + memberName;
                }
                else
                {
                    memberName = currentMember.Member.Name;
                }
                currentExpression = currentMember.Expression;
            }

            if (this._modelType != currentExpression.Type)
            {
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a member that is not from type {1}.",
                    expression.ToString(),
                    typeof(TModel)));
            }

            return memberName;
        }

        public string Parse(DataConstraint constraint)
        {
            if (constraint == null)
            {
                return null;
            }

            return this.ParseConstraint(constraint);
        }

        private string ParseConstraint(DataConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }

            var result = string.Empty;

            switch (constraint.Type)
            {
                case DataConstraintType.Group:
                    result = ParseGroup(constraint as DataConstraintGroup);
                    break;
                case DataConstraintType.Expression:
                    result = ParseExpression(constraint as DataConstraintExpression);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported type of {constraint.Type}");
            }

            return result;
        }

        private string ParseGroup(DataConstraintGroup constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }

            if (constraint.Constraints == null || constraint.Constraints.Length == 0)
            {
                throw new InvalidOperationException("Undefined Constraints");
            }

            var expressions = constraint.Constraints.Select(item => this.ParseConstraint(item)).ToArray();

            return this._builder.OpeningBracket +
                (constraint.Operation == DataConstraintGroupOperation.And ? this._builder.MakeAndStatement(expressions) : this._builder.MakeOrStatement(expressions))
                + this._builder.ClosingBracket;
        }

        private string ParseExpression(DataConstraintExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var result = string.Empty;

            switch (expression.Operation)
            {
                case DataConstraintOperation.Eq:
                    var leftExpression = this.ParseOperand(expression.LeftOperand);
                    var rightExpression = this.ParseOperand(expression.RightOperand);
                    result = this._builder.MakeEqualToStatement(leftExpression, rightExpression);
                    break;
                case DataConstraintOperation.Geq:
                    leftExpression = this.ParseOperand(expression.LeftOperand);
                    rightExpression = this.ParseOperand(expression.RightOperand);
                    result = this._builder.MakeGreaterThanOrEqualToStatement(leftExpression, rightExpression);
                    break;
                case DataConstraintOperation.Gt:
                    leftExpression = this.ParseOperand(expression.LeftOperand);
                    rightExpression = this.ParseOperand(expression.RightOperand);
                    result = this._builder.MakeGreaterThanStatement(leftExpression, rightExpression);
                    break;
                case DataConstraintOperation.Leq:
                    leftExpression = this.ParseOperand(expression.LeftOperand);
                    rightExpression = this.ParseOperand(expression.RightOperand);
                    result = this._builder.MakeLessThanOrEqualToStatement(leftExpression, rightExpression);
                    break;
                case DataConstraintOperation.Lt:
                    leftExpression = this.ParseOperand(expression.LeftOperand);
                    rightExpression = this.ParseOperand(expression.RightOperand);
                    result = this._builder.MakeLessThanStatement(leftExpression, rightExpression);
                    break;
                case DataConstraintOperation.Neq:
                    leftExpression = this.ParseOperand(expression.LeftOperand);
                    rightExpression = this.ParseOperand(expression.RightOperand);
                    result = this._builder.MakeNotEqualToStatement(leftExpression, rightExpression);
                    break;
                case DataConstraintOperation.IsNull:
                    if (expression.LeftOperand != null)
                    {
                        leftExpression = this.ParseOperand(expression.LeftOperand);
                        result = this._builder.MakeNullStatement(leftExpression);
                    }
                    else
                    {
                        rightExpression = this.ParseOperand(expression.RightOperand);
                        result = this._builder.MakeNullStatement(rightExpression);
                    }
                    break;
                case DataConstraintOperation.IsNotNull:
                    if (expression.LeftOperand != null)
                    {
                        leftExpression = this.ParseOperand(expression.LeftOperand);
                        result = this._builder.MakeNotNullStatement(leftExpression);
                    }
                    else
                    {
                        rightExpression = this.ParseOperand(expression.RightOperand);
                        result = this._builder.MakeNotNullStatement(rightExpression);
                    }
                    break;
                case DataConstraintOperation.Like:
                    leftExpression = this.ParseOperand(expression.LeftOperand);
                    rightExpression = this.ParseOperand(expression.RightOperand);
                    result = this._builder.MakeLikeStatement(leftExpression, rightExpression);
                    break;
                case DataConstraintOperation.NotLike:
                    leftExpression = this.ParseOperand(expression.LeftOperand);
                    rightExpression = this.ParseOperand(expression.RightOperand);
                    result = this._builder.MakeNotLikeStatement(leftExpression, rightExpression);
                    break;
                case DataConstraintOperation.In:
                    leftExpression = this.ParseOperand(expression.LeftOperand);
                    var expressions = this.ParseOperands(expression.RightOperand as DataConstraintValuesOperand);
                    result = this._builder.MakeInStatement(leftExpression, expressions);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported operation type of {expression.Operation}");
            }

            return result;
        }

        private string ParseOperand(DataConstraintOperand operand)
        {
            if (operand == null)
            {
                throw new ArgumentNullException(nameof(operand));
            }

            if (operand.Type == DataConstraintOperandType.Values)
            {
                throw new InvalidOperationException("Incorrect use value type this context");
            }

            var result = string.Empty;

            switch (operand.Type)
            {
                case DataConstraintOperandType.Column:
                    result = this.ParseOperandColumn(operand as DataConstraintColumnOperand);
                    break;
                case DataConstraintOperandType.Value:
                    result = this.ParseOperandValue(operand as DataConstraintValueOperand);
                    break;
                default:
                    throw new InvalidOperationException("Incorrect use value type this context");
            }

            return result;
        }

        private string ParseOperandValue(DataConstraintValueOperand operand)
        {
            if (operand == null)
            {
                throw new ArgumentNullException(nameof(operand));
            }

            var result = string.Empty;

            switch (operand.DataType)
            {
                case CommonDataType.String:
                    result = this._builder.EncodeValue(operand.StringValue);
                    break;
                case CommonDataType.Boolean:
                    result = this._builder.EncodeValue(operand.BooleanValue.Value);
                    break;
                case CommonDataType.Integer:
                    result = this._builder.EncodeValue(operand.IntegerValue ?? this._nullForInt);
                    break;
                case CommonDataType.DateTime:
                    result = this._builder.EncodeValue(operand.DateTimeValue ?? this._nullForDateTime);
                    break;
                case CommonDataType.Double:
                    result = this._builder.EncodeValue(operand.DoubleValue ?? this._nullForDouble);
                    break;
                case CommonDataType.Bytes:
                default:
                    throw new InvalidOperationException("Incorrect use value type this context");
            }

            return result;
        }

        private string ParseOperandColumn(DataConstraintColumnOperand operand)
        {
            if (operand == null)
            {
                throw new ArgumentNullException(nameof(operand));
            }

            if (string.IsNullOrEmpty(operand.ColumnName))
            {
                throw new ArgumentNullException(nameof(operand.ColumnName));
            }

            if (!this._mappingEntries.ContainsKey(operand.ColumnName))
            {
                throw new InvalidFilterCriteriaException($"Incorrect column name {operand.ColumnName}");
            }

            var mappingEntry = this._mappingEntries[operand.ColumnName];
            var result = string.Empty;

            if (string.IsNullOrEmpty(operand.Alias))
            {
               result = this._builder.EncodeFieldName(mappingEntry.TargetName);
            }
            else
            {
                result = this._builder.EncodeFieldName(operand.Alias, mappingEntry.TargetName);
            }

            return result;
        }

        private string[] ParseOperands(DataConstraintValuesOperand operand)
        {
            if (operand == null)
            {
                throw new ArgumentNullException(nameof(operand));
            }

            string[] result = null;

            switch (operand.DataType)
            {
                case CommonDataType.String:
                    if (operand.StringValues == null)
                    {
                        throw new ArgumentNullException(nameof(operand.StringValues));
                    }
                    result = operand.StringValues.Select(v => this._builder.EncodeValue(v)).ToArray();
                    break;
                case CommonDataType.Boolean:
                    if (operand.BooleanValues == null)
                    {
                        throw new ArgumentNullException(nameof(operand.BooleanValues));
                    }
                    result = operand.BooleanValues.Select(v => this._builder.EncodeValue(v.Value)).ToArray();
                    break;
                case CommonDataType.Integer:
                    if (operand.IntegerValues == null)
                    {
                        throw new ArgumentNullException(nameof(operand.IntegerValues));
                    }
                    result = operand.IntegerValues.Select(v => this._builder.EncodeValue(v ?? this._nullForInt)).ToArray();
                    break;
                case CommonDataType.DateTime:
                    if (operand.DateTimeValues == null)
                    {
                        throw new ArgumentNullException(nameof(operand.DateTimeValues));
                    }
                    result = operand.DateTimeValues.Select(v => this._builder.EncodeValue(v ?? this._nullForDateTime)).ToArray();
                    break;
                case CommonDataType.Double:
                    if (operand.DoubleValues == null)
                    {
                        throw new ArgumentNullException(nameof(operand.DoubleValues));
                    }
                    result = operand.DoubleValues.Select(v => this._builder.EncodeValue(v ?? this._nullForDouble)).ToArray();
                    break;
                case CommonDataType.Bytes:
                default:
                    throw new InvalidOperationException("Incorrect use value type this context");
            }

            return result;
        }

    }
}
