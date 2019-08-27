﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.EventSystem;
using Atdi.Modules.Sdrn.Server.Events;
using Atdi.AppUnits.Sdrn.MasterServer.LoadData;



namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.PipelineHandlers
{
    /// <summary>
    /// Класс, выполняющий группровку сенсоров по списку наименований AggregationServer'ов
    /// </summary>
    public class GenerateMeasTasksPipelineHandler
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public GenerateMeasTasksPipelineHandler(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }


        public void Handle(ref ClientMeasTaskPipebox data)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.GenerateMeasTasksPipelineHandler, this))
            {
                var allListMeasSubTaskSensors = new List<MeasSubTaskSensor>();
                var listMeasTasks = new List<MeasTask>();
                var AggregationServerList = new List<string>();
                var dictionaryAggregationWithSensor = new Dictionary<long, string>();
                var aggregationServerList = new List<string>();
                var listSensorsWithoutLinkAggrServer = new List<long>();
                var sensor = new LoadSensor(this._dataLayer, this._logger);
                var AllMasterSensors = new List<MeasSensor>();
                var measSubTasks = data.MeasTaskPipeBox.MeasSubTasks.ToList();
                var prepareSendEvents = data.PrepareSendEvents;
                if ((measSubTasks != null) && (measSubTasks.Count > 0))
                {
                    for (int i = 0; i < measSubTasks.Count; i++)
                    {
                        var measSubTask = measSubTasks[i];
                        if ((measSubTask.MeasSubTaskSensors != null) && (measSubTask.MeasSubTaskSensors.Length > 0))
                        {
                            for (int j = 0; j < measSubTask.MeasSubTaskSensors.Length; j++)
                            {
                                if (sensor.GetAggregationServerBySensorId(measSubTask.MeasSubTaskSensors[j].SensorId, out string Name, out string TechId, out string AggregationServerId))
                                {
                                    AllMasterSensors.Add(new MeasSensor() {
                                         SensorId = new MeasSensorIdentifier()
                                         {
                                              Value = measSubTask.MeasSubTaskSensors[j].SensorId
                                         },
                                          SensorName = Name,
                                          SensorTechId = TechId
                                    });
                                    if (!AggregationServerList.Contains(AggregationServerId))
                                    {
                                        if (!string.IsNullOrEmpty(AggregationServerId))
                                        {
                                            AggregationServerList.Add(AggregationServerId);
                                        }
                                    }
                                    if (!dictionaryAggregationWithSensor.ContainsKey(measSubTask.MeasSubTaskSensors[j].SensorId))
                                    {
                                        dictionaryAggregationWithSensor.Add(measSubTask.MeasSubTaskSensors[j].SensorId, AggregationServerId);
                                    }
                                }
                                else
                                {
                                    listSensorsWithoutLinkAggrServer.Add(measSubTask.MeasSubTaskSensors[j].SensorId);
                                }
                            }
                        }
                    }


                    if ((AggregationServerList != null) && (AggregationServerList.Count > 0))
                    {
                        for (int i = 0; i < AggregationServerList.Count; i++)
                        {
                            var sensorsListWithAggregationServer = dictionaryAggregationWithSensor.ToList();
                            var nextMeasTask = data.MeasTaskPipeBox;
                            var sensorsWithAggregationServer = sensorsListWithAggregationServer.FindAll(c => c.Value == AggregationServerList[i]);
                            if ((sensorsWithAggregationServer != null) && (sensorsWithAggregationServer.Count > 0))
                            {
                                var listMeasSubTask = new List<MeasSubTask>();
                                var listMeasSensors = new List<MeasSensor>();

                                for (int l = 0; l < measSubTasks.Count; l++)
                                {
                                    var measSubTask = measSubTasks[l];
                                    var listMeasSubTaskSensors = new List<MeasSubTaskSensor>();
                                    if ((measSubTask.MeasSubTaskSensors != null) && (measSubTask.MeasSubTaskSensors.Length > 0))
                                    {
                                        for (int m = 0; m < measSubTask.MeasSubTaskSensors.Length; m++)
                                        {
                                            for (int j = 0; j < sensorsWithAggregationServer.Count; j++)
                                            {
                                                if (sensorsWithAggregationServer[j].Key == measSubTask.MeasSubTaskSensors[m].SensorId)
                                                {
                                                    listMeasSubTaskSensors.Add(
                                                        new MeasSubTaskSensor()
                                                        {
                                                            Count = measSubTask.MeasSubTaskSensors[m].Count,
                                                            Id = measSubTask.MeasSubTaskSensors[m].Id,
                                                            SensorId = measSubTask.MeasSubTaskSensors[m].SensorId,
                                                            Status = measSubTask.MeasSubTaskSensors[m].Status,
                                                            TimeNextTask = measSubTask.MeasSubTaskSensors[m].TimeNextTask,
                                                            MasterId = measSubTask.MeasSubTaskSensors[m].Id
                                                        }
                                                        );
                                                    if (listMeasSensors.Find(x => x.SensorId.Value == measSubTask.MeasSubTaskSensors[m].SensorId) == null)
                                                    {
                                                        var findSensor = AllMasterSensors.Find(x => x.SensorId.Value == measSubTask.MeasSubTaskSensors[m].SensorId);
                                                        if (findSensor != null)
                                                        {
                                                            listMeasSensors.Add(new MeasSensor() { SensorId = new MeasSensorIdentifier() { Value = measSubTask.MeasSubTaskSensors[m].SensorId }, SensorName = findSensor.SensorName, SensorTechId = findSensor.SensorTechId });
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    if (listMeasSubTaskSensors.Count > 0)
                                    {
                                        nextMeasTask = new MeasTask()
                                        {
                                            CreatedBy = data.MeasTaskPipeBox.CreatedBy,
                                            DateCreated = data.MeasTaskPipeBox.DateCreated,
                                            ExecutionMode = data.MeasTaskPipeBox.ExecutionMode,
                                            Id = data.MeasTaskPipeBox.Id,
                                            MeasTimeParamList = data.MeasTaskPipeBox.MeasTimeParamList,
                                            Name = data.MeasTaskPipeBox.Name,
                                            Prio = data.MeasTaskPipeBox.Prio,
                                            Status = data.MeasTaskPipeBox.Status,
                                            TypeMeasurements = data.MeasTaskPipeBox.TypeMeasurements
                                        };


                                        listMeasSubTask.Add(new MeasSubTask()
                                        {
                                            Id = new MeasTaskIdentifier()
                                            {
                                                Value = measSubTask.Id.Value,
                                            },
                                            Interval = measSubTask.Interval,
                                            MeasSubTaskSensors = listMeasSubTaskSensors.ToArray(),
                                            Status = measSubTask.Status,
                                            TimeStart = measSubTask.TimeStart,
                                            TimeStop = measSubTask.TimeStop
                                        });

                                        allListMeasSubTaskSensors.AddRange(listMeasSubTaskSensors);
                                    }
                                }

                                nextMeasTask.MeasSubTasks = listMeasSubTask.ToArray();
                                nextMeasTask.Sensors = listMeasSensors.ToArray();
                                listMeasTasks.Add(nextMeasTask);
                                aggregationServerList.Add(AggregationServerList[i]);

                                if (listMeasSensors != null)
                                {
                                    for (int v = 0; v < listMeasSensors.Count; v++)
                                    {
                                        for (int k = 0; k < prepareSendEvents.Length; k++)
                                        {
                                            var subTaskSensor = allListMeasSubTaskSensors.Find(x => x.SensorId == listMeasSensors[v].SensorId.Value);
                                            var getDictionaryAggregationWithSensor = dictionaryAggregationWithSensor.ToList();
                                            var nameAggregationServerInstance = getDictionaryAggregationWithSensor.Find(x => x.Key == listMeasSensors[v].SensorId.Value);
                                            if ((!string.IsNullOrEmpty(nameAggregationServerInstance.Value)) && (subTaskSensor != null))
                                            {
                                                if (prepareSendEvents[k].SensorId == listMeasSensors[v].SensorId.Value)
                                                {
                                                    var measTaskIds = string.Format("{0}_SDRN.SubTaskSensorId.{1}_", prepareSendEvents[k].MeasurementType.ToString(), subTaskSensor.Id);
                                                    var measTaskEventToAggregationServer = new OnMeasTaskToMasterServerEvent()
                                                    {
                                                        SensorId = prepareSendEvents[k].SensorId,
                                                        MeasTaskId = prepareSendEvents[k].MeasTaskId,
                                                        SensorName = prepareSendEvents[k].SensorName,
                                                        EquipmentTechId = prepareSendEvents[k].EquipmentTechId,
                                                        Name = $"On{prepareSendEvents[k].ActionType}MeasTaskCreateToMasterServerEvent",
                                                        MeasTaskIds = measTaskIds,
                                                        SubTaskSensorId = prepareSendEvents[k].SubTaskSensorId,
                                                        AggregationInstance = nameAggregationServerInstance.Value
                                                    };
                                                    this._eventEmitter.Emit(measTaskEventToAggregationServer, new EventEmittingOptions()
                                                    {
                                                        Rule = EventEmittingRule.Default,
                                                        Destination = new string[] { $"SubscriberOn{prepareSendEvents[k].ActionType}MeasTaskCreateToMasterServerEvent" }
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        data.MeasTasksWithAggregationServerPipeBox = listMeasTasks.ToArray();
                        data.AggregationServerInstancesPipeBox = aggregationServerList.ToArray();
                    }
                    if ((listSensorsWithoutLinkAggrServer != null) && (listSensorsWithoutLinkAggrServer.Count > 0))
                    {
                        for (int i = 0; i < listSensorsWithoutLinkAggrServer.Count; i++)
                        {
                            for (int k = 0; k < prepareSendEvents.Length; k++)
                            {
                                if (prepareSendEvents[k].SensorId == listSensorsWithoutLinkAggrServer[i])
                                {
                                    var measTaskEvent = new OnMeasTaskEvent()
                                    {
                                        SensorId = prepareSendEvents[k].SensorId,
                                        MeasTaskId = prepareSendEvents[k].MeasTaskId,
                                        SensorName = prepareSendEvents[k].SensorName,
                                        EquipmentTechId = prepareSendEvents[k].EquipmentTechId,
                                        Name = $"On{prepareSendEvents[k].ActionType}MeasTaskEvent",
                                        MeasTaskIds = prepareSendEvents[k].MeasTaskIds,
                                        SubTaskSensorId = prepareSendEvents[k].SubTaskSensorId
                                    };
                                    this._eventEmitter.Emit(measTaskEvent, new EventEmittingOptions()
                                    {
                                        Rule = EventEmittingRule.Default,
                                        Destination = new string[] { $"SubscriberOn{prepareSendEvents[k].ActionType}MeasTaskEvent" }
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
