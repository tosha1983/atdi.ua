using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.MS.PrimaryHandlers";

    }

    static class Categories
    {
        public static readonly EventCategory Disposing = "Disposing";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Executing = "Executing";
        public static readonly EventCategory Running = "Running";
        public static readonly EventCategory Registering = "Registering";
        public static readonly EventCategory Finalizing = "Finalizing";
        public static readonly EventCategory Handling = "Handling";
        public static readonly EventCategory PipelineHandling = "PipelineHandling";
        public static readonly EventCategory Converting = "Converting";

        public static readonly EventCategory Creating = "Creating";
        public static readonly EventCategory Finishing = "Finishing";

        public static readonly EventCategory Initilazing = "Initilazing";
        public static readonly EventCategory Callbacking = "Callbacking";
        public static readonly EventCategory Subscribes = "Subscribes";

        public static readonly EventCategory OnReceivedNewTaskCreateToMasterServerEvent = "OnReceivedNewTaskCreateToMasterServerEvent";
        public static readonly EventCategory OnReceivedDelTaskCreateToMasterServerEvent = "OnReceivedDelTaskCreateToMasterServerEvent";
        public static readonly EventCategory OnReceivedRunTaskCreateToMasterServerEvent = "OnReceivedRunTaskCreateToMasterServerEvent";
        public static readonly EventCategory OnReceivedStopTaskCreateToMasterServerEvent = "OnReceivedStopTaskCreateToMasterServerEvent";

        public static readonly EventCategory RegistrationSensorOnMasterServerHandler = "RegistrationSensorOnMasterServerHandler";
        public static readonly EventCategory CommandsMasterServerSendEventPipelineHandler = "CommandsMasterServerSendEventPipelineHandler";
        public static readonly EventCategory MeasTasksMasterServerSendEventPipelineHandler = "MeasTasksMasterServerSendEventPipelineHandler";
        public static readonly EventCategory GenerateMeasTasksPipelineHandler = "GenerateMeasTasksPipelineHandler";
        

    }

    static class Events
    {


        public static readonly EventText HandlerLoadListShortSensorMethod = "Call 'LoadListShortSensor' method";
        public static readonly EventText HandlerLoadShortSensorMethod = "Call 'LoadShortSensor' method";
        public static readonly EventText HandlerCallLoadObjectSensorMethod = "Call 'LoadObjectSensor' method";
        public static readonly EventText HandlerLoadObjectSensorMethod = "Call 'LoadObjectSensor' method";
        public static readonly EventText StartOperationWriting = "Start of validation operation and writing to main tables";
        public static readonly EventText EndOperationWriting = "End of validation operation and writing to main tables";
        public static readonly EventText IsAlreadySaveResults = "ResultId = {0} is already recorded in the XBS_RESMEASRAW table with Id = {1}, repeated recording is canceled";
        public static readonly EventText HandlerTypeWasRegistred = "The event subscriber type was registered successfully: '{0}'";
        public static readonly EventText HandlerCallDeleteResultFromDBMethod = "Call 'DeleteResultFromDB' method";
        public static readonly EventText HandlerCallGetMeasResultsByTaskIdMethod = "Call 'GetMeasResultsByTaskId' method";
        public static readonly EventText HandlerCallRunMeasTaskMethod = "Call 'RunMeasTask' method";
        public static readonly EventText HandlerCallStopMeasTaskMethod = "Call 'StopMeasTask' method";
        public static readonly EventText HandlerCallGetMeasResultsHeaderByTaskIdMethod = "Call 'GetMeasResultsHeaderByTaskId' method";
        public static readonly EventText HandlerCallGetMeasResultsHeaderSpecialMethod = "Call 'GetMeasResultsHeaderSpecial' method";
        public static readonly EventText HandlerCallDeleteMeasTaskMethod = "Call 'DeleteMeasTask' method";
        public static readonly EventText HandlerMeasTaskProcessEnd = "End 'MeasTaskProcess' action '{0}'";
        public static readonly EventText MeasTimeParamListIncorrect = "Incorrect task parameters (MeasTimeParamList.TimeStart or MeasTimeParamList.PerStart) greater  (MeasTimeParamList.TimeStop or MeasTimeParamList.PerStop)";
        public static readonly EventText HandlerDeleteMeasTaskProcess = "Start 'DeleteMeasTask' for ID  '{0}'";
        public static readonly EventText HandlerRunMeasTaskProcess = "Start 'RunMeasTask' for ID '{0}'";
        public static readonly EventText HandlerStopMeasTaskProcess = "Start 'StopMeasTask' for ID '{0}'";
        public static readonly EventText HandlerMeasTaskProcessStart = "Call 'MeasTaskProcess' method";
        public static readonly EventText HandlerCallShortReadTaskMethod = "Call 'ShortReadTask' method";
        public static readonly EventText HandlerCallGetMeasurementResultByResIdMethod = "Call 'GetMeasurementResultByResId' method";
        public static readonly EventText HandlerCallGetResMeasStationMethod = "Call 'GetResMeasStation' method";
        public static readonly EventText HandlerCallReadResultResMeasStationMethod = "Call 'ReadResultResMeasStation' method";
        public static readonly EventText HandlerCallGetResMeasStationHeaderByResIdMethod = "Call 'ReadResultResMeasStation' method";
        public static readonly EventText HandlerCallGetSensorPoligonPointMethod = "Call 'GetSensorPoligonPoint' method";
        public static readonly EventText HandlerGetShortMeasResStationMethod = "Call 'GetShortMeasResStation' method";
        public static readonly EventText HandlerGetShortMeasResultsMethod = "Call 'GetShortMeasResults' method";
        public static readonly EventText HandlerGetShortMeasResultsByDateMethod = "Call 'GetShortMeasResultsByDate' method";
        public static readonly EventText HandlerGetShortMeasResultsByIdMethod = "Call 'GetShortMeasResultsById' method";
        public static readonly EventText HandlerGetShortMeasResultsByTaskIdMethod = "Call 'GetShortMeasResultsByTaskId' method";
        public static readonly EventText HandlerGetShortMeasResultsByTypeAndTaskIdMethod = "Call 'GetShortMeasResultsByTypeAndTaskId' method";
        public static readonly EventText HandlerGetShortMeasResultsSpecialMethod = "Call 'GetShortMeasResultsSpecial' method";
        public static readonly EventText HandlerGetShortMeasTaskMethod = "Call 'GetShortMeasTask' method";
        public static readonly EventText HandlerGetShortMeasTasksMethod = "Call 'GetShortMeasTasks' method";
        public static readonly EventText HandlerCalcAppUnitMethod = "Call 'CalcAppUnit' method";
        public static readonly EventText HandlerGetStationDataForMeasurementsByTaskIdMethod = "Call 'GetStationDataForMeasurementsByTaskId' method";
        public static readonly EventText HandlerGetStationLevelsByTaskMethod = "Call 'GetStationLevelsByTask' method";
        public static readonly EventText HandlerCallGetRoutesMethod = "Call 'GetRoutes' method";

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
