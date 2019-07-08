using Atdi.Contracts.WcfServices.Sdrn.Server;
using System;
using System.ServiceModel;
using Atdi.Platform.Logging;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;

namespace Atdi.WcfServices.Sdrn.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SdrnsController : WcfServiceBase<ISdrnsController>, ISdrnsController
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public SdrnsController(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer,  ILogger logger)
        {
            this._eventEmitter = eventEmitter;
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public bool AddAssociationStationByEmitting(long[] emittingsId, long AssociatedStationID, string AssociatedStationTableName)
        {
            var saveResDb = new SaveResults(_dataLayer, _logger);
            return saveResDb.AddAssociationStationByEmitting(emittingsId, AssociatedStationID, AssociatedStationTableName);
        }

        public MeasTaskIdentifier CreateMeasTask(MeasTask task)
        {
            var createMeasTaskHandler = new CreateMeasTaskHandler(_eventEmitter, _dataLayer,  _logger);
            return createMeasTaskHandler.Handle(task);
        }

        public bool DeleteEmitting(long[] emittingsId)
        {
            var saveResDb = new SaveResults(_dataLayer, _logger);
            return saveResDb.DeleteEmitting(emittingsId);
        }

        public CommonOperationDataResult<int> DeleteMeasResults(MeasurementResultsIdentifier MeasResultsId)
        {
            var saveResDb = new SaveResults(_dataLayer, _logger);
            return saveResDb.DeleteResultFromDB(MeasResultsId, Status.Z.ToString());
        }

        public CommonOperationResult DeleteMeasTask(MeasTaskIdentifier taskId)
        {
            var measTaskProcess = new MeasTaskProcess(_eventEmitter, _dataLayer, _logger);
            return measTaskProcess.DeleteMeasTask(taskId);
        }

        public MeasurementResults[] GetMeasResultsByTaskId(MeasTaskIdentifier taskId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetMeasResultsByTaskId(taskId.Value);
        }

        public MeasurementResults[] GetMeasResultsHeaderByTaskId(MeasTaskIdentifier taskId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetMeasResultsHeaderByTaskId(taskId.Value);
        }

        public MeasurementResults[] GetMeasResultsHeaderSpecial(MeasurementType measurementType)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetMeasResultsHeaderSpecial(measurementType);
        }

        public MeasTask GetMeasTaskHeader(MeasTaskIdentifier taskId)
        {
            var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
            return loadMeasTask.GetMeasTaskHeader(taskId);
        }

        public MeasTask GetMeasTaskById(long id)
        {
            var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
            return loadMeasTask.GetMeasTaskById(id);
        }

        public MeasurementResults GetMeasurementResultByResId(long ResId, bool isLoadAllData, double? StartFrequency_Hz, double? StopFrequency_Hz)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetMeasurementResultByResId(ResId, isLoadAllData, StartFrequency_Hz, StopFrequency_Hz);
        }

        public ReferenceLevels GetReferenceLevelsByResultId(long resId, bool isLoadAllData, double? StartFrequency_Hz, double? StopFrequency_Hz)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetReferenceLevelsByResultId(resId, isLoadAllData, StartFrequency_Hz, StopFrequency_Hz);
        }

        public ResultsMeasurementsStation[] GetResMeasStation(long ResId, long StationId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetResMeasStation(ResId, StationId);
        }

        public ResultsMeasurementsStation GetResMeasStationById(long StationId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.ReadResultResMeasStation(StationId);
        }

        public ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(long ResId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetResMeasStationHeaderByResId(ResId);
        }

        public Route[] GetRoutes(long MeasResultsId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetRoutes(MeasResultsId);
        }

        public Sensor GetSensor(SensorIdentifier sensorId)
        {
            var loadSensor = new LoadSensor(_dataLayer, _logger);
            return loadSensor.LoadObjectSensor(sensorId.Value);
        }

        public SensorPoligonPoint[] GetSensorPoligonPoint(long MeasResultsId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetSensorPoligonPoint(MeasResultsId);
        }


        public Sensor[] GetSensors(ComplexCondition condition)
        {
            var loadSensor = new LoadSensor(_dataLayer, _logger);
            return loadSensor.LoadObjectSensor(condition);
        }

        public ShortResultsMeasurementsStation[] GetShortMeasResStation(long MeasResultsId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetShortMeasResStation(MeasResultsId);
        }

        public ShortMeasurementResults[] GetShortMeasResults()
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetShortMeasResults();
        }

        public ShortMeasurementResults[] GetShortMeasResultsByDate(GetShortMeasResultsByDateValue constraint)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetShortMeasResultsByDate(constraint);
        }

        public ShortMeasurementResults GetShortMeasResultsById(MeasurementResultsIdentifier measResultsId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetShortMeasResultsById(measResultsId);
        }

        public ShortMeasurementResults[] GetShortMeasResultsByTaskId(MeasTaskIdentifier taskId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetShortMeasResultsByTaskId(taskId.Value);
        }

        public ShortMeasurementResults[] GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, long taskId)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetShortMeasResultsByTypeAndTaskId(measurementType, taskId);
        }

        public ShortMeasurementResults[] GetShortMeasResultsSpecial(MeasurementType measurementType)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetShortMeasResultsSpecial(measurementType);
        }

        public ShortMeasTask GetShortMeasTask(MeasTaskIdentifier taskId)
        {
            var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
            return  loadMeasTask.GetShortMeasTask(taskId.Value);
        }

        public ShortMeasTask[] GetShortMeasTasks()
        {
            var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
            return loadMeasTask.GetShortMeasTasks();
        }

        public ShortSensor GetShortSensor(SensorIdentifier sensorId)
        {
            var loadSensor = new LoadSensor(_dataLayer, _logger);
            return loadSensor.LoadShortSensor(sensorId.Value);
        }

        public ShortSensor[] GetShortSensors()
        {
            var loadSensor = new LoadSensor(_dataLayer, _logger);
            return loadSensor.LoadListShortSensor();
        }

        public SOFrequency[] GetSOformMeasResultStation(GetSOformMeasResultStationValue options)
        {
            var analitics = new AnaliticsUnit(_dataLayer, _logger);
            return analitics.CalcAppUnit(options);
        }

        public StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(MeasTaskIdentifier taskId)
        {
            var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
            return loadMeasTask.GetStationDataForMeasurementsByTaskId(taskId.Value);
        }

        public StationLevelsByTask[] GetStationLevelsByTask(LevelsByTaskParams levelParams)
        {
            var calcStationLevelsByTask = new CalcStationLevelsByTask(_dataLayer, _logger);
            return calcStationLevelsByTask.GetStationLevelsByTask(levelParams);
        }

        public CommonOperationResult RunMeasTask(MeasTaskIdentifier taskId)
        {
            var measTaskProcess = new MeasTaskProcess(_eventEmitter, _dataLayer, _logger);
            return measTaskProcess.RunMeasTask(taskId);
        }

        public CommonOperationResult StopMeasTask(MeasTaskIdentifier taskId)
        {
            var measTaskProcess = new MeasTaskProcess(_eventEmitter, _dataLayer, _logger);
            return measTaskProcess.StopMeasTask(taskId);
        }

        public Emitting[] GetEmittingsByIcsmId(long[] ids, string icsmTableName)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetEmittingsByIcsmId(ids, icsmTableName);
        }

        public SignalingSysInfo[] GetSignalingSysInfos(long measResultId, decimal freq_Hz)
        {
            throw new NotImplementedException();
        }
    }
   

}


