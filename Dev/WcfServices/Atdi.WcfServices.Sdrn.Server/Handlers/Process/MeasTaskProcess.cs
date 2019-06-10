﻿using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Linq;
using Atdi.Modules.Sdrn.Server.Events;

namespace Atdi.WcfServices.Sdrn.Server
{
    public class MeasTaskProcess
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;


        public MeasTaskProcess(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }

        /// <summary>
        /// Create new task (Save in DB)
        /// </summary>
        /// <param name="measTask"></param>
        /// <param name="ActionType"></param>
        /// <returns></returns>
        public long? CreateNewMeasTask(MeasTask measTask, string ActionType)
        {
            long? NewIdMeasTask = -1;
            if (measTask != null)
            {
                var saveMeasTask = new SaveMeasTask(_dataLayer, _logger);
                var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
                var data = measTask;
                var listMeastTask = loadMeasTask.ShortReadTask(data.Id.Value);
                if (listMeastTask.Count == 0)
                {
                    data.UpdateStatus(ActionType);
                    NewIdMeasTask = saveMeasTask.SaveMeasTaskInDB(data);
                }
                else
                {
                    data = loadMeasTask.ShortReadTask(data.Id.Value)[0];
                    data.UpdateStatus(ActionType);
                    saveMeasTask.SetStatusTasksInDB(data, data.Status);
                }
            }
            return NewIdMeasTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measTask"></param>
        /// <param name="sensorIds"></param>
        /// <param name="actionType"></param>
        /// <param name="isOnline"></param>
        /// <param name="isSuccess"></param>
        /// <param name="idTask"></param>
        /// <returns></returns>
        public bool Process(MeasTask measTask, long[] sensorIds, string actionType, bool isOnline, out bool isSuccess, out long? idTask, bool isSendMessageToBus)
        {
            isSuccess = false;
            idTask = null;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, string.Format(Events.HandlerMeasTaskProcessStart.Text, actionType));
                var saveMeasTask = new SaveMeasTask(_dataLayer, _logger);
                var loadSensor = new LoadSensor(_dataLayer, _logger);
                long? IdTsk = null;
                if (measTask != null)
                {
                    for (int d = 0; d < sensorIds.Length; d++)
                    {
                        long SensorId = sensorIds[d];
                        var fndSensor = loadSensor.LoadObjectSensor(SensorId);
                        if (fndSensor != null)
                        {
                            if ((fndSensor.Name != null) && (fndSensor.Equipment != null))
                            {
                                if (actionType == MeasTaskMode.New.ToString())
                                {
                                    if ((measTask.MeasTimeParamList.PerStart > measTask.MeasTimeParamList.PerStop) || (measTask.MeasTimeParamList.TimeStart > measTask.MeasTimeParamList.TimeStop))
                                    {
                                        this._logger.Error(Contexts.ThisComponent, Categories.Processing, Events.MeasTimeParamListIncorrect.Text);
                                        throw new Exception(Events.MeasTimeParamListIncorrect.Text);
                                    }
                                    else
                                    {
                                        measTask.CreateAllSubTasks();
                                        if (measTask.RefSituation != null)
                                        {
                                            for (int p = 0; p < measTask.RefSituation.Length; p++)
                                            {
                                                var refSituation = measTask.RefSituation[p];
                                                if (refSituation != null)
                                                {
                                                    for (int z = 0; z < refSituation.ReferenceSignal.Length; z++)
                                                    {
                                                        if (refSituation.ReferenceSignal[z] != null)
                                                        {
                                                            MeasTaskExtend.SetDefaultSignalMask(ref refSituation.ReferenceSignal[z]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if ((measTask.Stations != null) && (measTask.Stations.ToList().FindAll(e => e.StationId.Value == SensorId) != null))
                                {
                                    measTask.UpdateStatusSubTasks(SensorId, actionType, isOnline);
                                    if ((actionType == MeasTaskMode.New.ToString()) && (IdTsk == null))
                                    {
                                        IdTsk = CreateNewMeasTask(measTask, MeasTaskMode.New.ToString());
                                    }
                                    else if ((actionType == MeasTaskMode.Stop.ToString()) && (measTask.Id!=null))
                                    {
                                        saveMeasTask.SetStatusTasksInDB(measTask, Status.F.ToString());
                                        IdTsk = measTask.Id.Value;
                                    }
                                    else if ((actionType == MeasTaskMode.Run.ToString()) && (measTask.Id != null))
                                    {
                                        saveMeasTask.SetStatusTasksInDB(measTask, Status.A.ToString());
                                        IdTsk = measTask.Id.Value;
                                    }
                                    else if ((actionType == MeasTaskMode.Del.ToString()) && (measTask.Id != null))
                                    {
                                        saveMeasTask.SetStatusTasksInDB(measTask, Status.Z.ToString());
                                        IdTsk = measTask.Id.Value;
                                    }

                                    if ((IdTsk != null) && (isSendMessageToBus))
                                    {
                                        var measTaskIds = "";
                                        if (measTask.MeasSubTasks != null)
                                        {
                                            for (int f = 0; f < measTask.MeasSubTasks.Length; f++)
                                            {
                                                var SubTask = measTask.MeasSubTasks[f];
                                                if (SubTask.MeasSubTaskStations != null)
                                                {
                                                    for (int g = 0; g < SubTask.MeasSubTaskStations.Length; g++)
                                                    {
                                                        var SubTaskStation = SubTask.MeasSubTaskStations[g];
                                                        measTaskIds = string.Format("{0}|{1}|{2}|{3}", IdTsk.Value, SubTask.Id.Value, SubTaskStation.Id, SubTaskStation.StationId.Value);
                                                        if (actionType != MeasTaskMode.New.ToString())
                                                        {
                                                            var masTaskEvent = new OnMeasTaskEvent()
                                                            {
                                                                SensorId = SensorId,
                                                                MeasTaskId = IdTsk.Value,
                                                                SensorName = fndSensor.Name,
                                                                EquipmentTechId = fndSensor.Equipment.TechId,
                                                                Name = $"On{actionType}MeasTaskEvent",
                                                                MeasTaskIds = measTaskIds
                                                            };
                                                            this._eventEmitter.Emit(masTaskEvent, new EventEmittingOptions()
                                                            {
                                                                Rule = EventEmittingRule.Default,
                                                                Destination = new string[] { $"SubscriberOn{actionType}MeasTaskEvent" }
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (actionType == MeasTaskMode.New.ToString())
                                        {
                                            var masTaskEvent = new OnMeasTaskEvent()
                                            {
                                                SensorId = SensorId,
                                                MeasTaskId = IdTsk.Value,
                                                SensorName = fndSensor.Name,
                                                EquipmentTechId = fndSensor.Equipment.TechId,
                                                Name = $"On{actionType}MeasTaskEvent",
                                                MeasTaskIds = measTaskIds
                                            };
                                            this._eventEmitter.Emit(masTaskEvent, new EventEmittingOptions()
                                            {
                                                Rule = EventEmittingRule.Default,
                                                Destination = new string[] { $"SubscriberOn{actionType}MeasTaskEvent" }
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    idTask = IdTsk;
                    if (idTask != null)
                    {
                        isSuccess = true;
                    }
                }
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, string.Format(Events.HandlerMeasTaskProcessEnd.Text, actionType));
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }


        public CommonOperationResult DeleteMeasTask(MeasTaskIdentifier taskId)
        {
            var result = new CommonOperationResult();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallDeleteMeasTaskMethod.Text);
                var loadTasks = new LoadMeasTask(_dataLayer, _logger);
                if (taskId != null)
                {
                    this._logger.Info(Contexts.ThisComponent, string.Format(Events.HandlerDeleteMeasTaskProcess.Text, taskId.Value));
                    var Res = loadTasks.ShortReadTask(taskId.Value);
                    MeasTask mt = null;
                    if (Res.Count > 0) mt = Res[0];
                    if (mt != null)
                    {
                        var SensorIds = new List<long>();
                        if (mt.MeasSubTasks != null)
                        {

                            for (int i = 0; i < mt.MeasSubTasks.Length; i++)
                            {
                                var item = mt.MeasSubTasks[i];

                                for (int j = 0; j < item.MeasSubTaskStations.Length; j++)
                                {
                                    var u = item.MeasSubTaskStations[j];

                                    if (!SensorIds.Contains(u.StationId.Value))
                                    {
                                        SensorIds.Add(u.StationId.Value);
                                    }
                                }
                            }
                        }

                        if (mt.Stations != null)
                        {
                            for (int d = 0; d < mt.Stations.Length; d++)
                            {
                                var item = mt.Stations[d];

                                if (item.StationId.Value > 0)
                                {
                                    if (!SensorIds.Contains(item.StationId.Value))
                                    {
                                        SensorIds.Add(item.StationId.Value);
                                    }
                                }
                            }
                        }

                        long? id = null;
                        var measTaskedit = new MeasTask() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MaxTimeBs = mt.MaxTimeBs, MeasDtParam = mt.MeasDtParam, MeasFreqParam = mt.MeasFreqParam, MeasLocParams = mt.MeasLocParams, MeasOther = mt.MeasOther, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, OrderId = mt.OrderId, Prio = mt.Prio, ResultType = mt.ResultType, Stations = mt.Stations, Status = mt.Status, Task = mt.Task, Type = mt.Type };
                        bool isSuccessTemp = false;

                        var massSensor = SensorIds.ToArray();
                        if (massSensor.Length > 0)
                        {
                            Process(measTaskedit, massSensor, MeasTaskMode.Stop.ToString(), false, out isSuccessTemp, out id, false);
                            result.State = isSuccessTemp == true ? CommonOperationState.Success : CommonOperationState.Fault;
                        }
                        else
                        {
                            result.State = CommonOperationState.Fault;
                        }
                        Process(measTaskedit, massSensor, MeasTaskMode.Del.ToString(), false, out isSuccessTemp, out id, true);
                        result.State = isSuccessTemp == true ? CommonOperationState.Success : CommonOperationState.Fault;


                        var saveResDb = new SaveResults(_dataLayer, _logger);
                        var valDelRes = saveResDb.DeleteResultFromDB(new MeasurementResultsIdentifier() { MeasTaskId = new MeasTaskIdentifier() { Value = taskId.Value } }, Status.Z.ToString());  
                        if (valDelRes.State== CommonOperationState.Fault)
                        {
                            result.State = CommonOperationState.Fault;
                        }
                    }
                    else result.State = CommonOperationState.Fault;
                }
                else
                {
                    result.State = CommonOperationState.Fault;
                }

            }
            catch (Exception e)
            {
                result.State = CommonOperationState.Fault;
                result.FaultCause = e.Message;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return result;
        }

        public CommonOperationResult RunMeasTask(MeasTaskIdentifier taskId)
        {
            var result = new CommonOperationResult();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallRunMeasTaskMethod.Text);
                var loadTasks = new LoadMeasTask(_dataLayer, _logger);
                if (taskId != null)
                {
                    this._logger.Info(Contexts.ThisComponent, string.Format(Events.HandlerRunMeasTaskProcess.Text, taskId.Value));
                    var Res = loadTasks.ShortReadTask(taskId.Value);
                    MeasTask mt = null;
                    if (Res.Count > 0) mt = Res[0];
                    if (mt != null)
                    {
                        var SensorIds = new List<long>();
                        if (mt.MeasSubTasks != null)
                        {
                            for (int d = 0; d < mt.MeasSubTasks.Length; d++)
                            {
                                var item = mt.MeasSubTasks[d];
                                for (int r = 0; r < item.MeasSubTaskStations.Length; r++)
                                {
                                    var measSubTaskStations = item.MeasSubTaskStations[r];

                                    if (!SensorIds.Contains(measSubTaskStations.StationId.Value))
                                    {
                                        SensorIds.Add(measSubTaskStations.StationId.Value);
                                    }
                                }
                            }
                        }

                        if (mt.Stations != null)
                        {
                            for (int d = 0; d < mt.Stations.Length; d++)
                            {
                                var item = mt.Stations[d];

                                if (item.StationId.Value > 0)
                                {
                                    if (!SensorIds.Contains(item.StationId.Value))
                                    {
                                        SensorIds.Add(item.StationId.Value);
                                    }
                                }
                            }
                        }

                        var measTaskedit = new MeasTask() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MaxTimeBs = mt.MaxTimeBs, MeasDtParam = mt.MeasDtParam, MeasFreqParam = mt.MeasFreqParam, MeasLocParams = mt.MeasLocParams, MeasOther = mt.MeasOther, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, OrderId = mt.OrderId, Prio = mt.Prio, ResultType = mt.ResultType, Stations = mt.Stations, Status = mt.Status, Task = mt.Task, Type = mt.Type };
                        var massSensor = SensorIds.ToArray();
                        if (massSensor.Length > 0)
                        {
                            bool isSuccess = false;
                            long? id = null;
                            Process(measTaskedit, massSensor, MeasTaskMode.Run.ToString(), false, out isSuccess, out id, true);
                            result.State = CommonOperationState.Success;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.State = CommonOperationState.Fault;
                result.FaultCause = e.Message;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return result;
        }

        public CommonOperationResult StopMeasTask(MeasTaskIdentifier taskId)
        {
            var result = new CommonOperationResult();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallStopMeasTaskMethod.Text);
                var loadTasks = new LoadMeasTask(_dataLayer, _logger);
                if (taskId != null)
                {
                    this._logger.Info(Contexts.ThisComponent, string.Format(Events.HandlerStopMeasTaskProcess.Text, taskId.Value));
                    var Res = loadTasks.ShortReadTask(taskId.Value);
                    MeasTask mt = null;
                    if (Res.Count > 0) mt = Res[0];
                    if (mt != null)
                    {
                        var SensorIds = new List<long>();
                        if (mt.MeasSubTasks != null)
                        {
                            for (int d = 0; d < mt.MeasSubTasks.Length; d++)
                            {
                                var item = mt.MeasSubTasks[d];
                                for (int r = 0; r < item.MeasSubTaskStations.Length; r++)
                                {
                                    var measSubTaskStations = item.MeasSubTaskStations[r];

                                    if (!SensorIds.Contains(measSubTaskStations.StationId.Value))
                                    {
                                        SensorIds.Add(measSubTaskStations.StationId.Value);
                                    }
                                }
                            }
                        }

                        if (mt.Stations != null)
                        {
                            for (int d = 0; d < mt.Stations.Length; d++)
                            {
                                var item = mt.Stations[d];

                                if (item.StationId.Value > 0)
                                {
                                    if (!SensorIds.Contains(item.StationId.Value))
                                    {
                                        SensorIds.Add(item.StationId.Value);
                                    }
                                }
                            }
                        }

                        var measTaskedit = new MeasTask() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MaxTimeBs = mt.MaxTimeBs, MeasDtParam = mt.MeasDtParam, MeasFreqParam = mt.MeasFreqParam, MeasLocParams = mt.MeasLocParams, MeasOther = mt.MeasOther, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, OrderId = mt.OrderId, Prio = mt.Prio, ResultType = mt.ResultType, Stations = mt.Stations, Status = mt.Status, Task = mt.Task, Type = mt.Type };
                        var massSensor = SensorIds.ToArray();
                        if (massSensor.Length > 0)
                        {
                            bool isSuccess = false;
                            long? id = null;
                            Process(measTaskedit, massSensor, MeasTaskMode.Stop.ToString(), false, out isSuccess, out id, true);
                            result.State = CommonOperationState.Success;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.State = CommonOperationState.Fault;
                result.FaultCause = e.Message;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return result;
        }

    }
}


