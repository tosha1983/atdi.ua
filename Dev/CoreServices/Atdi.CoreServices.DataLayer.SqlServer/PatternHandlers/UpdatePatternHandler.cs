using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.CoreServices.DataLayer.SqlServer.PatternHandlers
{
    class UpdatePatternHandler : LoggedObject, IQueryPatternHandler
    {
        public UpdatePatternHandler(ILogger logger) : base(logger)
        {
        }

        void IQueryPatternHandler.Handle<TPattern>(TPattern queryPattern, SqlServerExecuter executer)
        {
            var pattern = queryPattern as PS.UpdatePattern;

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
                    // возврат одного значения - первого поля запроса
                    var result = pattern.AsResult<EngineExecutionScalarResult>();
                    result.Value = executer.ExecuteScalar(command);
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

        private void BuildIteration(PS.UpdatePattern pattern, BuildingContex context)
        {
            for (int i = 0; i < pattern.Expressions.Length; i++)
            {
                var expression = pattern.Expressions[i];
                switch (expression.Kind)
                {
                    case PS.QueryExpressionKind.Select:
                        this.BuildExpression(expression as PS.SelectExpression, context);
                        break;
                    case PS.QueryExpressionKind.Update:
                        var updateExpression = expression as PS.UpdateExpression;
                        context.Builder.ExpresaionAlias(i, updateExpression.Target.Alias);
                        this.BuildExpression(updateExpression, context);
                        break;
                    case PS.QueryExpressionKind.Insert:
                    case PS.QueryExpressionKind.Delete:
                    default:
                        throw new InvalidOperationException($"Unsupported query expression with kind '{expression.Kind}'");
                }
                
            }
        }

        /// <summary>
        /// Идея выборки записей во временную таблицу потом на основании нее апдейт - пок
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="context"></param>
        private void BuildExpression(PS.SelectExpression expression, BuildingContex context)
        {
            throw new NotImplementedException();
        }

        private void BuildExpression(PS.UpdateExpression expression, BuildingContex context)
        {
            var sqlJoins = new List<string[]>();
            var sqlFrom = string.Empty;
            var sqlWhere = string.Empty;
            var setClauses = new string[expression.Values.Length];

            for (int i = 0; i < expression.Values.Length; i++)
            {
                var value = expression.Values[i];
                var parameter = default(EngineCommandParameter);
                // генерация значения
                if (value.Expression is PS.GeneratedValueExpression generatedValue)
                {
                    parameter = context.CreateParameter(value.Property.Owner.Alias, value.Property.Name, value.Property.DataType, EngineParameterDirection.Output);

                    if (generatedValue.Operation == PS.GeneratedValueOperation.SetDefault)
                    {
                        context.Builder.SetDefault(parameter);
                    }
                    else if (generatedValue.Operation == PS.GeneratedValueOperation.SetNext)
                    {
                        context.Builder.GenerateNextValue(parameter);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported generated value operation '{generatedValue.Operation}'");
                    }
                }
                else if (value.Expression is PS.ConstantValueExpression constantValue)
                {
                    parameter = context.CreateParameter(value.Property.Owner.Alias, value.Property.Name, value.Property.DataType, EngineParameterDirection.Input);
                    parameter.SetValue(constantValue.Value);
                }
                else if (value.Expression is PS.ReferenceValueExpression referenceValue)
                {
                    parameter = context.GetParameter(referenceValue.Member.Owner.Alias, referenceValue.Member.Name);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported Value Expression Type '{value.Expression.GetType().FullName}'");
                }

                string alias = context.EnsureSourceAlias(value.Property.Owner);
                var setClause = context.Builder.CreateSetClause(alias, value.Property.Name, context.Builder.CreateParameterExpression(parameter.Name));
                setClauses[i] = setClause;
            }

            sqlFrom = context.BuildFromExpression(expression.Target);

            if (expression.Joins != null && expression.Joins.Length > 0)
            {
                for (int i = 0; i < expression.Joins.Length; i++)
                {
                    var join = expression.Joins[i];
                    var sqlJoion = context.BuildJoinExpression(join);
                    sqlJoins.Add(sqlJoion);
                }
            }

            if (expression.Condition != null)
            {
                sqlWhere = context.BuildConditionExpression(expression.Condition);
            }

            context.Builder.Update(expression.Target.Schema, expression.Target.Name, setClauses, sqlFrom, sqlJoins.ToArray(), sqlWhere);
        }
    }
}
