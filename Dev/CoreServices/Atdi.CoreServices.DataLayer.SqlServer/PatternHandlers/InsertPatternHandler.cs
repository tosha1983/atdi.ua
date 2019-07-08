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

            var context = new BuildingContex();

            this.BuildIteration(pattern, context);

            var command = context.BuildCommand();

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
                    this.DecodePrimaryKey(pattern, context, command);
                    return;
                case EngineExecutionResultKind.Reader:
                    if (pattern.Result is EngineExecutionReaderResult readerResult)
                    {
                        executer.ExecuteReader(command, sqlReader =>
                        {
                            readerResult.Handler(new EngineDataReader(sqlReader, context.Mapper));
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported result object type '{pattern.Result.GetType().FullName}'");
                    }
                    return;
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result kind '{pattern.Result.Kind}'");
            }
        }

        private void DecodePrimaryKey(PS.InsertPattern pattern, BuildingContex contex, EngineCommand command)
        {
            var pkFields = pattern.PrimaryKeyFields;
            if (pkFields == null || pkFields.Length == 0)
            {
                throw new InvalidOperationException("Primary key fields are not defined");
            }

            var instance = pattern.AsResult<EngineExecutionScalarResult>().Value;
            if (instance == null)
            {
                throw new InvalidOperationException("Primary key instance are not defined");
            }
            var instanceType = instance.GetType();
            for (int i = 0; i < pkFields.Length; i++)
            {
                var pkField = pkFields[i];
                var parameter = contex.GetParameter(pkField.Owner.Alias, pkField.Name);
                var property = instanceType.GetProperty(pkField.Property);
                if (property == null)
                {
                    throw new InvalidOperationException($"Primary key instance does not contain a property named '{pkField.Property}'");
                }
                property.SetValue(instance, parameter.Value);
            }
        }

        /// <summary>
        /// Построитель конструкцции следующего вида
        /// /* Iterration: #1 */
        /// 
        /// /* Expression: index = #1, alias = 'atdi.DataContract.Entites.BaseEntity1' */
        /// SET @P_С1_2 = ROWID()
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
        private void BuildIteration(PS.InsertPattern pattern, BuildingContex contex)
        {
            for (int i = 0; i < pattern.Expressions.Length; i++)
            {
                var expression = pattern.Expressions[i];
                contex.Builder.ExpresaionAlias(i, expression.Target.Alias);

                var fields = new List<string>();
                var values = new List<EngineCommandParameter>();

                var identities = new List<EngineCommandParameter>();

                for (int j = 0; j < expression.Values.Length; j++)
                {
                    var value = expression.Values[j];

                    // генерация значения
                    if (value.Expression is PS.GeneratedValueExpression generatedValue)
                    {
                        var parameter = contex.CreateParameter(value.Property.Owner.Alias, value.Property.Name, value.Property.DataType, EngineParameterDirection.Output);

                        // исключаем поля айдентити из прямой вставки в поле
                        if (generatedValue.Operation == PS.GeneratedValueOperation.SetNext 
                            && (value.Property.DataType == DataModels.DataType.Integer
                            || value.Property.DataType == DataModels.DataType.Long
                            || value.Property.DataType == DataModels.DataType.Short))
                        {
                            identities.Add(parameter);
                        }
                        else
                        {
                            if (generatedValue.Operation == PS.GeneratedValueOperation.SetDefault)
                            {
                                contex.Builder.SetDefault(parameter);
                            }
                            else if (generatedValue.Operation == PS.GeneratedValueOperation.SetNext)
                            {
                                contex.Builder.GenerateNextValue(parameter);
                            }
                            fields.Add(value.Property.Name);
                            values.Add(parameter);
                        }
                    }
                    else if (value.Expression is PS.ConstantValueExpression constantValue)
                    {
                        fields.Add(value.Property.Name);
                        var parameter = contex.CreateParameter(value.Property.Owner.Alias, value.Property.Name, value.Property.DataType, EngineParameterDirection.Input);
                        parameter.SetValue(constantValue.Value);
                        values.Add(parameter);
                    }
                    else if (value.Expression is PS.ReferenceValueExpression referenceValue)
                    {
                        fields.Add(value.Property.Name);
                        var parameter = contex.GetParameter(referenceValue.Member.Owner.Alias, referenceValue.Member.Name);
                        values.Add(parameter);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported Value Expression Type '{value.Expression.GetType().FullName}'");
                    }
                }

                // генерируем код создания записи
                contex.Builder.Insert(expression.Target.Schema, expression.Target.Name, fields.ToArray(), values.ToArray());

                // получаем айденти
                if (identities.Count > 0)
                {
                    // генерируем получения автоинкрементных полей
                    identities.ForEach(p => contex.Builder.GenerateNextValue(p));
                }
            }
        }

        
    }
}
