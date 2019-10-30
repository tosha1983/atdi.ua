using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using Atdi.Contracts.Sdrn.Server;


namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    public class MeasTasksPipelineHandler : IPipelineHandler<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;

        public MeasTasksPipelineHandler(IPipelineSite pipelineSite, IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
            this._pipelineSite = pipelineSite;
        }

        /// <summary>
        /// Метод выполняющий создание нового таска на основе параметра MeasTaskPipeBox,
        /// который передается с WCF - сервиса
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ClientMeasTaskPiperesult Handle(ClientMeasTaskPipebox data, IPipelineContext<ClientMeasTaskPipebox, ClientMeasTaskPiperesult> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MeasTasksPipelineHandler, this))
            {
                PrepareSendEvent[] prepareSendEvents = null;
                var measTask = data.MeasTaskPipeBox;
                var loadSensor = new LoadSensor(_dataLayer, _logger);
                var measTaskProcess = new MeasTaskProcess(this._eventEmitter, this._dataLayer, this._environment, this._messagePublisher, this._logger);
                var measTaskIdentifier = new MeasTaskIdentifier();
                try
                {
                    using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
                    {
                        if (measTask.Id == null) measTask.Id = measTaskIdentifier;
                        if (measTask.Status == null) measTask.Status = Status.N.ToString();
                        var SensorIds = new List<long>();
                        if (measTask.Sensors != null)
                        {
                            for (int u = 0; u < measTask.Sensors.Length; u++)
                            {
                                var station = measTask.Sensors[u];
                                if (station.SensorId != null)
                                {
                                    if (station.SensorId.Value > 0)
                                    {
                                        if (!SensorIds.Contains(station.SensorId.Value))
                                        {
                                            var sensorIdentifier = loadSensor.LoadSensorId(station.SensorId.Value, out string sensorName, out string sensorTechId);
                                            if (sensorIdentifier != null)
                                            {
                                                if (sensorIdentifier.Value > 0)
                                                {
                                                    SensorIds.Add(station.SensorId.Value);
                                                    station.SensorName = sensorName;
                                                    station.SensorTechId = sensorTechId;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var massSensor = SensorIds.ToArray();
                        if (massSensor.Length > 0)
                        {
                            measTaskProcess.Process(measTask, massSensor, MeasTaskMode.New.ToString(), false, out bool isSuccessTemp, out long? ID, true, out prepareSendEvents);
                            if (ID != null)
                            {
                                measTaskIdentifier.Value = ID.Value;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    measTaskIdentifier.Value = -1;
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
                if (measTaskIdentifier.Value > 0)
                {
                    data.MeasTaskPipeBox = measTask;
                    data.PrepareSendEvents = prepareSendEvents;
                }
                else
                {
                    this._logger.Exception(Contexts.ThisComponent,  new Exception(Exceptions.ForTheNewTaskNonExistentSensors));
                    throw new Exception(Exceptions.ForTheNewTaskNonExistentSensors);
                }
                // передача в обработчик ClientMeasTasksSendPipelineHandler 
                var res = context.GoAhead(data);
                if (res == null)
                {
                    var site = this._pipelineSite.GetByName<ClientMeasTaskPipebox, ClientMeasTaskPiperesult>(Pipelines.ClientMeasTaskSendEvents);
                    var resultSendEvent = site.Execute(new ClientMeasTaskPipebox()
                    {
                        MeasTaskPipeBox = data.MeasTaskPipeBox,
                        PrepareSendEvents = data.PrepareSendEvents
                    });

                    return resultSendEvent;
                }
                else
                {
                    return res;
                }
            }
        }
    }
}
