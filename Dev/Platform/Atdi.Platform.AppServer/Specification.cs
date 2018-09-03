using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;

namespace Atdi.Platform.AppServer
{
    public static class Specification
    {
    }

    static class Contexts
    {
        public static readonly EventContext AppServerInstaller = "AppServer Installer";
        public static readonly EventContext AppServerHost = "AppServer Host";
    }

    static class Categories
    {
        public static readonly EventCategory Initialization = "Initialization";
        public static readonly EventCategory Installation = "Installation";
        public static readonly EventCategory Stopping = "Stopping";
        public static readonly EventCategory Starting = "Starting";
        public static readonly EventCategory Disposabling = "Disposabling";
    }

    static class Events
    {
        public static readonly EventText StartedInstall = "The Server Host is installing";
        public static readonly EventText FinishedInstall = "The Server Host installed";

        public static readonly EventText StartedInitServerHost = "The Server Host is initializing";
        public static readonly EventText CreatedServerHost = "The Server Host with name '{0}' was created successfully";

        public static readonly EventText ServerComponentsIsInstalling = "The Server Components are installing";
        public static readonly EventText ServerComponentsInstalled = "The Server Components installed: perfom {0} of components";

        public static readonly EventText ServerHostIsStarting = "The Server Host is starting";
        public static readonly EventText ServerHostStarted = "The Server Host started successfully";

        public static readonly EventText ServerHostIsStoping = "The Server Host is stoping";
        public static readonly EventText ServerHostStopped = "The Server Host stopped successfully";

        public static readonly EventText ServerComponentsIsActivating = "The Server Components are activating";
        public static readonly EventText ServerComponentsActivated = "The Server Components activated successfully";

        public static readonly EventText ServerComponentsIsDeactivating = "The Server Components are deactivating";
        public static readonly EventText ServerComponentsDeactivated = "The Server Components deactivated successfully";

        public static readonly EventText ServerHostIsDisposabling = "The Server Host is disposabling";
        public static readonly EventText ServerHostDisposabled = "The Server Host disposabled";

        public static readonly EventText ServerComponentsIsUninstalling = "The Server Components are uninstalling";
        public static readonly EventText ServerComponentsUninstalled = "The Server Components uninstallied";

        public static readonly EventText ServerComponentIsInstalling = "The Server Component is installing. Type is {0}, instance '{1}', assembly {2}";
        public static readonly EventText ServerComponentDidNotInstall = "The Server Component did not install. Type is {0}, instance '{1}', assembly {2}";
        public static readonly EventText ServerComponentInstalled = "The Server Component installed successfully";

    }

    static class TraceScopeNames
    {
        public static readonly TraceScopeName Constructor = "Constructor";
    }

    static class Exceptions
    {
        public static readonly string IncorrectStateForStarting = "Incorrect current host state for starting";
        public static readonly string IncorrectStateForStopping = "Incorrect current host state for stopping";
    }
}
