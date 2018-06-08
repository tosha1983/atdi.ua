using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.LegacyServices.Icsm;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class QueryExecutor : LoggedObject, IQueryExecutor
    {
        private readonly IDataEngine _dataEngine;
        private readonly IEngineSyntax _syntax;
        private readonly ConditionParser _conditionParser;
        private readonly IcsmOrmQueryBuilder _icsmOrmQueryBuilder;
        private readonly IParserQuery _parserQuery;

        public QueryExecutor(IDataEngine dataEngine, IcsmOrmQueryBuilder icsmOrmQueryBuilder, ILogger logger) : base(logger)
        {
            
            this._dataEngine = dataEngine;
            this._syntax = dataEngine.Syntax;
            this._conditionParser = new ConditionParser(dataEngine.Syntax);
            this._icsmOrmQueryBuilder = icsmOrmQueryBuilder;
            this._parserQuery = icsmOrmQueryBuilder.GetParserQuery();
            logger.Debug(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, Events.CreatedInstanceOfQueryExecutor);
        }

        public IParserQuery GetQueryParser()
        {
            return this._parserQuery;
        }

        public TResult Fetch<TResult>(IQuerySelectStatement statement, Func<IDataReader, TResult> handler)
        {
            try
            {
                var command = this.BuildSelectCommand(statement as QuerySelectStatement);

                var result = default(TResult);
                _dataEngine.Execute(command, reader =>
                {
                    result = handler(reader);
                });
                return result;
            }
            catch(Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.FetchingData, e);
                throw;
            }
        }

        public TResult Fetch<TModel, TResult>(IQuerySelectStatement<TModel> statement, Func<IDataReader<TModel>, TResult> handler)
        {
            try
            {
                var objectStatment = statement as QuerySelectStatement<TModel>;
                var command = this.BuildSelectCommand(objectStatment.Statement);

                var result = default(TResult);
                _dataEngine.Execute(command, reader =>
                {
                    var typedReader = new QueryDataReader<TModel>(reader);
                    result = handler(typedReader);
                });
                return result;
            }
            catch(Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.FetchingData, e);
                throw;
            }
        }

        private EngineCommand BuildSelectCommand(QuerySelectStatement statement)
        {
            var command = new EngineCommand();

            var rootStatement = this._icsmOrmQueryBuilder.BuildSelectStatement(statement);

            var fromExpression = this._dataEngine.Syntax.FromExpression(rootStatement, "A");

            var selectColumns = new string[statement.Table.SelectColumns.Count];
            var index = 0;
            foreach (var column in statement.Table.SelectColumns.Values)
            {
                /// todo alias
                selectColumns[index++] = this._syntax.ColumnExpression(this._syntax.EncodeFieldName("A", column.Name), column.Name);
            }

            // add conditions
            var whereExpression = this.CreateWhereExpression(statement.Conditions, command.Parameters);

            // add order by
            string[] orderByColumns = null;
            var sortColumns = statement.Orders;
            if (sortColumns != null && sortColumns.Count > 0)
            {
                orderByColumns = new string[sortColumns.Count];
                index = 0;
                foreach (var sortColumn in sortColumns)
                {
                    var encodeColumn = this._syntax.EncodeFieldName("A", sortColumn.Column.Alias);
                    orderByColumns[index++] = _syntax.SortedColumn(encodeColumn, sortColumn.Direction);
                }
            }

            // add on top (n)
            var limit = statement.Limit;
            
            // add group by

            command.Text = this._syntax.SelectExpression(selectColumns, fromExpression, whereExpression, orderByColumns, limit);
            return command;
        }

        private string CreateWhereExpression(List<Condition> conditions, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return null;
            }

            var expressions = conditions.Select(condition => this._conditionParser.Parse(condition, parameters)).ToArray();

            var result = this._syntax.Constraint.JoinExpressions(LogicalOperator.And, expressions);

            return result;
        }
    }
}
