using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.Monitoring;
using Atdi.CoreServices.Monitoring.Statistics;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.AppServer;
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

            this.Container
                .Register<StatisticCollector, StatisticCollector>(ServiceLifetime.Singleton);

            this.Container
                .Register<IStatisticEntryKeys, StatisticEntryKeys>(ServiceLifetime.Singleton);

            this.Container
                .Register<IStatisticCounterKeys, StatisticCounterKeys>(ServiceLifetime.Singleton);

            this.Container
                .Register<IStatisticEntries, StatisticEntries>(ServiceLifetime.Singleton);

            this.Container
                .Register<IStatisticCounters, StatisticCounters>(ServiceLifetime.Singleton);

            this.Container
                .Register<IStatisticCurrentCounters, StatisticCurrentCounters>(ServiceLifetime.Singleton);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var eventSite = this.Resolver.Resolve<ILogEventSite>();

            var producer = this.Resolver.Resolve<IEventsProducer>();

            producer.AddConsumer((IEventsConsumer)eventSite);

            this.Logger.Info("Monitoring", "Initialization", "The monitoring events consumer was installed");


            var hostLoader = this.Resolver.Resolve<IServerHostLoader>();

            hostLoader.RegisterTrigger("Running the statistics collector", () =>
            {

                var statistics = this.Resolver.Resolve<IStatistics>();
                statistics.Set(STS.OS.Version.Name, Environment.OSVersion.VersionString);
                statistics.Set(STS.OS.Version.Number, Environment.OSVersion.Version.ToString());
                statistics.Set(STS.OS.Version.ServicePack, Environment.OSVersion.ServicePack);
                statistics.Set(STS.OS.Version.Platform, Environment.OSVersion.Platform.ToString());
                statistics.Set(STS.OS.Is64Bit, Environment.Is64BitOperatingSystem.ToString());

                statistics.Set(STS.Host.Name, Environment.MachineName);
                statistics.Set(STS.Host.CPU.Cores, Environment.ProcessorCount);

                statistics.Set(STS.Process.Is64Bit, Environment.Is64BitOperatingSystem.ToString());
                statistics.Set(STS.Process.UserName, Environment.UserName);
                statistics.Set(STS.Process.Directory , Environment.CurrentDirectory);
                statistics.Set(STS.Process.CommandLine, Environment.CommandLine);
                //Process.GetCurrentProcess().

                var stsCollector = this.Resolver.Resolve<StatisticCollector>();
                stsCollector.Run();
            });

        }
    }
}
