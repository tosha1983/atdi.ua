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

namespace Atdi.AppServer.Sdrn.ControlA.Svchost
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

        private IServerHost _serverHost;

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
                this._serverHost = PlatformConfigurator.BuildHost();
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
