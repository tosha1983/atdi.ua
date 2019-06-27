using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.SqlServer
{
    class Mapper
    {
        private readonly Dictionary<string, string> _mapByPath;
        private readonly Dictionary<string, string> _mapByAlias;
        public Mapper()
        {
            this._mapByAlias = new Dictionary<string, string>();
            this._mapByPath = new Dictionary<string, string>();
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
    class BuildingContex
    {
        private readonly CommandBuilder _builder;
        private readonly Dictionary<string, EngineCommandParameter> _parametersByMember;
        private readonly Dictionary<string, EngineCommandParameter> _parametersByName;
        private readonly Mapper _mapper;

        private int _iterationIndex;
        private int _iterationCounter;

        public BuildingContex()
        {
            this._builder = new CommandBuilder();
            this._parametersByMember = new Dictionary<string, EngineCommandParameter>();
            this._parametersByName = new Dictionary<string, EngineCommandParameter>();
            this._mapper = new Mapper();
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

        public EngineCommandParameter CreateParameter(string alias, string memberName, DataType dataType, EngineParameterDirection direction)
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

        public Mapper Mapper => _mapper;
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
        public void GenerateNextValue(EngineCommandParameter parameter)
        {
            switch (parameter.DataType)
            {
                case DataModels.DataType.Long:
                    _sql.AppendLine($"SET @{parameter.Name} = @@IDENTITY;");
                    break;
                case DataModels.DataType.Integer:
                    _sql.AppendLine($"SET @{parameter.Name} = @@IDENTITY;");
                    break;
                case DataModels.DataType.Short:
                    _sql.AppendLine($"SET @{parameter.Name} = @@IDENTITY;");
                    break;
                case DataModels.DataType.Guid:
                    _sql.AppendLine($"SET @{parameter.Name} = NEWID();");
                    break;
                case DataModels.DataType.DateTime:
                    _sql.AppendLine($"SET @{parameter.Name} = GETDATE();");
                    break;
                case DataModels.DataType.DateTimeOffset:
                    _sql.AppendLine($"SET @{parameter.Name} = SYSDATETIMEOFFSET();");
                    break;
                case DataModels.DataType.Time:
                    _sql.AppendLine($"SET @{parameter.Name} = GETDATE();");
                    break;
                case DataModels.DataType.Date:
                    _sql.AppendLine($"SET @{parameter.Name} = GETDATE();");
                    break;
                
                default:
                    throw new InvalidOperationException($"Unsupported type to generate next value {parameter.DataType}");
            }
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
                    _sql.AppendLine($"SET @{parameter.Name} = '';");
                    break;
                case DataModels.DataType.Boolean:
                    _sql.AppendLine($"SET @{parameter.Name} = 1;");
                    break;
                case DataModels.DataType.Integer:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.DateTime:
                    _sql.AppendLine($"SET @{parameter.Name} = GETDATE();");
                    break;
                case DataModels.DataType.DateTimeOffset:
                    _sql.AppendLine($"SET @{parameter.Name} = SYSDATETIMEOFFSET();");
                    break;
                case DataModels.DataType.Double:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.Float:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.Decimal:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.Byte:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.Bytes:
                    _sql.AppendLine($"SET @{parameter.Name} = 0x00;");
                    break;
                case DataModels.DataType.Guid:
                    _sql.AppendLine($"SET @{parameter.Name} = NEWID();");
                    break;
                case DataModels.DataType.Time:
                    _sql.AppendLine($"SET @{parameter.Name} = GETDATE();");
                    break;
                case DataModels.DataType.Date:
                    _sql.AppendLine($"SET @{parameter.Name} = GETDATE();");
                    break;
                case DataModels.DataType.Long:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.Short:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.Char:
                    _sql.AppendLine($"SET @{parameter.Name} = '';");
                    break;
                case DataModels.DataType.SignedByte:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.UnsignedShort:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.UnsignedInteger:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
                    break;
                case DataModels.DataType.UnsignedLong:
                    _sql.AppendLine($"SET @{parameter.Name} = 0;");
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
            var fieldsClause = string.Join(", ", fields.Select(f => $"[{f}]").ToArray());
            _sql.AppendLine($"INSERT INTO [{schema}].[{source}]({fieldsClause})");

            var valuesClause = string.Join(", ", values.Select(v => $"@{v.Name}").ToArray());
            _sql.AppendLine($"VALUES({valuesClause});");
        }

        public void SelectIdentity(EngineCommandParameter idenityParameter)
        {
            _sql.AppendLine($"SET @{idenityParameter.Name} = @@IDENTITY;");
        }

        public string BuildComandText()
        {
            return _sql.ToString();
        }
    }
}
