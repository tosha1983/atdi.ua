using Atdi.Contracts.AppServices.WebQuery;

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
        }

       
    }
}
