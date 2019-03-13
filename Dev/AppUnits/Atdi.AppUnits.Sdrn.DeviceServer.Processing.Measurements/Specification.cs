﻿using Atdi.Platform;
using Atdi.Platform.Logging;
using System;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    static class Contexts
    {
        public static readonly EventContext SOTaskWorker = "SOTaskWorker";
    

        
    }

    static class Categories
    {
        public static readonly EventCategory Measurements = "Measurements";
    }

    static class Events
    {
        public static readonly EventText StartSOTaskWorker = "Start SOTaskWorker context.Task.Id = {0}";
        public static readonly EventText TaskIsCancled = " Task context.Task.Id = {0} is canceled";
        public static readonly EventText SendMeasureTraceCommandToController = " Send 'MeasureTraceCommand' to controller Id = {0}";
        public static readonly EventText HandlingErrorSendCommandController = "Handling error send command  Id = {0} to controller";
        public static readonly EventText SleepThread = "Sleep thread for command id = {0} at time {1}";
        public static readonly EventText MaximumDurationMeas = "Maximum duration meas can not be -1";

        


    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {

        public static readonly string UnknownErrorSOTaskWorker = "Unknown error in SOTaskWorker";

       
    }
}