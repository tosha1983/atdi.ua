using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.Oracle
{
    class OracleBuildingContex
    {
        private readonly CommandBuilder _builder;
        private readonly Dictionary<string, EngineCommandParameter> _parametersByMember;
        private readonly Dictionary<string, EngineCommandParameter> _parametersByName;
        private int _iterationIndex;
        private int _iterationCounter;

        public OracleBuildingContex()
        {
            this._builder = new CommandBuilder();
            this._parametersByMember = new Dictionary<string, EngineCommandParameter>();
            this._parametersByName = new Dictionary<string, EngineCommandParameter>();
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

        public EngineCommandParameter CreateParameter(string alias, string memberName, DataModels.DataType dataType, EngineParameterDirection direction)
        {
            ++this._iterationCounter;
            var parameter = new EngineCommandParameter()
            {
                Name = $"P_I{this._iterationIndex}_C{this._iterationCounter}",
                DataType = dataType,
                Direction = direction
            };
            var key = BuildKey(alias, memberName);
            _parametersByMember.Add(key, parameter);
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
        ///  - для целого числа @@IDENTITY
        ///  - для GUID - NEWID()
        ///  - для даты - GETDATE()
        /// </summary>
        /// <param name="parameter"></param>
        public void GenerateNextValue(EngineCommandParameter parameter, EngineObject engineObject)
        {
            switch (parameter.DataType)
            {
                case DataModels.DataType.Long:
                    _sql.AppendLine($"SELECT GetID('{engineObject.Name}') INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Integer:
                    _sql.AppendLine($"SELECT GetID('{engineObject.Name}') INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Short:
                    _sql.AppendLine($"SELECT GetID('{engineObject.Name}') INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Guid:
                    _sql.AppendLine($"SELECT SYS_GUID() INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.DateTime:
                    _sql.AppendLine($"SELECT GETDATE() INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.DateTimeOffset:
                    _sql.AppendLine($"SELECT SYSDATETIMEOFFSET() INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Time:
                    _sql.AppendLine($"SELECT GETDATE() INTO :{parameter.Name} FROM DUAL;");
                    break;
                case DataModels.DataType.Date:
                    _sql.AppendLine($"SELECT GETDATE() INTO :{parameter.Name} FROM DUAL;");
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported type to generate next value {parameter.DataType}");
            }
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
        ///  - для целого числа @@IDENTITY
        ///  - для GUID - NEWID()
        ///  - для даты - GETDATE()
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
                    _sql.AppendLine($" :{parameter.Name} := GETDATE();");
                    break;
                case DataModels.DataType.DateTimeOffset:
                    _sql.AppendLine($" :{parameter.Name} := SYSDATETIMEOFFSET();");
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
                    _sql.AppendLine($" :{parameter.Name} := GETDATE();");
                    break;
                case DataModels.DataType.Date:
                    _sql.AppendLine($" :{parameter.Name} := GETDATE();");
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
                    _sql.AppendLine($" :{parameter.Name} = 0;");
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

        public void Insert(string schema, string source, string[] fields, EngineCommandParameter[] values)
        {
            var fieldsClause = string.Join(", ", fields.Select(f => $"{f}").ToArray());
            _sql.AppendLine($"INSERT INTO {schema}.{source}({fieldsClause})");

            var valuesClause = string.Join(", ", values.Select(v => $":{v.Name}").ToArray());
            _sql.AppendLine($"VALUES({valuesClause});");
        }

        public void ForScalar(string schema, string cursorName, string source, Dictionary<string, string> fields,  EngineExecutionResultKind engineExecutionResultKind)
        {
            if (engineExecutionResultKind == EngineExecutionResultKind.Scalar)
            {
                var fieldsName = string.Join(", ", fields.Select(f => $"{f.Value}").ToArray());
                var fieldsValues = string.Join("AND ", fields.Select(f => $"{f.Value.ToString()} = :{f.Key.ToString()}").ToArray());
                _sql.AppendLine($" OPEN :{cursorName} FOR SELECT {fieldsName} FROM {schema}.{source} WHERE {fieldsValues}; ");
            }
        }

        public string BuildComandText()
        {
            return _sql.ToString();
        }
    }
}