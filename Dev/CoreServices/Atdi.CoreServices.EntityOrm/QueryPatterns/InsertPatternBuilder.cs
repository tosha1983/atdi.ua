using Atdi.DataModels;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.DataLayer;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm.QueryPatterns
{
    
    internal sealed class InsertPatternBuilder : LoggedObject, IPatternBuilder
    {
        private readonly IEntityOrm _entityOrm;
        private readonly DataTypeSystem _dataTypeSystem;

        public InsertPatternBuilder(IEntityOrm entityOrm, DataTypeSystem dataTypeSystem, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
            this._dataTypeSystem = dataTypeSystem;
        }

        public TResult BuildAndExecute<TResult>(PatternExecutionContex<TResult> executionContex)
        {
            var statement = executionContex.Statement as QueryInsertStatement;

            // построение запроса согласно патерна
            var pattern = this.Build(statement);
            
            // выполняем запросы согласно патерна
            return this.Execute(executionContex, statement, pattern);
        }

        private PS.InsertPattern Build(QueryInsertStatement statement)
        {
            var insertDescriptor = statement.InsertDecriptor;
            var selectDescriptor = statement.SelectDecriptor;


            var expressions = new List<PS.InsertExpression>();

            // в начале создаем записи в таблицах согласно цепочки наследования
            var baseEntitesExpressions = this.BuildInsertExpressionForBaseEntities(insertDescriptor);
            if (baseEntitesExpressions.Length > 0)
            {
                expressions.AddRange(baseEntitesExpressions);
            }

            // затем в таблицы основной сущности
            var mainEntityExpression = this.BuildInsertExpressionForMainEntity(insertDescriptor);
            expressions.Add(mainEntityExpression);

            // затем если есть ссылки на поля расширения, вставляем туда
            var extensionEntitesExpressions = this.BuildInsertExpressionForExtentionEntities(insertDescriptor);

            var pattren = new PS.InsertPattern
            {
                Expressions = expressions.ToArray()
            };

            return pattren;
        }

        private TResult Execute<TResult>(PatternExecutionContex<TResult> executionContex, QueryInsertStatement statement, PS.InsertPattern pattern)
        {
            var result = default(TResult);


            switch (executionContex.ResultKind)
            {
                case EngineExecutionResultKind.None:
                    executionContex.Executer.Execute(pattern);
                    break;
                case EngineExecutionResultKind.RowsAffected:
                    var execResult = pattern.DefResult<EngineExecutionRowsAffectedResult>();
                    executionContex.Executer.Execute(pattern);
                    result = (TResult)(object)execResult.RowsAffected; 
                    break;
                case EngineExecutionResultKind.Scalar:
                    // скалярный тип возвращает первичный ключ ввиде объекта IEntityName_PK
                    var scalarResult = pattern.DefResult<EngineExecutionScalarResult>();
                    scalarResult.Value = this.BuildPrimaryKeyObject(statement.InsertDecriptor.Entity);
                    executionContex.Executer.Execute(pattern);
                    result = (TResult)scalarResult.Value;
                    break;
                case EngineExecutionResultKind.Reader:
                    break;
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result kind '{executionContex.ResultKind}'");
            }


            return result;
        }

        private object BuildPrimaryKeyObject(IEntityMetadata entity)
        {
            var primaryKey = entity.DefinePrimaryKey();

            return null;
        }
        private PS.InsertExpression[] BuildInsertExpressionForBaseEntities(InsertQueryDescriptor descriptor)
        {
            var result = new List<PS.InsertExpression>();

            return result.ToArray();
        }
        private PS.InsertExpression BuildInsertExpressionForMainEntity(InsertQueryDescriptor descriptor)
        {
            var expression = new PS.InsertExpression
            {
                Alias = descriptor.Entity.QualifiedName,
                Target = this.BuildTargetByEntity(descriptor.Entity)
            };

            var owner = PS.EngineSource.CreateBy(expression.Target, descriptor.Entity.QualifiedName);

            var setValues = new List<PS.SetValueExpression>();

            // При налчии факта наследования, нужно автоматически внести сохранение полей первичного ключа

            // Фаза формирования локальных значений
            var fieldValues = descriptor.GetValues();
            // устанавливаем значение локальным полям
            for (int i = 0; i < fieldValues.Length; i++)
            {
                var fieldValue = fieldValues[i];
                if (fieldValue.Descriptor.IsLocal)
                {
                    // пытаемся определить им яв хранилище, если не удалось
                    // это означает что в данном контексте нельзя использовать поле
                    if (!fieldValue.Descriptor.TrySourceName(out string sourceName))
                    {
                        throw new InvalidOperationException($"Can't use the field '{fieldValue.Descriptor}' in the current context");
                    }

                    // подготовка значения поля
                    var storeValue = _dataTypeSystem.GetEncoder(fieldValue.Descriptor.Field.DataType).Encode(fieldValue.Store);
                    
                    // строим часть паттерна
                    var setValue = new PS.SetValueExpression
                    {
                        Property = new PS.DataEngineMember
                        {
                            Name = sourceName,
                            Owner = owner,
                            DataType = _dataTypeSystem.GetSourceDataType(fieldValue.Descriptor.Field.DataType.SourceVarType)
                        },
                        Expression = PS.ValueSourceExpression.CreateBy(storeValue)
                    };

                    setValues.Add(setValue);
                }
            }
            expression.Values = setValues.ToArray();
            return expression;
        }

        private PS.InsertExpression[] BuildInsertExpressionForExtentionEntities(InsertQueryDescriptor descriptor)
        {
            var result = new List<PS.InsertExpression>();

            return result.ToArray();
        }

        private PS.EngineObject BuildTargetByEntity(IEntityMetadata  entity)
        {
            var dataSource = entity.DataSource;
            switch (dataSource.Object)
            {
                case DataSourceObject.Table:
                    var table = new PS.EngineTable
                    {
                        Name = dataSource.Name,
                        Schema = dataSource.Schema
                    };
                    table.PrimaryKey = this.BuildTargetPrimaryKey(table, entity);
                    return table;
                case DataSourceObject.View:
                    var view = new PS.EngineTable
                    {
                        Name = dataSource.Name,
                        Schema = dataSource.Schema
                    };
                    return view;
                case DataSourceObject.Query:
                case DataSourceObject.File:
                default:
                    throw new InvalidOperationException($"Unsupported object type '{entity.DataSource.Object}'");
            }
        }

        private PS.EngineTablePrimaryKey BuildTargetPrimaryKey(PS.EngineTable owner, IEntityMetadata entity)
        {
            var primaryKey = new PS.EngineTablePrimaryKey()
            {
                
            };

            var pkFields = new List<PS.PrimaryKeyField>();
            var entityPrimaryKey = entity.DefinePrimaryKey();
            if (entityPrimaryKey != null && entityPrimaryKey.FieldRefs != null)
            {
                var fs = entityPrimaryKey.FieldRefs.Values.ToArray();
                for (int i = 0; i < fs.Length; i++)
                {
                    var entityPkField = fs[i];
                    var pkTargetField = new PS.PrimaryKeyField
                    {
                        Owner = new PS.EngineObjectEngineSource { Source = owner },
                        Name = entityPkField.Field.SourceName,
                        DataType = _dataTypeSystem.GetSourceDataType(entityPkField.Field.DataType.SourceVarType),
                        Generated = entityPkField.Field.DataType.Autonum != null
                    };
                    pkFields.Add(pkTargetField);
                }
            }
            primaryKey.Fields = pkFields.ToArray();
            primaryKey.Owner = new PS.EngineObjectEngineSource { Source = owner };
            return primaryKey;
        }
    }
}
