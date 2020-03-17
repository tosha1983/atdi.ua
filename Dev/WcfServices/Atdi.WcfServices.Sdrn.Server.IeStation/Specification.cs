using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    static class Contexts
    {
        public static readonly EventContext ThisComponent = "SDRN.Server.WCF";
    }

    static class Categories
    {
        public static readonly EventCategory Declaring = "Declaring";
        public static readonly EventCategory Registration = "Registration";
        public static readonly EventCategory Processing = "Processing";
        public static readonly EventCategory ImportData = "ImportData";
    }

    static class Events
    {
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
        public static readonly EventText HandlerCallLoadObjectSensorMethod = "Call 'LoadObjectSensor' method";
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
        public static readonly EventText HandlerLoadShortSensorMethod = "Call 'LoadShortSensor' method";
        public static readonly EventText HandlerLoadListShortSensorMethod = "Call 'LoadListShortSensor' method";
        public static readonly EventText HandlerCalcAppUnitMethod = "Call 'CalcAppUnit' method";
        public static readonly EventText HandlerGetStationDataForMeasurementsByTaskIdMethod = "Call 'GetStationDataForMeasurementsByTaskId' method";
        public static readonly EventText HandlerGetStationLevelsByTaskMethod = "Call 'GetStationLevelsByTask' method";
        public static readonly EventText HandlerCallGetRoutesMethod = "Call 'GetRoutes' method";
        public static readonly EventText HandlerLoadObjectSensorMethod = "Call 'LoadObjectSensor' method";
        public static readonly EventText HandlerGetAllRefSpectrumMethod = "Call 'GetAllRefSpectrum' method";
        public static readonly EventText HandlerImportRefSpectrumMethod = "Call 'ImportRefSpectrum' method";
        public static readonly EventText HandlerSynchroAreasMethod = "Call 'SynchroAreas' method";
        public static readonly EventText HandlerSynchroStationsExtendedMethod = "Call 'SynchroStationsExtended' method";
        public static readonly EventText HandlerCreateDataSynchronizationProcessMethod = "Call 'CreateDataSynchronizationProcess' method";
        public static readonly EventText HandlerCurrentDataSynchronizationProcessMethod = "Call 'CurrentDataSynchronizationProcess' method";
        public static readonly EventText HandlerSaveOutputProtocolsToDBMethod = "Call 'SaveOutputProtocolsToDB' method";
        public static readonly EventText HandlerClearProtocolMethod = "Call 'ClearProtocol' method";
        public static readonly EventText HandlerSaveDataSynchronizationProcessToDBMethod = "Call 'SaveDataSynchronizationProcessToDB' method";
        public static readonly EventText HandlerGetRefSpectrumByIdsMethod = "Call 'GetRefSpectrumByIds' method";
        public static readonly EventText HandlerGetProtocolsByParametersMethod = "Call 'GetProtocolsByParameters' method";
        public static readonly EventText OneElementsStationsIsEmpty = "One of the elements of the array of stations transmitted from the client side is empty!";
        public static readonly EventText DataSynchronizationProcessCompleted = "Synchronization process is completed!";
        public static readonly EventText DeleteDuplicateRefSpectrumRecords = "Call 'DeleteDuplicateRefSpectrumRecords' method";
        public static readonly EventText GetGroupSensors = "Call 'GetGroupSensors' method";
        public static readonly EventText SelectGroupSensor = "Call 'SelectGroupSensor' method";
        public static readonly EventText CountUniqueStations = "Call 'CountUniqueStations' method";
        public static readonly EventText FillStationDataToCorrespond = "Call 'FillStationDataToCorrespond' method";
        public static readonly EventText DeleteUnestimatedEmittings = "Call 'DeleteUnestimatedEmittings' method";
        public static readonly EventText FillEmittingDataToCorrespond = "Call 'FillEmittingDataToCorrespond' method";
        public static readonly EventText SynchroEmittings = "Call 'SynchroEmittings' method";
        public static readonly EventText FillingProtocolStationData = "Call 'FillingProtocolStationData' method";
        public static readonly EventText OrderProtocols = "Call 'OrderProtocols' method";
        public static readonly EventText GetEmittings = "Call 'GetEmittings' method";
        public static readonly EventText GetHeadRefSpectrumIdsBySDRN = "Call 'GetHeadRefSpectrumIdsBySDRN' method";
        public static readonly EventText GetSensorIdsBySDRN = "Call 'GetSensorIdsBySDRN' method";
        public static readonly EventText GetAreas = "Call 'GetAreas' method";
        public static readonly EventText GetStationExtended = "Call 'GetStationExtended' method";
        public static readonly EventText RecoveryDataSynchronizationProcess = "Call 'RecoveryDataSynchronizationProcess' method";
        public static readonly EventText RunDataSynchronizationProcess = "Call 'RunDataSynchronizationProcess' method";
        public static readonly EventText UpdateStatusStationExtended = "Call 'UpdateStatusStationExtended' method";

        

    }
    static class TraceScopeNames
    {
        public static readonly TraceScopeName MessageProcessing = "Message processing";
    }

    static class Exceptions
    {
        //public static readonly string ServiceHostWasNotInitialized = "The service host was not initialized";
    }
}
