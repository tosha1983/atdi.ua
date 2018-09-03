using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IDataEngine
    {
        IDataEngineConfig Config { get; }

        void Execute(EngineCommand command, Action<System.Data.IDataReader> handler);

        int Execute(EngineCommand command);

        object ExecuteScalar(EngineCommand command);

        // To Do: Have to do it
        IEngineSyntax Syntax { get; }
    }
}
