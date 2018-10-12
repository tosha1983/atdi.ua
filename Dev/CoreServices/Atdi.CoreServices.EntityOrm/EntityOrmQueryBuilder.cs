using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;


namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class EntityOrmQueryBuilder : LoggedObject, IDisposable
    {
        private readonly IDataEngine _dataEngine;
        private readonly IEngineSyntax _syntax;
        private readonly ConditionParser _conditionParser;
        private readonly IEntityOrm _entityMetadata;

        
        public EntityOrmQueryBuilder(IDataEngine dataEngine, IEntityOrm entityMetadata, ILogger logger) : base(logger)
        {
            this._entityMetadata = entityMetadata;
            this._dataEngine = dataEngine;
            //this._syntax = dataEngine.Syntax;
            //this._conditionParser = new ConditionParser(dataEngine.Syntax);
            logger.Debug(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, Events.CreatedInstanceOfQueryBuilder);
        }
       
         public string BuildSelectStatement(QuerySelectStatement statement, IDictionary<string, EngineCommandParameter> parameters)
         {
             try
             {
                 var selectedColumns = statement.Table.Columns.Values.ToArray();
                 var conditionsColumns = new List<ColumnOperand>();
                 AppendColumnsFromConditions(statement.Conditions, conditionsColumns);
                 var whereColumns = conditionsColumns.ToArray();

                 var sortColumns = statement.Orders == null ? new QuerySelectStatement.OrderByColumnDescriptor[] { } : statement.Orders.ToArray();


                 var fieldCount = selectedColumns.Length + whereColumns.Length + sortColumns.Length;
                 var fieldPaths = new string[fieldCount];

                 int index = 0;
                 for (int i = 0; i < selectedColumns.Length; i++)
                 {
                     var column = selectedColumns[i];
                     if (string.IsNullOrEmpty(column.Alias))
                     {
                         column.Alias = "col_" + index.ToString();  //column.Name; 
                     }
                     fieldPaths[index++] = column.Name;
                 }
                 for (int i = 0; i < whereColumns.Length; i++)
                 {
                     var column = whereColumns[i];
                     fieldPaths[index++] = column.ColumnName;
                 }
                 for (int i = 0; i < sortColumns.Length; i++)
                 {
                     var column = sortColumns[i];
                     fieldPaths[index++] = column.Column.Name;
                 }

                 IEntityMetadata entityMetadata = _entityMetadata.GetEntityMetadata(statement.Table.Name);
                 IFieldMetadata[] dbFields = null;
                 string  fromExpression = "";
                 //var fromExpression = this._entityMetadata.BuildJoinStatement(this._dataEngine, statement, fieldPaths, out IFieldMetadata[] dbFields);


                 index = 0;

                 var columnExpressions = new string[selectedColumns.Length];
                 for (int i = 0; i < selectedColumns.Length; i++)
                 {
                     var column = selectedColumns[i];
                     var dbField = dbFields[index++];
                     columnExpressions[i] = this._syntax.ColumnExpression(this._syntax.EncodeFieldName(dbField.Name), column.Alias);
                 }

                 // to build the where section
                 for (int i = 0; i < whereColumns.Length; i++)
                 {
                     var column = whereColumns[i];
                     var dbField = dbFields[index++];
                     column.ColumnName = dbField.Name;
                 }
                 var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);

                 // to build the order by section
                 var orderByColumns = new string[sortColumns.Length];
                 for (int i = 0; i < sortColumns.Length; i++)
                 {
                     var column = sortColumns[i];
                     var dbField = dbFields[index++];
                     var encodeColumn = "";
                     encodeColumn = this._syntax.EncodeFieldName(dbField.Name);
                     orderByColumns[i] = _syntax.SortedColumn(encodeColumn, column.Direction);
                 }

                 // add on top (n)
                 var limit = statement.Limit;

                 // add group by

                 var selectStatement = this._syntax.SelectExpression(columnExpressions, fromExpression, whereExpression, orderByColumns, limit);
                 return selectStatement;
             }
             catch (Exception e)
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

         public string BuildSetValuesExpression(List<ColumnValue> columns, IDictionary<string, EngineCommandParameter> parameters)
         {
             if (columns == null || columns.Count == 0)
             {
                 return null;
             }
             var values = new string[columns.Count];
             for (int i = 0; i < columns.Count; i++)
             {
                 var column = columns[i];
                 var parameter = new EngineCommandParameter
                 {
                     DataType = column.DataType,
                     Name = "v_" + column.Name,
                     Value = column.GetValue()
                 };
                 parameters.Add(parameter.Name, parameter);
                 values[i] = this._syntax.SetColumnValueExpression(this._syntax.EncodeFieldName(column.Name), this._syntax.EncodeParameterName(parameter.Name));
             }

             var result = string.Join("," + Environment.NewLine, values);
             return result;
         }

         public string BuildDeleteStatement(QueryDeleteStatement statement, IDictionary<string, EngineCommandParameter> parameters)
         {
             try
             {
                 var columns = new List<ColumnOperand>();
                 AppendColumnsFromConditions(statement.Conditions, columns);
                 var columnsArray = columns.ToArray();

                 IEntityMetadata entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                 var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, statement.TableName);

                IFieldMetadata[] dbFields = null;
                 string fromStatement="";
                // var fromStatement = this._entityMetadata.BuildJoinStatement(this._syntax, statement.TableName, columnsArray.Select(c => c.ColumnName).ToArray(), out IFieldMetadata[] dbFields);

                 for (int i = 0; i < columnsArray.Length; i++)
                 {
                     var column = columnsArray[i];
                     var dbField = dbFields[i];
                     column.ColumnName = dbField.Name;
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

         public string BuildInsertStatement(QueryInsertStatement statement, IDictionary<string, EngineCommandParameter> parameters)
         {
             try
             {
                 IEntityMetadata entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                 var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, statement.TableName);
                 var changedColumns = new string[statement.ColumnsValues.Count];
                 var selectedParameters = new string[statement.ColumnsValues.Count];

                 for (int i = 0; i < statement.ColumnsValues.Count; i++)
                 {
                     var column = statement.ColumnsValues[i];
                     var parameter = new EngineCommandParameter
                     {
                         DataType = column.DataType,
                         Name = "v_" + column.Name,
                         Value = column.GetValue()
                     };
                     parameters.Add(parameter.Name, parameter);
                     selectedParameters[i] = this._syntax.EncodeParameterName(parameter.Name);
                     changedColumns[i] = this._syntax.EncodeFieldName(column.Name);
                 }

                 var columnsExpression = string.Join(", ", changedColumns);
                 var valuesExpression = string.Join(", ", selectedParameters);


                 var insertStatement = this._syntax.InsertExpression(sourceExpression, columnsExpression, valuesExpression);
                 return insertStatement;
             }
             catch (Exception e)
             {
                 this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                 throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
             }
         }

         public string BuildUpdateStatement(QueryUpdateStatement statement, IDictionary<string, EngineCommandParameter> parameters)
         {
             try
             {
                 var columns = new List<ColumnOperand>();
                 AppendColumnsFromConditions(statement.Conditions, columns);
                 var columnsArray = columns.ToArray();

                 IEntityMetadata entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                 var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, statement.TableName);
                 var valuesExpression = this.BuildSetValuesExpression(statement.ColumnsValues, parameters);

                 string fromStatement = "";
                 IFieldMetadata[] dbFields = null;
                 //var fromStatement = this._entityMetadata.BuildJoinStatement(this._syntax, statement.TableName, columnsArray.Select(c => c.ColumnName).ToArray(), out IFieldMetadata[] dbFields);

                 for (int i = 0; i < columnsArray.Length; i++)
                 {
                     var column = columnsArray[i];
                     var dbField = dbFields[i];
                     column.ColumnName = dbField.Name;
                 }
                 var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);

                 var updateStatement = this._syntax.UpdateExpression(sourceExpression, valuesExpression, fromStatement, whereExpression);
                 return updateStatement;
             }
             catch (Exception e)
             {
                 this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                 throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
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
