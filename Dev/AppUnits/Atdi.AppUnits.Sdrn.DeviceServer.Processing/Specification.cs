using Atdi.Platform;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.Processing";
        public static readonly EventContext TaskActivator = "SDRN.TaskActivator";
        public static readonly EventContext ProcessingDispatcher = "SDRN.ProcessingDispatcher";
        public static readonly EventContext WorkScheduler = "SDRN.WorkScheduler";
        public static readonly EventContext TaskStarter = "SDRN.TaskStarter";
    }

    static class Categories
    {
        public static readonly EventCategory Running = "Running";

        public static readonly EventCategory Creating = "Creating";
        public static readonly EventCategory Finishing = "Finishing";
    }

    static class Events
    {
        public static readonly EventText AutomaticTaskActivationWasStarted = "Automatic task activation was started";
        public static readonly EventText AutomaticTaskActivationWasFinished = "Automatic task activation was finished";

        public static readonly EventText AutomaticTaskWorkerCodeHasCompleted = "Automatic task worker code has completed its execution: TaskType = '{0}', ProcessType = '{1}'";

        public static readonly EventText ProcessContextWasCreated = "Process context was created: Name = '{0}', Type = {1}";

        public static readonly EventText WorkTaskHasBeenCreated = "Work task has been created: WorkContext = '{0}', Key = '{1}'";
        public static readonly EventText WorkTaskHasFinished = "Work task has finished: WorkContext = '{0}', Key = '{1}'";

        public static readonly EventText TaskIsBeingRunSync = "Task is being run synchronously: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskHasBeenRunSync = "Task has been run synchronously: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskIsBeingRunAsync = "Task is being run asynchronously: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskHasBeenRunAsync = "Task has been run asynchronously: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskIsBeingRunParallel = "Task is being run parallel: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskHasBeenRunParallel = "Task has been run parallel: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";

    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {
        public static readonly ExceptionText ErrorCccurredWhileStartingTask = "Error occurred while starting the task: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
    }
}
