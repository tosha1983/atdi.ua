﻿using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    internal class PatternBuilderFactory: LoggedObject
    {
        private static readonly IStatisticCounterKey EntityHitsCounterKey = STS.DefineCounterKey("ORM.Entity.Hits");
        private static readonly IStatisticCounterKey EntityErrorsCounterKey = STS.DefineCounterKey("ORM.Entity.Errors");

        private readonly Dictionary<Type, QueryPatterns.IPatternBuilder> _builders;
        private readonly IStatistics _statistics;
        private readonly IStatisticCounter _entityHitsCounter;
        private readonly IStatisticCounter _entityErrorsCounter;

        public PatternBuilderFactory(DataTypeSystem dataTypeSystem, IStatistics statistics,  ILogger logger)
            : base(logger)
        {
            this._statistics = statistics;
            this._entityHitsCounter = _statistics.Counter(EntityHitsCounterKey);
            this._entityErrorsCounter = _statistics.Counter(EntityErrorsCounterKey);

            this._builders = new Dictionary<Type, QueryPatterns.IPatternBuilder>
            {
                [typeof(QueryInsertStatement)] = new QueryPatterns.InsertPatternBuilder(dataTypeSystem, statistics, logger),
                [typeof(QuerySelectStatement)] = new QueryPatterns.SelectPatternBuilder(dataTypeSystem, statistics, logger),
                [typeof(QueryUpdateStatement)] = new QueryPatterns.UpdatePatternBuilder(dataTypeSystem, statistics, logger),
                [typeof(QueryDeleteStatement)] = new QueryPatterns.DeletePatternBuilder(dataTypeSystem, statistics, logger)
            };

            logger.Verbouse(Contexts.EntityOrm, Categories.Creation, Events.ObjectWasCreated.With("PatternBuilderFactory"));
            
        }

        public TResult BuildAndExecute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex)
        {
            if (executionContex == null)
            {
                throw new ArgumentNullException(nameof(executionContex));
            }

            if (executionContex.Statement == null)
            {
                throw new ArgumentNullException(nameof(executionContex.Statement));
            }

            var statementType = executionContex.Statement.GetType();
            if (!this._builders.TryGetValue(statementType, out QueryPatterns.IPatternBuilder patternBuilder))
            {
                statementType = statementType.BaseType;
                if (!this._builders.TryGetValue(statementType, out patternBuilder))
                {
                    throw new InvalidProgramException($"Not found a pattern builder by query statement type '{statementType.FullName}'");
                }
                    
            }

            try
            {
                this._entityHitsCounter?.Increment();

                var result = patternBuilder.BuildAndExecute(executionContex);
                return result;
            }
            catch (Exception e)
            {
                this._entityErrorsCounter?.Increment();
                this.Logger.Exception(Contexts.EntityOrm, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToBuildAndExecute.With(statementType.FullName), e);
            }
            
        }
    }
}
