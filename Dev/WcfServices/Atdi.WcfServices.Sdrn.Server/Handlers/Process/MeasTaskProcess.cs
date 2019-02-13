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
using System.Linq;


namespace Atdi.WcfServices.Sdrn.Server
{
    public class MeasTaskProcess
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;



        public MeasTaskProcess(ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._environment = environment;
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }

        public int? CreateNewMeasTask(MeasTask measTask, string ActionType)
        {
            int? NewIdMeasTask = -1;
            if (measTask != null)
            {
                SaveMeasTask saveMeasTask = new SaveMeasTask(_environment, _messagePublisher, _eventEmitter, _dataLayer, _logger);
                LoadMeasTask loadMeasTask = new LoadMeasTask(_environment, _messagePublisher, _eventEmitter, _dataLayer, _logger);
                MeasTask Data_ = measTask;
                List<MeasTask> listMeastTask = loadMeasTask.ShortReadTask(Data_.Id.Value);
                if (listMeastTask.Count == 0)
                {
                    Data_.UpdateStatus(ActionType);
                    NewIdMeasTask = saveMeasTask.SaveMeasTaskInDB(Data_);
                }
                else
                {
                    Data_ = loadMeasTask.ShortReadTask(Data_.Id.Value)[0];
                    Data_.UpdateStatus(ActionType);
                    saveMeasTask.SetStatusTasksInDB(Data_, Data_.Status);
                }
            }
            return NewIdMeasTask;
        }

        public bool Process(MeasTask measTask, List<int> SensorIds, string ActionType, bool isOnline, out bool isSuccess, out int? IdTask)
        {
            IdTask = null;
            SaveMeasTask saveMeasTask = new SaveMeasTask(_environment, _messagePublisher, _eventEmitter, _dataLayer, _logger);
            LoadSensor loadSensor = new LoadSensor(_environment, _messagePublisher, _eventEmitter, _dataLayer, _logger);
            int? IdTsk = null;
            isSuccess = false;

            if (measTask != null)
            {
                if (ActionType == "Del")
                {
                    isSuccess = saveMeasTask.SetStatusTasksInDB(measTask, "Z");
                }
                foreach (int SensorId in SensorIds)
                {
                    var fndSensor = loadSensor.LoadObjectSensor(SensorId);
                    if (fndSensor != null)
                    {
                        if (fndSensor.Name != null)
                        {
                            if (ActionType == "New")
                            {
                                measTask.CreateAllSubTasks();
                            }
                            if (measTask.Stations.ToList().FindAll(e => e.StationId.Value == SensorId) != null)
                            {
                                measTask.UpdateStatusSubTasks(SensorId, ActionType, isOnline);
                                if (ActionType == "New")
                                {
                                    IdTsk = CreateNewMeasTask(measTask, "New");
                                    if (IdTsk != null)
                                    {
                                        this._eventEmitter.Emit($"On{ActionType}CreateMeasTaskEvent", IdTsk.Value.ToString());
                                    }
                                }
                                else
                                {
                                    IdTsk = measTask.Id.Value;
                                    if (IdTsk != null)
                                    {
                                        this._eventEmitter.Emit($"On{ActionType}CreateMeasTaskEvent", IdTsk.Value.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                IdTask = IdTsk;
            }
            return isSuccess;
        }
    }
}


