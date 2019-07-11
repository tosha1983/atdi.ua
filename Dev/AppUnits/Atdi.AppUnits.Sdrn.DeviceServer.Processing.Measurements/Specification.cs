using Atdi.Platform;
using Atdi.Platform.Logging;
using System;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    static class Contexts
    {
        public static readonly EventContext SOTaskWorker = "SOTaskWorker";
        public static readonly EventContext SignalizationTaskWorker = "SignalizationTaskWorker";
        public static readonly EventContext BandWidthTaskWorker = "BandWidthTaskWorker";
        public static readonly EventContext SysInfoTaskWorker = "SysInfoTaskWorker";
        public static readonly EventContext SignalizationTaskResultHandler = "SignalizationTaskResultHandler";


    }

    static class Categories
    {
        public static readonly EventCategory Measurements = "Measurements";
    }

    static class Events
    {
        public static readonly EventText StartSOTaskId = "Start SOTask Id = {0}";
        public static readonly EventText StartSOTaskWorker = "Start SOTaskWorker context.Task.Id = {0}";
        public static readonly EventText TaskIsCancled = " Task context.Task.Id = {0} is canceled";
        public static readonly EventText SendMeasureTraceCommandToController = " Send 'MeasureTraceCommand' to controller Id = {0}";
        public static readonly EventText HandlingErrorSendCommandController = "Handling error send command  Id = {0} to controller";
        public static readonly EventText SleepThread = "Sleep thread for command id = {0} at time {1}";
        public static readonly EventText MaximumDurationMeas = "Maximum duration meas can not be -1";
        public static readonly EventText FinishedBandWidthTaskWorker = "Finished 'BandWidthTaskWorker'";
        public static readonly EventText FinishedSysInfoTaskWorker = "Finished 'SysInfoTaskWorker'";


        public static readonly EventText StartSignalizationTaskWorker = "Start SignalizationTaskWorker context.Task.Id = {0}";
        public static readonly EventText StartBandWidthTaskWorker = "Start BandWidthTaskWorker context.Task.Id = {0}";
        public static readonly EventText GetEmittingDetailedNull = "'GetEmittingDetailed' returned NULL";
        public static readonly EventText CalcGroupingNull = "'CalcGrouping' returned NULL";

        public static readonly EventText StartSysInfoTaskWorker = "Start SysInfoTaskWorker context.Task.Id = {0}";


    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {

        public static readonly string UnknownErrorSOTaskWorker = "Unknown error in SOTaskWorker";
        public static readonly string UnknownErrorSignalizationTaskWorker = "Unknown error in SignalizationTaskWorker";
        public static readonly string UnknownErrorBandWidthTaskWorker = "Unknown error in BandWidthTaskWorker";
        public static readonly string UnknownErrorSysInfoTaskWorker = "Unknown error in SysInfoTaskWorker";
        public static readonly string ErrorConvertToDispatchProcess = "Error convert to DispatchProcess";
        public static readonly string ParentProcessIsNull = "Parent process is null";
        public static readonly string ParentProcessIsNotTypeDispatchProcess = "Parent process is not type 'DispatchProcess'";
        public static readonly string AfterConvertParentProcessIsNull = "After convert parent process Is Null value";
        
    }
}
