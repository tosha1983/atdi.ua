using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.DataLayer;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = Atdi.DataModels.DataConstraint;

namespace Atdi.CoreServices.EntityOrm.QueryPatterns
{
    class BuildingContext
    {
        private class IterationContext
        {
            private readonly Dictionary<string, PS.EngineObject> _objects;
            public IterationContext(string name)
            {
                this.Name = name;
                this._objects = new Dictionary<string, PS.EngineObject>();
            }

            public string Name { get;  }

            public PS.EngineObject EnsureTarget(string path, IEntityMetadata entity)
            {
                var alias = this.BuildAlias(path, entity.QualifiedName);
                if (!_objects.TryGetValue(alias, out PS.EngineObject engineObject))
                {
                    var dataSource = entity.DataSource;
                    switch (dataSource.Object)
                    {
                        case DataSourceObject.Table:
                            var table = new PS.EngineTable
                            {
                                Alias = alias,
                                Name = dataSource.Name,
                                Schema = dataSource.Schema
                            };
                            table.PrimaryKey = this.BuildTargetPrimaryKey(table, entity);
                            _objects.Add(alias, table);
                            return table;
                        case DataSourceObject.View:
                            var view = new PS.EngineTable
                            {
                                Alias = alias,
                                Name = dataSource.Name,
                                Schema = dataSource.Schema
                            };
                            _objects.Add(alias, view);
                            return view;
                        case DataSourceObject.Service:
                            var service = new PS.EngineService
                            {
                                Alias = alias,
                                Name = dataSource.Name,
                                Schema = dataSource.Schema
                            };
                            service.PrimaryKey = this.BuildTargetPrimaryKey(service, entity);
                            _objects.Add(alias, service);
                            return service;
                        case DataSourceObject.Query:
                        case DataSourceObject.File:
                        default:
                            throw new InvalidOperationException($"Unsupported object type '{entity.DataSource.Object}'");
                    }
                }
                return engineObject;
            }

            private PS.EngineTablePrimaryKey BuildTargetPrimaryKey(PS.TargetObject owner, IEntityMetadata entity)
            {
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
                            DataType = entityPkField.Field.DataType.SourceCodeVarType,
                            Generated = entityPkField.Field.DataType.Autonum != null
                        };
                        pkFields.Add(pkTargetField);
                    }
                }
                var primaryKey = new PS.EngineTablePrimaryKey()
                {
                    Fields = pkFields.ToArray(),
                    Owner = owner
                };
                return primaryKey;
            }

            private string BuildAlias(string path, string entityName)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return $"[{this.Name}].ROOT.[{entityName}]";
                }
                return $"[{this.Name}].ROOT.{path}.[{entityName}]";
            }
        }

        private int _aliasCounter = 0;
        private readonly DataTypeSystem _dataTypeSystem;
        private readonly Dictionary<string, IterationContext> _iterations;
        private IterationContext _currentContext;

        public BuildingContext(string contextName, DataTypeSystem dataTypeSystem)
        {
            this._dataTypeSystem = dataTypeSystem;
            this._iterations = new Dictionary<string, IterationContext>();
            this.SwithContext(contextName);
        }

        public IFieldMetadata[] BasePrimaryKeyFields { get; set; }

        public PS.TargetObject PrimaryKeyOwner { get; set; }

        public string GenerateAlias(IEntityMetadata entity)
        {
            ++_aliasCounter;
            return $"{entity.Name}_{_aliasCounter}";
        }

        public PS.EngineObject BuildTargetByEntity(IEntityMetadata entity)
        {
            var dataSource = entity.DataSource;
            switch (dataSource.Object)
            {
                case DataSourceObject.Table:
                    var table = new PS.EngineTable
                    {
                        Alias = this.GenerateAlias(entity),
                        Name = dataSource.Name,
                        Schema = dataSource.Schema
                    };
                    table.PrimaryKey = this.BuildTargetPrimaryKey(table, entity);
                    return table;
                case DataSourceObject.View:
                    var view = new PS.EngineTable
                    {
                        Alias = this.GenerateAlias(entity),
                        Name = dataSource.Name,
                        Schema = dataSource.Schema
                    };
                    return view;
                case DataSourceObject.Service:
                    var service = new PS.EngineService
                    {
                        Alias = this.GenerateAlias(entity),
                        Name = dataSource.Name,
                        Schema = dataSource.Schema
                    };
                    service.PrimaryKey = this.BuildTargetPrimaryKey(service, entity);
                    return service;
                case DataSourceObject.Query:
                case DataSourceObject.File:
                default:
                    throw new InvalidOperationException($"Unsupported object type '{entity.DataSource.Object}'");
            }
        }

        private PS.EngineTablePrimaryKey BuildTargetPrimaryKey(PS.TargetObject owner, IEntityMetadata entity)
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


        private IterationContext EnsureIterationContext(string contextName)
        {
            if (!_iterations.TryGetValue(contextName, out IterationContext context))
            {
                context = new IterationContext(contextName);
                _iterations.Add(context.Name, context);
            }
            return context;
        }

        public PS.EngineObject EnsureTarget(string path,  IEntityMetadata entity)
        {
            return _currentContext.EnsureTarget(path, entity);
        }

        public PS.EngineObject EnsureTarget(FieldDescriptor descriptor)
        {
            return _currentContext.EnsureTarget(descriptor.ParentPath, descriptor.Field.Entity);
        }

        public void SwithContext(string contextName)
        {
            this._currentContext = this.EnsureIterationContext(contextName);
        }
    }

    class JoinedBuildingContext : BuildingContext
    {
        public JoinedBuildingContext(string contextName, DataTypeSystem dataTypeSystem)
            : base(contextName, dataTypeSystem)
        {
        }

        public IReadOnlyDictionary<string, FieldDescriptor> Cache { get; set; }

        public PS.ConditionExpression BuildJoinConditionUsePrimaryKey(PS.TargetObject baseObject, PS.TargetObject joinedObject)
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

        public PS.JoinExpression[] ScanAndBuildJoinExpressions(Dictionary<string, PS.TargetObject> baseObjects, FieldDescriptor[] fields, PS.TargetObject rootObject, IEntityMetadata rootEntity)
        {
            var joins = new List<PS.JoinExpression>();
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
                        var entityJoins = this.BuildEntityJoinExpressions(baseObject, baseEntity, baseField, basePath, reference.Entity);
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

        private PS.JoinExpression[] BuildEntityJoinExpressions(PS.TargetObject baseObject, IEntityMetadata baseEntity, IFieldMetadata baseField, string basePath, IEntityMetadata joinedEntity)
        {
            switch (baseField.SourceType)
            {
                case FieldSourceType.Reference:
                    return this.BuildEntityJoinExpressionsAsReference(baseObject, baseEntity, baseField as ReferenceFieldMetadata, basePath, joinedEntity);
                case FieldSourceType.Extension:
                    return this.BuildEntityJoinExpressionsAsExtension(baseObject, baseEntity, baseField as ExtensionFieldMetadata, basePath, joinedEntity);
                case FieldSourceType.Relation:
                    return this.BuildEntityJoinExpressionsAsRelation(baseObject, baseEntity, baseField as RelationFieldMetadata, basePath, joinedEntity);
                default:
                    throw new InvalidOperationException($"Cann't use the field '{basePath}' this context of the entity joining");
            }
        }

        private PS.JoinExpression[] BuildEntityJoinExpressionsAsExtension(PS.TargetObject baseObject, IEntityMetadata baseEntity, ExtensionFieldMetadata baseField, string basePath, IEntityMetadata joinedEntity)
        {
            var joins = new PS.JoinExpression[1];
            var currentObject = this.EnsureTarget(basePath, joinedEntity);
            var join = new PS.JoinExpression
            {
                Target = currentObject,
                Operation = PS.JoinOperationType.Left, // пока без жосткой связи
                Condition = this.BuildJoinConditionUsePrimaryKey(baseObject, currentObject)
            };
            joins[0] = join;
            return joins;
        }

        private PS.JoinExpression[] BuildEntityJoinExpressionsAsReference(PS.TargetObject baseObject, IEntityMetadata baseEntity, ReferenceFieldMetadata baseField, string basePath, IEntityMetadata joinedEntity)
        {
            var chainWithMe = joinedEntity.DefineInheritChainWithMe();
            var joins = new PS.JoinExpression[chainWithMe.Length];

            var rootEntity = chainWithMe[0];
            var rootObject = this.EnsureTarget(basePath, rootEntity);

            joins[0] = new PS.JoinExpression
            {
                Target = rootObject,
                Operation = PS.JoinOperationType.Left, // пока без жосткой связи
                Condition = this.BuildJoinConditionForReference(baseEntity, baseField, basePath, rootObject)
            };

            if (chainWithMe.Length > 1)
            {
                for (int i = 0; i < chainWithMe.Length; i++)
                {
                    var currentEntity = chainWithMe[i];
                    var currentObject = this.EnsureTarget(basePath, currentEntity);
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

        private PS.ConditionExpression BuildJoinConditionForReference(IEntityMetadata baseEntity, ReferenceFieldMetadata baseField, string basePath, PS.TargetObject joinedObject)
        {
            if (joinedObject is PS.EngineTable joinedTable)
            {
                var joinedPkFields = joinedTable.PrimaryKey.Fields;
                var conditions = new PS.TwoOperandConditionExpression[joinedPkFields.Length];
                for (int i = 0; i < joinedPkFields.Length; i++)
                {
                    var joinPkField = joinedPkFields[i];
                    var baseOperand = this.FindBaseMamberForReference(joinPkField, baseEntity, baseField, basePath);
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

        private PS.OperandExpression FindBaseMamberForReference(PS.PrimaryKeyField byKey, IEntityMetadata baseEntity, ReferenceFieldMetadata baseField, string basePath)
        {
            var sourceName = string.Empty;
            if (baseField.Mapping != null)
            {
                var pk = baseField.Mapping.Fields[byKey.Property];
                switch (pk.MatchWith)
                {
                    case PrimaryKeyMappedMatchWith.Field:
                        sourceName = (pk as IFieldPrimaryKeyFieldMappedMetadata).EntityField.SourceName;
                        break;
                    case PrimaryKeyMappedMatchWith.SourceName:
                        sourceName = (pk as ISourceNamePrimaryKeyFieldMappedMetadata).SourceName;
                        break;
                    case PrimaryKeyMappedMatchWith.Value:
                        return new PS.ValueOperandExpression
                        {
                            Expression = new PS.ConstantValueExpression(byKey.DataType)
                            {
                                Value = (pk as IValuePrimaryKeyFieldMappedMetadata).Value
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
                    Owner = this.EnsureTarget(FieldDescriptor.BuildParentPath(basePath), baseField.Entity)
                }
            };
            return operand;
        }

        private PS.JoinExpression[] BuildEntityJoinExpressionsAsRelation(PS.TargetObject baseObject, IEntityMetadata baseEntity, RelationFieldMetadata baseField, string basePath, IEntityMetadata joinedEntity)
        {
            throw new NotImplementedException("Unsupported Relation Field this context");
        }



        public PS.ConditionExpression BuildConditionExpressions(DataModels.DataConstraint.Condition[] conditions)
        {
            if (conditions.Length == 0)
            {
                return null;
            }
            if (conditions.Length == 1)
            {
                return this.BuildConditionExpression(conditions[0]);
            }

            var condition = new PS.ComplexConditionExpression
            {
                Operator = PS.ConditionLogicalOperator.And,
                Conditions = new PS.ConditionExpression[conditions.Length]
            };

            for (int i = 0; i < conditions.Length; i++)
            {
                condition.Conditions[i] = this.BuildConditionExpression(conditions[i]);
            }
            return condition;
        }

        private PS.ConditionExpression BuildConditionExpression(DataModels.DataConstraint.Condition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            switch (condition.Type)
            {
                case CS.ConditionType.Complex:
                    return this.BuildConditionExpression(condition as CS.ComplexCondition);
                case CS.ConditionType.Expression:
                    return this.BuildConditionExpression(condition as CS.ConditionExpression);
                default:
                    throw new InvalidOperationException($"Unsupported condition type '{condition.Type}'");
            }
        }

        private PS.ConditionExpression BuildConditionExpression(CS.ComplexCondition condition)
        {
            var conditionExpression = new PS.ComplexConditionExpression
            {
                Operator = PS.ConditionExpression.LogicalOperator(condition.Operator),
                Conditions = new PS.ConditionExpression[condition.Conditions.Length]
            };

            for (int i = 0; i < condition.Conditions.Length; i++)
            {
                conditionExpression.Conditions[i] = this.BuildConditionExpression(condition.Conditions[i]);
            }
            return conditionExpression;
        }

        private PS.ConditionExpression BuildConditionExpression(CS.ConditionExpression condition)
        {
            switch (condition.Operator)
            {
                case CS.ConditionOperator.Equal:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.Equal, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.GreaterEqual:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.GreaterEqual, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.GreaterThan:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.GreaterThan, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.LessEqual:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.LessEqual, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.LessThan:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.LessThan, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.NotEqual:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotEqual, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.IsNull:
                    return this.BuildOneOperandConditionExpression(PS.OneOperandOperator.IsNull, condition.LeftOperand);
                case CS.ConditionOperator.IsNotNull:
                    return this.BuildOneOperandConditionExpression(PS.OneOperandOperator.IsNotNull, condition.LeftOperand);
                case CS.ConditionOperator.Like:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.Like, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.NotLike:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotLike, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.In:
                    return this.BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator.In, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.NotIn:
                    return this.BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator.NotIn, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.Between:
                    return this.BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator.Between, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.NotBetween:
                    return this.BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator.NotBetween, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.BeginWith:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.BeginWith, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.EndWith:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.EndWith, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.Contains:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.Contains, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.NotBeginWith:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotBeginWith, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.NotEndWith:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotEndWith, condition.LeftOperand, condition.RightOperand);
                case CS.ConditionOperator.NotContains:
                    return this.BuildTwoOperandConditionExpression(PS.TwoOperandOperator.NotContains, condition.LeftOperand, condition.RightOperand);
                default:
                    throw new InvalidOperationException($"Unsupported condition operator '{condition.Operator}'");
            }
        }

        private PS.OneOperandConditionExpression BuildOneOperandConditionExpression(PS.OneOperandOperator @operator, CS.Operand operand)
        {
            var conditionExpression = new PS.OneOperandConditionExpression
            {
                Operator = @operator,
                Operand = this.BuildConditionOperandExpression(operand)
            };
            return conditionExpression;
        }

        private PS.TwoOperandConditionExpression BuildTwoOperandConditionExpression(PS.TwoOperandOperator @operator, CS.Operand leftOperand, CS.Operand rightOperand)
        {
            var conditionExpression = new PS.TwoOperandConditionExpression
            {
                Operator = @operator,
                LeftOperand = this.BuildConditionOperandExpression(leftOperand),
                RightOperand = this.BuildConditionOperandExpression(rightOperand)
            };
            return conditionExpression;
        }

        private PS.MoreOperandsConditionExpression BuildMoreOperandsConditionExpression(PS.MoreOperandsOperator @operator, CS.Operand leftOperand, CS.Operand rightOperand)
        {
            var conditionExpression = new PS.MoreOperandsConditionExpression
            {
                Operator = @operator,
                Test = this.BuildConditionOperandExpression(leftOperand)
            };

            if (rightOperand.Type != CS.OperandType.Values)
            {
                throw new InvalidOperationException($"Unsupported operand type '{rightOperand.Type}' this context. Exprected the operand type is Values");
            }
            var values = rightOperand as CS.ValuesOperand;
            var operands = this.BuildConditionOperandExpression(values);
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

        private PS.OperandExpression BuildConditionOperandExpression(CS.Operand operand)
        {
            switch (operand.Type)
            {
                case CS.OperandType.Column:
                    return this.BuildConditionOperandExpression(operand as CS.ColumnOperand);
                case CS.OperandType.Value:
                    return this.BuildConditionOperandExpression(operand as CS.ValueOperand);
                case CS.OperandType.Values:
                default:
                    throw new InvalidOperationException($"Unsupported operand type '{operand.Type}'");
            }
        }

        private PS.MemberOperandExpression BuildConditionOperandExpression(CS.ColumnOperand operand)
        {
            if (!this.Cache.TryGetValue(operand.ColumnName, out FieldDescriptor descriptor))
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
                    Owner = this.EnsureTarget(descriptor)
                }
            };

            return memberOperand;
        }
        private PS.ValueOperandExpression BuildConditionOperandExpression(CS.ValueOperand operand)
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

        private PS.ValueOperandExpression[] BuildConditionOperandExpression(CS.ValuesOperand operand)
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

    }
}
