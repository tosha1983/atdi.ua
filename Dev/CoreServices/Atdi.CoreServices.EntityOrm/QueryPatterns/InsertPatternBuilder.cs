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
        class InsertBuildingContext : BuildingContext
        {
            public InsertBuildingContext(string contextName, DataTypeSystem dataTypeSystem) 
                : base(contextName, dataTypeSystem)
            {
            }
        }

        private readonly IEntityOrm _entityOrm;
        private readonly DataTypeSystem _dataTypeSystem;

        public InsertPatternBuilder(IEntityOrm entityOrm, DataTypeSystem dataTypeSystem, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
            this._dataTypeSystem = dataTypeSystem;
        }

        public TResult BuildAndExecute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex)
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
            //var selectDescriptor = statement.SelectDecriptor;

            var expressions = new List<PS.InsertExpression>();
            var context = new InsertBuildingContext("singlton", _dataTypeSystem);
            var allValues = insertDescriptor.GetValues();
            var rootInsertEntity = insertDescriptor.Entity;

            // в начале создаем записи в таблицах согласно цепочки наследования
            var baseEntitesExpressions = this.BuildInsertExpressionForBaseEntities(rootInsertEntity, allValues, context);
            if (baseEntitesExpressions.Length > 0)
            {
                expressions.AddRange(baseEntitesExpressions);
            }

            // затем в таблицы основной сущности
            var mainEntityExpression = this.BuildInsertExpressionForMainEntity(rootInsertEntity, allValues, context);
            expressions.Add(mainEntityExpression);

            // затем если есть ссылки на поля расширения, вставляем туда
            var extensionEntitesExpressions = this.BuildInsertExpressionForExtentionEntities(rootInsertEntity, allValues, context);
            if (extensionEntitesExpressions.Length > 0)
            {
                expressions.AddRange(extensionEntitesExpressions);
            }

            var pattren = new PS.InsertPattern
            {
                Expressions = expressions.ToArray(),
                PrimaryKeyFields = context?.BasePrimaryKeyFields?
                    .Select(pkField => new PS.NamedEngineMember
                    {
                        Owner = context.PrimaryKeyOwner,
                        Name = pkField.SourceName,
                        Property = pkField.Name
                    }).ToArray()
            };

            return pattren;
        }

        private TResult Execute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex, QueryInsertStatement statement, PS.InsertPattern pattern)
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
                    scalarResult.Value = this._entityOrm.CreatePrimaryKeyInstance(statement.InsertDecriptor.Entity); ;
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


        private PS.InsertExpression[] BuildInsertExpressionForBaseEntities(IEntityMetadata rootEntity, FieldValueDescriptor[] allValues, InsertBuildingContext context)
        {
            var inheritChain = rootEntity.DefineInheritChain();
            var result = new PS.InsertExpression[inheritChain.Length];
            for (int i = 0; i < inheritChain.Length; i++)
            {
                var baseEntity = inheritChain[i];
                var baseValues = allValues
                    .Where(f => 
                        (f.Descriptor.OwnerField.SourceType == FieldSourceType.Column 
                        || f.Descriptor.OwnerField.SourceType == FieldSourceType.Reference)
                        && f.Descriptor.OwnerField.BelongsEntity(baseEntity)).ToArray();
                var baseInsertExpression = this.BuildInsertExpressionByEntity(baseEntity, baseValues, context);
                result[i] = baseInsertExpression;
            }
            return result;
        }

        private PS.InsertExpression BuildInsertExpressionForMainEntity(IEntityMetadata rootEntity, FieldValueDescriptor[] allValues, InsertBuildingContext context)
        {
            // только значения для локальных полей основной сущности
            var values = allValues
                .Where(f =>
                        (f.Descriptor.OwnerField.SourceType == FieldSourceType.Column
                        || f.Descriptor.OwnerField.SourceType == FieldSourceType.Reference)
                        && f.Descriptor.OwnerField.BelongsEntity(rootEntity)).ToArray();
            return BuildInsertExpressionByEntity(rootEntity, values, context);
        }

        private PS.InsertExpression[] BuildInsertExpressionForExtentionEntities(IEntityMetadata rootEntity, FieldValueDescriptor[] allValues, InsertBuildingContext context)
        {
            var result = new List<PS.InsertExpression>();

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
                if (extensionField.Required || extensionValues.Length > 0)
                {
                    var insertExpression = this.BuildInsertExpressionByEntity(extensionEntity, extensionValues, context);
                    result.Add(insertExpression);
                }
            }
            return result.ToArray();
        }

        private PS.InsertExpression BuildInsertExpressionByEntity(IEntityMetadata entity, FieldValueDescriptor[] fieldValues, InsertBuildingContext context)
        {
            var expression = new PS.InsertExpression
            {
                Target = context.BuildTargetByEntity(entity)
            };

            var setValues = new List<PS.SetValueExpression>();

            // поля первичного ключа достались от базового объекта
            if (context.BasePrimaryKeyFields != null)
            {
                //просто добавляем в набор как по ссылке
                for (int i = 0; i < context.BasePrimaryKeyFields.Length; i++)
                {
                    var primaryKeyField = context.BasePrimaryKeyFields[i];
                    var setValue = new PS.SetValueExpression
                    {
                        Property = new PS.DataEngineMember
                        {
                            // владелец поля таблица этой сущности
                            Owner = expression.Target,
                            Name = primaryKeyField.SourceName,
                            Property = primaryKeyField.Name,
                            DataType = _dataTypeSystem.GetSourceDataType(primaryKeyField.DataType.SourceVarType)
                        },
                        // ссылка на значение полученное
                        Expression = new PS.ReferenceValueExpression
                        {
                            Member = new PS.NamedEngineMember
                            {
                                Owner = context.PrimaryKeyOwner,
                                Name = primaryKeyField.SourceName,
                                Property = primaryKeyField.Name
                            }
                        }
                    };
                    setValues.Add(setValue);
                }
            }
            else if (entity.TryGetPrimaryKeyRefFields(out IPrimaryKeyFieldRefMetadata[] pkFields))
            {
                // есть свой набор полей первичного ключа
                // добавляем поля требующие автогенерации 
                // и формируем в контексте набор полей входящих в первичный ключ
                var pkLocalFields = new List<IFieldMetadata>();
                for (int i = 0; i < pkFields.Length; i++)
                {
                    var pkField = pkFields[i];
                    pkLocalFields.Add(pkField.Field);
                    if (pkField.Field.DataType.Autonum != null)
                    {
                        var valueDataType = _dataTypeSystem.GetSourceDataType(pkField.Field.DataType.SourceVarType);
                        var setValue = new PS.SetValueExpression
                        {
                            Property = new PS.DataEngineMember
                            {
                                // владелец поля таблица этой сущности
                                Owner = expression.Target,
                                Name = pkField.Field.SourceName,
                                Property = pkField.Field.Name,
                                DataType = valueDataType
                            },
                            // ссылка на значение полученное
                            Expression = new PS.GeneratedValueExpression(valueDataType)
                            {
                                Operation = PS.GeneratedValueOperation.SetNext
                            }
                        };
                        setValues.Add(setValue);
                    }
                }
                context.BasePrimaryKeyFields = pkLocalFields.ToArray();
                context.PrimaryKeyOwner = expression.Target;
            }
            // При налчии факта наследования, нужно автоматически внести сохранение полей первичного ключа

            // Фаза формирования локальных значений
            
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
