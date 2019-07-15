using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IDataLayerConfig
    {
        IDataEngineConfig GetEngineConfig<TContext>()
            where TContext : IDataContext, new();

        IDataEngineConfig GetEngineConfig(IDataContext dataContext);
    }
}
