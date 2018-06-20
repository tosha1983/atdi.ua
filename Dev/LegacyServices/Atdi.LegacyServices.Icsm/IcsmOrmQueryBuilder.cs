using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;

namespace Atdi.LegacyServices.Icsm
{
    internal sealed class IcsmOrmQueryBuilder : LoggedObject, IDisposable
    {
        private readonly IDataEngine _dataEngine;
        private readonly IEngineSyntax _syntax;
        private readonly ConditionParser _conditionParser;
        private readonly Orm.SchemasMetadata _schemasMetadata;

        public IcsmOrmQueryBuilder(IDataEngine dataEngine, Orm.SchemasMetadata schemasMetadata, ILogger logger) : base(logger)
        {
            this._dataEngine = dataEngine;
            this._syntax = dataEngine.Syntax;
            this._conditionParser = new ConditionParser(dataEngine.Syntax);
            this._schemasMetadata = schemasMetadata;

            logger.Debug(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, Events.CreatedInstanceOfQueryBuilder);
        }

        public string BuildSelectStatement(QuerySelectStatement statement, IDictionary<string, EngineCommandParameter> parameters)
        {
            try
            {
                var sourceColumns = statement.Table.Columns.Values;
                var fieldPaths = new string[statement.Table.Columns.Count];
                int index = 0;
                foreach (var column in sourceColumns)
                {
                    if (string.IsNullOrEmpty(column.Alias))
                    {
                        column.Alias = column.Name; 
                    }
                    fieldPaths[index++] = column.Name;
                }

                var rootStatement = this._schemasMetadata.BuildSelectStatement(_syntax, statement.Table.Name, fieldPaths);
                var fromExpression = this._dataEngine.Syntax.FromExpression(rootStatement, "A");

                var selectColumns = new string[statement.Table.SelectColumns.Count];
                index = 0;
                foreach (var column in statement.Table.SelectColumns.Values)
                {
                    /// todo alias
                    selectColumns[index++] = this._syntax.ColumnExpression(this._syntax.EncodeFieldName("A", column.Name), column.Name);
                }

                // add conditions
                var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);

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

                var selectStatement = this._syntax.SelectExpression(selectColumns, fromExpression, whereExpression, orderByColumns, limit);
                return selectStatement;
            }
            catch(Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
        }

        public string BuildWhereExpression(List<Condition> conditions, IDictionary<string, EngineCommandParameter> parameters)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return null;
            }

            var expressions = conditions.Select(condition => this._conditionParser.Parse(condition, parameters)).ToArray();
            var result = this._syntax.Constraint.JoinExpressions(LogicalOperator.And, expressions);
            return result;
        }

        public string BuildDeleteStatement(QueryDeleteStatement statement, IDictionary<string, EngineCommandParameter> parameters)
        {
            try
            {
                var columns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Conditions, columns);
                var columnsArray = columns.ToArray();

                var sourceExpression = this._syntax.EncodeTableName(this._schemasMetadata.DbSchema, statement.TableName);
                var fromStatement = this._schemasMetadata.BuildJoinStatement(this._syntax, statement.TableName, columnsArray.Select(c => c.ColumnName).ToArray(), out Orm.DbField[] dbFields);

                for (int i = 0; i < columnsArray.Length; i++)
                {
                    var column = columnsArray[i];
                    var dbField = dbFields[i];
                    column.ColumnName = dbField.m_logFld;
                    column.Source = dbField.m_idxTable.Tcaz;
                }
                var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);

                var deleteStatement = this._syntax.DeleteExpression(sourceExpression, fromStatement, whereExpression);
                return deleteStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildDeleteStatement, e);
            }
        }

        private void AppendColumnsFromConditions(List<Condition> conditions, List<ColumnOperand> columns)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return;
            }

            for (int i = 0; i < conditions.Count; i++)
            {
                AppendColumnsFromCondition(conditions[i], columns);
            }
        }

        private void AppendColumnsFromCondition(Condition condition, List<ColumnOperand> columns)
        {
            if (condition.Type == ConditionType.Complex)
            {
                var complexCondition = condition as ComplexCondition;
                var conditions = complexCondition.Conditions;
                if (conditions != null && conditions.Length > 0)
                {
                    for (int i = 0; i < conditions.Length; i++)
                    {
                        AppendColumnsFromCondition(conditions[i], columns);
                    }
                }
                return;
            }
            else if (condition.Type == ConditionType.Expression)
            {
                var conditionExpression = condition as ConditionExpression;
                AppendColumnFromOperand(conditionExpression.LeftOperand, columns);
                AppendColumnFromOperand(conditionExpression.RightOperand, columns);
            }
        }

        private void AppendColumnFromOperand(Operand operand, List<ColumnOperand> columns)
        {
            if (operand == null)
            {
                return;
            }
            if (operand.Type == OperandType.Column)
            {
                var columnOperand = operand as ColumnOperand;
                columns.Add(columnOperand);
            }
        }

        public void Dispose()
        {
        }
    }
}
