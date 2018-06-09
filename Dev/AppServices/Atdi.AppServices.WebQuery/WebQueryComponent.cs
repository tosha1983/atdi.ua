using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.AppServices.WebQuery;
using Atdi.Platform.AppComponent;

namespace Atdi.AppServices.WebQuery
{
    public sealed class WebQueryComponent : AppServicesComponent<IWebQuery>
    {
        public WebQueryComponent() 
            : base("WebQueryAppServices")
        {
        }

        protected override void OnInstall()
        {
            base.OnInstall();
            this.Container.Register<QueriesRepository>(Platform.DependencyInjection.ServiceLifetime.PerThread);
        }
    }
}
