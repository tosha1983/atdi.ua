using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Platform;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;


namespace Atdi.CoreServices.EntityOrm.QueryPatterns
{
    internal sealed class UpdatePatternBuilder : LoggedObject, IPatternBuilder
    {
        
        class UpdateBuildingContext : JoinedBuildingContext
        {
            public UpdateBuildingContext(string contextName, DataTypeSystem dataTypeSystem)
                : base(contextName, dataTypeSystem)
            {
            }
        }
        private static readonly IStatisticCounterKey CounterKey = STS.DefineCounterKey("ORM.Entity.Patterns.Update");
        private readonly IEntityOrm _entityOrm;
        private readonly DataTypeSystem _dataTypeSystem;
        private readonly IStatistics _statistics;
        private readonly IStatisticCounter _counter;

        public UpdatePatternBuilder(IEntityOrm entityOrm, DataTypeSystem dataTypeSystem, IStatistics statistics, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
            this._dataTypeSystem = dataTypeSystem;
            this._statistics = statistics;
            this._counter = _statistics.Counter(CounterKey);
        }

        public TResult BuildAndExecute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContext)
        {
            _counter?.Increment();

            var statement = executionContext.Statement as QueryUpdateStatement;

            // построение запроса согласно патерна
            var pattern = this.Build(statement);

            // выполняем запросы согласно патерна
            return this.Execute(executionContext, statement, pattern);
        }

        /// <summary>
        /// Патер имеет две ветки развития событий
        /// Первая ветка для простого изменения, при котором изменения нужно сделать только в одной таблице  
        /// Вторая - сложный случай который захватывает обновление сразу нескольких таблиц и возможны перекресные условия - 
        /// например нужно обновит ьтаблицу 1 и 2 и есть два условия на поле из таблицы 1 и на поле из таблицы 2 , нужно проанализировать условие если ли покрытие первичного ключа (задействоаны все все поля)
        /// тут в начале обычным запросом вычитываем первичным ключи в отдельную временную таблицу 
        ///  и затем для каждой таблицы по первичному ключу делаем изменения связываясь только стаблицой первичных ключей
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        private PS.UpdatePattern Build(QueryUpdateStatement statement)
        {
            var descriptor = statement.UpdateDecriptor;
            var context = new UpdateBuildingContext("singlton", _dataTypeSystem);
            context.Cache = descriptor.Cache;

            var setValues = descriptor.GetValues();
            if (setValues.Length == 0)
            {
                throw new InvalidOperationException("Undefined any fields to change");
            }

            // анализируем набор полей и на его основании принимаем решение это простой случай или нет
            var updatedEntity = setValues[0].Descriptor.Field.Entity;
            var isUseComplexWay = false; 
            if (setValues.Length > 1)
            {
                for (int i = 1; i < setValues.Length; i++)
                {
                    if (updatedEntity.QualifiedName != setValues[i].Descriptor.Field.Entity.QualifiedName)
                    {
                        isUseComplexWay = true;
                        break;
                    }
                }
            }

            var pattren = new PS.UpdatePattern();
            if (isUseComplexWay)
            {
                pattren.Expressions = this.BuildComplexWay(setValues, descriptor, context);
            }
            else
            {
                var firstField = setValues[0].Descriptor;
                var path = firstField.ParentPath;
                pattren.Expressions = this.BuildSimpleWay(path, updatedEntity, setValues, descriptor, context);
            }

            return pattren;
        }

        private TResult Execute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex, QueryUpdateStatement statement, PS.UpdatePattern pattern)
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

        /// <summary>
        /// Простой случай, покрывающий изменения только в одной таблицы сущности
        /// </summary>
        /// <param name="entity">сущность, от имени которой инциировано изменение</param>
        /// /// <param name="entity">реально изменяемая сущность</param>
        /// <param name="allValues"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private PS.UpdateExpression[] BuildSimpleWay(string updatedPath, IEntityMetadata updatedEntity, FieldValueDescriptor[] fieldValues, UpdateQueryDescriptor descriptor, UpdateBuildingContext context)
        {
            var expression = new PS.UpdateExpression
            {
                Target = context.EnsureTarget(updatedPath, updatedEntity)
            };
            var baseObjects = new Dictionary<string, PS.TargetObject>();
            if (!string.IsNullOrEmpty(updatedPath))
            {
                baseObjects.Add(updatedPath, expression.Target);
            }
      
            var setValues = new PS.SetValueExpression[fieldValues.Length];

            // устанавливаем значение локальным полям
            for (int i = 0; i < fieldValues.Length; i++)
            {
                var fieldValue = fieldValues[i];

                // пытаемся определить имя в хранилище, если не удалось
                // это означает что в данном контексте нельзя использовать поле
                if (!fieldValue.Descriptor.TrySourceName(out string sourceName))
                {
                    throw new InvalidOperationException($"Can't use the field '{fieldValue.Descriptor}' in the current context");
                }

                // подготовка значения поля
                var storeValue = _dataTypeSystem.GetEncoder(fieldValue.Descriptor.Field.DataType).Encode(fieldValue.Store);

                // строим часть паттерна
                var valueDataType = fieldValue.Descriptor.Field.DataType.SourceCodeVarType;
                var setValue = new PS.SetValueExpression
                {
                    Property = new PS.DataEngineMember
                    {
                        Name = sourceName,
                        Owner = expression.Target,
                        Property = fieldValue.Descriptor.Path,
                        DataType = valueDataType
                    },
                    Expression = PS.ValueExpression.CreateBy(storeValue, valueDataType)
                };

                setValues[i] = setValue;

            }
            expression.Values = setValues;

            // джойним базовые зависимости
            var joins = new List<PS.JoinExpression>();
            var chainEntities = descriptor.Entity.DefineInheritChainWithMe();
            for (int i = 0; i < chainEntities.Length; i++)
            {
                var chainEntity = chainEntities[i];
                if (chainEntity.QualifiedName == updatedEntity.QualifiedName)
                {
                    continue;
                }
                var currentObject = context.EnsureTarget(null, chainEntity);
                var join = new PS.JoinExpression
                {
                    Target = currentObject,
                    Operation = PS.JoinOperationType.Inner,
                    Condition = context.BuildJoinConditionUsePrimaryKey(expression.Target, currentObject)
                };
                joins.Add(join);
            }

            // джойним зависимости которые будут нужны для проверки условия (референсы и расширения)
            var referenceFields = descriptor.Cache.Values.Where(fd => fd.IsReferece).ToArray();
            var entityJoins = context.ScanAndBuildJoinExpressions(baseObjects, referenceFields, expression.Target, updatedEntity);
            if (entityJoins != null && entityJoins.Length > 0)
            {
                joins.AddRange(entityJoins);
            }

            expression.Joins = joins.ToArray();

            if (descriptor.Conditions != null)
            {
                expression.Condition = context.BuildConditionExpressions(descriptor.Conditions.ToArray());
            }

            return new PS.UpdateExpression[] { expression };
        }

        private PS.UpdateExpression[] BuildComplexWay(FieldValueDescriptor[] fieldValues, UpdateQueryDescriptor descriptor, UpdateBuildingContext context)
        {
            throw new NotImplementedException("Please temporarily use the simple update option not going beyond the changes in one table.");
            //var updateDescriptor = statement.UpdateDecriptor;

            //var expressions = new List<PS.UpdateExpression>();
            //var context = new UpdateBuildingContext("singlton", _dataTypeSystem);
            //var allValues = updateDescriptor.GetValues();
            //var rootUpdateEntity = updateDescriptor.Entity;

            //// в начале создаем записи в таблицах согласно цепочки наследования
            //var baseEntitesExpressions = this.BuildUpdateExpressionForBaseEntities(rootUpdateEntity, allValues, context);
            //if (baseEntitesExpressions.Length > 0)
            //{
            //    expressions.AddRange(baseEntitesExpressions);
            //}

            //// затем в таблицы основной сущности
            //var mainEntityExpression = this.BuildUpdateExpressionForMainEntity(rootUpdateEntity, allValues, context);
            //if (mainEntityExpression != null)
            //{
            //    expressions.Add(mainEntityExpression);
            //}

            //// затем если есть ссылки на поля расширения, вставляем туда
            //var extensionEntitesExpressions = this.BuildUpdateExpressionForExtentionEntities(rootUpdateEntity, allValues, context);
            //if (extensionEntitesExpressions.Length > 0)
            //{
            //    expressions.AddRange(extensionEntitesExpressions);
            //}

            //return expressions.ToArray();
        }

        private PS.UpdateExpression[] BuildUpdateExpressionForBaseEntities(IEntityMetadata rootEntity, FieldValueDescriptor[] allValues, UpdateBuildingContext context)
        {
            var inheritChain = rootEntity.DefineInheritChain();
            var result = new List<PS.UpdateExpression>();// new PS.UpdateExpression[inheritChain.Length];
            for (int i = 0; i < inheritChain.Length; i++)
            {
                var baseEntity = inheritChain[i];
                var baseValues = allValues
                    .Where(f =>
                        (f.Descriptor.OwnerField.SourceType == FieldSourceType.Column
                        || f.Descriptor.OwnerField.SourceType == FieldSourceType.Reference)
                        && f.Descriptor.OwnerField.BelongsEntity(baseEntity)).ToArray();
                if (baseValues.Length > 0)
                {
                    var baseUpdateExpression = this.BuildUpdateExpressionByEntity(baseEntity, baseValues, context);
                    result.Add(baseUpdateExpression);
                }
                
            }
            return result.ToArray();
        }

        private PS.UpdateExpression[] BuildUpdateExpressionForExtentionEntities(IEntityMetadata rootEntity, FieldValueDescriptor[] allValues, UpdateBuildingContext context)
        {
            var result = new List<PS.UpdateExpression>();

            var inheritChain = rootEntity.DefineInheritChain();
            // получаем список все полей ссылающихся на расширение
            var allExtensionFields = rootEntity.DefineFieldsWithInherited().Where(f => f.SourceType == FieldSourceType.Extension).Reverse().ToArray();

            for (int i = 0; i < allExtensionFields.Length; i++)
            {
                var extensionField = allExtensionFields[i];
                // если поле обязательное, то мы обязан ысоздать там запись даже есл ни одно из полей расширения не заявлено для сохранения
                // или поля расширения заявлены на вставку
                var extensionEntity = extensionField.AsExtension().ExtensionEntity;
                var extensionValues = allValues
                    .Where(f =>
                        f.Descriptor.OwnerField.SourceType == FieldSourceType.Extension
                    && f.Descriptor.OwnerField.AsExtension().ExtensionEntity.QualifiedName == extensionEntity.QualifiedName
                        ).ToArray();
                if (extensionValues.Length > 0)
                {
                    var insertExpression = this.BuildUpdateExpressionByEntity(extensionEntity, extensionValues, context);
                    result.Add(insertExpression);
                }
            }
            return result.ToArray();
        }

        private PS.UpdateExpression BuildUpdateExpressionForMainEntity(IEntityMetadata rootEntity, FieldValueDescriptor[] allValues, UpdateBuildingContext context)
        {
            // только значения для локальных полей основной сущности
            var values = allValues
                .Where(f =>
                        (f.Descriptor.OwnerField.SourceType == FieldSourceType.Column
                        || f.Descriptor.OwnerField.SourceType == FieldSourceType.Reference)
                        && f.Descriptor.OwnerField.BelongsEntity(rootEntity)).ToArray();
            if (values.Length > 0)
            {
                return BuildUpdateExpressionByEntity(rootEntity, values, context);
            }
            return null;
        }

        private PS.UpdateExpression BuildUpdateExpressionByEntity(IEntityMetadata entity, FieldValueDescriptor[] fieldValues, UpdateBuildingContext context)
        {
            var expression = new PS.UpdateExpression
            {
                Target = context.BuildTargetByEntity(entity)
            };

            var setValues = new List<PS.SetValueExpression>();

            // устанавливаем значение локальным полям
            for (int i = 0; i < fieldValues.Length; i++)
            {
                var fieldValue = fieldValues[i];

                //if(fieldValue.Descriptor.OwnerField.SourceType == FieldSourceType.Reference)
                //{
                //    System.Diagnostics.Debug.WriteLine(fieldValue.Descriptor);
                //}
                //if (fieldValue.Descriptor.OwnerField.SourceType == FieldSourceType.Extension)
                //{
                //    System.Diagnostics.Debug.WriteLine(fieldValue.Descriptor);
                //}

                // пытаемся определить имя в хранилище, если не удалось
                // это означает что в данном контексте нельзя использовать поле
                if (!fieldValue.Descriptor.TrySourceName(out string sourceName))
                {
                    throw new InvalidOperationException($"Can't use the field '{fieldValue.Descriptor}' in the current context");
                }

                // подготовка значения поля
                var storeValue = _dataTypeSystem.GetEncoder(fieldValue.Descriptor.Field.DataType).Encode(fieldValue.Store);

                // строим часть паттерна
                var valueDataType = _dataTypeSystem.GetSourceDataType(fieldValue.Descriptor.Field.DataType.SourceVarType);
                var setValue = new PS.SetValueExpression
                {
                    Property = new PS.DataEngineMember
                    {
                        Name = sourceName,
                        Owner = expression.Target,
                        Property = fieldValue.Descriptor.Path,
                        DataType = valueDataType
                    },
                    Expression = PS.ValueExpression.CreateBy(storeValue, valueDataType)
                };

                setValues.Add(setValue);

            }
            expression.Values = setValues.ToArray();



            return expression;
        }
    }
}
