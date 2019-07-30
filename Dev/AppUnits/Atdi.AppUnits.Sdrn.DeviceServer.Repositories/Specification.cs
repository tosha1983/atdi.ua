using Atdi.Platform;
using Atdi.Platform.Logging;
using System;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "Repositories";
    

        
    }

    static class Categories
    {
        public static readonly EventCategory LastUpdate = "LastUpdate";
        public static readonly EventCategory ErrorGetMessageFromDB = "Error get message from DB";
        public static readonly EventCategory ObjectWasNotSaved = "Object was not saved";
        public static readonly EventCategory TheFileWasNotDeleted = "The file was not deleted";
        public static readonly EventCategory TheFileWasNotUpdated = "The file was not updated";
    }

    static class Events
    {
      
        


    }
    static class TraceScopeNames
    {

    }

    static class Exceptions
    {

        public static readonly string UnknownErrorSOTaskWorker = "Unknown error";

       
    }
}
