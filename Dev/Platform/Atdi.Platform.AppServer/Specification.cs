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
        public static readonly EventContext AppServerInstaller = "Platform";
        public static readonly EventContext AppServerHost = "ServerHost";
    }

    static class Categories
    {
        public static readonly EventCategory Initialization = "Initialization";
        public static readonly EventCategory Installation = "Installation";
        public static readonly EventCategory Stopping = "Stopping";
        public static readonly EventCategory Starting = "Starting";
        public static readonly EventCategory Disposing = "Disposing";
        public static readonly EventCategory Triggering = "Triggering";
    }

    static class Events
    {
        public static readonly EventText StartedInstall = "Server host installation started";
        public static readonly EventText FinishedInstall = "Server host installation completed";

        public static readonly EventText StartedInitServerHost = "The server host started initialization";
        public static readonly EventText CreatedServerHost = "The server host was created: Name='{0}'";

        public static readonly EventText ServerComponentsIsInstalling = "Server components installation started";
        public static readonly EventText ServerComponentsInstalled = "Server components installation completed: perform {0} of components";

        public static readonly EventText ServerHostIsStarting = "The server host is starting";
        public static readonly EventText ServerHostStarted = "The server host started";

        public static readonly EventText ServerHostIsStopping = "The server host is stopping";
        public static readonly EventText ServerHostStopped = "The server host stopped";

        public static readonly EventText ServerComponentsIsActivating = "Server components activation started";
        public static readonly EventText ServerComponentsActivated = "Server components activation completed";

        public static readonly EventText ServerComponentsIsDeactivating = "Server components deactivation started";
        public static readonly EventText ServerComponentsDeactivated = "Server components deactivation completed";

        public static readonly EventText ServerHostIsDisposing = "The server host is disposing";
        public static readonly EventText ServerHostDisposed = "The server host disposed";

        public static readonly EventText ServerComponentsIsUninstalling = "Server components uninstalling started";
        public static readonly EventText ServerComponentsUninstalled = "Server components uninstalling completed";

        public static readonly EventText ServerComponentIsInstalling = "Component installation started: Type='{0}'";
        public static readonly EventText ServerComponentDidNotInstall = "The component failed to install: Type='{0}', Instance='{1}', Assembly='{2}'";
        public static readonly EventText ServerComponentInstalled = "Component installation completed: Type='{0}'";

        public static readonly EventText TriggerExecuting = "The trigger is executing: Context='{0}'";
        public static readonly EventText TriggerExecuted = "The trigger executed: Context='{0}'";

        public static readonly EventText ExecutingTriggerError = "Error occurred while executing the trigger: Context='{0}'";
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
