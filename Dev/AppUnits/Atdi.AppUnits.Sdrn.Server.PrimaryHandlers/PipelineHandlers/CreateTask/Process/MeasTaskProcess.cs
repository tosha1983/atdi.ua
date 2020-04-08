using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Linq;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Contracts.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;




namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    /// <summary>
    /// Менеджер процесса создания таска
    /// </summary>
    public class MeasTaskProcess
    {
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;


        public MeasTaskProcess(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
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
                var meastTask = loadMeasTask.ReadTask(data.Id.Value);
                if (meastTask == null)
                {
                    data.UpdateStatus(ActionType);
                    NewIdMeasTask = saveMeasTask.SaveMeasTaskInDB(data);
                }
                else
                {
                    if (meastTask.Id != null)
                    {
                        data = meastTask;
                        NewIdMeasTask = data.Id.Value;
                        data.UpdateStatus(ActionType);
                        saveMeasTask.SetStatusTasksInDB(data, data.Status);
                    }
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
        public bool Process(MeasTask measTask, long[] sensorIds, string actionType, bool isOnline, out bool isSuccess, out long? idTask, bool isSendMessageToBus, out PrepareSendEvent[] prepareSendEvents)
        {
            MeasTask measTaskTemp = null;
            var listPrepareSendEvents = new List<PrepareSendEvent>();
            prepareSendEvents = null;
            isSuccess = false;
            bool isGeneratedListEvent = false;
            idTask = null;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.MessageProcessing, string.Format(Events.HandlerMeasTaskProcessStart.Text, actionType));
                var saveMeasTask = new SaveMeasTask(_dataLayer, _logger);
                var loadSensor = new LoadSensor(_dataLayer, _logger);
                var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
                long? IdTsk = null;
                if (measTask != null)
                {
                    for (int d = 0; d < sensorIds.Length; d++)
                    {
                        long sensorId = sensorIds[d];
                        var fndSensor = loadSensor.LoadSensorId(sensorId, out string sensorName, out string sensorTechId);
                        if (fndSensor != null)
                        {
                            if ((sensorName != null) && (sensorTechId != null))
                            {
                                if (actionType == MeasTaskMode.New.ToString())
                                {
                                    if ((measTask.MeasTimeParamList.PerStart > measTask.MeasTimeParamList.PerStop) || (measTask.MeasTimeParamList.TimeStart > measTask.MeasTimeParamList.TimeStop))
                                    {
                                        this._logger.Error(Contexts.ThisComponent, Categories.MessageProcessing, Events.MeasTimeParamListIncorrect.Text);
                                        throw new Exception(Events.MeasTimeParamListIncorrect.Text);
                                    }
                                    else
                                    {
                                        if (measTask.MeasSubTasks != null)
                                        {
                                            for (int z=0; z< measTask.MeasSubTasks.Length; z++)
                                            {
                                                if (measTask.MeasSubTasks[z]!=null)
                                                {
                                                    if (measTask.MeasSubTasks[z].Id != null)
                                                    {
                                                        measTask.MeasSubTasks[z].Id = new MeasTaskIdentifier();
                                                    }
                                                }
                                            }
                                            measTask.Id = new MeasTaskIdentifier();
                                        }
                                        if (measTask.MeasSubTasks == null)
                                        {
                                            measTask.CreateAllSubTasks();
                                        }
                                        if (measTaskTemp != null)
                                        {
                                            measTask.Id = measTaskTemp.Id;
                                            measTask.MeasSubTasks = measTaskTemp.MeasSubTasks;
                                        }
                                        if (measTask is MeasTaskSignaling)
                                        {
                                            var measTaskSignaling = measTask as MeasTaskSignaling;
                                            if (measTaskSignaling.RefSituation != null)
                                            {
                                                for (int p = 0; p < measTaskSignaling.RefSituation.Length; p++)
                                                {
                                                    var refSituation = measTaskSignaling.RefSituation[p];
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
                                            measTask = measTaskSignaling;
                                        }
                                    }
                                }
                                if ((measTask.Sensors != null) && (measTask.Sensors.ToList().FindAll(e => e.SensorId.Value == sensorId) != null))
                                {
                                    measTask.UpdateStatusSubTasks(sensorId, actionType, isOnline);
                                    if ((actionType == MeasTaskMode.New.ToString()) && (IdTsk == null))
                                    {
                                        IdTsk = CreateNewMeasTask(measTask, MeasTaskMode.New.ToString());
                                        switch (measTask.TypeMeasurements)
                                        {
                                            case MeasurementType.AmplModulation:
                                            case MeasurementType.Bearing:
                                            case MeasurementType.FreqModulation:
                                            case MeasurementType.Frequency:
                                            case MeasurementType.Location:
                                            case MeasurementType.Offset:
                                            case MeasurementType.PICode:
                                            case MeasurementType.Program:
                                            case MeasurementType.SoundID:
                                            case MeasurementType.SubAudioTone:
                                                throw new NotImplementedException($"Not supported type {measTask.TypeMeasurements}");
                                            case MeasurementType.BandwidthMeas:
                                                if (measTask is MeasTaskBandWidth)
                                                {
                                                    var measBandWidth = (measTask as MeasTaskBandWidth);
                                                    measTaskTemp = new MeasTaskBandWidth() { CreatedBy = measTask.CreatedBy, DateCreated = measTask.DateCreated, ExecutionMode = measTask.ExecutionMode, Id = measTask.Id, MeasDtParam = measBandWidth.MeasDtParam, MeasFreqParam = measBandWidth.MeasFreqParam, MeasSubTasks = measTask.MeasSubTasks, MeasTimeParamList = measTask.MeasTimeParamList, Name = measTask.Name, Prio = measTask.Prio, Sensors = measTask.Sensors, Status = measTask.Status, TypeMeasurements = measTask.TypeMeasurements };
                                                }
                                                break;
                                            case MeasurementType.Level:
                                                if (measTask is MeasTaskLevel)
                                                {
                                                    var measLevel = (measTask as MeasTaskLevel);
                                                    measTaskTemp = new MeasTaskLevel() { CreatedBy = measTask.CreatedBy, DateCreated = measTask.DateCreated, ExecutionMode = measTask.ExecutionMode, Id = measTask.Id, MeasDtParam = measLevel.MeasDtParam, MeasFreqParam = measLevel.MeasFreqParam, MeasSubTasks = measTask.MeasSubTasks, MeasTimeParamList = measTask.MeasTimeParamList, Name = measTask.Name, Prio = measTask.Prio, Sensors = measTask.Sensors, Status = measTask.Status, TypeMeasurements = measTask.TypeMeasurements };
                                                }
                                                break;
                                            case MeasurementType.MonitoringStations:
                                                if (measTask is MeasTaskMonitoringStations)
                                                {
                                                    var measMonitoringStations = (measTask as MeasTaskMonitoringStations);
                                                    measTaskTemp = new MeasTaskMonitoringStations() { CreatedBy = measTask.CreatedBy, DateCreated = measTask.DateCreated, ExecutionMode = measTask.ExecutionMode, Id = measTask.Id, StationsForMeasurements = measMonitoringStations.StationsForMeasurements, MeasSubTasks = measTask.MeasSubTasks, MeasTimeParamList = measTask.MeasTimeParamList, Name = measTask.Name, Prio = measTask.Prio, Sensors = measTask.Sensors, Status = measTask.Status, TypeMeasurements = measTask.TypeMeasurements };
                                                }
                                                break;
                                            case MeasurementType.Signaling:
                                                if (measTask is MeasTaskSignaling)
                                                {
                                                    var measTaskSignaling = (measTask as MeasTaskSignaling);
                                                    measTaskTemp = new MeasTaskSignaling() { CreatedBy = measTask.CreatedBy, DateCreated = measTask.DateCreated, ExecutionMode = measTask.ExecutionMode, Id = measTask.Id, MeasDtParam = measTaskSignaling.MeasDtParam, MeasFreqParam = measTaskSignaling.MeasFreqParam, MeasSubTasks = measTask.MeasSubTasks, MeasTimeParamList = measTask.MeasTimeParamList, Name = measTask.Name, Prio = measTask.Prio, Sensors = measTask.Sensors, Status = measTask.Status, TypeMeasurements = measTask.TypeMeasurements, RefSituation = measTaskSignaling.RefSituation, SignalingMeasTaskParameters = measTaskSignaling.SignalingMeasTaskParameters };
                                                }
                                                break;
                                            case MeasurementType.SpectrumOccupation:
                                                if (measTask is MeasTaskSpectrumOccupation)
                                                {
                                                    var measTaskSpectrumOccupation = (measTask as MeasTaskSpectrumOccupation);
                                                    measTaskTemp = new MeasTaskSpectrumOccupation() { CreatedBy = measTask.CreatedBy, DateCreated = measTask.DateCreated, ExecutionMode = measTask.ExecutionMode, Id = measTask.Id, MeasDtParam = measTaskSpectrumOccupation.MeasDtParam, MeasFreqParam = measTaskSpectrumOccupation.MeasFreqParam, MeasSubTasks = measTask.MeasSubTasks, MeasTimeParamList = measTask.MeasTimeParamList, Name = measTask.Name, Prio = measTask.Prio, Sensors = measTask.Sensors, Status = measTask.Status, SpectrumOccupationParameters = measTaskSpectrumOccupation.SpectrumOccupationParameters, TypeMeasurements = measTaskSpectrumOccupation.TypeMeasurements };
                                                }
                                                break;
                                        }
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
                                    else if ((actionType == MeasTaskMode.Update.ToString()) && (measTask.Id != null))
                                    {
                                        saveMeasTask.UpdateMeasTaskParametersAndRecalcResults(measTask);
                                        IdTsk = measTask.Id.Value;
                                    }

                                    if ((IdTsk != null) && (isSendMessageToBus))
                                    {
                                        string measTaskIds = "";
                                        long subTaskSensor = -1;
                                        if ((measTask.MeasSubTasks != null) && (isGeneratedListEvent == false))
                                        {
                                            for (int f = 0; f < measTask.MeasSubTasks.Length; f++)
                                            {
                                                var SubTask = measTask.MeasSubTasks[f];
                                                if (SubTask.MeasSubTaskSensors != null)
                                                {
                                                    for (int g = 0; g < SubTask.MeasSubTaskSensors.Length; g++)
                                                    {
                                                        var SubTaskSensor = SubTask.MeasSubTaskSensors[g];
                                                        measTaskIds = string.Format("{0}_SDRN.SubTaskSensorId.{1}_", measTask.TypeMeasurements.ToString(), SubTaskSensor.Id);
                                                        subTaskSensor = SubTaskSensor.Id;
                                                        if (actionType != MeasTaskMode.New.ToString())
                                                        {
                                                            var getSensor = loadSensor.LoadSensorId(SubTaskSensor.SensorId, out string sensorNameTemp, out string sensorTechIdTemp);
                                                            if (getSensor != null)
                                                            {
                                                                var prepareSendEvent = new PrepareSendEvent()
                                                                {
                                                                    ActionType = actionType,
                                                                    EquipmentTechId = sensorTechIdTemp,
                                                                    MeasTaskId = IdTsk.Value,
                                                                    SensorId = getSensor.Value,
                                                                    SensorName = sensorNameTemp,
                                                                    MeasurementType = measTask.TypeMeasurements,
                                                                    MeasTaskIds = measTaskIds,
                                                                    SubTaskSensorId = subTaskSensor
                                                                };
                                                                listPrepareSendEvents.Add(prepareSendEvent);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            isGeneratedListEvent = true;
                                        }

                                        if (actionType == MeasTaskMode.New.ToString())
                                        {
                                            var prepareSendEvent = new PrepareSendEvent()
                                            {
                                                ActionType = actionType,
                                                EquipmentTechId = sensorTechId,
                                                MeasTaskId = IdTsk.Value,
                                                SensorId = sensorId,
                                                SensorName = sensorName,
                                                MeasurementType = measTask.TypeMeasurements,
                                                MeasTaskIds = measTaskIds, 
                                                SubTaskSensorId = subTaskSensor
                                            };
                                            listPrepareSendEvents.Add(prepareSendEvent);
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
                this._logger.Info(Contexts.ThisComponent, Categories.MessageProcessing, string.Format(Events.HandlerMeasTaskProcessEnd.Text, actionType));
            }
            catch (Exception e)
            {
                idTask = null;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            prepareSendEvents = listPrepareSendEvents.ToArray();
            return isSuccess;
        }


        public CommonOperation DeleteMeasTask(ref MeasTask task, out PrepareSendEvent[] prepareSendEventArr)
        {
            prepareSendEventArr = null;
            var prepareSendEvents = new List<PrepareSendEvent>();
            var result = new CommonOperation();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.MessageProcessing, Events.HandlerCallDeleteMeasTaskMethod.Text);
                var loadTasks = new LoadMeasTask(_dataLayer, _logger);
                if (task.Id != null)
                {
                    this._logger.Info(Contexts.ThisComponent, string.Format(Events.HandlerDeleteMeasTaskProcess.Text, task.Id.Value));
                    var Res = loadTasks.ReadTask(task.Id.Value);
                    MeasTask mt = null;
                    if (Res != null) mt = Res;
                    if (mt != null)
                    {
                        var SensorIds = new List<long>();
                        if (mt.MeasSubTasks != null)
                        {

                            for (int i = 0; i < mt.MeasSubTasks.Length; i++)
                            {
                                var item = mt.MeasSubTasks[i];

                                for (int j = 0; j < item.MeasSubTaskSensors.Length; j++)
                                {
                                    var u = item.MeasSubTaskSensors[j];

                                    if (!SensorIds.Contains(u.SensorId))
                                    {
                                        SensorIds.Add(u.SensorId);
                                    }
                                }
                            }
                        }

                        if (mt.Sensors != null)
                        {
                            for (int d = 0; d < mt.Sensors.Length; d++)
                            {
                                var item = mt.Sensors[d];

                                if (item.SensorId.Value > 0)
                                {
                                    if (!SensorIds.Contains(item.SensorId.Value))
                                    {
                                        SensorIds.Add(item.SensorId.Value);
                                    }
                                }
                            }
                        }

                        long? id = null;
                        var measTaskedit = new MeasTask();

                        switch (mt.TypeMeasurements)
                        {
                            case MeasurementType.AmplModulation:
                            case MeasurementType.Bearing:
                            case MeasurementType.FreqModulation:
                            case MeasurementType.Frequency:
                            case MeasurementType.Location:
                            case MeasurementType.Offset:
                            case MeasurementType.PICode:
                            case MeasurementType.Program:
                            case MeasurementType.SoundID:
                            case MeasurementType.SubAudioTone:
                                throw new NotImplementedException($"Not supported type {mt.TypeMeasurements}");
                            case MeasurementType.BandwidthMeas:
                                if (mt is MeasTaskBandWidth)
                                {
                                    var measBandWidth = (mt as MeasTaskBandWidth);
                                    measTaskedit = new MeasTaskBandWidth() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measBandWidth.MeasDtParam, MeasFreqParam = measBandWidth.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio,  Sensors = mt.Sensors, Status = mt.Status,  TypeMeasurements = mt.TypeMeasurements };
                                }
                                break;
                            case MeasurementType.Level:
                                if (mt is MeasTaskLevel)
                                {
                                    var measLevel = (mt as MeasTaskLevel);
                                    measTaskedit = new MeasTaskLevel() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measLevel.MeasDtParam, MeasFreqParam = measLevel.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio, Sensors = mt.Sensors, Status = mt.Status, TypeMeasurements = mt.TypeMeasurements };
                                }
                                break;
                            case MeasurementType.MonitoringStations:
                                if (mt is MeasTaskMonitoringStations)
                                {
                                    var measMonitoringStations = (mt as MeasTaskMonitoringStations);
                                    measTaskedit = new MeasTaskMonitoringStations() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, StationsForMeasurements = measMonitoringStations.StationsForMeasurements, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio, Sensors = mt.Sensors, Status = mt.Status, TypeMeasurements = mt.TypeMeasurements };
                                }
                                break;
                            case MeasurementType.Signaling:
                                if (mt is MeasTaskSignaling)
                                {
                                    var measTaskSignaling = (mt as MeasTaskSignaling);
                                    measTaskedit = new MeasTaskSignaling() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measTaskSignaling.MeasDtParam, MeasFreqParam = measTaskSignaling.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio,  Sensors = mt.Sensors, Status = mt.Status,  TypeMeasurements = mt.TypeMeasurements, RefSituation = measTaskSignaling.RefSituation, SignalingMeasTaskParameters = measTaskSignaling.SignalingMeasTaskParameters };
                                }
                                break;
                            case MeasurementType.SpectrumOccupation:
                                if (mt is MeasTaskSpectrumOccupation)
                                {
                                    var measTaskSpectrumOccupation = (mt as MeasTaskSpectrumOccupation);
                                    measTaskedit = new MeasTaskSpectrumOccupation() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measTaskSpectrumOccupation.MeasDtParam, MeasFreqParam = measTaskSpectrumOccupation.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio,  Sensors = mt.Sensors, Status = mt.Status,  SpectrumOccupationParameters = measTaskSpectrumOccupation.SpectrumOccupationParameters, TypeMeasurements = measTaskSpectrumOccupation.TypeMeasurements };
                                }
                                break;
                        }

                        bool isSuccessTemp = false;
                        PrepareSendEvent[] prepareSendEventsStop = null;
                        PrepareSendEvent[] prepareSendEventsDelete = null;

                        var massSensor = SensorIds.ToArray();
                        if (massSensor.Length > 0)
                        {
                            Process(measTaskedit, massSensor, MeasTaskMode.Stop.ToString(), false, out isSuccessTemp, out id, false, out prepareSendEventsStop);
                            result.State = isSuccessTemp == true ? CommonOperationState.Success : CommonOperationState.Fault;
                        }
                        else
                        {
                            result.State = CommonOperationState.Fault;
                        }
                        Process(measTaskedit, massSensor, MeasTaskMode.Del.ToString(), false, out isSuccessTemp, out id, true, out prepareSendEventsDelete);
                        result.State = isSuccessTemp == true ? CommonOperationState.Success : CommonOperationState.Fault;


                        var saveResDb = new SaveResults(_dataLayer, _logger);
                        var valDelRes = saveResDb.DeleteResultFromDB(task.Id.Value, Status.Z.ToString());  
                        if (valDelRes.State== CommonOperationState.Fault)
                        {
                            result.State = CommonOperationState.Fault;
                        }
                        task = measTaskedit;

                        prepareSendEvents.AddRange(prepareSendEventsStop);
                        prepareSendEvents.AddRange(prepareSendEventsDelete);
                        prepareSendEventArr = prepareSendEvents.ToArray();
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

        public CommonOperation RunMeasTask(ref MeasTask task, out PrepareSendEvent[] prepareSendEventArr)
        {
            prepareSendEventArr = null;
            var result = new CommonOperation();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.MessageProcessing, Events.HandlerCallRunMeasTaskMethod.Text);
                var loadTasks = new LoadMeasTask(_dataLayer, _logger);
                if (task.Id != null)
                {
                    this._logger.Info(Contexts.ThisComponent, string.Format(Events.HandlerRunMeasTaskProcess.Text, task.Id.Value));
                    var Res = loadTasks.ReadTask(task.Id.Value);
                    MeasTask mt = null;
                    if (Res!=null) mt = Res;
                    if (mt != null)
                    {
                        var SensorIds = new List<long>();
                        if (mt.MeasSubTasks != null)
                        {
                            for (int d = 0; d < mt.MeasSubTasks.Length; d++)
                            {
                                var item = mt.MeasSubTasks[d];
                                for (int r = 0; r < item.MeasSubTaskSensors.Length; r++)
                                {
                                    var measSubTaskSensor= item.MeasSubTaskSensors[r];

                                    if (!SensorIds.Contains(measSubTaskSensor.SensorId))
                                    {
                                        SensorIds.Add(measSubTaskSensor.SensorId);
                                    }
                                }
                            }
                        }

                        if (mt.Sensors != null)
                        {
                            for (int d = 0; d < mt.Sensors.Length; d++)
                            {
                                var item = mt.Sensors[d];

                                if (item.SensorId.Value > 0)
                                {
                                    if (!SensorIds.Contains(item.SensorId.Value))
                                    {
                                        SensorIds.Add(item.SensorId.Value);
                                    }
                                }
                            }
                        }

                        var measTaskedit = new MeasTask();

                        switch (mt.TypeMeasurements)
                        {
                            case MeasurementType.AmplModulation:
                            case MeasurementType.Bearing:
                            case MeasurementType.FreqModulation:
                            case MeasurementType.Frequency:
                            case MeasurementType.Location:
                            case MeasurementType.Offset:
                            case MeasurementType.PICode:
                            case MeasurementType.Program:
                            case MeasurementType.SoundID:
                            case MeasurementType.SubAudioTone:
                                throw new NotImplementedException($"Not supported type {mt.TypeMeasurements}");
                            case MeasurementType.BandwidthMeas:
                                if (mt is MeasTaskBandWidth)
                                {
                                    var measBandWidth = (mt as MeasTaskBandWidth);
                                    measTaskedit = new MeasTaskBandWidth() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measBandWidth.MeasDtParam, MeasFreqParam = measBandWidth.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio, Sensors = mt.Sensors, Status = mt.Status,  TypeMeasurements = mt.TypeMeasurements };
                                }
                                break;
                            case MeasurementType.Level:
                                if (mt is MeasTaskLevel)
                                {
                                    var measLevel = (mt as MeasTaskLevel);
                                    measTaskedit = new MeasTaskLevel() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measLevel.MeasDtParam, MeasFreqParam = measLevel.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio,  Sensors = mt.Sensors, Status = mt.Status,  TypeMeasurements = mt.TypeMeasurements };
                                }
                                break;
                            case MeasurementType.MonitoringStations:
                                if (mt is MeasTaskMonitoringStations)
                                {
                                    var measMonitoringStations = (mt as MeasTaskMonitoringStations);
                                    measTaskedit = new MeasTaskMonitoringStations() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, StationsForMeasurements = measMonitoringStations.StationsForMeasurements, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio, Sensors = mt.Sensors, Status = mt.Status,  TypeMeasurements = mt.TypeMeasurements };
                                }
                                break;
                            case MeasurementType.Signaling:
                                if (mt is MeasTaskSignaling)
                                {
                                    var measTaskSignaling = (mt as MeasTaskSignaling);
                                    measTaskedit = new MeasTaskSignaling() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measTaskSignaling.MeasDtParam, MeasFreqParam = measTaskSignaling.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio,  Sensors = mt.Sensors, Status = mt.Status,  TypeMeasurements = mt.TypeMeasurements, RefSituation = measTaskSignaling.RefSituation, SignalingMeasTaskParameters = measTaskSignaling.SignalingMeasTaskParameters };
                                }
                                break;
                            case MeasurementType.SpectrumOccupation:
                                if (mt is MeasTaskSpectrumOccupation)
                                {
                                    var measTaskSpectrumOccupation = (mt as MeasTaskSpectrumOccupation);
                                    measTaskedit = new MeasTaskSpectrumOccupation() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measTaskSpectrumOccupation.MeasDtParam, MeasFreqParam = measTaskSpectrumOccupation.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio,  Sensors = mt.Sensors, Status = mt.Status, SpectrumOccupationParameters = measTaskSpectrumOccupation.SpectrumOccupationParameters, TypeMeasurements = measTaskSpectrumOccupation.TypeMeasurements };
                                }
                                break;
                        }

                        var massSensor = SensorIds.ToArray();
                        if (massSensor.Length > 0)
                        {
                            bool isSuccess = false;
                            long? id = null;
                            Process(measTaskedit, massSensor, MeasTaskMode.Run.ToString(), false, out isSuccess, out id, true, out prepareSendEventArr);
                            task = measTaskedit;
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

        public CommonOperation StopMeasTask(ref MeasTask task, out PrepareSendEvent[] prepareSendEventArr)
        {
            prepareSendEventArr = null;
            var result = new CommonOperation();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.MessageProcessing, Events.HandlerCallStopMeasTaskMethod.Text);
                var loadTasks = new LoadMeasTask(_dataLayer, _logger);
                if (task.Id != null)
                {
                    this._logger.Info(Contexts.ThisComponent, string.Format(Events.HandlerStopMeasTaskProcess.Text, task.Id.Value));
                    var Res = loadTasks.ReadTask(task.Id.Value);
                    MeasTask mt = null;
                    if (Res != null) mt = Res;
                    if (mt != null)
                    {
                        var SensorIds = new List<long>();
                        if (mt.MeasSubTasks != null)
                        {
                            for (int d = 0; d < mt.MeasSubTasks.Length; d++)
                            {
                                var item = mt.MeasSubTasks[d];
                                for (int r = 0; r < item.MeasSubTaskSensors.Length; r++)
                                {
                                    var measSubTaskSensor = item.MeasSubTaskSensors[r];

                                    if (!SensorIds.Contains(measSubTaskSensor.SensorId))
                                    {
                                        SensorIds.Add(measSubTaskSensor.SensorId);
                                    }
                                }
                            }
                        }

                        if (mt.Sensors != null)
                        {
                            for (int d = 0; d < mt.Sensors.Length; d++)
                            {
                                var item = mt.Sensors[d];

                                if (item.SensorId.Value > 0)
                                {
                                    if (!SensorIds.Contains(item.SensorId.Value))
                                    {
                                        SensorIds.Add(item.SensorId.Value);
                                    }
                                }
                            }
                        }

                        var measTaskedit = new MeasTask();

                        switch (mt.TypeMeasurements)
                        {
                            case MeasurementType.AmplModulation:
                            case MeasurementType.Bearing:
                            case MeasurementType.FreqModulation:
                            case MeasurementType.Frequency:
                            case MeasurementType.Location:
                            case MeasurementType.Offset:
                            case MeasurementType.PICode:
                            case MeasurementType.Program:
                            case MeasurementType.SoundID:
                            case MeasurementType.SubAudioTone:
                                throw new NotImplementedException($"Not supported type {mt.TypeMeasurements}");
                            case MeasurementType.BandwidthMeas:
                                if (mt is MeasTaskBandWidth)
                                {
                                    var measBandWidth = (mt as MeasTaskBandWidth);
                                    measTaskedit = new MeasTaskBandWidth() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measBandWidth.MeasDtParam, MeasFreqParam = measBandWidth.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio,  Sensors = mt.Sensors, Status = mt.Status,  TypeMeasurements = mt.TypeMeasurements };
                                }
                                break;
                            case MeasurementType.Level:
                                if (mt is MeasTaskLevel)
                                {
                                    var measLevel = (mt as MeasTaskLevel);
                                    measTaskedit = new MeasTaskLevel() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measLevel.MeasDtParam, MeasFreqParam = measLevel.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio, Sensors = mt.Sensors, Status = mt.Status, TypeMeasurements = mt.TypeMeasurements };
                                }
                                break;
                            case MeasurementType.MonitoringStations:
                                if (mt is MeasTaskMonitoringStations)
                                {
                                    var measMonitoringStations = (mt as MeasTaskMonitoringStations);
                                    measTaskedit = new MeasTaskMonitoringStations() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, StationsForMeasurements = measMonitoringStations.StationsForMeasurements, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio, Sensors = mt.Sensors, Status = mt.Status,  TypeMeasurements = mt.TypeMeasurements  };
                                }
                                break;
                            case MeasurementType.Signaling:
                                if (mt is MeasTaskSignaling)
                                {
                                    var measTaskSignaling = (mt as MeasTaskSignaling);
                                    measTaskedit = new MeasTaskSignaling() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measTaskSignaling.MeasDtParam, MeasFreqParam = measTaskSignaling.MeasFreqParam,  MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio,  Sensors = mt.Sensors, Status = mt.Status,  TypeMeasurements = mt.TypeMeasurements, RefSituation = measTaskSignaling.RefSituation, SignalingMeasTaskParameters = measTaskSignaling.SignalingMeasTaskParameters };
                                }
                                break;
                            case MeasurementType.SpectrumOccupation:
                                if (mt is MeasTaskSpectrumOccupation)
                                {
                                    var measTaskSpectrumOccupation = (mt as MeasTaskSpectrumOccupation);
                                    measTaskedit = new MeasTaskSpectrumOccupation() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MeasDtParam = measTaskSpectrumOccupation.MeasDtParam, MeasFreqParam = measTaskSpectrumOccupation.MeasFreqParam, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, Prio = mt.Prio,  Sensors = mt.Sensors, Status = mt.Status,  SpectrumOccupationParameters = measTaskSpectrumOccupation.SpectrumOccupationParameters, TypeMeasurements = measTaskSpectrumOccupation.TypeMeasurements };
                                }
                                break;
                        }
                        
                        var massSensor = SensorIds.ToArray();
                        if (massSensor.Length > 0)
                        {
                            bool isSuccess = false;
                            long? id = null;
                            Process(measTaskedit, massSensor, MeasTaskMode.Stop.ToString(), false, out isSuccess, out id, true, out prepareSendEventArr);
                            task = measTaskedit;
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

        public CommonOperation UpdateMeasTask(ref MeasTask data, out PrepareSendEvent[] prepareSendEventArr)
        {
            prepareSendEventArr = null;
            var result = new CommonOperation();
            try
            {
                PrepareSendEvent[] prepareSendEvents = null;
                var measTask = data;
                var loadSensor = new LoadSensor(_dataLayer, _logger);
                var measTaskProcess = new MeasTaskProcess(this._eventEmitter, this._dataLayer, this._environment, this._messagePublisher, this._logger);
                var measTaskIdentifier = new MeasTaskIdentifier();

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
                        measTaskProcess.Process(measTask, massSensor, MeasTaskMode.Update.ToString(), false, out bool isSuccessTemp, out long? ID, true, out prepareSendEvents);
                        prepareSendEventArr = prepareSendEvents;
                        data = measTask;
                        result.State = CommonOperationState.Success;
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


