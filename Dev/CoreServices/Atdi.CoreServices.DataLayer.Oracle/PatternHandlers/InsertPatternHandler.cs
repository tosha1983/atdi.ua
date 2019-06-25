using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.Oracle.PatternHandlers
{
    class OracleInsertPatternHandler : LoggedObject, IOracleQueryPatternHandler
    {
        public OracleInsertPatternHandler(ILogger logger) : base(logger)
        {
        }

        public void Handle<TPattern>(TPattern queryPattern, OracleExecuter executer)
            where TPattern : class, IEngineQueryPattern
        {
            var pattern = queryPattern as PS.InsertPattern;

            var command = BuildCommand(pattern);

            switch (pattern.Result.Kind)
            {
                case EngineExecutionResultKind.None:
                    executer.ExecuteNonQuery(command);
                    return;
                case EngineExecutionResultKind.RowsAffected:
                    pattern.AsResult<EngineExecutionRowsAffectedResult>()
                        .RowsAffected = executer.ExecuteNonQuery(command);
                    return;
                case EngineExecutionResultKind.Scalar:
                    // возврат первичного ключа через исходящие параметры
                    var result = pattern.AsResult<EngineExecutionScalarResult>();
                    executer.ExecuteScalar(command);
                    result.Value = this.DecodePrimaryKey(pattern, command);
                    return;
                case EngineExecutionResultKind.Reader:
                    executer.ExecuteReader(command, sqlReader =>
                    {
                        if (pattern.Result is EngineExecutionReaderResult<System.Data.IDataReader> result1)
                        {
                            result1.Handler(sqlReader);
                        }
                        if (pattern.Result is EngineExecutionReaderResult<IEngineDataReader> result2)
                        {
                            result2.Handler(new OrcaleEngineDataReader(sqlReader));
                        }
                    });
                    return;
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result type '{pattern.Result.Kind}'");
            }
        }

        private object DecodePrimaryKey(PS.InsertPattern pattern, EngineCommand command)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private EngineCommand BuildCommand(PS.InsertPattern pattern)
        {
            var builder = new OracleCommandBuilder();

            builder.StartIteration();

            var allParameters = new Dictionary<string, EngineCommandParameter>();

            for (int i = 0; i < pattern.Expressions.Length; i++)
            {
                var expression = pattern.Expressions[i];
                builder.ExpresaionAlias(i, expression.Alias);

                var fields = new List<string>();
                var values = new List<EngineCommandParameter>();

                for (int j = 0; j < expression.Values.Length; j++)
                {
                    var value = expression.Values[j];
                    fields.Add(value.Property.Name);
                    if (value.Expression is PS.ConstantValueExpression constantValue)
                    {
                        var parameter = new EngineCommandParameter
                        {
                            DataType = value.Property.DataType,
                            Direction = EngineParameterDirection.Input,
                            Value = constantValue.Value,
                            Name = value.Property.Name
                        };
                        values.Add(parameter);
                    }
                    //if (value.Expression is PS.ReferenceValueExpression referenceValue)
                    //{
                    //    referenceValue.Value.Owner.
                    //}
                    else
                    {
                        throw new InvalidOperationException($"Unsupported Value Expression Type '{value.Expression.GetType().FullName}'");
                    }
                }
                EngineCommandParameter engineCommandParameter = null;
                var engineTable = expression.Target as EngineTable;
                if (engineTable!=null)
                {
                    var primaryKey = engineTable.PrimaryKey;
                    if (primaryKey != null)
                    {
                        for (int n=0; n< primaryKey.Fields.Length; n++)
                        {
                            var field = primaryKey.Fields[n] as PrimaryKeyField;
                            if (((field.DataType == DataModels.DataType.Integer) || (field.DataType == DataModels.DataType.Long)
                                    || (field.DataType == DataModels.DataType.UnsignedInteger)
                                    || (field.DataType == DataModels.DataType.UnsignedLong)
                                    || (field.DataType == DataModels.DataType.Short) || (field.DataType == DataModels.DataType.UnsignedShort)) && (field.Generated==true))
                            {
                                engineCommandParameter = new EngineCommandParameter()
                                {
                                    DataType = field.DataType,
                                    Direction = EngineParameterDirection.InputOutput,
                                    Name = field.Name
                                };
                                break;
                            }
                        }
                    }
                }
                builder.Insert(expression.Target.Schema, expression.Target.Name, fields.ToArray(), values.ToArray(), engineCommandParameter);
            }

            return builder.GetCommand();
        }

        
    }
}
