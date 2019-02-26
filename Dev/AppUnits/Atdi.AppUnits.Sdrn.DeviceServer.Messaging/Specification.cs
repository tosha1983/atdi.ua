﻿using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.Messaging";
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


    }

    static class Events
    {
        public static readonly EventText MessageIsBeingHandled = "The message is being handled: Type = '{0}'";
        public static readonly EventText IncorrectMessage = "Incorrect message";
        public static readonly EventText StartProcessSendMeasTask = "Start process SendMeasTaskHandler";

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

    }
}