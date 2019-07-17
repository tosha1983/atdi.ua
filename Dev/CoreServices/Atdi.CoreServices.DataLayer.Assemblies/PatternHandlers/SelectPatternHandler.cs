using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.Assemblies.PatternHandlers
{
    class SelectPatternHandler : LoggedObject, IQueryPatternHandler
    {
        public SelectPatternHandler(ILogger logger) : base(logger)
        {
        }

        void IQueryPatternHandler.Handle<TPattern>(TPattern queryPattern, ServiceObjectResolver resolver)
        {
            var pattern = queryPattern as PS.SelectPattern;

            switch (pattern.Result.Kind)
            {
                case EngineExecutionResultKind.Scalar:
                    // возврат одного значения - первого поля запроса
                    var result = pattern.AsResult<EngineExecutionScalarResult>();
                    result.Value = null;
                    return;
                case EngineExecutionResultKind.Reader:
                    if (pattern.Result is EngineExecutionReaderResult readerResult)
                    {
                        readerResult.Handler(new EngineDataReader(pattern, resolver));
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported result object type '{pattern.Result.GetType().FullName}'");
                    }
                    return;
                case EngineExecutionResultKind.RowsAffected:
                case EngineExecutionResultKind.None:
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result kind '{pattern.Result.Kind}'");
            }
        }
    }
}
