using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.CoreServices.DataLayer.Oracle.PatternHandlers
{
    class OracleDeletePatternHandler : LoggedObject, IOracleQueryPatternHandler
    {
        public OracleDeletePatternHandler(ILogger logger) : base(logger)
        {
        }

        void IOracleQueryPatternHandler.Handle<TPattern>(TPattern queryPattern, OracleExecuter executer)
        {
            var pattern = queryPattern as PS.DeletePattern;

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
                // нечего больше возвращать, возможно вбудущем сделаем возврат первичных ключей при определнных сценариях
                case EngineExecutionResultKind.Scalar:
                    //// возврат одного значения - первого поля запроса
                    //var result = pattern.AsResult<EngineExecutionScalarResult>();
                    //result.Value = executer.ExecuteScalar(command);
                    //return;
                case EngineExecutionResultKind.Reader:
                    //if (pattern.Result is EngineExecutionReaderResult readerResult)
                    //{
                    //    executer.ExecuteReader(command, sqlReader =>
                    //    {
                    //        readerResult.Handler(new EngineDataReader(sqlReader, context.Mapper));
                    //    });
                    //}
                    //else
                    //{
                    //    throw new InvalidOperationException($"Unsupported result object type '{pattern.Result.GetType().FullName}'");
                    //}
                    //return;
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result kind '{pattern.Result.Kind}'");
            }
        }

        private void BuildIteration(PS.DeletePattern pattern, OracleBuildingContex context)
        {
            //context.Builder.SetBegin();

            for (int i = 0; i < pattern.Expressions.Length; i++)
            {
                var expression = pattern.Expressions[i];
                switch (expression.Kind)
                {
                    case PS.QueryExpressionKind.Select:
                        this.BuildExpression(expression as PS.SelectExpression, context);
                        break;
                    case PS.QueryExpressionKind.Delete:
                    
                        var deleteExpression = expression as PS.DeleteExpression;
                        context.Builder.ExpresaionAlias(i, deleteExpression.Target.Alias);
                        this.BuildExpression(deleteExpression, context);
                        break;
                    case PS.QueryExpressionKind.Insert:
                    case PS.QueryExpressionKind.Update:
                    default:
                        throw new InvalidOperationException($"Unsupported query expression with kind '{expression.Kind}'");
                }
                
            }
            //context.Builder.SetEnd();
        }

        /// <summary>
        /// Идея выборки записей во временную таблицу потом на основании нее апдейт - пок
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="context"></param>
        private void BuildExpression(PS.SelectExpression expression, OracleBuildingContex context)
        {
            throw new NotImplementedException();
        }

        private void BuildExpression(PS.DeleteExpression expression, OracleBuildingContex context)
        {
            var sqlJoins = new List<string[]>();
            var sqlFrom = string.Empty;
            var sqlWhere = string.Empty;

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
                sqlWhere = context.BuildConditionExpression(expression.Condition); //+";";
            }

            context.Builder.Delete(expression.Target.Schema, expression.Target.Name, sqlFrom, sqlJoins.ToArray(), sqlWhere);
        }
    }
}
