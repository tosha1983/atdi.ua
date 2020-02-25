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
        public ResultsMeasurementsStation[] GetResMeasStationHeaderByResIdWithFilter(long ResId, ResultsMeasurementsStationFilters filter)
        {
            var loadResults = new LoadResults(_dataLayer, _logger);
            return loadResults.GetResMeasStationHeaderByResId(ResId, filter);
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
            var site = this._pipelineSite.GetByName<SdrnsServer.InitOnlineMeasurementPipebox, SdrnsServer.InitOnlineMeasurementPipebox>(SdrnsServer.Pipelines.ClientInitOnlineMeasurement);
            var resultPipebox = site.Execute(new SdrnsServer.InitOnlineMeasurementPipebox()
            {
               Period = options.Period,
               SensorId = options.SensorId
            });
            return new OnlineMeasurementInitiationResult()
            {
                Allowed = resultPipebox.Allowed,
                Message = resultPipebox.Message,
                ServerToken = resultPipebox.ServerToken
            };
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
        public bool UpdateSensorTitle(long id, string title)
        {
            var saveDb = new SaveSensor(_dataLayer, _logger);
            return saveDb.UpdateSensorTitle(id, title);
        }

        /// <summary>
        /// Import RefSpectrum into DB
        /// </summary>
        /// <param name="refSpectrum"></param>
        /// <returns></returns>

        public long? ImportRefSpectrum(RefSpectrum refSpectrum)
        {
            var importRefSpectrum = new ImportRefSpectrumData(_dataLayer, _logger);
            return importRefSpectrum.ImportSpectrum(refSpectrum);
        }


        /// <summary>
        /// Get all RefSpectrum
        /// </summary>
        /// <returns></returns>
        public RefSpectrum[] GetAllRefSpectrum()
        {
            var loadSynchroProcessData = new LoadSynchroProcessData(_dataLayer, _logger);
            return loadSynchroProcessData.GetAllRefSpectrum();
        }

        /// <summary>
        /// Data all DataSynchronizationProcess
        /// </summary>
        /// <returns></returns>
        public DataSynchronizationProcess CurrentDataSynchronizationProcess()
        {
            var loadSynchroProcessData = new LoadSynchroProcessData(_dataLayer, _logger);
            return loadSynchroProcessData.CurrentDataSynchronizationProcess();
        }

        public bool DeleteRefSpectrum(long[] RefSpectrumIdsBySDRN)
        {
            var importRefSpectrum = new ImportRefSpectrumData(_dataLayer, _logger);
            return importRefSpectrum.DeleteRefSpectrum(RefSpectrumIdsBySDRN);
        }
        

        /// <summary>
        /// Run DataSynchronizationProcess
        /// </summary>
        /// <returns></returns>
        public bool RunDataSynchronizationProcess(DataSynchronizationBase dataSynchronization, long[] RefSpectrumIdsBySDRN, long[] sensorIdsBySDRN, Area[] areas, StationExtended[] stationsExtended)
        {
            try
            {
                var runSynchroProcess = new RunSynchroProcess(_dataLayer, _logger);
                System.Threading.Tasks.Task.Run(() =>   
                {
                    runSynchroProcess.RunDataSynchronizationProcess(dataSynchronization, RefSpectrumIdsBySDRN, sensorIdsBySDRN, areas, stationsExtended);
                });
                return true;
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
                return false;
            }
        }

        /// <summary>
        /// Get Protocols by parameters
        /// </summary>
        /// <param name="createdBy"> DataSynchronizationBase.CreatedBy</param>
        /// <param name="DateCreated">DataSynchronizationBase.CreatedBy</param>
        /// <param name="DateMeas">DataSynchronizationBase.DateCreated</param>
        /// <param name="freq">DataRefSpectrum.Freq_Mhz</param>
        /// <param name="probability">ProtocolsWithEmittings.probability</param>
        /// <param name="standard">StationExtended.standard</param>
        /// <param name="province">StationExtended.Province</param>
        /// <param name="ownerName">StationExtended.OwnerName</param>
        /// <param name="permissionNumber">StationExtended.permissionNumber</param>
        /// <param name="permissionStart">StationExtended.permissionStart</param>
        /// <param name="permissionStop">StationExtended.PermissionStop</param>
        /// <returns></returns>
        public Protocols[] GetProtocolsByParameters(string createdBy,
                                                    DateTime? DateCreated,
                                                    DateTime? DateMeas,
                                                    double? freq,
                                                    double? probability,
                                                    string standard,
                                                    string province,
                                                    string ownerName,
                                                    string permissionNumber,
                                                    DateTime? permissionStart,
                                                    DateTime? permissionStop)
        {
            throw new NotImplementedException();
        }
    }
}


