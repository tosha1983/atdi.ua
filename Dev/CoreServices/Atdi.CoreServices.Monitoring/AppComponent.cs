using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.Monitoring;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.CoreServices.Monitoring
{
    public sealed class AppComponent : ComponentBase
    {
        public AppComponent()
            : base(
                  name: "MonitoringCoreServices", 
                  type: ComponentType.CoreServices, 
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            var config = this.Config.Extract<AppComponentConfig>();
            this.Container
                .RegisterInstance(config, ServiceLifetime.Singleton);

            this.Container
                .Register<ILogEventSite, LogEventSite>(ServiceLifetime.Singleton);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var eventSite = this.Resolver.Resolve<ILogEventSite>();

            var producer = this.Resolver.Resolve<IEventsProducer>();

            producer.AddConsumer((IEventsConsumer)eventSite);

            this.Logger.Info("Monitoring", "Initialization", "The monitoring events consumer was installed");

        }
    }
}
