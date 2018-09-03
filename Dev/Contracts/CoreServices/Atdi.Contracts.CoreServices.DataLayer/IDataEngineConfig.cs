using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IDataEngineConfig
    {
        string ContextName { get; }

        DataEngineType Type { get; }

        string ConnectionString { get; }

        string ProviderName { get; }
    }
}