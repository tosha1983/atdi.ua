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
        public static readonly EventCategory BusManagerInit = "Init BusManager";
        public static readonly EventCategory ArchiveResult = "Archive Result";
        public static readonly EventCategory SendActivitySensor = "Send activity sensor";
        public static readonly EventCategory StopMeasSdrTask = "Stop MeasSdrTask";
        public static readonly EventCategory SendMeasSdrTask = "Send MeasSdrTask";
        public static readonly EventCategory GetIdentTaskFromMeasTaskSDR = "Get MeasSdrTask properties";
        public static readonly EventCategory RecievedSensorLocation = "Recieved location sensor";
        public static readonly EventCategory StartGPS = "Start GPS";


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
        public static readonly EventText BusManagerInit = "Init BusManager";
        public static readonly EventText SuccessfullySavedIntoTableNH_MeasSDRResults = "Successfully saved into table - NH_MeasSDRResults";
        public static readonly EventText SuccessfullySavedIntoTableNHFSEMPLES = "Successfully saved into table - NH_FSEMPLES";
        public static readonly EventText SuccessfullySavedIntoTablenHMeasResultsLevel = "Successfully saved into table - nHMeasResultsLevel";
        public static readonly EventText SuccessfullySavedIntoTablenHMeasResultsFreq = "Successfully saved into table - nHMeasResultsFreq";
        public static readonly EventText SuccessfullySavedIntoTablenhFMeasSDRLoc = "Successfully saved into table - nhFMeasSDRLoc";
        public static readonly EventText ArchiveResult = "Archive result";
        public static readonly EventText SendActivitySensor = "Send activity sensor";
        public static readonly EventText StopMeasSdrTask = "stop meas task with ID = '{0}'";
        public static readonly EventText RecievedEventStop = "Recieved event stop meas task with ID = '{0}'";
        public static readonly EventText RecievedSensorLocation = "Recieved sensor location with ID = '{0}'";
        public static readonly EventText resHNotNull = "resH.TaskId = '{0}', resH.NN = '{1}', mDR.Id = '{2}'";
        public static readonly EventText resNotNull = "res.TaskId = '{0}', res.NN = '{1}', mDR.Id = '{2}'";
        public static readonly EventText SuccessfullySavedIntoTablenhMeasTaskSDR = "Successfully saved into table - nhMeasTaskSDR";
        public static readonly EventText SuccessfullySavedIntoTablenMeasSDRFREQPARAM = "Successfully saved into table - nMeasSDRFREQPARAM";
        public static readonly EventText SuccessfullySavedIntoTableNH_MeasSDRFreq = "Successfully saved into table - NH_MeasSDRFreq";
        public static readonly EventText SuccessfullySavedIntoTablenMeas_SDRLocParam = "Successfully saved into table - nMeas_SDRLocParam";
        public static readonly EventText SuccessfullySavedIntoTablenNHMeasSDRSoParam = "Successfully saved into table - nNHMeasSDRSoParam";
        public static readonly EventText RemoveRecordFromTableNH_MeasTaskSDR = "Remove record from table - NH_MeasTaskSDR";
        public static readonly EventText ArchiveRecordTableNH_MeasTaskSDR = "Archive record  table - NH_MeasTaskSDR";
        public static readonly EventText SuccessfullySavedIntoTablenHSensLocation = "Successfully saved into table - nHSensLocation";
        public static readonly EventText SuccessfullySavedIntoTableNH_Sensor = "Successfully saved into table - NH_Sensor";
        public static readonly EventText SuccessfullySavedIntoTableNH_SensorAntenna = "Successfully saved into table - NH_SensorAntenna";
        public static readonly EventText SuccessfullySavedIntoTableNH_AntennaPattern = "Successfully saved into table - NH_AntennaPattern";
        public static readonly EventText SuccessfullySavedIntoTableNH_SensorEquip = "Successfully saved into table - NH_SensorEquip";
        public static readonly EventText SuccessfullySavedIntoTablenHSensorEquipSensitivity = "Successfully saved into table - nHSensorEquipSensitivity";
        public static readonly EventText RecievedMeasTaskWithID = "Recieved meas task with ID = '{0}'";
        public static readonly EventText GetIdentTaskFromMeasTaskSDR = "Get MeasSdrTask properties";
        public static readonly EventText UnableStartGNSSWrapper = "Unable start GNSSWrapper due to {0}";
        public static readonly EventText UnableStopGNSSWrapper = "Unable stop GNSSWrapper due to {0}";
        public static readonly EventText UnableLoadSettings = "Unable load settings from {0} properly due to {1}, default settings will be used instead";
        
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
