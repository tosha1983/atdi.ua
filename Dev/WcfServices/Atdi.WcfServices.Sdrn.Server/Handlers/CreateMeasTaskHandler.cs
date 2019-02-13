using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;


namespace Atdi.WcfServices.Sdrn.Server
{
    public class CreateMeasTaskHandler 
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

    

        public CreateMeasTaskHandler(ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._environment = environment;
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }



        public MeasTaskIdentifier Handle(MeasTask task)
        {
            var loadSensor = new LoadSensor(_environment, _messagePublisher, _eventEmitter, _dataLayer, _logger);
            var measTaskProcess = new MeasTaskProcess(_environment, _messagePublisher, _eventEmitter, _dataLayer, _logger);
            var measTask = task;
            var measTaskIdentifier = new MeasTaskIdentifier();
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.Processing, this))
            {
                if (measTask.Id == null) measTask.Id = measTaskIdentifier;
                if (measTask.Status == null) measTask.Status = "N";
                var SensorIds = new List<int>();
                if (measTask.Stations != null)
                {
                    foreach (var station in measTask.Stations)
                    {
                        if (station.StationId != null)
                        {
                            if (station.StationId != null)
                            {
                                if (station.StationId.Value > 0)
                                {
                                    if (!SensorIds.Contains(station.StationId.Value))
                                    {
                                        var sens = loadSensor.LoadObjectSensor(station.StationId.Value);
                                        if (sens != null)
                                        {
                                            if (sens.Id != null)
                                            {
                                                if (sens.Id.Value > 0)
                                                {
                                                    SensorIds.Add(station.StationId.Value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (SensorIds.Count > 0)
                {
                    bool isSuccessTemp = false;
                    int? ID = null;
                    measTaskProcess.Process(task, SensorIds, "New", false, out isSuccessTemp, out ID);
                    measTaskIdentifier.Value = ID.Value;
                }
            }
            return measTaskIdentifier;
        }

    }
}


