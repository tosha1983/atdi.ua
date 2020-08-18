using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using CS = Atdi.DataModels.DataConstraint;
using Atdi.Platform;

namespace Atdi.CoreServices.EntityOrm.QueryPatterns
{

    internal sealed class SelectPatternBuilder : LoggedObject, IPatternBuilder
    {
        class SelectBuildingContext : BuildingContext
        {
            public SelectBuildingContext(string contextName, DataTypeSystem dataTypeSystem) 
                : base(contextName, dataTypeSystem)
            {
            }

            public IDataTypeMetadata[] TypeMetadatas { get; set; }

            public IReadOnlyDictionary<string, FieldDescriptor> Fields { get; set; }
        }

        private static readonly IStatisticCounterKey CounterKey = STS.DefineCounterKey("ORM.Entity.Patterns.Select");

       
        private readonly DataTypeSystem _dataTypeSystem;
        private readonly IStatistics _statistics;
        private readonly IStatisticCounter _counter;

        public SelectPatternBuilder(DataTypeSystem dataTypeSystem, IStatistics statistics, ILogger logger) : base(logger)
        {
	        this._dataTypeSystem = dataTypeSystem;
            this._statistics = statistics;
            this._counter = _statistics.Counter(CounterKey);
        }

        public TResult BuildAndExecute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContext)
        {
            _counter?.Increment();

            var statement = executionContext.Statement as QuerySelectStatement;

            var context = new SelectBuildingContext("singlton", _dataTypeSystem);
            // построение запроса согласно патерна
            var pattern = this.Build(statement, context);

            // выполняем запросы согласно патерна
            return this.Execute(executionContext, pattern, context);
        }

        private PS.SelectPattern Build(QuerySelectStatement statement, SelectBuildingContext context)
        {
            // формируем столбур выборки состоящий из самой базовой таблицы и приджойненых к ней наследников
            var query = statement.SelectDescriptor;
            context.Fields = query.Cache;

            var selectExpression = new PS.SelectExpression()
            {
                Distinct = query.IsDistinct,
                Limit = query.Limit
            };

            // раскидываем колонки по сущностям
            var chainWithMe = query.Entity.DefineInheritChainWithMe();

            var joins = new List<PS.JoinExpression>();

            for (int i = 0; i < chainWithMe.Length; i++)
            {
                var currentEntity = chainWithMe[i];
                var currentObject = context.EnsureTarget(null, currentEntity);
                // нулевой будет корнем выболрки при любых раскладах, он же диктует превичный ключ
                if (i == 0)
                {
                    selectExpression.From = currentObject;
                }
                else
                {
                    var join = new PS.JoinExpression
                    {
                        Target = currentObject,
                        Operation = PS.JoinOperationType.Inner,
                        Condition = this.BuildJoinConditionUsePrimaryKey(selectExpression.From, currentObject)
                    };
                    joins.Add(join);
                }
            }

            var referenceFields = query.Cache.Values.Where(fd => fd.IsReferece).ToArray();
            var entityJoins = this.ScanAndBuildJoinExpressions(referenceFields, selectExpression.From, chainWithMe[0], context);
            if (entityJoins != null && entityJoins.Length > 0)
            {
                joins.AddRange(entityJoins);
            }

            var columns = this.BuildColumns(query.SelectableFields.Values.ToArray(), context);
            selectExpression.Columns = columns.ToArray();
            selectExpression.Joins = joins.ToArray();

            if (query.Conditions != null)
            {
                selectExpression.Condition = this.BuildConditionExpressions(query.Conditions.ToArray(), context);
            }

            selectExpression.OffsetRows = -1;

			if (query.SortableFields != null)
            {
                selectExpression.Sorting = this.BuildSorting(query.SortableFields.ToArray(), context);

                if (query.OffsetRows >= 0)
                {
	                selectExpression.OffsetRows = query.OffsetRows;

                }
            }

            var pattern = new PS.SelectPattern
            {
                Expressions = new PS.SelectExpression[] { selectExpression }
            };

            return pattern;
        }

        private PS.JoinExpression[] ScanAndBuildJoinExpressions(FieldDescriptor[] fields, PS.TargetObject rootObject, IEntityMetadata rootEntity, SelectBuildingContext context)
        {
            var joins = new List<PS.JoinExpression>();
            var baseObjects = new Dictionary<string, PS.TargetObject>();
            for (int i = 0; i < fields.Length; i++)
            {
                var descriptor = fields[i];
                var reference = descriptor.Reference.Next;
                var baseObject = rootObject;
                var baseEntity = rootEntity;
                var baseField = descriptor.OwnerField;
                var basePath = descriptor.OwnerField.Name;
                while (reference != null)
                {
                    if (!baseObjects.ContainsKey(basePath))
                    {
                        var entityJoins = this.BuildEntityJoinExpressions(baseObject, baseEntity, baseField, basePath, reference.Entity, context);
                        joins.AddRange(entityJoins);
                        baseObject = entityJoins[0].Target;
                        baseObjects.Add(basePath, baseObject);
                    }
                    
                    if (reference.Next != null)
                    {
                        baseObject = baseObjects[basePath];
                        baseEntity = reference.Entity;
                        baseField = reference.Field;
                        basePath = reference.Path;
                    }
                    reference = reference.Next;
                }
            }
            return joins.ToArray();
        }

        private PS.JoinExpression[] BuildEntityJoinExpressions(PS.TargetObject baseObject, IEntityMetadata baseEntity, IFieldMetadata baseField, string basePath, IEntityMetadata joinedEntity, SelectBuildingContext context)
        {
            switch (baseField.SourceType)
            {
                case FieldSourceType.Reference:
                    return this.BuildEntityJoinExpressionsAsReference(baseObject, baseEntity, baseField as ReferenceFieldMetadata, basePath, joinedEntity, context);
                case FieldSourceType.Extension:
                    return this.BuildEntityJoinExpressionsAsExtension(baseObject, baseEntity, baseField as ExtensionFieldMetadata, basePath, joinedEntity, context);
                case FieldSourceType.Relation:
                    return this.BuildEntityJoinExpressionsAsRelation(baseObject, baseEntity, baseField as RelationFieldMetadata, basePath, joinedEntity, context);
                default:
                    throw new InvalidOperationException($"Cann't use the field '{basePath}' this context of the entity joining");
            }
        }

        private PS.JoinExpression[] BuildEntityJoinExpressionsAsExtension(PS.TargetObject baseObject, IEntityMetadata baseEntity, ExtensionFieldMetadata baseField, string basePath, IEntityMetadata joinedEntity, SelectBuildingContext context)
        {
            var joins = new PS.JoinExpression[1];
            var currentObject = context.EnsureTarget(basePath, joinedEntity);
            var join = new PS.JoinExpression
            {
                Target = currentObject,
                Operation = PS.JoinOperationType.Left, // пока без жосткой связи
                Condition = this.BuildJoinConditionUsePrimaryKey(baseObject, currentObject)
            };
            joins[0] = join;
            return joins;
        }

        private PS.JoinExpression[] BuildEntityJoinExpressionsAsReference(PS.TargetObject baseObject, IEntityMetadata baseEntity, ReferenceFieldMetadata baseField, string basePath, IEntityMetadata joinedEntity, SelectBuildingContext context)
        {
            var chainWithMe = joinedEntity.DefineInheritChainWithMe();
            var joins = new PS.JoinExpression[chainWithMe.Length];

            var rootEntity = chainWithMe[0];
            var rootObject = context.EnsureTarget(basePath, rootEntity);

            joins[0] = new PS.JoinExpression
            {
                Target = rootObject,
                Operation = PS.JoinOperationType.Left, // пока без жосткой связи
                Condition = this.BuildJoinConditionForReference(baseEntity, baseField, basePath, rootObject, context)
            };

            if (chainWithMe.Length > 1)
            {
                for (int i = 0; i < chainWithMe.Length; i++)
                {
                    var currentEntity = chainWithMe[i];
                    var currentObject = context.EnsureTarget(basePath, currentEntity);
                    var join = new PS.JoinExpression
                    {
                        Target = currentObject,
                        Operation = PS.JoinOperationType.Left, // пока без жосткой связи
                        Condition = this.BuildJoinConditionUsePrimaryKey(rootObject, currentObject)
                    };
                    joins[i] = join;
                }
            }
            
            return joins;
        }

        private PS.ConditionExpression BuildJoinConditionForReference(IEntityMetadata baseEntity, ReferenceFieldMetadata baseField, string basePath, PS.TargetObject joinedObject, SelectBuildingContext context)
        {
            if (joinedObject is PS.EngineTable joinedTable)
            {
                var joinedPkFields = joinedTable.PrimaryKey.Fields;
                var conditions = new PS.TwoOperandConditionExpression[joinedPkFields.Length];
                for (int i = 0; i < joinedPkFields.Length; i++)
                {
                    var joinPkField = joinedPkFields[i];
                    var baseOperand = this.FindBaseMamberForReference(joinPkField, baseEntity, baseField, basePath, context);
                    conditions[i] = new PS.TwoOperandConditionExpression
                    {
                        Operator = PS.TwoOperandOperator.Equal,
                        LeftOperand = new PS.MemberOperandExpression
                        {
                            Member = joinPkField
                        },
                        RightOperand = baseOperand
                    };
                }
                var joinCondition = new PS.ComplexConditionExpression
                {
                    Operator = PS.ConditionLogicalOperator.And,
                    Conditions = conditions
                };

                return joinCondition;
            }
            throw new InvalidOperationException($"No support for this type of object: joined = '{joinedObject.GetType().FullName}'");
        }

        private PS.OperandExpression FindBaseMamberForReference(PS.PrimaryKeyField byKey, IEntityMetadata baseEntity, ReferenceFieldMetadata baseField, string basePath, SelectBuildingContext context)
        {
            var sourceName = string.Empty;
            if ( baseField.Mapping != null)
            {
                var pk = baseField.Mapping.Fields[byKey.Property];
                switch (pk.MatchWith)
                {
                    case PrimaryKeyMappedMatchWith.Field:
                        sourceName = (pk as IFieldPrimaryKeyFieldMappedMetadata).EntityField.SourceName;
                        break;
                    case PrimaryKeyMappedMatchWith.SourceName :
                        sourceName = (pk as ISourceNamePrimaryKeyFieldMappedMetadata).SourceName;
                        break;
                    case PrimaryKeyMappedMatchWith.Value:
                        return new PS.ValueOperandExpression
                        {
                            Expression = new PS.ConstantValueExpression(byKey.DataType)
                            {
                                Value = (pk as IValuePrimaryKeyFieldMappedMetadata).Value.GetValue()
                            }
                        };
                    default:
                        break;
                }
            }
            else
            {
                sourceName = $"{baseField.Name}_{byKey.Name}";
            }
            var operand = new PS.MemberOperandExpression
            {
                Member = new PS.DataEngineMember
                {
                    DataType = byKey.DataType,
                    Name = sourceName,
                    Property = basePath + "." + byKey.Property,
                    Owner = context.EnsureTarget(FieldDescriptor.BuildParentPath(basePath), baseField.Entity)
                }
            };
            return operand;
        }

        private PS.JoinExpression[] BuildEntityJoinExpressionsAsRelation(PS.TargetObject baseObject, IEntityMetadata baseEntity, RelationFieldMetadata baseField, string basePath, IEntityMetadata joinedEntity, SelectBuildingContext context)
        {
            var joins = new List<PS.JoinExpression>();
            return joins.ToArray();
        }

        private PS.ColumnExpression[] BuildColumns(FieldDescriptor[] selectableFields, SelectBuildingContext context)
        {
            var columns = new PS.ColumnExpression[selectableFields.Length];
            for (int i = 0; i < selectableFields.Length; i++)
            {
                var descriptor = selectableFields[i];
                columns[i] = new PS.MemberColumnExpression
                {
                    Member = new PS.DataEngineMember
                    {
                        DataType = descriptor.Field.DataType.SourceCodeVarType,
                        Name = descriptor.Field.SourceName,
                        Property = descriptor.Path,
                        Owner = context.EnsureTarget(descriptor)
                    }
                };
            }
            return columns;
        }

        private PS.SortExpression[] BuildSorting(SortableFieldDescriptor[] sortableFields, SelectBuildingContext context)
        {
            var result = new PS.SortExpression[sortableFields.Length];
            for (int i = 0; i < sortableFields.Length; i++)
            {
                var sortableField = sortableFields[i];
                result[i] = new PS.SortExpression
                {
                    Direction = sortableField.Direction == DataModels.DataConstraint.SortDirection.Ascending ? PS.SortingDirection.Ascending : PS.SortingDirection.Descending,
                    Member = new PS.NamedEngineMember
                    {
                        Name = sortableField.Field.Field.SourceName,
                        Property = sortableField.Field.Path,
                        Owner = context.EnsureTarget(sortableField.Field)
                    }
                };
            }
            return result;
        }

        private PS.ConditionExpression BuildConditionExpressions(DataModels.DataConstraint.Condition[] conditions, SelectBuildingContext context)
        {
            if (conditions.Length == 0)
            {
                return null;
            }
            if (conditions.Length == 1)
            {
                return this.BuildConditionExpression(conditions[0], context);
            }

            var condition = new PS.ComplexConditionExpression
            {
                Operator = PS.ConditionLogicalOperator.And,
                Conditions = new PS.ConditionExpression[conditions.Length]
            };

            for (int i = 0; i < conditions.Length; i++)
            {
                condition.Conditions[i] = this.BuildConditionExpression(conditions[i], context);
            }
            return condition;
        }

        private PS.ConditionExpression BuildConditionExpression(DataModels.DataConstraint.Condition condition, SelectBuildingContext context)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            switch (condition.Type)
            {
                case CS.ConditionType.Complex:
                    return this.BuildConditionExpression(condition as CS.ComplexCondition, context);
                case CS.ConditionType.Expression:
                    return this.BuildConditionExpression(condition as CS.ConditionExpression, context);
                default:
                    throw new InvalidOperationException($"Unsupported condition type '{condition.Type}'");
            }
        }

        private PS.ConditionExpression BuildConditionExpression(CS.ComplexCondition condition, SelectBuildingContext context)
        {
            var conditionExpression = new PS.ComplexConditionExpression
            {
                Operator = PS.ConditionExpression.LogicalOperator(condition.Operator),
                Conditions = new PS.ConditionExpression[condition.Conditions.Length]
            };

            for (int i = 0; i < condition.Conditions.Length; i++)
            {
                conditionExpression.Conditions[i] = this.BuildConditionExpression(condition.Conditions[i], context);
            }
            return conditionExpression;
        }

        private PS.ConditionExpression BuildConditionExpression(CS.ConditionExpression condition, SelectBuildingContext context)
        {
            switch (condition.Operator)
            {
                case CS.ConditionOperator.Equal:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.Equal, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.GreaterEqual:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.GreaterEqual, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.GreaterThan:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.GreaterThan, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.LessEqual:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.LessEqual, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.LessThan:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.LessThan, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.NotEqual:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotEqual, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.IsNull:
                    return this.BuildOneOperandConditionExpression(PS.OneOperandOperator.IsNull, condition.LeftOperand, context);
                case CS.ConditionOperator.IsNotNull:
                    return this.BuildOneOperandConditionExpression(PS.OneOperandOperator.IsNotNull, condition.LeftOperand, context);
                case CS.ConditionOperator.Like:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.Like, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.NotLike:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotLike, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.In:
                    return this.BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator.In, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.NotIn:
                    return this.BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator.NotIn, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.Between:
                    return this.BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator.Between, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.NotBetween:
                    return this.BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator.NotBetween, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.BeginWith:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.BeginWith, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.EndWith:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.EndWith, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.Contains:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.Contains, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.NotBeginWith:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotBeginWith, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.NotEndWith:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotEndWith, condition.LeftOperand, condition.RightOperand, context);
                case CS.ConditionOperator.NotContains:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotContains, condition.LeftOperand, condition.RightOperand, context);
                default:
                    throw new InvalidOperationException($"Unsupported condition operator '{condition.Operator}'");
            }
        }

        private PS.OneOperandConditionExpression BuildOneOperandConditionExpression(PS.OneOperandOperator @operator, CS.Operand operand, SelectBuildingContext context)
        {
            var conditionExpression = new PS.OneOperandConditionExpression
            {
                Operator = @operator,
                Operand = this.BuildConditionOperandExpression(operand, context)
            };
            return conditionExpression;
        }

        private PS.TwoOperandConditionExpression BuildTwoOperandConditionExpression(PS.TwoOperandOperator @operator, CS.Operand leftOperand, CS.Operand rightOperand, SelectBuildingContext context)
        {
            var conditionExpression = new PS.TwoOperandConditionExpression
            {
                Operator = @operator,
                LeftOperand = this.BuildConditionOperandExpression(leftOperand, context),
                RightOperand = this.BuildConditionOperandExpression(rightOperand, context)
            };
            return conditionExpression;
        }

        private PS.MoreOperandsConditionExpression BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator @operator, CS.Operand leftOperand, CS.Operand rightOperand, SelectBuildingContext context)
        {
            var conditionExpression = new PS.MoreOperandsConditionExpression
            {
                Operator = @operator,
                Test = this.BuildConditionOperandExpression(leftOperand, context)
            };

            if (rightOperand.Type != CS.OperandType.Values)
            {
                throw new InvalidOperationException($"Unsupported operand type '{rightOperand.Type}' this context. Exprected the operand type is Values");
            }
            var values = rightOperand as CS.ValuesOperand;
            var operands = this.BuildConditionOperandExpression(values, context);
            switch (@operator)
            {
                case PS.MoreOperandsOperator.In:
                    if (operands.Length < 1)
                    {
                        throw new InvalidOperationException($"Invalid number of values for operator In");
                    }
                    break;
                case PS.MoreOperandsOperator.NotIn:
                    if (operands.Length < 1)
                    {
                        throw new InvalidOperationException($"Invalid number of values for operator Not In");
                    }
                    break;
                case PS.MoreOperandsOperator.Between:
                    if (operands.Length < 2)
                    {
                        throw new InvalidOperationException($"Invalid number of values for operator Between");
                    }
                    break;
                case PS.MoreOperandsOperator.NotBetween:
                    if (operands.Length < 2)
                    {
                        throw new InvalidOperationException($"Invalid number of values for operator Not Between");
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported operands operator '{@operator}'");
            }
            conditionExpression.Operands = operands;
            return conditionExpression;
        }

        private PS.OperandExpression BuildConditionOperandExpression(CS.Operand operand, SelectBuildingContext context)
        {
            switch (operand.Type)
            {
                case CS.OperandType.Column:
                    return this.BuildConditionOperandExpression(operand as CS.ColumnOperand, context);
                case CS.OperandType.Value:
                    return this.BuildConditionOperandExpression(operand as CS.ValueOperand, context);
                case CS.OperandType.Values:
                default:
                    throw new InvalidOperationException($"Unsupported operand type '{operand.Type}'");
            }
        }

        private PS.MemberOperandExpression BuildConditionOperandExpression(CS.ColumnOperand operand, SelectBuildingContext context)
        {
            if (!context.Fields.TryGetValue(operand.ColumnName, out FieldDescriptor descriptor))
            {
                throw new InvalidOperationException($"A field is not found by path '{operand.ColumnName}'");
            }
            var memberOperand = new PS.MemberOperandExpression
            {
                Member = new PS.DataEngineMember
                {
                    Name = descriptor.Field.SourceName,
                    DataType = descriptor.Field.DataType.SourceCodeVarType,
                    Property = descriptor.Path,
                    Owner = context.EnsureTarget(descriptor)
                }
            };

            return memberOperand;
        }
        private PS.ValueOperandExpression BuildConditionOperandExpression(CS.ValueOperand operand, SelectBuildingContext context)
        {
            var valueOperand = new PS.ValueOperandExpression
            {
                Expression = new PS.ConstantValueExpression(operand.DataType)
                {
                    Value = operand.GetValue(),
                }
            };
            return valueOperand;
        }

        private PS.ValueOperandExpression[] BuildConditionOperandExpression(CS.ValuesOperand operand, SelectBuildingContext context)
        {
            switch (operand.DataType)
            {
                case DataModels.DataType.String:
                    return ((CS.StringValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Boolean:
                    return ((CS.BooleanValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Integer:
                    return ((CS.IntegerValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.DateTime:
                    return ((CS.DateTimeValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Double:
                    return ((CS.DoubleValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Float:
                    return ((CS.FloatValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Decimal:
                    return ((CS.DecimalValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Byte:
                    return ((CS.ByteValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Bytes:
                    return ((CS.BytesValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Guid:
                    return ((CS.GuidValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.DateTimeOffset:
                    return ((CS.DateTimeOffsetValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Time:
                    return ((CS.TimeValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Date:
                    return ((CS.DateValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Long:
                    return ((CS.LongValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Short:
                    return ((CS.ShortValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Char:
                    return ((CS.CharValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.SignedByte:
                    return ((CS.SignedByteValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.UnsignedShort:
                    return ((CS.UnsignedShortValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.UnsignedInteger:
                    return ((CS.StringValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.UnsignedLong:
                    return ((CS.UnsignedLongValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.ClrType:
                    return ((CS.ClrTypeValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.ClrEnum:
                    return ((CS.ClrEnumValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Xml:
                    return ((CS.XmlValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Json:
                    return ((CS.JsonValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Chars:
                    return ((CS.CharsValuesOperand)operand).Values.Select(v => new PS.ValueOperandExpression { Expression = (new PS.ConstantValueExpression(operand.DataType) { Value = v }) }).ToArray();
                case DataModels.DataType.Undefined:
                default:
                    throw new InvalidOperationException($"Unsupported data type '{operand.Type}'");
            }
        }

        private PS.ConditionExpression BuildJoinConditionUsePrimaryKey(PS.TargetObject baseObject, PS.TargetObject joinedObject)
        {
            if (baseObject is PS.EngineTable baseTable && joinedObject is PS.EngineTable joinedTable)
            {
                var basePkFields = baseTable.PrimaryKey.Fields;
                var conditions = new PS.TwoOperandConditionExpression[basePkFields.Length];
                for (int i = 0; i < basePkFields.Length; i++)
                {
                    var basePkField = basePkFields[i];
                    var joinPkField = joinedTable.PrimaryKey.Fields.First(f => f.Name == basePkField.Name);
                    conditions[i] = new PS.TwoOperandConditionExpression
                    {
                       Operator = PS.TwoOperandOperator.Equal,
                       LeftOperand = new PS.MemberOperandExpression
                       {
                           Member = joinPkField
                       },
                       RightOperand = new PS.MemberOperandExpression
                       {
                           Member = basePkField
                       }
                    };
                }
                var joinCondition = new PS.ComplexConditionExpression
                {
                    Operator = PS.ConditionLogicalOperator.And,
                    Conditions = conditions
                };

                return joinCondition;
            }
            throw new InvalidOperationException($"No support for this type of object: base = '{baseObject.GetType().FullName}, joined = '{joinedObject.GetType().FullName}'");
        }

        private TResult Execute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex, PS.SelectPattern pattern, SelectBuildingContext context)
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
                    // скалярный тип возвращает что то одно... пока то что сможет вернуть провайдер
                    var scalarResult = pattern.DefResult<EngineExecutionScalarResult>();
                    executionContex.Executer.Execute(pattern);
                    result = (TResult)scalarResult.Value;
                    break;
                case EngineExecutionResultKind.Reader:
                    var readerResult = pattern.DefResult<EngineExecutionReaderResult>();
                    readerResult.Handler = (reader) => 
                    {
                        var dataTypes = new IDataTypeMetadata[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var path = reader.GetPath(i);
                            dataTypes[i] = context.Fields[path].Field.DataType;
                        }
                        if (executionContex is PatternExecutionContexWithHandler<TResult, TModel> conetx1)
                        {
                            var ormReader = new QueryDataReader<TModel>(reader, dataTypes, _dataTypeSystem);
                            result = conetx1.Handler(ormReader);
                        }
                        else if ((object)executionContex is PatternExecutionContexWithHandler<TResult> conetx2)
                        {
                            var ormReader = new QueryDataReader(reader, dataTypes, _dataTypeSystem);
                            result = conetx2.Handler(ormReader);
                        }
                    };
                    executionContex.Executer.Execute(pattern);
                    break;
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result kind '{executionContex.ResultKind}'");
            }


            return result;
        }
    }
}
