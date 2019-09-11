using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.AS.PrimaryHandlers";

    }

    static class Categories
    {
        public static readonly EventCategory Disposing = "Disposing";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Executing = "Executing";
        public static readonly EventCategory Running = "Running";
        public static readonly EventCategory RegistrationAggregationServer = "RegistrationAggregationServer";
        public static readonly EventCategory Finalizing = "Finalizing";
        public static readonly EventCategory Handling = "Handling";
        public static readonly EventCategory Converting = "Converting";

        public static readonly EventCategory Creating = "Creating";
        public static readonly EventCategory Finishing = "Finishing";

        public static readonly EventCategory Initilazing = "Initilazing";
        public static readonly EventCategory Callbacking = "Callbacking";
        public static readonly EventCategory OnRegisterAggregationServerEvent = "OnRegisterAggregationServerEvent";
        public static readonly EventCategory ClientMeasTasksPipelineHandler = "ClientMeasTasksPipelineHandler";
        public static readonly EventCategory UpdateFieldAggregationServerInstanceInSensor = "UpdateFieldAggregationServerInstanceInSensor";
        public static readonly EventCategory MeasTaskCommandsHandler = "MeasTaskCommandsHandler";
        public static readonly EventCategory MeasTaskCreateOnAggServerHandler = "MeasTaskCreateOnAggServerHandler";
        public static readonly EventCategory MeasTaskCommandsOnAggServerHandler = "MeasTaskCommandsOnAggServerHandler";
        public static readonly EventCategory CommandsOnAggServerPipelineHandler = "CommandsOnAggServerPipelineHandler";
        public static readonly EventCategory MeasTasksOnAggServerSendEventPipelineHandler = "MeasTasksOnAggServerSendEventPipelineHandler";
        public static readonly EventCategory CommandsOnAggServerSendEventPipelineHandler = "CommandsOnAggServerSendEventPipelineHandler";
        public static readonly EventCategory MeasTasksOnAggServerPipelineHandler = "MeasTasksOnAggServerPipelineHandler";
        public static readonly EventCategory OnlineMeasOnAggServerHandler = "OnlineMeasOnAggServerHandler";
        

        public static readonly EventCategory MessageProcessing = "Message processing";
        public static readonly EventCategory EventProcessing = "Processing";
        public static readonly EventCategory OnOnlineMeasurementResponseDeviceEvent = "OnOnlineMeasurementResponseDeviceEvent";
        public static readonly EventCategory OnOnlineMeasurementStatusSubscriberEvent = "OnOnlineMeasurementStatusSubscriberEvent";
        public static readonly EventCategory OnlineMeasOnAggServerPipelineHandler = "OnlineMeasOnAggServerPipelineHandler";
        

    }

    static class Events
    {


        public static readonly EventText HandlerCallLoadObjectSensorMethod = "Call 'LoadObjectSensor' method";
        public static readonly EventText HandlerCallLoadAllSensorsMethod = "Call 'LoadAllSensors' method";
        public static readonly EventText TaskNotContainSensors = "Task not contain sensors, therefore, it cannot be transferred to processing further";


    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName ExecutingAdapterCommand = "({4} - {5}) Executing adapter command = '{0}', Type = '{1}', Id = '{2}', AdapterType = '{3}'";
        //public static readonly TraceScopeName InvokingAdapterCommand = "Invoking command - Id='{0}'";
        //public static readonly TraceScopeName HandlingResult = "Id = '{0}', CommandType = '{1}', ResultType = '{2}', PartIndex = '{3}', Status = '{4}'";
        //public static readonly TraceScopeName ConvertingResult = "Id = '{0}', FromType = '{1}', ResultType = '{2}', PartIndex = '{3}', Status = '{4}'";
    }

    static class Exceptions
    {
        //public static readonly ExceptionText ErrorOccurredWhileStartingTask = "An error occurred while starting the task: TaskType = '{0}', TaskId = '{1}', Process = '{2}'";
        //public static readonly ExceptionText ErrorOccurredWhileInvokingControllerCommand = "An error occurred while invoking a controller command: CommandType = '{0}', CommandId = '{1}'";

    }
}
