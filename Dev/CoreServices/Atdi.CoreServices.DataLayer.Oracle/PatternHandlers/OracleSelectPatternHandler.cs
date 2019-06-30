using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.Oracle.PatternHandlers
{
    class OracleSelectPatternHandler : LoggedObject, IOracleQueryPatternHandler
    {
        public OracleSelectPatternHandler(ILogger logger) : base(logger)
        {
        }

        void IOracleQueryPatternHandler.Handle<TPattern>(TPattern queryPattern, OracleExecuter executer)
        {
            var pattern = queryPattern as PS.InsertPattern;

            var context = new OracleBuildingContex();

            this.BuildIteration(pattern, context);

            var command = context.BuildCommand();

            switch (pattern.Result.Kind)
            {
                case EngineExecutionResultKind.RowsAffected:
                    pattern.AsResult<EngineExecutionRowsAffectedResult>()
                        .RowsAffected = executer.ExecuteNonQuery(command);
                    return;
                case EngineExecutionResultKind.Scalar:
                    // возврат одного значения - первого поля запроса
                    var result = pattern.AsResult<EngineExecutionScalarResult>();
                    result.Value = executer.ExecuteScalar(command);
                    return;
                case EngineExecutionResultKind.Reader:
                    if (pattern.Result is EngineExecutionReaderResult readerResult)
                    {
                        executer.ExecuteReader(command, sqlReader =>
                        {
                            readerResult.Handler(new OracleEngineDataReader(sqlReader, context.Mapper));
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported result object type '{pattern.Result.GetType().FullName}'");
                    }
                    return;
                case EngineExecutionResultKind.None:
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result kind '{pattern.Result.Kind}'");
            }
        }

        private void BuildIteration(PS.InsertPattern pattern, OracleBuildingContex contex)
        {

        }

    }
}
