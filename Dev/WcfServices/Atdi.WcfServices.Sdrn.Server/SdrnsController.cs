using Atdi.Contracts.WcfServices.Sdrn.Server;
using System;
using System.ServiceModel;
using Atdi.Platform.Logging;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;


namespace Atdi.WcfServices.Sdrn.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SdrnsController : WcfServiceBase<ISdrnsController>, ISdrnsController
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public SdrnsController(ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._environment = environment;
            this._messagePublisher = messagePublisher;
            this._eventEmitter = eventEmitter;
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public MeasTaskIdentifier CreateMeasTask(MeasTask task)
        {
            CreateMeasTaskHandler createMeasTaskHandler = new CreateMeasTaskHandler(_environment, _messagePublisher, _eventEmitter, _dataLayer, _logger);
            return createMeasTaskHandler.Handle(task);
        }

        public CommonOperationDataResult<int> DeleteMeasResults(MeasurementResultsIdentifier MeasResultsId)
        {
            throw new NotImplementedException();
        }

        public CommonOperationResult DeleteMeasTask(MeasTaskIdentifier taskId)
        {
            throw new NotImplementedException();
        }

        public MeasurementResults[] GetMeasResultsByTaskId(MeasTaskIdentifier taskId)
        {
            throw new NotImplementedException();
        }

        public MeasurementResults[] GetMeasResultsHeaderByTaskId(MeasTaskIdentifier taskId)
        {
            throw new NotImplementedException();
        }

        public MeasurementResults[] GetMeasResultsHeaderSpecial(MeasurementType measurementType)
        {
            throw new NotImplementedException();
        }

        public MeasTask GetMeasTaskHeader(MeasTaskIdentifier taskId)
        {
            throw new NotImplementedException();
        }

        public MeasurementResults GetMeasurementResultByResId(int ResId)
        {
            throw new NotImplementedException();
        }

        public ResultsMeasurementsStation[] GetResMeasStation(int MeasResultsId, int StationId)
        {
            throw new NotImplementedException();
        }

        public ResultsMeasurementsStation GetResMeasStationById(int StationId)
        {
            throw new NotImplementedException();
        }

        public ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(int ResId)
        {
            throw new NotImplementedException();
        }

        public Route[] GetRoutes(int MeasResultsId)
        {
            throw new NotImplementedException();
        }

        public Sensor GetSensor(SensorIdentifier sensorId)
        {
            throw new NotImplementedException();
        }

        public SensorPoligonPoint[] GetSensorPoligonPoint(int MeasResultsId)
        {
            throw new NotImplementedException();
        }

        public Sensor[] GetSensors()
        {
            throw new NotImplementedException();
        }

        public ShortResultsMeasurementsStation[] GetShortMeasResStation(int MeasResultsId)
        {
            throw new NotImplementedException();
        }

        public ShortMeasurementResults[] GetShortMeasResults()
        {
            throw new NotImplementedException();
        }

        public ShortMeasurementResults[] GetShortMeasResultsByDate(GetShortMeasResultsByDateValue constraint)
        {
            throw new NotImplementedException();
        }

        public ShortMeasurementResults GetShortMeasResultsById(MeasurementResultsIdentifier measResultsId)
        {
            throw new NotImplementedException();
        }

        public ShortMeasurementResults[] GetShortMeasResultsByTaskId(MeasTaskIdentifier taskId)
        {
            throw new NotImplementedException();
        }

        public ShortMeasurementResults[] GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, int taskId)
        {
            throw new NotImplementedException();
        }

        public ShortMeasurementResults[] GetShortMeasResultsSpecial(MeasurementType measurementType)
        {
            throw new NotImplementedException();
        }

        public ShortMeasTask GetShortMeasTask(MeasTaskIdentifier taskId)
        {
            throw new NotImplementedException();
        }

        public ShortMeasTask[] GetShortMeasTasks()
        {
            throw new NotImplementedException();
        }

        public ShortSensor GetShortSensor(SensorIdentifier sensorId)
        {
            throw new NotImplementedException();
        }

        public ShortSensor[] GetShortSensors()
        {
            throw new NotImplementedException();
        }

        public SOFrequency[] GetSOformMeasResultStation(GetSOformMeasResultStationValue options)
        {
            throw new NotImplementedException();
        }

        public StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(MeasTaskIdentifier taskId)
        {
            throw new NotImplementedException();
        }

        public StationLevelsByTask[] GetStationLevelsByTask(LevelsByTaskParams levelParams)
        {
            throw new NotImplementedException();
        }

        public CommonOperationResult RunMeasTask(MeasTaskIdentifier taskId)
        {
            throw new NotImplementedException();
        }

        public CommonOperationResult StopMeasTask(MeasTaskIdentifier taskId)
        {
            throw new NotImplementedException();
        }
    }

    

}


