using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IDataEngine
    {
        IDataEngineConfig Config { get; }

        IEngineExecuter CreateExecuter();

        // To Do: Have to do it
        IEngineSyntax Syntax { get; }

    }

    public static class DataEngineExtensions
    {

        public static void Execute(this IDataEngine dataEngine, EngineCommand command, Action<System.Data.IDataReader> handler)
        {
            var pattern = new Patterns.EngineCommandPattern
            {
                Command = command
            };
            pattern.DefResult<EngineExecutionReaderResult<System.Data.IDataReader>>()
                .Handler = (reader) => handler(reader);

            using (var executor = dataEngine.CreateExecuter())
            {
                executor.Execute(pattern);
            }
        }

        public static void Execute(this IDataEngine dataEngine, EngineCommand command, Action<IEngineDataReader> handler)
        {
            var pattern = new Patterns.EngineCommandPattern
            {
                Command = command
            };
            pattern.DefResult<EngineExecutionReaderResult<IEngineDataReader>>()
                .Handler = (reader) => handler(reader);

            using (var executor = dataEngine.CreateExecuter())
            {
                executor.Execute(pattern);
            }
        }

        public static int Execute(this IDataEngine dataEngine, EngineCommand command)
        {
            var pattern = new Patterns.EngineCommandPattern
            {
                Command = command
            };
            pattern.DefResult<EngineExecutionRowsAffectedResult>();

            using (var executor = dataEngine.CreateExecuter())
            {
                executor.Execute(pattern);
            }

            return pattern.AsResult<EngineExecutionRowsAffectedResult>().RowsAffected;
        }

        public static object ExecuteScalar(this IDataEngine dataEngine, EngineCommand command)
        {
            var pattern = new Patterns.EngineCommandPattern
            {
                Command = command
            };
            pattern.DefResult<EngineExecutionScalarResult<object>>();

            using (var executor = dataEngine.CreateExecuter())
            {
                executor.Execute(pattern);
            }

            return pattern.AsResult<EngineExecutionScalarResult<object>>().Value;
        }
    }
}
