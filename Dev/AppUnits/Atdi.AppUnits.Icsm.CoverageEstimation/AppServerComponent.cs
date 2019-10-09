using Atdi.Platform.AppComponent;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Workflows;

namespace Atdi.AppUnits.Icsm.CoverageEstimation
{
    public class AppServerComponent : AppUnitComponent
    {
        public AppServerComponent()
            : base("IcsmCoverageEstimationAppUnit")
        {

        }

        protected override void OnInstallUnit()
        {
            var unitConfig = this.Config.Extract<AppServerComponentConfig>();
            this.Container.RegisterInstance(unitConfig, ServiceLifetime.Singleton);

            this.Container.Register<EstimationJobExecutor>(ServiceLifetime.Singleton);
            
        }

        protected override void OnActivateUnit()
        {
            var hostLoader = this.Resolver.Resolve<IServerHostLoader>();
            var jobBroker = this.Resolver.Resolve<IJobBroker>();

            // вешаем триггер, который запустить повторяющийся джоб
            hostLoader.RegisterTrigger("Running Coverage Estimation Job", () =>
            {
                var job = new JobDefinition<EstimationJobExecutor>()
                {
                    Name = "Coverage Estimation",
                    Repeatable = true,
                    Recoverable = true
                };

                var token = jobBroker.Run(job);

                Logger.Info(Contexts.ThisComponent, Categories.Initializing, $"The Coverage Estimation Job is launched: {token}");
            });
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
        }
    }
}
