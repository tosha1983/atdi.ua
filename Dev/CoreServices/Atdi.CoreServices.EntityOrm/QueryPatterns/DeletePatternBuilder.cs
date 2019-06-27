using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm.QueryPatterns
{
    internal sealed class DeletePatternBuilder : LoggedObject, IPatternBuilder
    {
        private readonly IEntityOrm _entityOrm;
        private readonly DataTypeSystem _dataTypeSystem;

        public DeletePatternBuilder(IEntityOrm entityOrm, DataTypeSystem dataTypeSystem, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
            this._dataTypeSystem = dataTypeSystem;
        }

        public TResult BuildAndExecute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex)
        {
            var statement = executionContex.Statement as QueryDeleteStatement;

            var result = default(TResult);
            var descriptor = statement.DeleteDecriptor;


            return result;
        }
    }
}
