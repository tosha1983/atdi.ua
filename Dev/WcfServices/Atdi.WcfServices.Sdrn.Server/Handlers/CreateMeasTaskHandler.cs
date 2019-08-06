﻿using System.Collections.Generic;
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
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;


        public CreateMeasTaskHandler(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }


        public MeasTaskIdentifier Handle(MeasTask task)
        {
            var loadSensor = new LoadSensor(_dataLayer, _logger);
            var measTaskProcess = new MeasTaskProcess(_eventEmitter, _dataLayer, _logger);
            var measTask = task;
            var measTaskIdentifier = new MeasTaskIdentifier();
            try
            {
                using (this._logger.StartTrace(Contexts.ThisComponent, Categories.Processing, this))
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
                                        var sens = loadSensor.LoadObjectSensor(station.SensorId.Value);
                                        if (sens != null)
                                        {
                                            if (sens.Id != null)
                                            {
                                                if (sens.Id.Value > 0)
                                                {
                                                    SensorIds.Add(station.SensorId.Value);
                                                }
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
                        measTaskProcess.Process(task, massSensor, MeasTaskMode.New.ToString(), false, out bool isSuccessTemp, out long? ID, true);
                        if (ID != null)
                        {
                            measTaskIdentifier.Value = ID.Value;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return measTaskIdentifier;
        }

    }
}


