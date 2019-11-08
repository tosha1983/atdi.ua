using Atdi.Contracts.AppServices.WebQuery;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.AppComponent;

namespace Atdi.AppServices.FileStorage
{
    public sealed class AppServerComponent : AppServicesComponent<IFileStorage>
    {
        public AppServerComponent() 
            : base("FileStorageAppServices")
        {
        }

        protected override void OnInstall()
        {
            base.OnInstall();
            var config = this.Config.Extract<ConfigFileStorage>();
            this.Container.RegisterInstance(config, ServiceLifetime.Singleton);
        }


    }
}
