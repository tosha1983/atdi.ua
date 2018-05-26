using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IDataEngine
    {
        IDataEngineConfig Config { get; }

        void Execute(EngineCommand command, Action<IDataReader> handler);

        int Execute(EngineCommand command);

        object ExecuteScalar(EngineCommand command);
    }
}
