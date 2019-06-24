using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.SqlServer.PatternHandlers
{
    class InsertPatternHandler : LoggedObject, IQueryPatternHandler
    {
        public InsertPatternHandler(ILogger logger) : base(logger)
        {
        }

        public void Handle<TPattern>(TPattern queryPattern, SqlServerExecuter executer)
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
                            result2.Handler(new EngineDataReader(sqlReader));
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
        /// Построитель конструкцции следующего вида
        /// /* Iterration: #1 */
        /// 
        /// /* Expression: index = #1, alias = 'atdi.DataContract.Entites.BaseEntity1' */
        /// INSERT INTO [SCHEMA_NAME].[BASE_TABLE_NAME](F1, F2)
        /// VALUES(@P_С1_1, @P_С1_2)
        /// SELECT @P_С1_3 = @@IDENTITY()
        /// 
        /// /* Expression: index = #2, alias = 'atdi.DataContract.Entites.Entity1' */
        /// INSERT INTO [SCHEMA_NAME].[MAIN_TABLE_NAME](F_PK, F1, F2)
        /// VALUES(@P_С1_3, @P_С1_4, @P_С1_5)
        /// 
        /// /* Expression Alias (3): atdi.DataContract.Entites.ExtensionEntity1 */
        /// INSERT INTO [SCHEMA_NAME].[MAIN_TABLE_NAME](F_PK, F1, F2)
        /// VALUES(@P_С1_3, @P_С1_6, @P_С1_7)
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private EngineCommand BuildCommand(PS.InsertPattern pattern)
        {
            var builder = new CommandBuilder();

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
                            Value = constantValue.Value
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
                builder.Insert(expression.Target.Schema, expression.Target.Name, fields.ToArray(), values.ToArray());
            }

            return builder.GetCommand();
        }

        
    }
}
