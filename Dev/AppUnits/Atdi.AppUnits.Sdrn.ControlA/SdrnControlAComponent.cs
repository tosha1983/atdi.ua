using Atdi.AppUnits.Sdrn.ControlA.Bus;

namespace Atdi.AppUnits.Sdrn.ControlA
{
    public class SdrnControlAComponent : AppUnitComponent
    {

        public SdrnControlAComponent() 
            : base("SdrnControlAAppUnit")
        {
            
        }


        protected override void OnInstallUnit()
        {
            var busManager = new Launcher(this.Logger, this.Config);
            this.Container.RegisterInstance(typeof(Launcher), busManager, Platform.DependencyInjection.ServiceLifetime.Singleton);
        }

        protected override void OnActivateUnit()
        {
        }

        protected override void OnDeactivateUnit()
        {
        }
        protected override void OnUninstallUnit()
        {
            Launcher._messagePublisher.Dispose();
            Launcher._messageDispatcher.Deactivate();
            Launcher._messageDispatcher.Dispose();
            Launcher._gate.Dispose();
            Launcher._sdr = null;
        }
    }
}
