using Atdi.Platform;
using Atdi.Platform.Logging;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    static class Contexts
    {
        
        public static readonly EventContext SendResultsWorker = "SendResultsWorker";
        public static readonly EventContext MeasurementTaskWorker = "MeasurementTaskWorker";
        public static readonly EventContext DeferredTaskWorker = "DeferredTaskWorker";
        public static readonly EventContext CommandTaskWorker = "CommandTaskWorker";
        public static readonly EventContext GPSWorker = "GPSWorker";
        public static readonly EventContext ProcessingTaskFromDBWorker = "ProcessingTaskFromDBWorker";
        public static readonly EventContext RegisterSensorTaskWorker = "RegisterSensorTaskWorker";
        public static readonly EventContext DispatcherWorker = "DispatcherWorker";
        public static readonly EventContext SendRegistrationResultTaskWorker = "SendRegistrationResultTaskWorker";
        public static readonly EventContext EventTaskWorker = "EventTaskWorker";
        public static readonly EventContext DeviceCommandTaskWorker = "DeviceCommandTaskWorker";
        public static readonly EventContext EventCommand = "EventCommand";

        
    }

    static class Categories
    {
        public static readonly EventCategory Processing = "Processing";
    }

    static class Events
    {
        public static readonly EventText ReceivedSensorUpdatingConfirmation = "Received sensor updating confirmation";
        public static readonly EventText ReceivedSensorRegistrationConfirmation = "Received sensor registration confirmation";
        public static readonly EventText SensorInformationSendToSDRNS = "Sensor information succesfully sended in SDRNS";
        public static readonly EventText SensorInformationRecordedDB = "Sensor information succesfully recorded in database";
        public static readonly EventText SensorInformationUpdatedInDb = "Sensor information succesfully updated in database";
        public static readonly EventText SensorInformationNotRecordedDB = "Sensor information not recorded in database";
        public static readonly EventText MessageTimedOut = "Message Timed Out";
        public static readonly EventText StartDeferredTaskWorker = "Start DeferredTaskWorker context.Task.Id = {0}";
        public static readonly EventText StartRegisterSensorTaskWorker = "Start RegisterSensorTaskWorker context.Task.Id = {0}";
        public static readonly EventText StartDispatcherWorker = "Start DispatcherWorker context.Task.Id = {0}";
        public static readonly EventText StartSendRegistrationResultTaskWorker = "Start SendRegistrationResultTaskWorker context.Task.Id = {0}";
        public static readonly EventText StartEventTaskWorker = "Start EventTaskWorker context.Task.Id = {0}";
        public static readonly EventText StartDeferredTask = "Start deferred task Task.Id = {0}";
        public static readonly EventText EndDeferredTask = "End deferred task Task.Id = {0}";
        public static readonly EventText StartProcessingTaskFromDBWorker = "Start ProcessingTaskFromDBWorker context.Task.Id = {0}";
        public static readonly EventText StartGPSWorker = "Start GPSWorker context.Task.Id = {0}";
        public static readonly EventText StartQueueEventTaskWorker = "Start QueueEventTaskWorker context.Task.Id = {0}";
        public static readonly EventText StartTaskQueueEventTaskWorker = "Start QueueEventTaskWorker Task.Id = {0}";
        public static readonly EventText EndTaskQueueEventTaskWorker = "End QueueEventTaskWorker Task.Id = {0}";
        public static readonly EventText StartTaskProcessingTaskFromDBWorker = "Start ProcessingTaskFromDBWorker Task.Id = {0}";
        public static readonly EventText EndTaskProcessingTaskFromDBWorker = "End ProcessingTaskFromDBWorker Task.Id = {0}";


        public static readonly EventText SensorAlreadyExists = "Sensor with Name = '{0}' and TechId = '{1}' already registered on the SDRN server";

        public static readonly EventText StartDeviceCommandTaskWorker = "Start DeviceCommandTaskWorker context.Task.Id = {0}";
        public static readonly EventText EndDeviceCommandTaskWorker = "End DeviceCommandTaskWorker context.Task.Id = {0}";

    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {

        public static readonly string DeviceCommandTaskWorker = "Unknown error in DeviceCommandTaskWorker";
        public static readonly string UnknownErrorEventTaskWorker = "Unknown error in EventTaskWorker";
        public static readonly string UnknownErrorSendRegistrationResultTaskWorker = "Unknown error in SendRegistrationResultTaskWorker";
        public static readonly string UnknownErrorDispatcherWorker = "Unknown error in DispatcherWorker";
        public static readonly string UnknownErrorRegisterSensorTaskWorker = "Unknown error in RegisterSensorTaskWorker";
        public static readonly string UnknownErrorGPSWorker = "Unknown error in GPSWorker";
        public static readonly string UnknownErrorDeferredTaskWorker = "Unknown error in DeferredTaskWorker";
        public static readonly string UnknownErrorSendResultsWorker = "Unknown error in SendResultsWorker";
        public static readonly string UnknownErrorQueueEventTaskWorker = "Unknown error in QueueEventTaskWorker";
        public static readonly string MeasurementTypeNotsupported = $"MeasurementType {0} not supported";
        public static readonly string DeviceServerCanNotBeStarted = "The DeviceServer can not be started, because there is no information about the registered object in the database";
        public static readonly string NotFoundInformationWithGetDevicesProperties = "Not found information about sensor by 'GetDevicesProperties'";
        public static readonly string NotFoundInformationAboutSensor = "Not found information about sensor";
        public static readonly string UnknownErrorEventCommand = "Unknown error in EventCommand";
        
    }
}
