using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using Atdi.CoreServices.DataLayer.Oracle;
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

            var context = new OracleBuildingContex();

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
                    executer.ExecuteReader(command, sqlReader =>
                    {
                        if (pattern.Result is EngineExecutionReaderResult<System.Data.IDataReader> result1)
                        {
                            result1.Handler(sqlReader);
                        }
                        if (pattern.Result is EngineExecutionReaderResult<IEngineDataReader> result2)
                        {
                            result2.Handler(new OracleEngineDataReader(sqlReader, context.Mapper));
                        }
                    });
                    return;
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result type '{pattern.Result.Kind}'");
            }
        }

        private void DecodePrimaryKey(PS.InsertPattern pattern, OracleBuildingContex contex, EngineCommand command)
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
        /// EXAMPLE:
        /// 
        /// BEGIN
        /// /* Expression: index = #0, alias = 'Sensor_1' */
        /// SELECT GetID('SENSOR') INTO :P_I1_C1 FROM DUAL;
        /// INSERT INTO ICST.SENSOR(ID, NAME, TECHID, CREATEDBY)
        /// VALUES(:P_I1_C1, :P_I1_C2, :P_I1_C3, :P_I1_C4);
        /// OPEN :P_I1_C5_CURSOR_REF0 FOR SELECT ID FROM ICST.SENSOR WHERE ID = :P_I1_C1;
        /// END;
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private void BuildIteration(PS.InsertPattern pattern, OracleBuildingContex contex)
        {
            contex.Builder.SetBegin();

            var identityFields = new Dictionary<string, string>();
            for (int i = 0; i < pattern.Expressions.Length; i++)
            {
                var expression = pattern.Expressions[i];
                contex.Builder.ExpresaionAlias(i, expression.Target.Alias);

                var fields = new List<string>();
                var values = new List<EngineCommandParameter>();

                //var identityFields = new Dictionary<string,string>();


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

                            identityFields.Add(parameter.Name, value.Property.Name);

                            contex.Builder.GenerateNextValue(parameter, value.Property.Owner as EngineObject);
                            fields.Add(value.Property.Name);
                            values.Add(parameter);
                        }
                        else
                        {
                            if (generatedValue.Operation == PS.GeneratedValueOperation.SetDefault)
                            {
                                contex.Builder.SetDefault(parameter);
                            }
                            else if (generatedValue.Operation == PS.GeneratedValueOperation.SetNext)
                            {
                                contex.Builder.GenerateNextValue(parameter, value.Property.Owner as EngineObject);
                            }
                            fields.Add(value.Property.Name);
                            values.Add(parameter);
                        }
                    }
                    else if (value.Expression is PS.ConstantValueExpression constantValue)
                    {
                        if (!fields.Contains(value.Property.Name))
                        {
                            fields.Add(value.Property.Name);
                            var parameter = contex.CreateParameter(value.Property.Owner.Alias, value.Property.Name, value.Property.DataType, EngineParameterDirection.Input);
                            parameter.SetValue(constantValue.Value);
                            values.Add(parameter);
                        }
                    }
                    else if (value.Expression is PS.ReferenceValueExpression referenceValue)
                    {
                        if (!fields.Contains(value.Property.Name))
                        {
                            fields.Add(value.Property.Name);
                            var parameter = contex.GetParameter(referenceValue.Member.Owner.Alias, referenceValue.Member.Name);
                            values.Add(parameter);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported Value Expression Type '{value.Expression.GetType().FullName}'");


                    }
                }

                // генерируем код создания записи
                contex.Builder.Insert(expression.Target.Schema, expression.Target.Name, fields.ToArray(), values.ToArray());

                // поиск сущности, для которой нужно открыть курсор (если задан параметр EngineExecutionResultKind.Scalar)
                if (pattern.Result.Kind == EngineExecutionResultKind.Scalar)
                {
                    var patternResultPK = (pattern.Result as EngineExecutionScalarResult).Value.GetType();
                    var getNameEntityResult = patternResultPK.Name.Replace("_PK_Proxy", "");
                    int index = expression.Target.Alias.LastIndexOf("_");
                    if (index >= 0)
                    {
                        var getNameExpressionTargetEntity = expression.Target.Alias.Substring(0, index);
                        if (getNameExpressionTargetEntity == getNameEntityResult)
                        {
                            if (identityFields.Count > 0)
                            {
                                var parameter = contex.CreateParameter($"REF{i}", $"REF{i}", DataModels.DataType.Undefined, EngineParameterDirection.Output, $"REF{i}");
                                contex.Builder.OpenCursor(expression.Target.Schema, parameter.Name, expression.Target.Name, identityFields);
                            }
                        }
                    }
                }
            }

            contex.Builder.SetEnd();
        }
    }
}


