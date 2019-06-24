using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm.QueryPatterns
{
    internal sealed class UpdatePatternBuilder : LoggedObject, IPatternBuilder
    {
        private readonly IEntityOrm _entityOrm;
        private readonly DataTypeSystem _dataTypeSystem;

        public UpdatePatternBuilder(IEntityOrm entityOrm, DataTypeSystem dataTypeSystem, ILogger logger) : base(logger)
        {
            this._entityOrm = entityOrm;
            this._dataTypeSystem = dataTypeSystem;
        }

        public TResult BuildAndExecute<TResult>(PatternExecutionContex<TResult> executionContex)
        {
            var statement = executionContex.Statement as QueryUpdateStatement;

            var result = default(TResult);
            var descriptor = statement.UpdateDecriptor;


            return result;
        }
    }
}
