using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.ControlA
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN Device Control A";
    }

    static class Categories
    {
        public static readonly EventCategory MessageCheckLocation = "Check location";
        public static readonly EventCategory MeasSdrTaskUpdateStatus = "Update MeasSdrTask";
        public static readonly EventCategory UpdateSensorStatus = "Update sensor";
        public static readonly EventCategory LoadSensor = "Load sensor";
        public static readonly EventCategory CreateNewObjectSensor = "Create sensor";
        public static readonly EventCategory LoadActiveTaskSdrResults = "Load active TaskSdrResults";
        public static readonly EventCategory ProcessMeasurements = "Process measurements";
        public static readonly EventCategory FindMeasTaskSDR = "Find MeasTaskSDR";
        public static readonly EventCategory GetAllMeasTaskSDR = "Get all MeasTaskSDR";
        public static readonly EventCategory LoadMeasTaskSDR = "Load MeasTaskSDR";
        public static readonly EventCategory GetMaxIdFromResults = "Get maximum id from TaskSdrResults";
        public static readonly EventCategory LoadDataMeasByTaskId = "Load data meas by TaskId";
        public static readonly EventCategory SaveisSendMeasTaskSDRResults = "Save TaskSDRResults";
        public static readonly EventCategory SaveStatusMeasTaskSDRResults = "Save status TaskSDRResults";
        public static readonly EventCategory SaveStatusMeasTaskSDR = "Save status MeasTaskSDR";
        public static readonly EventCategory SaveStatusResultSDR = "Save status MeasSDRResults";
        public static readonly EventCategory SaveMeasResultSDR = "Save  MeasSDRResults";
        public static readonly EventCategory CreateNewMeasTaskSDR = "Create new MeasTaskSDR";
        public static readonly EventCategory DeleteMeasTaskSDR = "Delete MeasTaskSDR";
        public static readonly EventCategory ArchiveMeasTaskSDR = "Archive MeasTaskSDR";
        

    }

    static class Events
    {
        public static readonly EventText CheckLocation = "Check location coordinates";
        public static readonly EventText UpdateStatus = "Update status MeasSdrTask";
        public static readonly EventText UpdateSensorStatus = "Update status MeasSdrTask";
        public static readonly EventText LoadSensor = "Load sensor";
        public static readonly EventText CreateNewObjectSensor = "Create sensor";
        public static readonly EventText LoadActiveTaskSdrResults = "Load results";
        public static readonly EventText ProcessMeasurements = "Process measurements";
        public static readonly EventText FindMeasTaskSDR = "Find MeasTaskSDR";
        public static readonly EventText GetAllMeasTaskSDR = "Get all MeasTaskSDR";
        public static readonly EventText LoadMeasTaskSDR = "Load MeasTaskSDR";
        public static readonly EventText GetMaxIdFromResults = "Get maximum id from TaskSdrResults";
        public static readonly EventText LoadDataMeasByTaskId = "Load data meas by TaskId";
        public static readonly EventText SaveisSendMeasTaskSDRResults = "Save TaskSDRResults";
        public static readonly EventText SaveStatusMeasTaskSDRResults = "Save status TaskSDRResults";
        public static readonly EventText SaveStatusMeasTaskSDR = "Save status MeasTaskSDR";
        public static readonly EventText SaveStatusResultSDR = "Save status MeasSDRResults";
        public static readonly EventText SaveMeasResultSDR = "Save  MeasSDRResults";
        public static readonly EventText CreateNewMeasTaskSDR = "Create new MeasTaskSDR";
        public static readonly EventText DeleteMeasTaskSDR = "Delete MeasTaskSDR";
        public static readonly EventText ArchiveMeasTaskSDR = "Archive MeasTaskSDR";
        public static readonly EventText TaskProperties = "TaskId = {0}, NN = {1}";
    }
    static class TraceScopeNames
    {
        //public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {
        //public static readonly string ServiceHostWasNotInitialized = "The service host was not initialized";
    }
}
