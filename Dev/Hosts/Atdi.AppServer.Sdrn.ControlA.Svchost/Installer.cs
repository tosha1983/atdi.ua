using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Atdi.AppServer.Svchost
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public Installer()
        {
            InitializeComponent();

            processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller = new ServiceInstaller();
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "Atdi.AppServer.Sdrn.ControlA";
            serviceInstaller.DisplayName = "ATDI Application ControlA";
            serviceInstaller.DelayedAutoStart = true;
            serviceInstaller.StartType = ServiceStartMode.Manual;


            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);

        }
    }
}
