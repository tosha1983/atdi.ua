using Atdi.Contracts.WcfServices.Sdrn.Server;
using System;
using System.ServiceModel;
using Atdi.Platform.Logging;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using DM = Atdi.DataModels.Sdrns.Server.Entities;
using ES = Atdi.DataModels.Sdrns.Server.Events;
using System.Collections.Generic;
using Atdi.Platform.Workflows;
using SdrnsServer = Atdi.DataModels.Sdrns.Server;

namespace Atdi.WcfServices.Sdrn.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SdrnsController : WcfServiceBase<ISdrnsController>, ISdrnsController
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;
        private readonly IPipelineSite _pipelineSite;

        public SdrnsController(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, IPipelineSite pipelineSite, ILogger logger)
        {
            this._eventEmitter = eventEmitter;
            this._dataLayer = dataLayer;
            this._logger = logger;
            this._pipelineSite = pipelineSite;
        }

        public bool AddAssociationStationByEmitting(long[] emittingsId, long AssociatedStationID, string AssociatedStationTableName)
        {
            var saveResDb = new SaveResults(_dataLayer, _logger);
            return saveResDb.AddAssociationStationByEmitting(emittingsId, AssociatedStationID, AssociatedStationTableName);
        }

        public MeasTaskIdentifier CreateMeasTask(MeasTask task)
        {
            var site = this._pipelineSite.GetByName<SdrnsServer.ClientMeasTaskPipebox, SdrnsServer.ClientMeasTaskPiperesult>(SdrnsServer.Pipelines.ClientMeasTasks);
            var result = site.Execute(new SdrnsServer.ClientMeasTaskPipebox()
            {
                MeasTaskPipeBox = MapperForMeasTask.ToMap(task),
                MeasTaskModePipeBox = SdrnsServer.MeasTaskMode.New
            });
            return new MeasTaskIdentifier() { Value = result.MeasTaskIdPipeResult };
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
            var site = this._pipelineSite.GetByName<SdrnsServer.ClientMeasTaskPipebox, SdrnsServer.ClientMeasTaskPiperesult>(SdrnsServer.Pipelines.ClientCommands);
            var result = site.Execute(new SdrnsServer.ClientMeasTaskPipebox()
            {
                MeasTaskPipeBox = new SdrnsServer.MeasTask()
                {
                    Id = new SdrnsServer.MeasTaskIdentifier()
                    {
                        Value = taskId.Value
                    }
                },
                MeasTaskModePipeBox = SdrnsServer.MeasTaskMode.Del
            });

            var commonOperationResult = new CommonOperationResult();
            commonOperationResult.FaultCause = result.CommonOperationPipeBoxResult.FaultCause;
            switch (result.CommonOperationPipeBoxResult.State)
            {
                case SdrnsServer.CommonOperationState.Fault:
                    commonOperationResult.State = CommonOperationState.Fault;
                    break;
                case SdrnsServer.CommonOperationState.Success:
                    commonOperationResult.State = CommonOperationState.Success;
                    break;
            }
            return commonOperationResult;
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
            var site = this._pipelineSite.GetByName<SdrnsServer.ClientMeasTaskPipebox, SdrnsServer.ClientMeasTaskPiperesult>(SdrnsServer.Pipelines.ClientCommands);
            var result = site.Execute(new SdrnsServer.ClientMeasTaskPipebox()
            {
                MeasTaskPipeBox = new SdrnsServer.MeasTask()
                {
                    Id = new SdrnsServer.MeasTaskIdentifier()
                    {
                        Value = taskId.Value
                    }
                },
                MeasTaskModePipeBox = SdrnsServer.MeasTaskMode.Run
            });

            var commonOperationResult = new CommonOperationResult();
            commonOperationResult.FaultCause = result.CommonOperationPipeBoxResult.FaultCause;
            switch (result.CommonOperationPipeBoxResult.State)
            {
                case SdrnsServer.CommonOperationState.Fault:
                    commonOperationResult.State = CommonOperationState.Fault;
                    break;
                case SdrnsServer.CommonOperationState.Success:
                    commonOperationResult.State = CommonOperationState.Success;
                    break;
            }
            return commonOperationResult;
        }

        public CommonOperationResult StopMeasTask(MeasTaskIdentifier taskId)
        {
            var site = this._pipelineSite.GetByName<SdrnsServer.ClientMeasTaskPipebox, SdrnsServer.ClientMeasTaskPiperesult>(SdrnsServer.Pipelines.ClientCommands);
            var result = site.Execute(new SdrnsServer.ClientMeasTaskPipebox()
            {
                MeasTaskPipeBox = new SdrnsServer.MeasTask()
                {
                    Id = new SdrnsServer.MeasTaskIdentifier()
                    {
                        Value = taskId.Value
                    }
                },
                MeasTaskModePipeBox = SdrnsServer.MeasTaskMode.Stop
            });

            var commonOperationResult = new CommonOperationResult();
            commonOperationResult.FaultCause = result.CommonOperationPipeBoxResult.FaultCause;
            switch (result.CommonOperationPipeBoxResult.State)
            {
                case SdrnsServer.CommonOperationState.Fault:
                    commonOperationResult.State = CommonOperationState.Fault;
                    break;
                case SdrnsServer.CommonOperationState.Success:
                    commonOperationResult.State = CommonOperationState.Success;
                    break;
            }
            return commonOperationResult;
        }

        public Emitting[] GetEmittingsByIcsmId(long[] ids, string icsmTableName)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetEmittingsByIcsmId(ids, icsmTableName);
        }

        public SignalingSysInfo[] GetSignalingSysInfos(long measResultId, double freq_Hz)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetSignalingSysInfos(measResultId, freq_Hz);

        }

        public OnlineMeasurementInitiationResult InitOnlineMeasurement(OnlineMeasurementOptions options)
        {
            
            // 1. Поиск сенсора - нету - отказ
            // 2. Поиск уже иницированых измерений по сенсору, 
            //    если активно отказ 
            //    иначе нужно обновит состояние в БД об измирении и отменить его со стороны сервера по причин езавершения времени отведенного клиентом для измерения
            // 3. Сгенерировать серверный токен и создать щапись об измерении в БД
            // 4. Подготовить и вернуть результат

            try
            {
                if (options == null)
                {
                    throw new ArgumentNullException(nameof(options));
                }

                if (options.Period.TotalMinutes <= 0)
                {
                    throw new ArgumentException("Incorrect value of Period.");
                }
                using (var dbScope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    
                    var result = new OnlineMeasurementInitiationResult();

                    // step 1

                    var sensorQuery = _dataLayer.GetBuilder<DM.ISensor>()
                        .From()
                        .Select(c => c.Id)
                        .Where(c => c.Id, ConditionOperator.Equal, options.SensorId);

                    var sensorExists = dbScope.Executor.ExecuteAndFetch(sensorQuery, reader =>
                    {
                        var exists = reader.Read();
                        if (exists)
                        {
                            exists = reader.GetValue(c => c.Id) == options.SensorId;
                            // read some data od the Sensor with ID = options.SensorId
                        }
                        return exists;
                    });

                    if (!sensorExists)
                    {
                        result.Allowed = false;
                        result.Message = $"Not found a sensor with ID #{options.SensorId}.";
                        return result;
                    }

                    // step 2

                    var onlineMeasQuery = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                        .From()
                        .Where(c => c.SENSOR.Id, ConditionOperator.Equal, options.SensorId)
                        .Where(c => c.StatusCode, ConditionOperator.NotIn,  
                            (byte)OnlineMeasurementStatus.CanceledByClient,
                            (byte)OnlineMeasurementStatus.CanceledBySensor,
                            (byte)OnlineMeasurementStatus.CanceledByServer,
                            (byte)OnlineMeasurementStatus.DeniedBySensor,
                            (byte)OnlineMeasurementStatus.DeniedByServer)
                        .Select(
                            c => c.Id,
                            c => c.CreatedDate,
                            c => c.StatusCode,
                            c => c.PeriodMinutes,
                            c => c.ServerToken,
                            c => c.StartTime,
                            c => c.FinishTime);

                    var onlineMeases = dbScope.Executor.ExecuteAndFetch(onlineMeasQuery, reader =>
                    {
                        var list = new List<Handlers.OnlineMeasurement.InitiationOnlineMesurementModel>();
                        while(reader.Read())
                        {
                            var model = new Handlers.OnlineMeasurement.InitiationOnlineMesurementModel
                            {
                                Id = reader.GetValue(c => c.Id),
                                CreatedDate = reader.GetValue(c => c.CreatedDate),
                                FinishTime = reader.GetValue(c => c.FinishTime),
                                PeriodMinutes = reader.GetValue(c => c.PeriodMinutes),
                                ServerToken = reader.GetValue(c => c.ServerToken),
                                StartTime = reader.GetValue(c => c.StartTime),
                                Status = reader.GetValue(c => c.StatusCode)
                            };
                            list.Add(model);
                        }
                        return list.ToArray();
                    });
                    for (int i = 0; i < onlineMeases.Length; i++)
                    {
                        var meas = onlineMeases[i];
                        if (meas.Status == (byte)OnlineMeasurementStatus.Initiation || meas.Status == (byte)OnlineMeasurementStatus.WaitSensor)
                        {
                            if ((DateTimeOffset.Now - meas.CreatedDate).TotalMinutes > meas.PeriodMinutes)
                            {
                                var updateQuery = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                                    .Update()
                                    .SetValue(c => c.StatusCode, (byte)OnlineMeasurementStatus.CanceledByServer)
                                    .SetValue(c => c.StatusNote, "CanceledByServer: Measurement period was expired")
                                    .SetValue(c => c.FinishTime, DateTimeOffset.Now)
                                    .Where(c => c.Id, ConditionOperator.Equal, meas.Id);

                                dbScope.Executor.Execute(updateQuery);
                            }
                            else
                            {
                                result.Allowed = false;
                                result.Message = $"The sensor is busy with another measurement (meas token is '{meas.ServerToken}')";
                                return result;
                            }
                        }
                        else if (meas.Status == (byte)OnlineMeasurementStatus.SonsorReady)
                        {
                            if ((DateTimeOffset.Now - (meas.StartTime??meas.CreatedDate)).TotalMinutes > meas.PeriodMinutes)
                            {
                                var updateQuery = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                                    .Update()
                                    .SetValue(c => c.StatusCode, (byte)OnlineMeasurementStatus.CanceledByServer)
                                    .SetValue(c => c.StatusNote, "CanceledByServer: Measurement period was expired")
                                    .SetValue(c => c.FinishTime, DateTimeOffset.Now)
                                    .Where(c => c.Id, ConditionOperator.Equal, meas.Id);

                                dbScope.Executor.Execute(updateQuery);
                            }
                            else
                            {
                                result.Allowed = false;
                                result.Message = $"The sensor is busy with another measurement (meas token is '{meas.ServerToken}')";
                                return result;
                            }
                        }
                    }

                    // step 3

                    var serverToken = Guid.NewGuid();
                    result.ServerToken = serverToken.ToByteArray();

                    var insert = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                        .Insert()
                        .SetValue(c => c.PeriodMinutes, Convert.ToInt32(options.Period.TotalMinutes))
                        .SetValue(c => c.SENSOR.Id, options.SensorId)
                        .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                        .SetValue(c => c.StatusCode, (byte)OnlineMeasurementStatus.Initiation)
                        .SetValue(c => c.StatusNote, "Initiation: SDRN Server sent request to the Sensor")
                        .SetValue(c => c.ServerToken, serverToken);

                    var pk = dbScope.Executor.Execute<DM.IOnlineMesurement_PK>(insert);

                    // step 4 - generate event
                    var initEvent = new ES.OnlineMeasurement.OnInitOnlineMeasurement(this.GetType().FullName)
                    {
                        OnlineMeasId = pk.Id
                    };
                    this._eventEmitter.Emit(initEvent);

                    result.Allowed = true;
                    return result;
                }
                    
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, (EventCategory)"InitOnlineMeasurement",  e, this);
                throw;
            }
            

        }

        public SensorAvailabilityDescriptor GetSensorAvailabilityForOnlineMesurement(byte[] serverToken)
        {

            try
            {
                if (serverToken == null)
                {
                    throw new ArgumentNullException(nameof(serverToken));
                }

                var tokenGuid = new Guid(serverToken);

                using (var dbScope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    var result = new SensorAvailabilityDescriptor();

                    var query = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                        .From()
                        .Where(c => c.ServerToken, ConditionOperator.Equal, tokenGuid)
                        .Select(c => c.Id,
                            c => c.SensorToken,
                            c => c.StatusCode,
                            c => c.StatusNote,
                            c => c.WebSocketUrl);

                    var measExists = dbScope.Executor.ExecuteAndFetch(query, reader =>
                    {
                        var exists = reader.Read();
                        if (exists)
                        {
                            result.SensorToken = reader.GetValue(c => c.SensorToken);
                            result.Status = (OnlineMeasurementStatus)reader.GetValue(c => c.StatusCode);
                            result.Message = reader.GetValue(c => c.StatusNote);
                            result.WebSocketUrl = reader.GetValue(c => c.WebSocketUrl);
                        }
                        return exists;
                    });

                    if (!measExists)
                    {
                        result.Status = OnlineMeasurementStatus.DeniedByServer;
                        result.Message = $"Not found a online measurement data by server token";
                        return result;
                    }

                    return result;
                }

            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, (EventCategory)"GetSensorAvailabilityForOnlineMesurement", e, this);
                throw;
            }
        }
    }
   

}


