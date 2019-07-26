using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Atdi.Contracts.CoreServices.DataLayer;
using System.IO;
using System.Xml;

namespace Atdi.CoreServices.DataLayer.Assemblies
{
    class EngineDataReader : IEngineDataReader
    {
        private readonly PS.SelectPattern _selectPattern;
        private readonly ServiceObjectResolver _resolver;
        private int _index;
        private PS.SelectExpression _expression;
        private IEnumerable _data;
        private IEnumerator _reader;
        private PS.DataEngineMember[] _properties;
        private Type _recordType;
        private int _count;

        public EngineDataReader(PS.SelectPattern selectPattern, ServiceObjectResolver resolver)
        {
            this._selectPattern = selectPattern;
            this._resolver = resolver;
            this._index = -1;
            this.IsClosed = !this.NextResult();
        }

        public int Depth => throw new NotImplementedException();

        public bool IsClosed { get; set; }

        public int RecordsAffected => throw new NotImplementedException();

        public int FieldCount => this._expression.Columns.Length;

        public string GetAlias(int i)
        {
            return _properties[i].Name;
        }

        public bool GetBool(int i)
        {
            return (bool)this.GetValueByIndex(i);
        }

        public byte GetByte(int i)
        {
            return (byte)this.GetValueByIndex(i);
        }

        public byte[] GetBytes(int i)
        {
            return (byte[])this.GetValueByIndex(i);
        }

        public char GetChar(int i)
        {
            return (char)this.GetValueByIndex(i);
        }

        public char[] GetChars(int i)
        {
            return (char[])this.GetValueByIndex(i);
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)this.GetValueByIndex(i);
        }

        public DateTimeOffset GetDateTimeOffset(int i)
        {
            return (DateTimeOffset)this.GetValueByIndex(i);
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)this.GetValueByIndex(i);
        }

        public double GetDouble(int i)
        {
            return (double)this.GetValueByIndex(i);
        }

        public Type GetFieldType(int i)
        {
            var property = _recordType.GetProperty(_properties[i].Name);
            return property.PropertyType;
        }

        public float GetFloat(int i)
        {
            return (float)this.GetValueByIndex(i);
        }

        public Guid GetGuid(int i)
        {
            return (Guid)this.GetValueByIndex(i);
        }

        public short GetInt16(int i)
        {
            return (short)this.GetValueByIndex(i);
        }

        public int GetInt32(int i)
        {
            return (int)this.GetValueByIndex(i);
        }

        public long GetInt64(int i)
        {
            return (long)this.GetValueByIndex(i);
        }

        public int GetOrdinalByAlias(string alias)
        {
            for (int i = 0; i < _properties.Length; i++)
            {
                var property = _properties[i];
                if (property.Name == alias)
                {
                    return i;
                }
            }
            throw new KeyNotFoundException(alias);
        }

        public int GetOrdinalByPath(string path)
        {
            for (int i = 0; i < _properties.Length; i++)
            {
                var property = _properties[i];
                if (property.Property == path)
                {
                    return i;
                }
            }
            throw new KeyNotFoundException(path);
        }

        public long GetPartBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
        {
            throw new NotImplementedException();
        }

        public long GetPartChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
        {
            throw new NotImplementedException();
        }

        public string GetPath(int i)
        {
            return _properties[i].Property;
        }

        public sbyte GetSByte(int i)
        {
            return (sbyte)this.GetValueByIndex(i);
        }

        public Stream GetStream(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (string)this.GetValueByIndex(i);
        }

        public TextReader GetTextReader(int i)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetTimeSpan(int i)
        {
            return (TimeSpan)this.GetValueByIndex(i);
        }

        public ushort GetUInt16(int i)
        {
            return (ushort)this.GetValueByIndex(i);
        }

        public uint GetUInt32(int i)
        {
            return (uint)this.GetValueByIndex(i);
        }

        public ulong GetUInt64(int i)
        {
            return (ulong)this.GetValueByIndex(i);
        }

        public object GetValue(int i, Type type)
        {
            if (type == typeof(int))
            {
                return this.GetInt32(i);
            }
            if (type == typeof(long))
            {
                return this.GetInt64(i);
            }
            if (type == typeof(string))
            {
                return this.GetString(i);
            }
            if (type == typeof(float))
            {
                return this.GetFloat(i);
            }
            if (type == typeof(double))
            {
                return this.GetDouble(i);
            }
            if (type == typeof(decimal))
            {
                return this.GetDecimal(i);
            }
            if (type == typeof(bool))
            {
                return this.GetBool(i);
            }
            if (type == typeof(byte))
            {
                return this.GetByte(i);
            }
            if (type == typeof(byte[]))
            {
                return this.GetBytes(i);
            }
            if (type == typeof(short))
            {
                return this.GetInt16(i);
            }
            if (type == typeof(Guid))
            {
                return this.GetGuid(i);
            }
            if (type == typeof(DateTime))
            {
                return this.GetDateTime(i);
            }
            if (type == typeof(DateTimeOffset))
            {
                return this.GetDateTimeOffset(i);
            }
            if (type == typeof(TimeSpan))
            {
                return this.GetTimeSpan(i);
            }
            if (type == typeof(object))
            {
                return this.GetValue(i);
            }
            throw new InvalidOperationException(Exceptions.NotSupportedFieldType.With(type.FullName));
        }

        public object GetValue(int i)
        {
            return this.GetValueByIndex(i);
        }

        public XmlReader GetXmlReader(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            var value = this.GetValueByIndex(i);
            return value == null;
        }

        private object GetValueByIndex(int i)
        {
            if (_recordType == null || _reader.Current == null)
            {
                return null;
            }
            var property = _recordType.GetProperty(_properties[i].Name);
            if (property == null)
            {
                return null;
            }
            var value = property.GetValue(_reader.Current);
            return value;
        }

        public bool NextResult()
        {
            if (_index + 1 == _selectPattern.Expressions.Length)
            {
                return false;
            }
            ++_index;
            this._count = 0;
            this._expression = this._selectPattern.Expressions[_index];
            this._properties = this._expression.Columns.Select(c => ((PS.MemberColumnExpression)c).Member).ToArray();
            this._data = _resolver.Resolve(((PS.EngineService)this._expression.From).Name);
            this._reader = this._data.GetEnumerator();
            return true;
        }

        public bool Read()
        {
            var limit = _expression.Limit;
            if (limit != null 
                && limit.Type == DataModels.DataConstraint.LimitValueType.Records 
                && limit.Value > 0 
                && limit.Value == _count)
            {
                return false;
            }

            while (this._reader.MoveNext())
            {
                _recordType = _reader.Current?.GetType();
                if (_expression.Condition == null || this.CheckCondition(_expression.Condition))
                {
                    ++this._count;
                    return true;
                }
            }

            _recordType = null;
            return false;
        }

        private bool CheckCondition(PS.ConditionExpression condition)
        {
            switch (condition.Kind)
            {
                case PS.ConditionKind.Complex:
                    return CheckCondition(condition as PS.ComplexConditionExpression);
                case PS.ConditionKind.OneOperand:
                    return CheckCondition(condition as PS.OneOperandConditionExpression);
                case PS.ConditionKind.TwoOperand:
                    return CheckCondition(condition as PS.TwoOperandConditionExpression);
                case PS.ConditionKind.More:
                    return CheckCondition(condition as PS.MoreOperandsConditionExpression);
                default:
                    throw new InvalidOperationException($"Unsupported condition kind '{condition.Kind}'");
            }
        }

        private bool CheckCondition(PS.ComplexConditionExpression complexCondition)
        {
            for (int i = 0; i < complexCondition.Conditions.Length; i++)
            {
                var condition = complexCondition.Conditions[i];
                var result = this.CheckCondition(condition);

                if (complexCondition.Operator == PS.ConditionLogicalOperator.And && result == false)
                {
                    return false;
                }
                if (complexCondition.Operator == PS.ConditionLogicalOperator.Or && result == true)
                {
                    return true;
                }
            }
            if (complexCondition.Operator == PS.ConditionLogicalOperator.And)
            {
                return true;
            }
            if (complexCondition.Operator == PS.ConditionLogicalOperator.Or)
            {
                return false;
            }
            throw new InvalidOperationException($"Unsupported complex conditions operator '{complexCondition.Operator}'");
        }


        private bool CheckCondition(PS.OneOperandConditionExpression condition)
        {
            var value = this.GetOperandValue(condition.Operand);
            switch (condition.Operator)
            {
                case PS.OneOperandOperator.IsNull:
                    return value == null;
                case PS.OneOperandOperator.IsNotNull:
                    return value != null;
                default:
                    throw new InvalidOperationException($"Unsupported condition operator '{condition.Operator}'");
            }
        }

        private bool CheckCondition(PS.TwoOperandConditionExpression condition)
        {
            var left = this.GetOperandValue(condition.LeftOperand);
            var right = this.GetOperandValue(condition.RightOperand);
            switch (condition.Operator)
            {
                case PS.TwoOperandOperator.Equal:
                    return ApplyEqualOperator(left, right);
                case PS.TwoOperandOperator.GreaterEqual:
                    return ApplyEqualOperator(left, right);
                case PS.TwoOperandOperator.LessEqual:
                    return ApplyEqualOperator(left, right);
                case PS.TwoOperandOperator.NotEqual:
                    return !ApplyEqualOperator(left, right);
                case PS.TwoOperandOperator.GreaterThan:
                case PS.TwoOperandOperator.LessThan:
                case PS.TwoOperandOperator.Like:
                case PS.TwoOperandOperator.NotLike:
                case PS.TwoOperandOperator.BeginWith:
                case PS.TwoOperandOperator.EndWith:
                case PS.TwoOperandOperator.Contains:
                case PS.TwoOperandOperator.NotBeginWith:
                case PS.TwoOperandOperator.NotEndWith:
                case PS.TwoOperandOperator.NotContains:
                default:
                    throw new InvalidOperationException($"Unsupported condition operator '{condition.Operator}'");
            }
        }
        private bool CheckCondition(PS.MoreOperandsConditionExpression condition)
        {
            switch (condition.Operator)
            {
                case PS.MoreOperandsOperator.In:
                case PS.MoreOperandsOperator.NotIn:
                case PS.MoreOperandsOperator.Between:
                case PS.MoreOperandsOperator.NotBetween:
                default:
                    throw new InvalidOperationException($"Unsupported condition operator '{condition.Operator}'");
            }
        }

        private object GetOperandValue(PS.OperandExpression operand)
        {
            switch (operand.Kind)
            {
                case PS.OperandKind.Member:
                    var operandAsMember = operand as PS.MemberOperandExpression;
                    var ordinal = this.GetOrdinalByAlias(operandAsMember.Member.Name);
                    return this.GetValueByIndex(ordinal);
                case PS.OperandKind.Value:
                    var operandAsValue = operand as PS.ValueOperandExpression;
                    if (operandAsValue.Expression.Kind == PS.ValueExpressionKind.Constant)
                    {
                        return (operandAsValue.Expression as PS.ConstantValueExpression).Value;
                    }
                    throw new InvalidOperationException($"Can not use value expression this context");
                default:
                    throw new InvalidOperationException($"Can not use value expression this context");
            }
        }

        private bool ApplyEqualOperator(object left, object right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            if (left == null && right != null)
            {
                return false;
            }
            if (left != null && right == null)
            {
                return false;
            }

            if (left == right)
            {
                return true;
            }

            if (left.Equals(right))
            {
                return true;
            }
            if (right.Equals(left))
            {
                return true;
            }

            var leftType = left.GetType();
            var rightType = right.GetType();

            if (leftType.IsEnum && rightType == typeof(int))
            {
                return ((int)left) == (int)right;
            }
            if (leftType.IsEnum && rightType == typeof(long))
            {
                return ((long)left) == (long)right;
            }
            if (leftType.IsEnum && rightType == typeof(short))
            {
                return ((short)left) == (short)right;
            }
            if (leftType.IsEnum && rightType == typeof(byte))
            {
                return ((byte)left) == (byte)right;
            }
            if (leftType.IsEnum && rightType == typeof(string))
            {
                return left.ToString() == (string)right;
            }

            if (rightType.IsEnum && leftType == typeof(int))
            {
                return ((int)left) == (int)right;
            }
            if (rightType.IsEnum && leftType == typeof(long))
            {
                return ((long)left) == (long)right;
            }
            if (rightType.IsEnum && leftType == typeof(short))
            {
                return ((short)left) == (short)right;
            }
            if (rightType.IsEnum && leftType == typeof(byte))
            {
                return ((byte)left) == (byte)right;
            }
            if (rightType.IsEnum && leftType == typeof(string))
            {
                return right.ToString() == (string)left;
            }

            return false;
        }
    }
}
