using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.Oracle
{

    class Mapper
    {
        private readonly Dictionary<string, string> _mapByPath;
        private readonly Dictionary<string, string> _mapByAlias;
        private readonly string _prefix;
        private int _counter;

        public Mapper(string prefix)
        {
            this._prefix = prefix;
            this._mapByAlias = new Dictionary<string, string>();
            this._mapByPath = new Dictionary<string, string>();
            this._counter = 0;
        }

        public string EnsureAlias(string path)
        {
            if (!_mapByPath.TryGetValue(path, out string alias))
            {
                alias = $"{_prefix}{++_counter}";
                this.Append(alias, path);
            }
            return alias;
        }



        public void Append(string alias, string path)
        {
            this._mapByPath[path] = alias;
            this._mapByAlias[alias] = path;
        }

        public string GetAlias(string path)
        {
            return this._mapByPath[path];
        }
        public string GetPath(string alias)
        {
            return this._mapByAlias[alias];
        }

    }
    class OracleBuildingContex
    {
        private readonly CommandBuilder _builder;
        private readonly Dictionary<string, EngineCommandParameter> _parametersByMember;
        private readonly Dictionary<string, EngineCommandParameter> _parametersByName;
        private readonly Mapper _columnMapper;
        private readonly Mapper _sourceMapper;

        private int _iterationIndex;
        private int _iterationCounter;

        public OracleBuildingContex()
        {
            this._builder = new CommandBuilder();
            this._parametersByMember = new Dictionary<string, EngineCommandParameter>();
            this._parametersByName = new Dictionary<string, EngineCommandParameter>();
            this._columnMapper = new Mapper("COL_");
            this._sourceMapper = new Mapper("SRC_");
            this._iterationIndex = 0;
            this._iterationCounter = 0;
            this.NextIteration();
        }

        public CommandBuilder Builder => this._builder;

        public void NextIteration()
        {
            ++this._iterationIndex;
            this._iterationCounter = 0;
            this._builder.StartIteration(this._iterationIndex);
        }

        public EngineCommandParameter CreateParameter(string alias, string memberName, DataModels.DataType dataType, EngineParameterDirection direction, string prefixRefCursor = null)
        {
            ++this._iterationCounter;
            var parameter = new EngineCommandParameter()
            {
                Name = prefixRefCursor ==null ? $"P_I{this._iterationIndex}_C{this._iterationCounter}" : $"P_I{this._iterationIndex}_C{this._iterationCounter}_CURSOR_{prefixRefCursor}",
                DataType = dataType,
                Direction = direction
            };
            var key = BuildKey(alias, memberName);
            _parametersByMember.Add(key, parameter);
            _parametersByName.Add(parameter.Name, parameter);
            return parameter;
        }

        public EngineCommandParameter CreateParameter(object value, DataModels.DataType dataType, EngineParameterDirection direction, string prefixRefCursor = null)
        {
            ++this._iterationCounter;
            var parameter = new EngineCommandParameter()
            {
                Name = prefixRefCursor == null ? $"P_I{this._iterationIndex}_C{this._iterationCounter}" : $"P_I{this._iterationIndex}_C{this._iterationCounter}_CURSOR_{prefixRefCursor}",
                DataType = dataType,
                Direction = direction
            };
            parameter.SetValue(value);
            _parametersByName.Add(parameter.Name, parameter);
            return parameter;
        }

        public EngineCommandParameter GetParameter(string alias, string memberName)
        {
            var key = BuildKey(alias, memberName);
            return _parametersByMember[key];
        }

        private static string BuildKey(string alias, string memberName)
        {
            return $"{alias}=>{memberName}";
        }

        public EngineCommand BuildCommand()
        {
            var command = new EngineCommand(this._parametersByName)
            {
                Text = _builder.BuildComandText()
            };

            return command;
        }

        public Mapper Mapper => _columnMapper;

        public string EnsureColumnAlias(MemberColumnExpression memberColumn)
        {
            return _columnMapper.EnsureAlias(memberColumn.Member.Property);
        }

        public string EnsureSourceAlias(TargetObject target)
        {
            return _sourceMapper.EnsureAlias(target.Alias);
        }

        public string BuildFromExpression(TargetObject target, out string sourceAliasName)
        {
            if (target is EngineObject fromSource)
            {
                var sourceAlias = this.EnsureSourceAlias(fromSource);
                sourceAliasName = sourceAlias;
                var sqlFrom = this.Builder.CreateSourceObject(fromSource.Schema, fromSource.Name, sourceAlias);
                return sqlFrom;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported source object type {target.GetType().FullName}");
            }
        }

        public string[] BuildJoinExpression(JoinExpression joinExpression)
        {
            var sqlJoinExpression = new string[2];
            if (joinExpression.Target is EngineObject joinTarget)
            {
                var sourceAlias = this.EnsureSourceAlias(joinTarget);
                var sqlConditions = this.BuildConditionExpression(joinExpression.Condition);

                sqlJoinExpression[0] = this.Builder.CreateJoinSourceObject(joinExpression.Operation, joinTarget.Schema, joinTarget.Name, sourceAlias, $"Key: <{joinTarget.Alias}>");
                sqlJoinExpression[1] = this.Builder.CreateJoinConditionExpression(sqlConditions);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported target object type '{joinExpression.Target.GetType().FullName}'");
            }
            return sqlJoinExpression;
        }

        public string BuildConditionExpression(ConditionExpression condition)
        {
            switch (condition.Kind)
            {
                case ConditionKind.Complex:
                    return this.BuildConditionExpression(condition as ComplexConditionExpression);
                case ConditionKind.OneOperand:
                    return this.BuildConditionExpression(condition as OneOperandConditionExpression);
                case ConditionKind.TwoOperand:
                    return this.BuildConditionExpression(condition as TwoOperandConditionExpression);
                case ConditionKind.More:
                    return this.BuildConditionExpression(condition as MoreOperandsConditionExpression);
                default:
                    throw new InvalidOperationException($"Unsupported condition kind '{condition.Kind}'");
            }
        }

        private string BuildConditionExpression(ComplexConditionExpression condition)
        {
            var sqlConditions = new string[condition.Conditions.Length];
            var before = sqlConditions.Length == 1 ? "" : "(";
            var after = sqlConditions.Length == 1 ? "" : ")";
            for (int i = 0; i < condition.Conditions.Length; i++)
            {
                sqlConditions[i] = before + this.BuildConditionExpression(condition.Conditions[i]) + after;
            }
            var sql = this.Builder.CreateConditionsExpression(condition.Operator, sqlConditions);
            return sql;
        }

        private string BuildConditionExpression(OneOperandConditionExpression condition)
        {
            var sqlOperand = this.BuildOperandExpression(condition.Operand);
            return this.Builder.CreateOneOperandExpression(condition.Operator, sqlOperand);
        }

        private string BuildConditionExpression(TwoOperandConditionExpression condition)
        {
            var sqlLeftOperand = this.BuildOperandExpression(condition.LeftOperand);
            var sqlRightOperand = this.BuildOperandExpression(condition.RightOperand);
            return this.Builder.CreateTwoOperandExpression(condition.Operator, sqlLeftOperand, sqlRightOperand);
        }

        private string BuildConditionExpression(MoreOperandsConditionExpression condition)
        {
            var sqlTestOperand = this.BuildOperandExpression(condition.Test);
            var sqlOperands = new string[condition.Operands.Length];
            for (int i = 0; i < condition.Operands.Length; i++)
            {
                sqlOperands[i] = this.BuildOperandExpression(condition.Operands[i]);
            }
            return this.Builder.CreateMoreOperandsExpression(condition.Operator, sqlTestOperand, sqlOperands);
        }

        private string BuildOperandExpression(OperandExpression operand)
        {
            switch (operand.Kind)
            {
                case OperandKind.Member:
                    return this.BuildOperandExpression(operand as MemberOperandExpression);
                case OperandKind.Value:
                    return this.BuildOperandExpression(operand as ValueOperandExpression);
                default:
                    throw new InvalidOperationException($"Unsupported operans kind '{operand.Kind}'");
            }
        }

        private string BuildOperandExpression(MemberOperandExpression operand)
        {
            string sourceAlias = this.EnsureSourceAlias(operand.Member.Owner);
            return this.Builder.CreateMemberOperandExpression(sourceAlias, operand.Member.Name);
        }

        private string BuildOperandExpression(ValueOperandExpression operand)
        {
            switch (operand.Expression.Kind)
            {
                case ValueExpressionKind.Constant:
                    return this.BuildOperandValueExpression(operand.Expression as ConstantValueExpression);
                case ValueExpressionKind.Reference:
                    return this.BuildOperandValueExpression(operand.Expression as ReferenceValueExpression);
                case ValueExpressionKind.Generated:
                    return this.BuildOperandValueExpression(operand.Expression as GeneratedValueExpression);
                default:
                    throw new InvalidOperationException($"Unsupported value operand expression kind '{operand.Expression.Kind}'");
            }
        }

        private string BuildOperandValueExpression(ConstantValueExpression operandValue)
        {
            var parameter = this.CreateParameter(operandValue.Value, operandValue.DataType, EngineParameterDirection.Input);
            return this.Builder.CreateParameterExpression(parameter.Name);
        }

        private string BuildOperandValueExpression(ReferenceValueExpression referenceValue)
        {
            var parameter = this.GetParameter(referenceValue.Member.Owner.Alias, referenceValue.Member.Name);
            return this.Builder.CreateParameterExpression(parameter.Name);
        }

        private string BuildOperandValueExpression(GeneratedValueExpression value)
        {
            throw new NotImplementedException();
        }
    }

    class CommandBuilder
    {
        private readonly StringBuilder _sql;

        public CommandBuilder()
        {
            this._sql = new StringBuilder();
        }

        public void StartIteration(int iteration)
        {
            _sql.AppendLine($"/* Iterration: #{iteration} */");
            _sql.AppendLine();
        }

        public void ExpresaionAlias(int index, string alias)
        {
            _sql.AppendLine($"/* Expression: index = #{index}, alias = '{alias}' */");
        }

        /// <summary>
        /// Автогенерация значения
        /// В зависимости от типа данных применяем разнгые функции
        ///  - для целого числа GetID
        ///  - для GUID - SYS_GUID()
        ///  - для даты - SYSDATE
        /// </summary>
        /// <param name="parameter"></param>
        public void GenerateNextValue(EngineCommandParameter parameter, EngineObject engineObject = null)
        {
            switch (parameter.DataType)
            {
                case DataModels.DataType.Long:
                    _sql.AppendLine($"SELECT {engineObject.Schema}.GetID('{engineObject.Name}') INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Integer:
                    _sql.AppendLine($"SELECT {engineObject.Schema}.GetID('{engineObject.Name}') INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Short:
                    _sql.AppendLine($"SELECT {engineObject.Schema}.GetID('{engineObject.Name}') INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Guid:
                    _sql.AppendLine($"SELECT SYS_GUID() INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.DateTime:
                    _sql.AppendLine($"SELECT SYSDATE INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.DateTimeOffset:
                    _sql.AppendLine($"SELECT SYSTIMESTAMP INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Time:
                    _sql.AppendLine($"SELECT SYSDATE INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Date:
                    _sql.AppendLine($"SELECT SYSDATE INTO :{parameter.Name} FROM DUAL;");
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported type to generate next value {parameter.DataType}");
            }
        }

        public string GetSortingDirection(SortingDirection direction)
        {
            switch (direction)
            {
                case SortingDirection.Ascending:
                    return "ASC";
                case SortingDirection.Descending:
                    return "DESC";
                default:
                    throw new InvalidOperationException($"Unsupported sorting direction '{direction}'");
            }
        }

        public string CreateSortingClause(SortingDirection direction, string sourceAlias, string sourceName)
        {
            return $"{sourceAlias}.{sourceName} {this.GetSortingDirection(direction)}";
        }

        public void SetBegin()
        {
            _sql.AppendLine($"BEGIN");
        }

        public void SetEnd()
        {
            _sql.AppendLine($"END;");
        }

        /// <summary>
        /// Автогенерация значения
        /// В зависимости от типа данных применяем разнгые функции
        ///  - для целого числа GetID
        ///  - для GUID - SYS_GUID()
        ///  - для даты - SYSDATE
        /// </summary>
        /// <param name="parameter"></param>
        public void SetDefault(EngineCommandParameter parameter)
        {
            switch (parameter.DataType)
            {
                case DataModels.DataType.String:
                    _sql.AppendLine($" :{parameter.Name} := '';");
                    break;
                case DataModels.DataType.Boolean:
                    _sql.AppendLine($" :{parameter.Name} := 1;");
                    break;
                case DataModels.DataType.Integer:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.DateTime:
                    _sql.AppendLine($" :{parameter.Name} := SYSDATE;");
                    break;
                case DataModels.DataType.DateTimeOffset:
                    _sql.AppendLine($" :{parameter.Name} := SYSTIMESTAMP;");
                    break;
                case DataModels.DataType.Double:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.Float:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.Decimal:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.Byte:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.Bytes:
                    _sql.AppendLine($" :{parameter.Name} := 0x00;");
                    break;
                case DataModels.DataType.Guid:
                    _sql.AppendLine($" :{parameter.Name} := SYS_GUID();");
                    break;
                case DataModels.DataType.Time:
                    _sql.AppendLine($" :{parameter.Name} := SYSDATE;");
                    break;
                case DataModels.DataType.Date:
                    _sql.AppendLine($" :{parameter.Name} := SYSDATE;");
                    break;
                case DataModels.DataType.Long:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.Short:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.Char:
                    _sql.AppendLine($" :{parameter.Name} := '';");
                    break;
                case DataModels.DataType.SignedByte:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.UnsignedShort:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.UnsignedInteger:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.UnsignedLong:
                    _sql.AppendLine($" :{parameter.Name} := 0;");
                    break;
                case DataModels.DataType.ClrType:
                case DataModels.DataType.ClrEnum:
                case DataModels.DataType.Xml:
                case DataModels.DataType.Json:
                case DataModels.DataType.Chars:
                case DataModels.DataType.Undefined:
                default:
                    break;
            }
        }

        public string CreateParameterExpression(string name)
        {
            return ":" + name;
        }

        public void Insert(string schema, string source, string[] fields, EngineCommandParameter[] values)
        {
            var fieldsClause = string.Join(", ", fields.Select(f => $"\"{f}\"").ToArray());
            _sql.AppendLine($"INSERT INTO {schema}.{source}({fieldsClause})");

            var valuesClause = string.Join(", ", values.Select(v => $":{v.Name}").ToArray());
            _sql.AppendLine($"VALUES({valuesClause});");
        }

        public void Update(string schema, string source, string[] setClauses, string from, string[][] joins = null, string where = null, string sourceAliasName = null)
        {

            _sql.AppendLine($"UPDATE {schema}.{source} SET");

            var valuesClause = "    " + string.Join("," + Environment.NewLine + "    ", setClauses);
            _sql.AppendLine(valuesClause);

            _sql.AppendLine($"WHERE rowid in (");

            _sql.AppendLine($"SELECT {sourceAliasName}.rowid FROM {from}");

            if (joins != null && joins.Length > 0)
            {
                for (int i = 0; i < joins.Length; i++)
                {
                    _sql.AppendLine("    " + joins[i][0]);
                    _sql.AppendLine("        " + joins[i][1]);
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                _sql.AppendLine($"WHERE");
                _sql.AppendLine($"    {where}");
            }
            _sql.AppendLine(")");
        }

        public void Delete(string schema, string source, string from, string[][] joins = null, string where = null, string sourceAliasName = null)
        {
            _sql.AppendLine($"DELETE FROM {from}");
            _sql.AppendLine($"WHERE rowid in (");
            _sql.AppendLine($"SELECT {sourceAliasName}.rowid FROM {from}");
            if (joins != null && joins.Length > 0)
            {
                for (int i = 0; i < joins.Length; i++)
                {
                    _sql.AppendLine("    " + joins[i][0]);
                    _sql.AppendLine("        " + joins[i][1]);
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                _sql.AppendLine($"WHERE");
                _sql.AppendLine($"    {where}");
            }
            _sql.AppendLine(")");
        }

        public string CreateSetClause(string alias, string name, string valueExpression)
        {
            return $"\"{name}\" = {valueExpression}";
        }

        public string CreateDistinct()
        {
            return $"DISTINCT";
        }

        public void Select(string[] columns, string from, string[][] joins = null, string where = null, string[] orderBy = null, string distinct = null, string limit = null)
        {
            if (string.IsNullOrEmpty(distinct))
            {
                _sql.AppendLine($"SELECT");
            }
            else
            {
                _sql.AppendLine($"SELECT {distinct}");
            }

            //if (!string.IsNullOrEmpty(limit))
            //{
                //_sql.AppendLine($"    {limit}");
            //}
            _sql.AppendLine(string.Join("," + Environment.NewLine, columns));

            this.FromWhere(from, joins, where, limit);

            if (orderBy != null && orderBy.Length > 0)
            {
                _sql.AppendLine($"ORDER BY");
                _sql.AppendLine($"    {string.Join(", ", orderBy)}");
            }
        }


        private void FromWhere(string from, string[][] joins, string where, string limit = null)
        {
            _sql.AppendLine($"FROM {from}");
            if (joins != null && joins.Length > 0)
            {
                for (int i = 0; i < joins.Length; i++)
                {
                    _sql.AppendLine("    " + joins[i][0]);
                    _sql.AppendLine("        " + joins[i][1]);
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                _sql.AppendLine($"WHERE");
                _sql.AppendLine($"    {where}");

                if (!string.IsNullOrEmpty(limit))
                {
                    _sql.AppendLine($"    {limit}");
                }
            }
        }

        public string CreateSelectColumn(string sourceAlias, string memberName, string columnAlias)
        {
            return $"    {sourceAlias}.\"{memberName}\"  {columnAlias}";
        }
        public string CreateSourceObject(string schema, string source, string alias)
        {
            return $"{schema}.\"{source}\" {alias}";
        }

        public string CreateTwoOperandExpression(TwoOperandOperator logOperator, string leftOperand, string rightOperand)
        {
            switch (logOperator)
            {
                case TwoOperandOperator.Equal:
                    return $"{leftOperand} = {rightOperand}";
                case TwoOperandOperator.GreaterEqual:
                    return $"{leftOperand} >= {rightOperand}";
                case TwoOperandOperator.GreaterThan:
                    return $"{leftOperand} > {rightOperand}";
                case TwoOperandOperator.LessEqual:
                    return $"{leftOperand} <= {rightOperand}";
                case TwoOperandOperator.LessThan:
                    return $"{leftOperand} < {rightOperand}";
                case TwoOperandOperator.NotEqual:
                    return $" ({leftOperand} <> {rightOperand}) OR ({leftOperand} IS  NULL)";
                case TwoOperandOperator.Like:
                    return $"{leftOperand} LIKE {rightOperand}";
                case TwoOperandOperator.NotLike:
                    return $"{leftOperand} NOT LIKE {rightOperand}";
                case TwoOperandOperator.BeginWith:
                    return $"{leftOperand} LIKE ({rightOperand} + '%')";
                case TwoOperandOperator.EndWith:
                    return $"{leftOperand} LIKE ('%' + {rightOperand})";
                case TwoOperandOperator.Contains:
                    return $"{leftOperand} LIKE ('%' + {rightOperand} + '%')";
                case TwoOperandOperator.NotBeginWith:
                    return $"{leftOperand} NOT LIKE ({rightOperand} + '%')";
                case TwoOperandOperator.NotEndWith:
                    return $"{leftOperand} NOT LIKE ('%' + {rightOperand})";
                case TwoOperandOperator.NotContains:
                    return $"{leftOperand} NOT LIKE ('%' + {rightOperand} + '%')";
                default:
                    throw new InvalidOperationException($"Unsupported two operand operator '{logOperator}'");
            }
        }

        public string CreateMemberOperandExpression(string sourceAlias, string sourceName)
        {
            return $"{sourceAlias}.{sourceName}";
        }

        public string CreateMoreOperandsExpression(MoreOperandsOperator logOperator, string testOperand, string[] operands)
        {
            if (operands == null || operands.Length == 0)
            {
                throw new ArgumentNullException(nameof(operands));
            }

            switch (logOperator)
            {
                case MoreOperandsOperator.In:
                    if (operands == null || operands.Length < 1)
                    {
                        throw new InvalidOperationException($"Invalid numer of operand for IN SQL Expression");
                    }
                    return $"{testOperand} IN ({string.Join(", ", operands)})";
                case MoreOperandsOperator.NotIn:
                    if (operands == null || operands.Length < 1)
                    {
                        throw new InvalidOperationException($"Invalid numer of operand for NOT IN SQL Expression");
                    }
                    return $"{testOperand} NOT IN ({string.Join(", ", operands)})";
                case MoreOperandsOperator.Between:
                    if (operands == null || operands.Length != 2)
                    {
                        throw new InvalidOperationException($"Invalid numer of operand for BETWEEN SQL Expression");
                    }
                    return $"{testOperand} BETWEEN {operands[0]} AND {operands[1]}";
                case MoreOperandsOperator.NotBetween:
                    if (operands == null || operands.Length != 2)
                    {
                        throw new InvalidOperationException($"Invalid numer of operand for BETWEEN SQL Expression");
                    }
                    return $"{testOperand} NOT BETWEEN {operands[0]} AND {operands[1]}";
                default:
                    throw new InvalidOperationException($"Unsupported more operands operator '{logOperator}'");
            }
        }


        public string CreateConditionsExpression(ConditionLogicalOperator joinOperator, string[] conditions)
        {
            return string.Join(" " + this.GetJoinOperator(joinOperator) + " ", conditions);
        }

        public string GetJoinOperator(ConditionLogicalOperator joinOperator)
        {
            return joinOperator == ConditionLogicalOperator.And ? "AND" : "OR";
        }

        public string CreateJoinSourceObject(JoinOperationType joinOperation, string schema, string source, string alias, string comment)
        {
            var joinKind = string.Empty;
            switch (joinOperation)
            {
                case JoinOperationType.Inner:
                    joinKind = "INNER JOIN";
                    break;
                case JoinOperationType.Left:
                    joinKind = "LEFT JOIN";
                    break;
                case JoinOperationType.Right:
                    joinKind = "RIGHT JOIN";
                    break;
                case JoinOperationType.Full:
                    joinKind = "FULL JOIN";
                    break;
                case JoinOperationType.Cross:
                    joinKind = "CROSS JOIN";
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(comment))
            {
                return $"{joinKind} {schema}.{source} {alias}";
            }
            return $"{joinKind} {schema}.{source} {alias} -- {comment}";
        }

        public string CreateJoinConditionExpression(string conditions)
        {
            return $"ON ({conditions})";
        }


        public string CreateOneOperandExpression(OneOperandOperator logOperator, string operand)
        {
            switch (logOperator)
            {
                case OneOperandOperator.IsNull:
                    return $"{operand} IS NULL";
                case OneOperandOperator.IsNotNull:
                    return $"{operand} IS NOT NULL";
                default:
                    throw new InvalidOperationException($"Unsupported one operand operator '{logOperator}'");
            }
        }

        public void OpenCursor(string schema, string cursorName, string source, Dictionary<string, string> fields)
        {
            var fieldsName = string.Join(", ", fields.Select(f => $"\"{f.Value}\"").ToArray());
            var fieldsValues = string.Join("AND ", fields.Select(f => $"\"{f.Value.ToString()}\" = :{f.Key.ToString()}").ToArray());
            _sql.AppendLine($" OPEN :{cursorName} FOR SELECT {fieldsName} FROM {schema}.{source} WHERE {fieldsValues}; ");
        }

        public string CreateCountLimt(long count)
        {
            return $" AND ROWNUM<= {count}";
        }

        public string CreatePercentLimt(long percent)
        {
            throw new NotImplementedException();
        }

        public string BuildComandText()
        {
            return _sql.ToString();
        }
    }
}