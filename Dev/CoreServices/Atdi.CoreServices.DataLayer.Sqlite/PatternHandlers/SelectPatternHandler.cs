using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.DataConstraint;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.Sqlite.PatternHandlers
{
    class SelectPatternHandler : LoggedObject, IQueryPatternHandler
    {
        private class SelectBuildingContex : BuildingContex
        {
            private EngineCommandParameter _rowCountParameter;

            public EngineCommandParameter RowCountParameter => _rowCountParameter;
            public void EnsureRowCountParameter()
            {
                if (this._rowCountParameter == null)
                {
                    _rowCountParameter = this.CreateParameter("COMMON", "ROWCOUNT", DataType.Integer, EngineParameterDirection.Output);
                }
            }
        }

        public SelectPatternHandler(ILogger logger) : base(logger)
        {
        }

        void IQueryPatternHandler.Handle<TPattern>(TPattern queryPattern, SqliteExecutor executor)
        {
            var pattern = queryPattern as PS.SelectPattern;

            var context = new SelectBuildingContex();

            this.BuildIteration(pattern, context);

            var command = context.BuildCommand();

            switch (pattern.Result.Kind)
            {
                case EngineExecutionResultKind.RowsAffected:
	                executor.ExecuteNonQuery(command);
                    pattern.AsResult<EngineExecutionRowsAffectedResult>()
                        .RowsAffected = (int)context.RowCountParameter.Value;
                    return;
                case EngineExecutionResultKind.Scalar:
                    // возврат одного значения - первого поля запроса
                    var result = pattern.AsResult<EngineExecutionScalarResult>();
                    result.Value = executor.ExecuteScalar(command);
                    return;
                case EngineExecutionResultKind.Reader:
                    if (pattern.Result is EngineExecutionReaderResult readerResult)
                    {
	                    executor.ExecuteReader(command, sqlReader =>
                        {
                            readerResult.Handler(new EngineDataReader(sqlReader, context.Mapper));
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
        ///     A1.T1_PK_1_ID AS FA1, -- 'Field 1 Path'
        ///     A1.T1_PK_2_ID AS FA2, -- 'Field 2 Path'
        /// FROM [Schema].[TABLE1] AS A1 -- BaseEntity Name
        ///     INNER JOIN [Schema].[TABLE2] AS A2 -- ChildEntity Name
        ///         ON (A2.T1_PK_1_ID = A1.T1_PK_1_ID AND A2.T1_PK_2_ID = A1.T1_PK_2_ID) 
        ///     INNER JOIN [Schema].[TABLE3] AS A3 -- ChildEntity Name
        ///         ON (A3.T1_PK_1_ID = A1.T1_PK_1_ID AND A2.T1_PK_2_ID = A1.T1_PK_2_ID) 
        ///     INNER JOIN [Schema].[TABLE4] AS A4 -- ExtensionEntity Name Requered = true
        ///         ON (A4.T1_PK_1_ID = A1.T1_PK_1_ID AND A4.T1_PK_2_ID = A1.T1_PK_2_ID)
        ///     LEFT JOIN [Schema].[TABLE4] AS A5 -- ExtensionEntity Name Requered = false
        ///         ON (A5.T1_PK_1_ID = A1.T1_PK_1_ID AND A4.T1_PK_2_ID = A1.T1_PK_2_ID)
        /// WHERE (A1.Name IS NOT NULL) AND (() OR ())
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="contex"></param>
        private void BuildIteration(PS.SelectPattern pattern, SelectBuildingContex context)
        {
            for (int i = 0; i < pattern.Expressions.Length; i++)
            {
                this.BuildExpression(pattern.Expressions[i], context);
                // коечто нужно добавть дл яподсчета кол-ва вычитанных запросом строк
                if (pattern.Result.Kind == EngineExecutionResultKind.RowsAffected)
                {
                    // генерируем парамтер кол-ва
                    context.EnsureRowCountParameter();
                    context.Builder.SetRowcount(context.RowCountParameter);
                }
            }

            
        }

        private void BuildExpression(PS.SelectExpression expression, SelectBuildingContex context)
        {
            var sqlColumns = new List<string>();
            var sqlJoins = new List<string[]>();
            var sqlWhere = string.Empty;
            var sqlDistinct = string.Empty;
            var sqlLimit = string.Empty;
            string[] sqlOrderBy = null;
            long fetch = -1;

			if (expression.Distinct)
            {
                sqlDistinct = context.Builder.CreateDistinct();
            }

            if (expression.Limit != null && expression.OffsetRows <= -1)
            {
                sqlLimit = this.BuildLimitExpression(expression, context);
            }
            else if (expression.Limit != null && expression.Limit.Type == LimitValueType.Records)
            {
	            fetch = expression.Limit.Value;
            }

			for (var i = 0; i < expression.Columns.Length; i++)
            {
                var column = expression.Columns[i];
                if (column is PS.MemberColumnExpression memberColumn)
                {
                    var columnAlias = context.EnsureColumnAlias(memberColumn);
                    var sourceAlias = context.EnsureSourceAlias(memberColumn.Member.Owner);
                    var sqlColumn = context.Builder.CreateSelectColumn(sourceAlias, memberColumn.Member.Name, columnAlias);
                    sqlColumns.Add(sqlColumn);
                }
            }

            var sqlFrom = context.BuildFromExpression(expression.From);

            if (expression.Joins != null && expression.Joins.Length > 0)
            {
                for (var i = 0; i < expression.Joins.Length; i++)
                {
                    var join = expression.Joins[i];
                    var sqlJoin = context.BuildJoinExpression(join);
                    sqlJoins.Add(sqlJoin);
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

            context.Builder.Select(sqlColumns.ToArray(), sqlFrom, sqlJoins.ToArray(), sqlWhere, sqlOrderBy, sqlDistinct, sqlLimit, expression.OffsetRows, fetch);
        }

        private string BuildLimitExpression(PS.SelectExpression expression, SelectBuildingContex context)
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

        private string[] BuildOrderByExpression(PS.SortExpression[] expressions, SelectBuildingContex context)
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
