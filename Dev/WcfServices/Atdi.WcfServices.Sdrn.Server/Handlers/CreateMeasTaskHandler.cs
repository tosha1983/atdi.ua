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
                        measTaskProcess.Process(task, SensorIds, MeasTaskMode.New.ToString(), false, out bool isSuccessTemp, out int? ID);
                        measTaskIdentifier.Value = ID.Value;
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


