using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.WebApiServices
{
    public abstract class WebApiServicesComponent : ComponentBase
    {
        public WebApiServicesComponent(string name, ComponentBehavior behavior) : base(name, ComponentType.WebApiServices, behavior)
        {
        }

        protected override void OnInstall()
        {
            this.Container.RegisterClassesBasedOn<WebApiController>(this.GetType().Assembly, ServiceLifetime.Transient);
        }
    }
}
