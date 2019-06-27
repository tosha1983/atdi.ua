using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS = Atdi.Contracts.CoreServices.DataLayer.Patterns;

namespace Atdi.CoreServices.DataLayer.SqlServer.PatternHandlers
{
    class SelectPatternHandler : LoggedObject, IQueryPatternHandler
    {
        public SelectPatternHandler(ILogger logger) : base(logger)
        {
        }

        void IQueryPatternHandler.Handle<TPattern>(TPattern queryPattern, SqlServerExecuter executer)
        {
            var pattern = queryPattern as PS.InsertPattern;

            var context = new BuildingContex();

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
                    executer.ExecuteReader(command, sqlReader =>
                    {
                        // тут нужно вернуть мапинг полей для чтения
                        if (pattern.Result is EngineExecutionReaderResult<System.Data.IDataReader> result1)
                        {
                            result1.Handler(sqlReader);
                        }
                        if (pattern.Result is EngineExecutionReaderResult<IEngineDataReader> result2)
                        {
                            result2.Handler(new EngineDataReader(sqlReader));
                        }
                    });
                    return;
                case EngineExecutionResultKind.None:
                case EngineExecutionResultKind.Custom:
                default:
                    throw new InvalidOperationException($"Unsupported result type '{pattern.Result.Kind}'");
            }
        }

        /// <summary>
        /// Генерация запроса
        /// SELECT
        ///     A1.T1_PK_1_ID AS FA1, -- 'Field 1 Path'
        ///     A1.T1_PK_2_ID AS FA2, -- 'Field 2 Path'
        /// FROM [Schema].[TABLE1] AS A1 -- BaseEntity Name
        ///     INNER JOIN [Schema].[TABLE2] AS A2 -- ChildEntity Name
        ///         ON (A2.T1_PK_1_ID = A1.T1_PK_1_ID AND A2.T1_PK_2_ID = A1.T1_PK_2_ID) 
        ///     INNER JOIN [Schema].[TABLE3] AS A3 -- ChildEntity Name
        ///         ON (A3.T1_PK_1_ID = A1.T1_PK_1_ID AND A2.T1_PK_2_ID = A1.T1_PK_2_ID) 
        ///     INNER JOIN [Schema].[TABLE4] AS A4 -- ExtensionEntity Name Requered = true
        ///         ON (A4.T1_PK_1_ID = A1.T1_PK_1_ID AND A4.T1_PK_2_ID = A1.T1_PK_2_ID)
        ///     LEFT JOIN [Schema].[TABLE4] AS A5 -- ExtensionEntity Name Requered = false
        ///         ON (A5.T1_PK_1_ID = A1.T1_PK_1_ID AND A4.T1_PK_2_ID = A1.T1_PK_2_ID)
        /// WHERE (A1.Name IS NOT NULL) AND (() OR ())
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="contex"></param>
        private void BuildIteration(PS.InsertPattern pattern, BuildingContex contex)
        {

        }

    }
}
