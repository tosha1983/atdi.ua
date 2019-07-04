using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.Oracle.PatternHandlers
{
    class OracleSelectPatternHandler : LoggedObject, IOracleQueryPatternHandler
    {
        public OracleSelectPatternHandler(ILogger logger) : base(logger)
        {
        }

        void IOracleQueryPatternHandler.Handle<TPattern>(TPattern queryPattern, OracleExecuter executer)
        {
            var pattern = queryPattern as PS.SelectPattern;

            var context = new OracleBuildingContex();

            this.BuildIteration(pattern, context);

            var command = context.BuildCommand();

            switch (pattern.Result.Kind)
            {
                case EngineExecutionResultKind.RowsAffected:
                    //pattern.AsResult<EngineExecutionRowsAffectedResult>()
                        //.RowsAffected = executer.ExecuteNonQuery(command);
                    int cnt = 0;
                   executer.ExecuteReader(command, reader =>
                    {
                       while (reader.Read())
                       {
                           cnt++;
                       }
                       return true;
                   });
                    pattern.AsResult<EngineExecutionRowsAffectedResult>()
                   .RowsAffected = cnt;
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
                            readerResult.Handler(new OracleEngineDataReader(sqlReader, context.Mapper));
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported result object type '{pattern.Result.GetType().FullName}'");
                    }
                    return;
                case EngineExecutionResultKind.None:
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result kind '{pattern.Result.Kind}'");
            }
        }

        /// <summary>
        /// Генерация запроса
        /// SELECT
        ///     A1.T1_PK_1_ID FA1, -- 'Field 1 Path'
        ///     A1.T1_PK_2_ID FA2, -- 'Field 2 Path'
        /// FROM [Schema].[TABLE1] A1 -- BaseEntity Name
        ///     INNER JOIN [Schema].[TABLE2] A2 -- ChildEntity Name
        ///         ON (A2.T1_PK_1_ID = A1.T1_PK_1_ID AND A2.T1_PK_2_ID = A1.T1_PK_2_ID) 
        ///     INNER JOIN [Schema].[TABLE3] A3 -- ChildEntity Name
        ///         ON (A3.T1_PK_1_ID = A1.T1_PK_1_ID AND A2.T1_PK_2_ID = A1.T1_PK_2_ID) 
        ///     INNER JOIN [Schema].[TABLE4] A4 -- ExtensionEntity Name Requered = true
        ///         ON (A4.T1_PK_1_ID = A1.T1_PK_1_ID AND A4.T1_PK_2_ID = A1.T1_PK_2_ID)
        ///     LEFT JOIN [Schema].[TABLE4] A5 -- ExtensionEntity Name Requered = false
        ///         ON (A5.T1_PK_1_ID = A1.T1_PK_1_ID AND A4.T1_PK_2_ID = A1.T1_PK_2_ID)
        /// WHERE (A1.Name IS NOT NULL) AND (() OR ())
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="contex"></param>
        private void BuildIteration(PS.SelectPattern pattern, OracleBuildingContex context)
        {
            //context.Builder.SetBegin();
            for (int i = 0; i < pattern.Expressions.Length; i++)
            {
                this.BuildExpression(pattern.Expressions[i], context);
            }
            //context.Builder.SetEnd();
        }

        private void BuildExpression(PS.SelectExpression expression, OracleBuildingContex context)
        {
            var sqlColumns = new List<string>();
            var sqlJoins = new List<string[]>();
            var sqlFrom = string.Empty;
            var sqlWhere = string.Empty;
            var sqlDistinct = string.Empty;
            var sqlLimit = string.Empty;
            string[] sqlOrderBy = null;

            if (expression.Destinct)
            {
                sqlDistinct = context.Builder.CreateDistinct();
            }

            if (expression.Limit != null)
            {
                sqlLimit = this.BuildLimitExpression(expression, context);
            }

            for (int i = 0; i < expression.Columns.Length; i++)
            {
                var column = expression.Columns[i];
                if (column is PS.MemberColumnExpression memberColumn)
                {
                    string columnAlias = context.EnsureColumnAlias(memberColumn);
                    string sourceAlias = context.EnsureSourceAlias(memberColumn.Member.Owner);
                    var sqlColumn = context.Builder.CreateSelectColumn(sourceAlias, memberColumn.Member.Name, columnAlias);
                    sqlColumns.Add(sqlColumn);
                }
            }

            sqlFrom = context.BuildFromExpression(expression.From);

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

            if (expression.Sorting != null && expression.Sorting.Length > 0)
            {
                sqlOrderBy = this.BuildOrderByExpression(expression.Sorting, context);
            }

            context.Builder.Select(sqlColumns.ToArray(), sqlFrom, sqlJoins.ToArray(), sqlWhere, sqlOrderBy, sqlDistinct, sqlLimit);

        }

        private string BuildLimitExpression(PS.SelectExpression expression, OracleBuildingContex context)
        {
            string sqlLimit;
            switch (expression.Limit.Type)
            {
                case DataModels.DataConstraint.LimitValueType.Records:
                    sqlLimit = context.Builder.CreateCountLimt(expression.Limit.Value);
                    break;
                case DataModels.DataConstraint.LimitValueType.Percent:
                    sqlLimit = context.Builder.CreatePercentLimt(expression.Limit.Value);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported records limit type '{expression.Limit.Type}'");
            }

            return sqlLimit;
        }

        private string[] BuildOrderByExpression(PS.SortExpression[] expressions, OracleBuildingContex context)
        {
            var result = new string[expressions.Length];
            for (int i = 0; i < expressions.Length; i++)
            {
                var expression = expressions[i];
                var sourceAlias = context.EnsureSourceAlias(expression.Member.Owner);
                result[i] = context.Builder.CreateSortingClause(expression.Direction, sourceAlias, expression.Member.Name);
            }
            return result;
        }
        

        
    }
}
