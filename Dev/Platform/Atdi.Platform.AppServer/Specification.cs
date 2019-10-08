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
        public static readonly EventText StartedInstall = "The server host is installing";
        public static readonly EventText FinishedInstall = "The server host has installed";

        public static readonly EventText StartedInitServerHost = "The server host is being initialized";
        public static readonly EventText CreatedServerHost = "The server host has been created successfully: Name='{0}'";

        public static readonly EventText ServerComponentsIsInstalling = "The server components installing started";
        public static readonly EventText ServerComponentsInstalled = "The server components installing completed successfully: perform {0} of components";

        public static readonly EventText ServerHostIsStarting = "The server host is starting";
        public static readonly EventText ServerHostStarted = "The server host has started successfully";

        public static readonly EventText ServerHostIsStopping = "The server host is stopping";
        public static readonly EventText ServerHostStopped = "The server host has stopped successfully";

        public static readonly EventText ServerComponentsIsActivating = "The server components activating started";
        public static readonly EventText ServerComponentsActivated = "The server components activating completed successfully";

        public static readonly EventText ServerComponentsIsDeactivating = "The server components deactivating started";
        public static readonly EventText ServerComponentsDeactivated = "The server components deactivating completed successfully";

        public static readonly EventText ServerHostIsDisposing = "The server host is disposing";
        public static readonly EventText ServerHostDisposed = "The server host has disposed successfully";

        public static readonly EventText ServerComponentsIsUninstalling = "The server components uninstalling started";
        public static readonly EventText ServerComponentsUninstalled = "The server components uninstalling completed successfully";

        public static readonly EventText ServerComponentIsInstalling = "The component installation started: Type='{0}'";
        public static readonly EventText ServerComponentDidNotInstall = "The component failed to install: Type='{0}', Instance='{1}', Assembly='{2}'";
        public static readonly EventText ServerComponentInstalled = "The component installation completed successfully: Type='{0}'";

        public static readonly EventText TriggerExecuting = "The trigger is executing: Context='{0}'";
        public static readonly EventText TriggerExecuted = "The trigger has executed: Context='{0}'";

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
