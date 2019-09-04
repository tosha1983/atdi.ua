using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.OnlineMeas";
        public static readonly EventContext WebSocket = "SDRN.WebSocket";
    }

    static class Categories
    {
        public static readonly EventCategory Disposing = "Disposing";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory Running = "Running";
        public static readonly EventCategory Registering = "Registering";
        public static readonly EventCategory Finalizing = "Finalizing";
        public static readonly EventCategory Handling = "Handling";
        public static readonly EventCategory Converting = "Converting";
        public static readonly EventCategory Initilazing = "Initilazing";
        public static readonly EventCategory ConfigLoading = "Config loading";
        public static readonly EventCategory GetMeasTaskFromBus = "Get MeasTask from bus";
        public static readonly EventCategory SendMeasTaskHandlerStart = "Get MeasTask from bus";
        public static readonly EventCategory SendCommandHandlerHandlerStart = "SendCommandHandler started";
        public static readonly EventCategory ClientReadyTakeMeasResultTaskWorker = "ClientReadyTakeMeasResultTaskWorker";
        public static readonly EventCategory ClientTaskRegistrationTaskWorker = "ClientTaskRegistrationTaskWorker";
        public static readonly EventCategory SendCommandForMeasResultTaskWorker = "SendCommandForMeasResultTaskWorker";
        public static readonly EventCategory SendCommandForRegistrationTaskWorker = "SendCommandForRegistrationTaskWorker";
        




    }

    static class Events
    {
        public static readonly EventText ReceivedSensorRegistrationConfirmation = "Received sensor registration confirmation";
        public static readonly EventText MessageIsBeingHandled = "The message is being handled: Type = '{0}'";
        public static readonly EventText IncorrectMessage = "Incorrect message";
        public static readonly EventText CreateNewTaskParameters = "Create new TaskParameters";
        public static readonly EventText UpdateTaskParameters = "Update TaskParameters";
        public static readonly EventText StartedEventTask = "Started EventTask Task.Id = {0}";
        public static readonly EventText StartedSendRegistrationResultTask = "Started SendRegistrationResultTask Task.Id = {0}";
        public static readonly EventText HandlingErrorSendCommandController = "Handling error send command  Id = {0} to controller";
        public static readonly EventText SleepThread = "Sleep thread for command id = {0} at time {1}";
        public static readonly EventText OnlineTaskIsCancled = "Online task is canceled";
        public static readonly EventText ErrorReceivingResult = "An error occurred while receiving the result for Online Measurement. Send cancellation message.";
        public static readonly EventText StartedClientTaskRegistrationTaskWorker = "Started ClientTaskRegistrationTaskWorker...";
        public static readonly EventText FinishClientTaskRegistrationTaskWorker = "Finish ClientTaskRegistrationTaskWorker...";
        public static readonly EventText StartedClientReadyTakeMeasResultTaskWorker = "Started ClientReadyTakeMeasResultTaskWorker...";
        public static readonly EventText StoppingThreadAnErrorCommunicatingAdapter = "Stopping a thread due to an error communicating with the adapter";


    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName HandlingResult = "Id = '{0}', CommandType = '{1}', ResultType = '{2}', PartIndex = '{3}', Status = '{4}'";
        
    }

    static class Exceptions
    {
        public static readonly string ConfigWasNotLoaded = "The config was not loaded";
        public static readonly string IncorrectMessageParams = "Incorrect message parameters";
        public static readonly string ErrorSaveSensorParametersInDB = "Error in method 'SaveSensorParametersInDB'";
        public static readonly string UnknownErrorsInSendMeasTaskHandler = "Unknown errors in 'SendMeasTaskHandler'";
        public static readonly string UnknownErrorsInSendCommandHandler = "Unknown errors in 'SendCommandHandler'";
        public static readonly string UnknownErrorsInSendRegistrationResultHandler = "Unknown errors in 'SendRegistrationResultHandler'";
        public static readonly string UnknownErrorsInSendUpdatingResultHandler = "Unknown errors in 'SendUpdatingResultHandler'";
        public static readonly string DeviceServerCanNotBeStarted = "The DeviceServer can not be started, because there is no information about the registered object in the database";


    }
}
