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
        class BuildingContext
        {
            private int _aliasCounter = 0;

            public IFieldMetadata[] BasePrimaryKeyFields { get; set; }
            public PS.TargetObject PrimaryKeyOwner { get; set; }

            public string GenerateAlias(IEntityMetadata entity)
            {
                ++_aliasCounter;
                return $"{entity.Name}_{_aliasCounter}";
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
            var context = new BuildingContext();
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


        private PS.InsertExpression[] BuildInsertExpressionForBaseEntities(IEntityMetadata rootEntity, FieldValueDescriptor[] allValues, BuildingContext context)
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
        private PS.InsertExpression BuildInsertExpressionForMainEntity(IEntityMetadata rootEntity, FieldValueDescriptor[] allValues, BuildingContext context)
        {
            // только значения для локальных полей основной сущности
            var values = allValues
                .Where(f =>
                        (f.Descriptor.OwnerField.SourceType == FieldSourceType.Column
                        || f.Descriptor.OwnerField.SourceType == FieldSourceType.Reference)
                        && f.Descriptor.OwnerField.BelongsEntity(rootEntity)).ToArray();
            return BuildInsertExpressionByEntity(rootEntity, values, context);
        }

        private PS.InsertExpression[] BuildInsertExpressionForExtentionEntities(IEntityMetadata rootEntity, FieldValueDescriptor[] allValues, BuildingContext context)
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
        private PS.InsertExpression BuildInsertExpressionByEntity(IEntityMetadata entity, FieldValueDescriptor[] fieldValues, BuildingContext context)
        {
            var expression = new PS.InsertExpression
            {
                Target = this.BuildTargetByEntity(entity, context)
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
                        var setValue = new PS.SetValueExpression
                        {
                            Property = new PS.DataEngineMember
                            {
                                // владелец поля таблица этой сущности
                                Owner = expression.Target,
                                Name = pkField.Field.SourceName,
                                Property = pkField.Field.Name,
                                DataType = _dataTypeSystem.GetSourceDataType(pkField.Field.DataType.SourceVarType)
                            },
                            // ссылка на значение полученное
                            Expression = new PS.GeneratedValueExpression
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
                //if (fieldValue.Descriptor.IsLocal)
                //{
                    // пытаемся определить имя в хранилище, если не удалось
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
                            Owner = expression.Target,
                            Property = fieldValue.Descriptor.Path,
                            DataType = _dataTypeSystem.GetSourceDataType(fieldValue.Descriptor.Field.DataType.SourceVarType)
                        },
                        Expression = PS.ValueExpression.CreateBy(storeValue)
                    };

                    setValues.Add(setValue);
                //}
            }
            expression.Values = setValues.ToArray();
            return expression;
        }

        

        private PS.EngineObject BuildTargetByEntity(IEntityMetadata  entity, BuildingContext context)
        {
            var dataSource = entity.DataSource;
            switch (dataSource.Object)
            {
                case DataSourceObject.Table:
                    var table = new PS.EngineTable
                    {
                        Alias = context.GenerateAlias(entity),
                        Name = dataSource.Name,
                        Schema = dataSource.Schema
                    };
                    table.PrimaryKey = this.BuildTargetPrimaryKey(table, entity);
                    return table;
                case DataSourceObject.View:
                    var view = new PS.EngineTable
                    {
                        Alias = context.GenerateAlias(entity),
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
                        Owner = owner,
                        Name = entityPkField.Field.SourceName,
                        Property = entityPkField.Field.Name,
                        DataType = _dataTypeSystem.GetSourceDataType(entityPkField.Field.DataType.SourceVarType),
                        Generated = entityPkField.Field.DataType.Autonum != null
                    };
                    pkFields.Add(pkTargetField);
                }
            }
            primaryKey.Fields = pkFields.ToArray();
            primaryKey.Owner = owner;
            return primaryKey;
        }
    }
}
