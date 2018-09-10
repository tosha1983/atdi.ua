using Atdi.Platform;
using Atdi.Platform.AppServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer.Services;
using Atdi.AppServer.Services.Sdrns;
using Atdi.AppServer.AppServices;
using Atdi.AppServer.CoreServices;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.AppServices.SdrnsController;


namespace Atdi.AppServer.Host.Svchost.Sdrns
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };


    public partial class HostService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private AppServerHost _serverHost;
         

        public HostService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (_serverHost != null)
            {
                return;
            }

            var serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 60000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            try
            {
                var sdrnsControllerAppServiceHost = new AppServiceHostServerComponent<SdrnsControllerAppService, GetMeasTaskAppOperationHandler>();
                var sdrnsControllerWcfServiceHost = new WcfServiceHostServerComponent<ISdrnsController, SdrnsControllerService>();
                var coreServicesComponent = new CoreServicesServerComponent();
                var sdrnsConfigurationController = new ConfigurationSdrnController.ConfigurationSdrnController();
                //_serverHost = AppServerHost.Create("SDRNAppServer", new List<Atdi.AppServer.IAppServerComponent> { sdrnsConfigurationController });
                _serverHost = AppServerHost.Create("WebQueryAppServer", new List<Atdi.AppServer.IAppServerComponent> { sdrnsControllerAppServiceHost, sdrnsControllerWcfServiceHost , coreServicesComponent, sdrnsConfigurationController });
                this._serverHost.Start();
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }
            catch (Exception)
            {
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
                throw;
            }
           
        }

        protected override void OnStop()
        {
            if (_serverHost == null)
            {
                return;
            }

            var serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 60000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            try
            {
                this._serverHost.Stop();
                this._serverHost.Dispose();
                this._serverHost = null;
            }
            catch (Exception)
            {
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
                throw;
            }
        }
    }

}


