using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer.DataEngines
{
    public interface ISqlServerDataEngine : IDataEngine
    {
        void SetConfig(IDataEngineConfig engineConfig);
    }
}
