using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
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
        private readonly Dictionary<Type, QueryPatterns.IPatternBuilder> _builders;

        public PatternBuilderFactory(IEntityOrm entityOrm, DataTypeSystem dataTypeSystem, ILogger logger)
            : base(logger)
        {
            this._builders = new Dictionary<Type, QueryPatterns.IPatternBuilder>
            {
                [typeof(QueryInsertStatement)] = new QueryPatterns.InsertPatternBuilder(entityOrm, dataTypeSystem, logger),
                [typeof(QuerySelectStatement)] = new QueryPatterns.SelectPatternBuilder(entityOrm, dataTypeSystem, logger),
                [typeof(QueryUpdateStatement)] = new QueryPatterns.UpdatePatternBuilder(entityOrm, dataTypeSystem, logger),
                [typeof(IQueryDeleteStatement)] = new QueryPatterns.DeletePatternBuilder(entityOrm, dataTypeSystem, logger)
            };

            logger.Verbouse(Contexts.EntityOrm, Categories.Creation, Events.ObjectWasCreated.With("PatternBuilderFactory"));
        }

        public TResult BuildAndExecute<TResult>(PatternExecutionContex<TResult> executionContex)
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
                var result = patternBuilder.BuildAndExecute(executionContex);
                return result;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.EntityOrm, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToBuildAndExecute.With(statementType.FullName), e);
            }
            
        }
    }
}
