﻿using Atdi.Platform;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer
{
    internal static class Monitoring
    {
        public static readonly IStatisticCounterKey AdaptersCountKey = STS.DefineCounterKey("SDRN.DeviceServer.Adapters.Count");
        public static readonly IStatisticCounterKey AdaptersCommandsHitsKey = STS.DefineCounterKey("SDRN.DeviceServer.Adapters.Commands.Hits");
        public static readonly IStatisticCounterKey AdaptersCommandsShotsKey = STS.DefineCounterKey("SDRN.DeviceServer.Adapters.Commands.Shots");
        public static readonly IStatisticCounterKey AdaptersCommandsCanceledKey = STS.DefineCounterKey("SDRN.DeviceServer.Adapters.Commands.Canceled");

        public static readonly IStatisticCounterKey AdaptersCommandsExecutionCountKey = STS.DefineCounterKey("SDRN.DeviceServer.Adapters.Commands.Execution.Running");
        public static readonly IStatisticCounterKey AdaptersCommandsExecutionAbortedKey = STS.DefineCounterKey("SDRN.DeviceServer.Adapters.Commands.Execution.Aborted");
        public static readonly IStatisticCounterKey AdaptersCommandsExecutionCanceledKey = STS.DefineCounterKey("SDRN.DeviceServer.Adapters.Commands.Execution.Canceled");
        public static readonly IStatisticCounterKey AdaptersCommandsExecutionCompletedKey = STS.DefineCounterKey("SDRN.DeviceServer.Adapters.Commands.Execution.Completed");

        public static readonly IStatisticCounterKey TasksHitsKey = STS.DefineCounterKey("SDRN.DeviceServer.Tasks.Hits");
        public static readonly IStatisticCounterKey TasksSyncKey = STS.DefineCounterKey("SDRN.DeviceServer.Tasks.Sync");
        public static readonly IStatisticCounterKey TasksAsyncKey = STS.DefineCounterKey("SDRN.DeviceServer.Tasks.Async");

        public static IStatisticCounterKey DefineTaskCounter(string context, string key)
        {
            return STS.DefineCounterKey(string.Intern($"SDRN.DeviceServer.Tasks.Workers.{context}.{key}"));
        }
        public static IStatisticCounterKey DefineAutoTaskCounter(string context, string key)
        {
            return STS.DefineCounterKey(string.Intern($"SDRN.DeviceServer.AutoTasks.Workers.{context}.{key}"));
        }

        public static IStatisticCounterKey DefineAdapterCommandCounter(string context, string key)
        {
            return STS.DefineCounterKey(string.Intern($"SDRN.DeviceServer.Adapters.Commands.Handlers.{context}.{key}"));
        }

        public static readonly IStatisticCounterKey ResultBufferCreatedKey = STS.DefineCounterKey("SDRN.DeviceServer.Results.Buffers.Created");
        public static readonly IStatisticCounterKey ResultBufferUsingKey = STS.DefineCounterKey("SDRN.DeviceServer.Results.Buffers.Using");
        public static readonly IStatisticCounterKey ResultBufferReleasedKey = STS.DefineCounterKey("SDRN.DeviceServer.Results.Buffers.Released");

        public static IStatisticCounterKey DefineAdapterResultHandlerCounter(string context, string key)
        {
            return STS.DefineCounterKey(string.Intern($"SDRN.DeviceServer.Results.Handlers.{context}.{key}"));
        }

        public static IStatisticCounterKey DefineAdapterResultWorkerCounter(string context, string key)
        {
            return STS.DefineCounterKey(string.Intern($"SDRN.DeviceServer.Results.Workers.{context}.{key}"));
        }

        public static readonly IStatisticCounterKey WorkSchedulerStartedKey = STS.DefineCounterKey("SDRN.DeviceServer.WorkScheduler.Started");
        public static readonly IStatisticCounterKey WorkSchedulerRunningKey = STS.DefineCounterKey("SDRN.DeviceServer.WorkScheduler.Running");
        public static readonly IStatisticCounterKey WorkSchedulerFinishedKey = STS.DefineCounterKey("SDRN.DeviceServer.WorkScheduler.Finished");

        public static readonly IStatisticCounterKey ProcessingStartedKey = STS.DefineCounterKey("SDRN.DeviceServer.Processing.Started");
        public static readonly IStatisticCounterKey ProcessingRunningKey = STS.DefineCounterKey("SDRN.DeviceServer.Processing.Running");
        public static readonly IStatisticCounterKey ProcessingFinishedKey = STS.DefineCounterKey("SDRN.DeviceServer.Processing.Finished");
    }

    internal static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.DeviceServer";
        public static readonly EventContext AdapterWorker = "SDRN.AdapterWorker";
        public static readonly EventContext Controller = "SDRN.Controller";
        public static readonly EventContext DevicesHost = "SDRN.DevicesHost";
        public static readonly EventContext ResultWorker = "SDRN.ResultWorker";
        public static readonly EventContext ResultHandler = "SDRN.ResultHandler";
        public static readonly EventContext ResultConvertor = "SDRN.ResultConvertor";

        public static readonly EventContext AutoTaskActivator = "SDRN.AutoTaskActivator";
        public static readonly EventContext ProcessingDispatcher = "SDRN.ProcessingDispatcher";
        public static readonly EventContext WorkScheduler = "SDRN.WorkScheduler";
        public static readonly EventContext TaskStarter = "SDRN.TaskStarter";
        public static readonly EventContext EventWaiter = "SDRN.EventWaiter";
        public static readonly EventContext TimerScheduler = "SDRN.TimerScheduler";
    }

    internal static class Categories
    {
        public static readonly EventCategory Disposing = "Disposing";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Executing = "Executing";
        public static readonly EventCategory Running = "Running";
        public static readonly EventCategory Registering = "Registering";
        public static readonly EventCategory Finalizing = "Finalizing";
        public static readonly EventCategory Handling = "Handling";
        public static readonly EventCategory Converting = "Converting";

        public static readonly EventCategory UpdateSensorStatus = "Update sensor";
        public static readonly EventCategory LoadSensor = "Load sensor";
        public static readonly EventCategory CreateNewObjectSensor = "Create sensor";

        public static readonly EventCategory Creating = "Creating";
        public static readonly EventCategory Finishing = "Finishing";

        public static readonly EventCategory Initializing = "Initializing";
        public static readonly EventCategory Callbacking = "Callbacking";


    }

    internal static class Events
    {


        public static readonly EventText DisconnectingAdapterError = "Error occurred while disconnecting the adapter: AdapterType = '{0}'";
        public static readonly EventText AbortAdapterThread = "Abort the thread of the adapter: AdapterType = '{0}'";
        public static readonly EventText ProcessingAdapterError = "Error occurred while processing the adapter: AdapterType = '{0}'";
        public static readonly EventText TookCommand = "The new command was took: AdapterType = '{0}', CommandType = '{1}', ParameterType = '{2}'";
        public static readonly EventText RejectedCommand = "The new command was rejected: AdapterType = '{0}', CommandType = '{1}', ParameterType = '{2}', Reasone = '{3}'";

        public static readonly EventText RejectedCommandByTimeout = "Timeout info: Delay = '{0}', Timeout = '{1}', Lateness = '{2}'";

        public static readonly EventText TransferCommand = "Transfer command to adapter: AdapterType = '{0}', CommandType = '{1}', ParameterType = '{2}'";
        public static readonly EventText FinalizedCommand = "The execution command was finalized: AdapterType = '{0}', CommandType = '{1}', ParameterType = '{2}'";

        public static readonly EventText CreatedAdapter = "The adapter object was created: AdapterType = '{0}'";
        public static readonly EventText ConnectedAdapter = "The adapter object was connected: AdapterType = '{0}'";

        public static readonly EventText RanAdapterThread = "The adapter thread was ran: AdapterType = '{0}'";

        public static readonly EventText RegisteredAdapter = "The adapter was registered: AdapterType = '{0}'";

        public static readonly EventText ProcessingResultError = "Error occurred while processing the result: AdapterType = '{0}', CommandType = '{1}'";

        public static readonly EventText DefiningResultHandlerError = "Error occurred while defining result handler: AdapterType = '{0}', CommandType = '{1}'";

        public static readonly EventText HandlingResultError = "Error occurred while processing the results: CommandType = '{0}', ResultType = '{1}', PartIndex = '{2}', Status = '{3}'";
        public static readonly EventText ConvertingResultError = "Error occurred while converting the results: FromType = '{0}', ResultType = '{1}', PartIndex = '{2}', Status = '{3}'";


        public static readonly EventText AutomaticTaskActivationWasStarted = "Automatic task activation was started";
        public static readonly EventText AutomaticTaskActivationWasFinished = "Automatic task activation was finished";
        public static readonly EventText AutomaticTasksDetected = "The Auto tasks were detected: Count = {0}";

        public static readonly EventText AutomaticTaskWorkerCodeHasCompleted = "Automatic task worker code has completed its execution: TaskType = '{0}', ProcessType = '{1}'";

        public static readonly EventText ProcessContextWasCreated = "The Process Context was created: Name = '{0}', Type = {1}";

        public static readonly EventText WorkTaskHasBeenCreated = "The Work Task has been created: WorkContext = '{0}', Key = '{1}'";
        public static readonly EventText WorkTaskHasFinished = "The Work Task has finished: WorkContext = '{0}', Key = '{1}'";

        public static readonly EventText TaskIsBeingRunSync = "Task is being run synchronously: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskHasBeenRunSync = "Task has been run synchronously: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskIsBeingRunAsync = "Task is being run asynchronously: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskHasBeenRunAsync = "Task has been run asynchronously: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskIsBeingRunParallel = "Task is being run parallel: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly EventText TaskHasBeenRunParallel = "Task has been run parallel: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";

        public static readonly EventText GateFactoryWasCreated = "The Gate Factory was created";
        public static readonly EventText GateConfigWasLoaded = "The Gate Configuration was loaded: API = '{0}', SDRN Server Instance = '{1}', Rabbit Host = '{2}'";
        public static readonly EventText GateWasCreated = "The Gate was created: ContextName = '{0}'";

        public static readonly EventText ProcessingObjectsWereRegistered = "The Processing Objects were registered";
        public static readonly EventText ControllerObjectsWereRegistered = "The Controller Objects were registered";

        public static readonly EventText TaskWorkerTypesWereRegistered = "The Task Worker Types were registered";
        public static readonly EventText ResultHandlerTypesWereRegistered = "The Result Handler Types were registered";
        public static readonly EventText ResultConvertorTypesWereRegistered = "The Result Convertor Types were registered";
        public static readonly EventText AdapterObjectsWereRegistered = "The Adapter Objects were registered";
        public static readonly EventText AutoTasksWereWtarted = "The Auto Tasks were started";


        public static readonly EventText CreatedEventWaiter = "The Event Waiter object was created";

        public static readonly EventText AdapterRegistrationTimedOut = "Adapter registration timed out: AdapterType = '{0}'";
        public static readonly EventText AdapterRegistrationFailed = "Adapter registration failed: AdapterType = '{0}'";
        public static readonly EventText AdapterRegistrationAborted = "Adapter registration aborted: AdapterType = '{0}'";
        public static readonly EventText AdapterRegistrationCompleted = "Adapter registration is completed successfully: AdapterType = '{0}', Assigned ID = '{1}'";

        public static readonly EventText ErrorOccurredDuringCallbackCall = "An error occurred during the callback call: Command Type = '{0}'";
        public static readonly EventText AbortSchedulerThread = "Abort the thread: Command Type = '{0}'";
        public static readonly EventText ProcessingSchedulerError = "Error occurred while processing the scheduler: Command Type = '{0}'";

    }

    internal static class TraceScopeNames
    {
        public static readonly TraceScopeName ExecutingAdapterCommand = "({4} - {5}) Executing adapter command = '{0}', Type = '{1}', Id = '{2}', AdapterType = '{3}'";
        public static readonly TraceScopeName InvokingAdapterCommand = "Invoking command - Id='{0}'";
        public static readonly TraceScopeName HandlingResult = "Id = '{0}', CommandType = '{1}', ResultType = '{2}', PartIndex = '{3}', Status = '{4}'";
        public static readonly TraceScopeName ConvertingResult = "Id = '{0}', FromType = '{1}', ResultType = '{2}', PartIndex = '{3}', Status = '{4}'";
    }

    internal static class Exceptions
    {
        public static readonly ExceptionText ErrorOccurredWhileStartingTask = "An error occurred while starting the task: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        public static readonly ExceptionText ErrorOccurredWhileInvokingControllerCommand = "An error occurred while invoking a controller command: CommandType = '{0}', CommandId = '{1}'";

    }
}
