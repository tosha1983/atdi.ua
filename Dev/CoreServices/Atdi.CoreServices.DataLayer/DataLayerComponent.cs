using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.AppServer;
using Atdi.Platform.Logging;

namespace Atdi.CoreServices.DataLayer
{
    public sealed class DataLayerComponent : ComponentBase
    {
        public DataLayerComponent()
            : base(
                  name: "DataLayer", 
                  type: ComponentType.CoreServices, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }
    }
}
