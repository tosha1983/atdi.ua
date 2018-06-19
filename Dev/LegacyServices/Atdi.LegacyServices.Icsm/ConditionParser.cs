using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;

namespace Atdi.LegacyServices.Icsm
{
    internal class ConditionParser
    {
        private readonly IEngineSyntax _syntax;


        public ConditionParser(IEngineSyntax syntax)
        {
            this._syntax = syntax;
        }

        public string Parse(Condition condition, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (condition == null)
            {
                return null;
            }

            return this.ParseCondition(condition, parameters);
        }

        private string ParseCondition(Condition condition, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var result = string.Empty;

            switch (condition.Type)
            {
                case ConditionType.Complex:
                    result = ParseComplexCondition(condition as ComplexCondition, parameters);
                    break;
                case ConditionType.Expression:
                    result = ParseExpression(condition as ConditionExpression, parameters);
                    break;
                default:
                    throw new InvalidOperationException($"The condition type with name '{condition.Type}' is not supported");
            }

            return result;
        }

        private string ParseComplexCondition(ComplexCondition condition, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (condition.Conditions == null || condition.Conditions.Length == 0)
            {
                return string.Empty;
            }

            var conditions = condition.Conditions;
            var expressions = new string[conditions.Length];
            for (int i = 0; i < conditions.Length; i++)
            {
                expressions[i] = this.ParseCondition(conditions[i], parameters);
            }

            return this._syntax.Constraint.JoinExpressions(condition.Operator, expressions);
        }

        private string ParseExpression(ConditionExpression expression, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var result = string.Empty;

            switch (expression.Operator)
            {
                case ConditionOperator.Equal:
                    var leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    var rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                    result = this._syntax.Constraint.Equal(leftExpression, rightExpression);
                    break;
                case ConditionOperator.GreaterEqual:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                    result = this._syntax.Constraint.GreaterEqual(leftExpression, rightExpression);
                    break;
                case ConditionOperator.GreaterThan:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                    result = this._syntax.Constraint.GreaterThan(leftExpression, rightExpression);
                    break;
                case ConditionOperator.LessEqual:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                    result = this._syntax.Constraint.LessEqual(leftExpression, rightExpression);
                    break;
                case ConditionOperator.LessThan:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                    result = this._syntax.Constraint.LessThan(leftExpression, rightExpression);
                    break;
                case ConditionOperator.NotEqual:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                    result = this._syntax.Constraint.NotEqual(leftExpression, rightExpression);
                    break;
                case ConditionOperator.IsNull:
                    if (expression.LeftOperand != null)
                    {
                        leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                        result = this._syntax.Constraint.IsNull(leftExpression);
                    }
                    else
                    {
                        rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                        result = this._syntax.Constraint.IsNull(rightExpression);
                    }
                    break;
                case ConditionOperator.IsNotNull:
                    if (expression.LeftOperand != null)
                    {
                        leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                        result = this._syntax.Constraint.IsNotNull(leftExpression);
                    }
                    else
                    {
                        rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                        result = this._syntax.Constraint.IsNotNull(rightExpression);
                    }
                    break;
                case ConditionOperator.Like:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                    result = this._syntax.Constraint.Like(leftExpression, rightExpression);
                    break;
                case ConditionOperator.NotLike:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    rightExpression = this.ParseOperand(expression.RightOperand, parameters);
                    result = this._syntax.Constraint.NotLike(leftExpression, rightExpression);
                    break;
                case ConditionOperator.In:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    var expressions = this.ParseOperands(expression.RightOperand as ValuesOperand, parameters);
                    result = this._syntax.Constraint.In(leftExpression, expressions);
                    break;
                case ConditionOperator.NotIn:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    expressions = this.ParseOperands(expression.RightOperand as ValuesOperand, parameters);
                    result = this._syntax.Constraint.NotIn(leftExpression, expressions);
                    break;
                case ConditionOperator.Between:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    expressions = this.ParseOperands(expression.RightOperand as ValuesOperand, parameters);
                    result = this._syntax.Constraint.Between(leftExpression, expressions[0], expressions[1]);
                    break;
                case ConditionOperator.NotBetween:
                    leftExpression = this.ParseOperand(expression.LeftOperand, parameters);
                    expressions = this.ParseOperands(expression.RightOperand as ValuesOperand, parameters);
                    result = this._syntax.Constraint.NotBetween(leftExpression, expressions[0], expressions[1]);
                    break;
                default:
                    throw new InvalidOperationException($"The condition operator with name '{expression.Operator}' is not supported");
            }

            return result;
        }

        private string ParseOperand(Operand operand, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (operand == null)
            {
                throw new ArgumentNullException(nameof(operand));
            }

            if (operand.Type == OperandType.Values)
            {
                throw new InvalidOperationException("Incorrect use value type this context");
            }

            var result = string.Empty;

            switch (operand.Type)
            {
                case OperandType.Column:
                    result = this.ParseOperandColumn(operand as ColumnOperand, parameters);
                    break;
                case OperandType.Value:
                    result = this.ParseOperandValue(operand as ValueOperand, parameters);
                    break;
                default:
                    throw new InvalidOperationException($"The operand type with name '{operand.Type}' is not supported in this context");
            }

            return result;
        }

        private string ParseOperandValue(ValueOperand valueOperand, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (valueOperand == null)
            {
                throw new ArgumentNullException(nameof(valueOperand));
            }

            var parameterIndex = parameters.Count + 1;
            var parameterName = "P" + parameterIndex.ToString();
            var parameter = new EngineCommandParameter()
            {
                DataType = valueOperand.DataType,
                Name = parameterName
            };
            

            switch (valueOperand.DataType)
            {
                case DataModels.DataType.String:
                    var valueAsString = valueOperand as StringValueOperand;
                    parameter.Value = valueAsString.Value;
                    break;
                case DataModels.DataType.Boolean:
                    var valueAsBoolean = valueOperand as BooleanValueOperand;
                    parameter.Value = valueAsBoolean.Value;
                    break;
                case DataModels.DataType.Integer:
                    var valueAsInteger = valueOperand as IntegerValueOperand;
                    parameter.Value = valueAsInteger.Value;
                    break;
                case DataModels.DataType.DateTime:
                    var valueAsDateTime = valueOperand as DateTimeValueOperand;
                    parameter.Value = valueAsDateTime.Value;
                    break;
                case DataModels.DataType.Double:
                    var valueAsDouble = valueOperand as DoubleValueOperand;
                    parameter.Value = valueAsDouble.Value;
                    break;
                case DataModels.DataType.Float:
                    var valueAsFloat = valueOperand as FloatValueOperand;
                    parameter.Value = valueAsFloat.Value;
                    break;
                case DataModels.DataType.Decimal:
                    var valueAsDecimal = valueOperand as DecimalValueOperand;
                    parameter.Value = valueAsDecimal.Value;
                    break;
                case DataModels.DataType.Byte:
                    var valueAsByte = valueOperand as ByteValueOperand;
                    parameter.Value = valueAsByte.Value;
                    break;
                case DataModels.DataType.Bytes:
                    var valueAsBytes = valueOperand as BytesValueOperand;
                    parameter.Value = valueAsBytes.Value;
                    break;
                default:
                    throw new InvalidOperationException($"The data type with name '{valueOperand.DataType}' is not supported in this context");
            }
            parameters.Add(parameterName, parameter);

            var result = this._syntax.EncodeParameterName(parameterName);
            return result;
        }

        private string ParseOperandColumn(ColumnOperand columnOperand, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (columnOperand == null)
            {
                throw new ArgumentNullException(nameof(columnOperand));
            }

            if (string.IsNullOrEmpty(columnOperand.ColumnName))
            {
                throw new ArgumentNullException(nameof(columnOperand.ColumnName));
            }

            var result = string.Empty;

            if (string.IsNullOrEmpty(columnOperand.Source))
            {
               result = this._syntax.EncodeFieldName(columnOperand.ColumnName);
            }
            else
            {
                result = this._syntax.EncodeFieldName(columnOperand.Source, columnOperand.ColumnName);
            }

            return result;
        }

        private string[] ParseOperands(ValuesOperand valuesOperand, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (valuesOperand == null)
            {
                throw new ArgumentNullException(nameof(valuesOperand));
            }

            string[] result = null;

            switch (valuesOperand.DataType)
            {
                case DataType.String:
                    var valuesAsString = valuesOperand as StringValuesOperand;
                    result = CreateParametersByValues(valuesAsString.Values, DataType.String, parameters);
                    break;
                case DataType.Boolean:
                    var valuesAsBoolean = valuesOperand as BooleanValuesOperand;
                    result = CreateParametersByValues(valuesAsBoolean.Values, DataType.Boolean, parameters);
                    break;
                case DataType.Integer:
                    var valuesAsInteger = valuesOperand as IntegerValuesOperand;
                    result = CreateParametersByValues(valuesAsInteger.Values, DataType.Integer, parameters);
                    break;
                case DataType.DateTime:
                    var valuesAsDateTime = valuesOperand as DateTimeValuesOperand;
                    result = CreateParametersByValues(valuesAsDateTime.Values, DataType.DateTime, parameters);
                    break;
                case DataType.Double:
                    var valuesAsDouble = valuesOperand as DoubleValuesOperand;
                    result = CreateParametersByValues(valuesAsDouble.Values, DataType.Double, parameters);
                    break;
                case DataType.Float:
                    var valuesAsFloat = valuesOperand as FloatValuesOperand;
                    result = CreateParametersByValues(valuesAsFloat.Values, DataType.Float, parameters);
                    break;
                case DataType.Decimal:
                    var valuesAsDecimal = valuesOperand as DecimalValuesOperand;
                    result = CreateParametersByValues(valuesAsDecimal.Values, DataType.Decimal, parameters);
                    break;
                case DataType.Byte:
                    var valuesAsByte = valuesOperand as ByteValuesOperand;
                    result = CreateParametersByValues(valuesAsByte.Values, DataType.Byte, parameters);
                    break;
                case DataType.Bytes:
                    var valuesAsBytes = valuesOperand as BytesValuesOperand;
                    result = CreateParametersByValues(valuesAsBytes.Values, DataType.Bytes, parameters);
                    break;
                default:
                    throw new InvalidOperationException($"The data type with name '{valuesOperand.DataType}' is not supported in this context");
            }

            return result;
        }


        private string[] CreateParametersByValues<TValue>(TValue[] values, DataType dataType, IDictionary<string, EngineCommandParameter> parameters)
        {
            var result = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                var parameterIndex = parameters.Count + 1;
                var parameterName = "P" + parameterIndex.ToString();
                var parameter = new EngineCommandParameter()
                {
                    DataType = dataType,
                    Name = parameterName,
                    Value = values[i]
                };
                parameters.Add(parameterName, parameter);
                result[i] = this._syntax.EncodeParameterName(parameterName);
            }
            return result;
        }

    }
}
