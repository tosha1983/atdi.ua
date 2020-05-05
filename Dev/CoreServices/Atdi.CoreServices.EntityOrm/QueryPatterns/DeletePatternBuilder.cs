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
    internal sealed class DeletePatternBuilder : LoggedObject, IPatternBuilder
    {
        class DeleteBuildingContext : JoinedBuildingContext
        {
            public DeleteBuildingContext(string contextName, DataTypeSystem dataTypeSystem)
                : base(contextName, dataTypeSystem)
            {
            }
        }

        private static readonly IStatisticCounterKey CounterKey = STS.DefineCounterKey("ORM.Entity.Patterns.Delete");
        private readonly IEntityOrm _entityOrm;
        private readonly DataTypeSystem _dataTypeSystem;
        private readonly IStatistics _statistics;
        private readonly IStatisticCounter _counter;

        public DeletePatternBuilder(IEntityOrm entityOrm, DataTypeSystem dataTypeSystem, IStatistics statistics, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
            this._dataTypeSystem = dataTypeSystem;
            this._statistics = statistics;
            this._counter = _statistics.Counter(CounterKey);
        }

        public TResult BuildAndExecute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContext)
        {
            _counter?.Increment();

            var statement = executionContext.Statement as QueryDeleteStatement;

            // построение запроса согласно патерна
            var pattern = this.Build(statement);

            // выполняем запросы согласно патерна
            return this.Execute(executionContext, statement, pattern);
        }

        /// <summary>
        /// Два варианта удаления: протсо случай, для одноталичной сущности и сложный для моного табличной
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        private PS.DeletePattern Build(QueryDeleteStatement statement)
        {
            var descriptor = statement.DeleteDecriptor;
            var context = new DeleteBuildingContext("singlton", _dataTypeSystem);
            context.Cache = descriptor.Cache;
            var pattren = new PS.DeletePattern();
            if (descriptor.Entity.Type == EntityType.Normal
                || descriptor.Entity.Type == EntityType.Extension)
            {
                // это однотабличные вариант - простой случай
                pattren.Expressions = this.BuildSimpleWay(statement, context);
            }
            else if (descriptor.Entity.Type == EntityType.Prototype
                || descriptor.Entity.Type == EntityType.Simple
                || descriptor.Entity.Type == EntityType.Role)
            {
                // это многтабличный вариант - сложный случай
                pattren.Expressions = this.BuildComplexWay(statement, context);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported entity type '{descriptor.Entity.Type}' in this context used");
            }
            return pattren;
        }

        private PS.DeleteExpression[] BuildSimpleWay(QueryDeleteStatement statement, DeleteBuildingContext context)
        {
            var descriptor = statement.DeleteDecriptor;
            var expression = new PS.DeleteExpression
            {
                Target = context.EnsureTarget(null, descriptor.Entity)
            };

            // джойним базовые зависимости
            var joins = new List<PS.JoinExpression>();
            var chainEntities = descriptor.Entity.DefineInheritChainWithMe();
            for (int i = 0; i < chainEntities.Length; i++)
            {
                var chainEntity = chainEntities[i];
                if (chainEntity.QualifiedName == descriptor.Entity.QualifiedName)
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
            var baseObjects = new Dictionary<string, PS.TargetObject>();
            var entityJoins = context.ScanAndBuildJoinExpressions(baseObjects, referenceFields, expression.Target, descriptor.Entity);
            if (entityJoins != null && entityJoins.Length > 0)
            {
                joins.AddRange(entityJoins);
            }

            expression.Joins = joins.ToArray();

            if (descriptor.Conditions != null)
            {
                expression.Condition = context.BuildConditionExpressions(descriptor.Conditions.ToArray());
            }

            return new PS.DeleteExpression[] { expression };
        }
        private PS.DeleteExpression[] BuildComplexWay(QueryDeleteStatement statement, DeleteBuildingContext context)
        {
            throw new NotImplementedException("Please temporarily use the simple deletion option not going beyond the changes in one table.");
        }


        private TResult Execute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex, QueryDeleteStatement statement, PS.DeletePattern pattern)
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
                case EngineExecutionResultKind.Reader:
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result kind '{executionContex.ResultKind}'");
            }


            return result;
        }

    }
}
