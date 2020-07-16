using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.Sqlite.PatternHandlers
{
    class EngineCommandPatternHandler : LoggedObject, IQueryPatternHandler
    {
        public EngineCommandPatternHandler(ILogger logger) : base(logger)
        {
        }

        public void Handle<TPattern>(TPattern queryPattern, SqliteExecutor executor)
            where TPattern : class, IEngineQueryPattern
        {
            var pattern = queryPattern as PS.EngineCommandPattern;

            switch (pattern.Result.Kind)
            {
                case EngineExecutionResultKind.None:
	                executor.ExecuteNonQuery(pattern.Command);
                    return;
                case EngineExecutionResultKind.RowsAffected:
                    pattern.AsResult<EngineExecutionRowsAffectedResult>()
                        .RowsAffected = executor.ExecuteNonQuery(pattern.Command);
                    return;
                case EngineExecutionResultKind.Scalar:
                    pattern.AsResult<EngineExecutionScalarResult<object>>()
                        .Value = executor.ExecuteScalar(pattern.Command);

                    return;
                case EngineExecutionResultKind.Reader:
	                executor.ExecuteReader(pattern.Command, sqlReader =>
                    {
                        if (pattern.Result is EngineExecutionReaderResult<IEngineDataReader> result2)
                        {
                            result2.Handler(new EngineDataReader(sqlReader, null));
                        }
                        else if (pattern.Result is EngineExecutionReaderResult<System.Data.IDataReader> result)
                        {
                            result.Handler(sqlReader);
                        }
                    });
                    return;
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result type '{pattern.Result.Kind}'");
            }
           
        }
    }
}
