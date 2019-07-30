using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns;
using Atdi.DataModels.Sdrns.Device;
using DM = Atdi.DataModels.Sdrns.Device;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Common;
using Atdi.Platform;
using Atdi.Platform.Caching;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendMeasResultsDeviceBusEvent", SubscriberName = "SendMeasResultsSubscriber")]
    public class SendMeasResultsSubscriber : SubscriberBase<DM.MeasResults>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IStatistics _statistics;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataCache<string, int> _taskIdentityCache;

        private readonly IStatisticCounter _messageProcessingHitsCounter;
        private readonly IStatisticCounter _sendMeasResultsErrorsCounter;
        private readonly IStatisticCounter _sendMeasResultsHitsCounter;
        private readonly IStatisticCounter _monitoringStationsCounter;
        private readonly IStatisticCounter _signalingCounter;
        private readonly IStatisticCounter _spectrumOccupationCounter;

        public SendMeasResultsSubscriber(
            IEventEmitter eventEmitter, 
            ISdrnMessagePublisher messagePublisher, 
            IMessagesSite messagesSite, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment, 
            IStatistics statistics,
            IDataCacheSite cacheSite,
            ILogger logger) 
            : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._statistics = statistics;
            this._eventEmitter = eventEmitter;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
            this._taskIdentityCache = cacheSite.Ensure(DataCaches.MeasTaskIdentity);

            if (this._statistics != null)
            {
                this._messageProcessingHitsCounter = _statistics.Counter(Monitoring.Counters.MessageProcessingHits);
                this._sendMeasResultsErrorsCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsErrors);
                this._sendMeasResultsHitsCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsHits);
                this._monitoringStationsCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsMonitoringStations);
                this._signalingCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsSignaling);
                this._spectrumOccupationCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsSpectrumOccupation);
            }

        }

        protected override void Handle(string sensorName, string sensorTechId, DM.MeasResults deliveryObject, long messageId)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                this._messageProcessingHitsCounter?.Increment();
                this._sendMeasResultsHitsCounter?.Increment();

                var status = SdrnMessageHandlingStatus.Unprocessed;
                bool isSuccessProcessed = false;
                var reasonFailure = "";
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {

                        if (deliveryObject.Measurement == MeasurementType.SpectrumOccupation)
                        {
                            this._spectrumOccupationCounter?.Increment();
                            isSuccessProcessed = SaveMeasResultSpectrumOccupation(deliveryObject, scope);
                        }
                        else if (deliveryObject.Measurement == MeasurementType.MonitoringStations)
                        {
                            this._monitoringStationsCounter?.Increment();
                            isSuccessProcessed = SaveMeasResultMonitoringStations(deliveryObject, scope);
                        }
                        else if (deliveryObject.Measurement == MeasurementType.Signaling)
                        {
                            this._signalingCounter?.Increment();
                            isSuccessProcessed = SaveMeasResultSignaling(deliveryObject, scope);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported MeasurementType '{deliveryObject.Measurement}'");
                        }
                    }
                }
                catch (Exception e)
                {
                    this._sendMeasResultsErrorsCounter?.Increment();
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    //status = SdrnMessageHandlingStatus.Error;
                    reasonFailure = e.StackTrace;
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    var deviceCommandResult = new DeviceCommand
                    {
                        EquipmentTechId = sensorTechId,
                        SensorName = sensorName,
                        SdrnServer = this._environment.ServerInstance,
                        Command = "SendMeasResultsConfirmed",
                        CommandId = "SendCommand",
                        CustTxt1 = "Success"
                    };

                    if (status == SdrnMessageHandlingStatus.Error)
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"" + reasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    else if (isSuccessProcessed)
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Success\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    else
                    {
                        deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"" + reasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    }
                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DeviceCommand>();
                    envelop.SensorName = sensorName;
                    envelop.SensorTechId = sensorTechId;
                    envelop.DeliveryObject = deviceCommandResult;
                    _messagePublisher.Send(envelop);
                }

            }
        }
        private bool SaveMeasResultSpectrumOccupation(DM.MeasResults measResult, IDataLayerScope scope)
        {
            try
            {
                if (string.IsNullOrEmpty(measResult.ResultId))
                {
                    WriteLog("Undefined value ResultId", "IResMeas", scope);
                    return false;
                }
                else if (measResult.ResultId.Length > 50)
                    measResult.ResultId.SubString(50);

                if (string.IsNullOrEmpty(measResult.TaskId))
                {
                    WriteLog("Undefined value TaskId", "IResMeas", scope);
                    return false;
                }
                else if (measResult.TaskId.Length > 200)
                    measResult.TaskId.SubString(200);

                if ((measResult.Status != null) && (measResult.Status.Length > 5))
                {
                    measResult.Status = "";
                }
                

                if (!(measResult.SwNumber >= 0 && measResult.SwNumber <= 10000))
                    WriteLog("Incorrect value SwNumber", "IResMeas", scope);

                if (!(measResult.ScansNumber >= 0 && measResult.ScansNumber <= 10000000))
                    WriteLog("Incorrect value ScansNumber", "IResMeas", scope);

                if (measResult.StartTime > measResult.StopTime)
                    WriteLog("StartTime must be less than StopTime", "IResMeas", scope);

                GetIds(measResult.ResultId, measResult.TaskId, out int subMeasTaskId, out int subMeasTaskStaId, out int sensorId, out int resultId, scope);
                long valInsResMeas = 0;
                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.SetValue(c => c.SUBTASK_SENSOR.Id, subMeasTaskStaId);
                var pk = scope.Executor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);
                valInsResMeas = pk.Id;

                if (valInsResMeas > 0)
                {
                    if (measResult.FrequencySamples != null)
                    {
                        bool validationResult = true;
                        foreach (var freqSample in measResult.FrequencySamples)
                        {
                            if (freqSample.Occupation_Pt < 0 || freqSample.Occupation_Pt > 100)
                            {
                                validationResult = false;
                                WriteLog("Incorrect value Occupation_Pt", "IFreqSample", scope);
                            }
                            if (freqSample.Freq_MHz < 0 || freqSample.Freq_MHz > 400000)
                            {
                                validationResult = false;
                                WriteLog("Incorrect value Freq_MHz", "IFreqSample", scope);
                            }
                            var builderInsertResLevels = this._dataLayer.GetBuilder<MD.IResLevels>().Insert();
                            if (freqSample.LevelMax_dBm >= -150 && freqSample.LevelMax_dBm <= 20)
                                builderInsertResLevels.SetValue(c => c.VMMaxLvl, freqSample.LevelMax_dBm);
                            if (freqSample.LevelMin_dBm >= -150 && freqSample.LevelMin_dBm <= 20)
                                builderInsertResLevels.SetValue(c => c.VMinLvl, freqSample.LevelMin_dBm);
                            if (freqSample.Level_dBm >= -150 && freqSample.Level_dBm <= 20)
                                builderInsertResLevels.SetValue(c => c.ValueLvl, freqSample.Level_dBm);
                            if (freqSample.Level_dBmkVm >= 10 && freqSample.Level_dBmkVm <= 140)
                                builderInsertResLevels.SetValue(c => c.ValueSpect, freqSample.Level_dBmkVm);
                            builderInsertResLevels.SetValue(c => c.OccupancySpect, freqSample.Occupation_Pt);
                            builderInsertResLevels.SetValue(c => c.FreqMeas, freqSample.Freq_MHz);
                            builderInsertResLevels.SetValue(c => c.RES_MEAS.Id, valInsResMeas);
                            if (validationResult)
                                scope.Executor.Execute(builderInsertResLevels);
                        }
                    }
                    if (this.ValidateGeoLocation(measResult.Location, "IResMeas", scope))
                    {
                        var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                        builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                        builderInsertResLocSensorMeas.SetValue(c => c.RES_MEAS.Id, valInsResMeas);
                        scope.Executor.Execute(builderInsertResLocSensorMeas);
                    }
                }

                return true;
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                return false;
            }
        }
        private bool SaveMeasResultMonitoringStations(DM.MeasResults measResult, IDataLayerScope scope)
        {
            try
            {
                if (string.IsNullOrEmpty(measResult.ResultId))
                {
                    WriteLog("Undefined value ResultId", "IResMeas", scope);
                    return false;
                }
                else if (measResult.ResultId.Length > 50)
                    measResult.ResultId.SubString(50);

                if (string.IsNullOrEmpty(measResult.TaskId))
                {
                    WriteLog("Undefined value TaskId", "IResMeas", scope);
                    return false;
                }
                else if (measResult.TaskId.Length > 200)
                    measResult.TaskId.SubString(200);

                if ((measResult.Status != null) && (measResult.Status.Length > 5))
                {
                    measResult.Status = "";
                }
                

                if (!(measResult.SwNumber >= 0 && measResult.SwNumber <= 10000))
                    WriteLog("Incorrect value SwNumber", "IResMeas", scope);

                if (measResult.StationResults == null || measResult.StationResults.Length == 0)
                {
                    WriteLog("Undefined values StationResults[]", "IResMeas", scope);
                    return false;
                }
                //if (measResult.Routes == null || measResult.Routes.Length == 0)
                //{
                    //WriteLog("Undefined values Routes[]", "IResMeas");
                    //return false;
                //}

                GetIds(measResult.ResultId, measResult.TaskId, out int subMeasTaskId, out int subMeasTaskStaId, out int sensorId, out int resultId, scope);
                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.DataRank, measResult.SwNumber);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.SetValue(c => c.SUBTASK_SENSOR.Id, subMeasTaskStaId);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                var idResMeas = scope.Executor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);

                if (measResult.Routes != null)
                {
                    foreach (Route route in measResult.Routes)
                    {
                        if (route.RoutePoints != null)
                        {
                            foreach (RoutePoint routePoint in route.RoutePoints)
                            {
                                var builderInsertroutePoints = this._dataLayer.GetBuilder<MD.IResRoutes>().Insert();
                                if (routePoint.StartTime > routePoint.FinishTime)
                                    WriteLog("StartTime must be less than FinishTime", "IResRoutesRaw", scope);

                                if (this.ValidateGeoLocation<RoutePoint>(routePoint, "IResRoutes", scope))
                                {
                                    builderInsertroutePoints.SetValue(c => c.Lat, routePoint.Lat);
                                    builderInsertroutePoints.SetValue(c => c.Lon, routePoint.Lon);
                                    builderInsertroutePoints.SetValue(c => c.Agl, routePoint.AGL);
                                    builderInsertroutePoints.SetValue(c => c.Asl, routePoint.ASL);
                                }
                                builderInsertroutePoints.SetValue(c => c.FinishTime, routePoint.FinishTime);
                                builderInsertroutePoints.SetValue(c => c.StartTime, routePoint.StartTime);
                                builderInsertroutePoints.SetValue(c => c.RouteId, route.RouteId);
                                builderInsertroutePoints.SetValue(c => c.PointStayType, routePoint.PointStayType.ToString());
                                builderInsertroutePoints.SetValue(c => c.RES_MEAS.Id, idResMeas.Id);
                                scope.Executor.Execute(builderInsertroutePoints);
                            }
                        }
                    }
                }
                if (measResult.StationResults != null)
                {
                    foreach (StationMeasResult station in measResult.StationResults)
                    {
                        station.StationId = station.StationId.SubString(50);
                        station.TaskGlobalSid = station.TaskGlobalSid.SubString(50);
                        station.RealGlobalSid = station.RealGlobalSid.SubString(50);
                        station.SectorId = station.SectorId.SubString(50);
                        station.Status = station.Status.SubString(5);
                        station.Standard = station.Standard.SubString(10);
                        if (!station.GeneralResult.RBW_kHz.HasValue || station.GeneralResult.RBW_kHz.Value < 0.001 || station.GeneralResult.RBW_kHz.Value > 100000)
                            station.GeneralResult.RBW_kHz = null;
                        if (!station.GeneralResult.VBW_kHz.HasValue || station.GeneralResult.VBW_kHz.Value < 0.001 || station.GeneralResult.VBW_kHz.Value > 100000)
                            station.GeneralResult.VBW_kHz = null;
                        if (!station.GeneralResult.CentralFrequency_MHz.HasValue || station.GeneralResult.CentralFrequency_MHz.Value < 0.001 || station.GeneralResult.CentralFrequency_MHz.Value > 400000)
                            station.GeneralResult.CentralFrequency_MHz = null;
                        if (!station.GeneralResult.CentralFrequencyMeas_MHz.HasValue || station.GeneralResult.CentralFrequencyMeas_MHz.Value < 0.001 || station.GeneralResult.CentralFrequencyMeas_MHz.Value > 400000)
                            station.GeneralResult.CentralFrequencyMeas_MHz = null;
                        if (!station.GeneralResult.SpectrumStartFreq_MHz.HasValue || station.GeneralResult.SpectrumStartFreq_MHz.Value < 0.001m || station.GeneralResult.SpectrumStartFreq_MHz.Value > 400000
                           || !station.GeneralResult.SpectrumSteps_kHz.HasValue || station.GeneralResult.SpectrumSteps_kHz.Value < 0.001m || station.GeneralResult.SpectrumSteps_kHz.Value > 100000)
                        {
                            station.GeneralResult.SpectrumStartFreq_MHz = null;
                            station.GeneralResult.SpectrumSteps_kHz = null;
                            station.GeneralResult.LevelsSpectrum_dBm = null;
                            station.GeneralResult.BandwidthResult = null;
                        }
                        if (station.GeneralResult.MeasStartTime > station.GeneralResult.MeasFinishTime)
                        {
                            WriteLog("MeasStartTime must be less than MeasFinishTime", "IResStGeneralRaw", scope);
                        }

                        var builderInsertResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().Insert();
                        builderInsertResMeasStation.SetValue(c => c.Status, station.Status);
                        builderInsertResMeasStation.SetValue(c => c.MeasGlobalSID, station.RealGlobalSid);
                        builderInsertResMeasStation.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                        builderInsertResMeasStation.SetValue(c => c.RES_MEAS.Id, idResMeas.Id);
                        builderInsertResMeasStation.SetValue(c => c.Standard, station.Standard);
                        if (int.TryParse(station.StationId, out int Idstation))
                            builderInsertResMeasStation.SetValue(c => c.ClientStationCode, Idstation);
                        if (int.TryParse(station.SectorId, out int IdSector))
                            builderInsertResMeasStation.SetValue(c => c.ClientSectorCode, IdSector);
                        
                        var valInsResMeasStation = scope.Executor.Execute<MD.IResMeasStation_PK>(builderInsertResMeasStation);

                        if (valInsResMeasStation.Id > 0)
                        {
                            if (measResult.SensorId != null)
                            {
                                var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Insert();
                                builderInsertLinkResSensor.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation.Id);
                                builderInsertLinkResSensor.SetValue(c => c.SENSOR.Id, (long)measResult.SensorId);
                                
                                scope.Executor.Execute<MD.ILinkResSensor_PK>(builderInsertLinkResSensor);
                            }

                            var generalResult = station.GeneralResult;
                            if (generalResult != null)
                            {
                                var IDResGeneral = InsertResStGeneral(station, valInsResMeasStation.Id, generalResult, scope);
                                if (IDResGeneral > 0)
                                {
                                    InsertResSysInfo(station, IDResGeneral, scope);
                                    InsertResStMaskElement(station, IDResGeneral, scope);
                                    InsertResStLevelCar(station, valInsResMeasStation.Id, scope);
                                    InsertBearing(valInsResMeasStation.Id, station, scope);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                return false;
            }
        }
        private bool SaveMeasResultSignaling(DM.MeasResults measResult, IDataLayerScope scope)
        {
            try
            {
                if (string.IsNullOrEmpty(measResult.ResultId))
                {
                    WriteLog("Undefined value ResultId", "IResMeas", scope);
                    return false;
                }
                else if (measResult.ResultId.Length > 50)
                    measResult.ResultId.SubString(50);

                if (string.IsNullOrEmpty(measResult.TaskId))
                {
                    WriteLog("Undefined value TaskId", "IResMeas", scope);
                    return false;
                }
                else if (measResult.TaskId.Length > 200)
                    measResult.TaskId.SubString(200);

                
                if ((measResult.Status != null) && (measResult.Status.Length > 5))
                {
                    measResult.Status = "";
                }
                

                if (!(measResult.ScansNumber >= 0 && measResult.ScansNumber <= 10000000))
                    WriteLog("Incorrect value ScansNumber", "IResMeas", scope);

                if (measResult.StartTime > measResult.StopTime)
                    WriteLog("StartTime must be less than StopTime", "IResMeas", scope);

                GetIds(measResult.ResultId, measResult.TaskId, out int subMeasTaskId, out int subMeasTaskStaId, out int sensorId, out int resultId, scope);

                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.SetValue(c => c.SUBTASK_SENSOR.Id, subMeasTaskStaId);
                var valInsResMeas = scope.Executor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);
                if (valInsResMeas.Id > 0)
                {
                    if (this.ValidateGeoLocation(measResult.Location, "IResMeas", scope))
                    {
                        var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                        builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                        builderInsertResLocSensorMeas.SetValue(c => c.RES_MEAS.Id, valInsResMeas.Id);
                        scope.Executor.Execute(builderInsertResLocSensorMeas);
                    }

                    if (measResult.RefLevels != null)
                    {
                        bool validationResult = true;
                        var refLevels = measResult.RefLevels;

                        List<float> listLevels = new List<float>();
                        foreach (float levels in refLevels.levels)
                        {
                            if (levels >= -200 && levels <= 50)
                                listLevels.Add(levels);
                            else
                                WriteLog("Incorrect value level", "IReferenceLevels", scope);
                        }
                        if (listLevels.Count > 0)
                            refLevels.levels = listLevels.ToArray();
                        else
                            validationResult = false;

                        if (refLevels.StartFrequency_Hz < 9000 || refLevels.StartFrequency_Hz > 400000000000)
                        {
                            validationResult = false;
                            WriteLog("Incorrect value StartFrequency_Hz", "IReferenceLevels", scope);
                        }
                        if (refLevels.StepFrequency_Hz < 1 || refLevels.StepFrequency_Hz > 1000000000)
                        {
                            validationResult = false;
                            WriteLog("Incorrect value StepFrequency_Hz", "IReferenceLevels", scope);
                        }
                        var builderInsertReferenceLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Insert();
                        builderInsertReferenceLevels.SetValue(c => c.StartFrequency_Hz, refLevels.StartFrequency_Hz);
                        builderInsertReferenceLevels.SetValue(c => c.StepFrequency_Hz, refLevels.StepFrequency_Hz);
                        if (refLevels.levels != null)
                        {
                            builderInsertReferenceLevels.SetValue(c => c.RefLevels, refLevels.levels);
                        }
                        builderInsertReferenceLevels.SetValue(c => c.RES_MEAS.Id, valInsResMeas.Id);
                        if (validationResult)
                            scope.Executor.Execute<MD.IReferenceLevels_PK>(builderInsertReferenceLevels);
                    }
                    if (measResult.Emittings != null)
                    {
                        foreach (Emitting emitting in measResult.Emittings)
                        {
                            bool validationResult = true;
                            if (!(emitting.StartFrequency_MHz >= 0.009 && emitting.StartFrequency_MHz <= 400000))
                            {
                                WriteLog("Incorrect value StartFrequency_MHz", "IEmitting", scope);
                                validationResult = false;
                            }
                            if (!(emitting.StopFrequency_MHz >= 0.009 && emitting.StopFrequency_MHz <= 400000))
                            {
                                WriteLog("Incorrect value StopFrequency_MHz", "IEmitting", scope);
                                validationResult = false;
                            }
                            if (emitting.StartFrequency_MHz > emitting.StopFrequency_MHz)
                            {
                                WriteLog("StartFrequency_MHz must be less than StopFrequency_MHz", "IEmitting", scope);
                                validationResult = false;
                            }
                            if (!validationResult)
                                continue;

                            var builderInsertEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Insert();
                            if (emitting.CurentPower_dBm >= -200 && emitting.CurentPower_dBm <= 50)
                                builderInsertEmitting.SetValue(c => c.CurentPower_dBm, emitting.CurentPower_dBm);
                            if (emitting.MeanDeviationFromReference >= 0 && emitting.MeanDeviationFromReference <= 1)
                                builderInsertEmitting.SetValue(c => c.MeanDeviationFromReference, emitting.MeanDeviationFromReference);
                            if (emitting.ReferenceLevel_dBm >= -200 && emitting.ReferenceLevel_dBm <= 50)
                                builderInsertEmitting.SetValue(c => c.ReferenceLevel_dBm, emitting.ReferenceLevel_dBm);
                            if (emitting.TriggerDeviationFromReference >= 0 && emitting.TriggerDeviationFromReference <= 1)
                                builderInsertEmitting.SetValue(c => c.TriggerDeviationFromReference, emitting.TriggerDeviationFromReference);
                            builderInsertEmitting.SetValue(c => c.RES_MEAS.Id, valInsResMeas.Id);
                            builderInsertEmitting.SetValue(c => c.SensorId, emitting.SensorId);
                            if (emitting.EmittingParameters != null)
                            {
                                if (emitting.EmittingParameters.StandardBW >= 0 && emitting.EmittingParameters.StandardBW <= 1000000)
                                {
                                    if (emitting.EmittingParameters.RollOffFactor >= 0 && emitting.EmittingParameters.RollOffFactor <= 2.5)
                                        builderInsertEmitting.SetValue(c => c.RollOffFactor, emitting.EmittingParameters.RollOffFactor);
                                    builderInsertEmitting.SetValue(c => c.StandardBW, emitting.EmittingParameters.StandardBW);
                                }
                            }
                            builderInsertEmitting.SetValue(c => c.StartFrequency_MHz, emitting.StartFrequency_MHz);
                            builderInsertEmitting.SetValue(c => c.StopFrequency_MHz, emitting.StopFrequency_MHz);
                            var levelsDistribution = emitting.LevelsDistribution;
                            if (levelsDistribution != null)
                            {
                                List<int> listLevels = new List<int>();
                                List<int> listCounts = new List<int>();
                                for (int i = 0; i < levelsDistribution.Count.Length; i++)
                                {
                                    if(levelsDistribution.Count[i] >= 0 && levelsDistribution.Count[i] <= Int32.MaxValue && levelsDistribution.Levels[i] >= -200 && levelsDistribution.Levels[i] <= 100)
                                    {
                                        listLevels.Add(levelsDistribution.Levels[i]);
                                        listCounts.Add(levelsDistribution.Count[i]);
                                    }
                                }
                                if (listLevels.Count > 0 && listCounts.Count > 0)
                                {
                                    builderInsertEmitting.SetValue(c => c.LevelsDistributionCount, listCounts.ToArray());
                                    builderInsertEmitting.SetValue(c => c.LevelsDistributionLvl, listLevels.ToArray());
                                }
                            }
                            if (emitting.SignalMask != null)
                            {
                                builderInsertEmitting.SetValue(c => c.Loss_dB, emitting.SignalMask.Loss_dB);
                                builderInsertEmitting.SetValue(c => c.Freq_kHz, emitting.SignalMask.Freq_kHz);
                            }
                            var valInsReferenceEmitting = scope.Executor.Execute<MD.IEmitting_PK>(builderInsertEmitting);

                            if (valInsReferenceEmitting.Id > 0)
                            {
                                if (emitting.WorkTimes != null)
                                {
                                    foreach (WorkTime workTime in emitting.WorkTimes)
                                    {
                                        bool validationTimeResult = true;
                                        if (workTime.StartEmitting > workTime.StopEmitting)
                                        {
                                            WriteLog("StartEmitting must be less than StopEmitting", "IWorkTime", scope);
                                            validationTimeResult = false;
                                        }
                                        if (!(workTime.PersentAvailability >= 0 && workTime.PersentAvailability <= 100))
                                        {
                                            WriteLog("Incorrect value PersentAvailability", "IWorkTime", scope);
                                            validationTimeResult = false;
                                        }

                                        if (!validationTimeResult)
                                            continue;

                                        var builderInsertIWorkTime = this._dataLayer.GetBuilder<MD.IWorkTime>().Insert();
                                        builderInsertIWorkTime.SetValue(c => c.EmittingId, valInsReferenceEmitting.Id);
                                        if (workTime.HitCount >= 0 && workTime.HitCount <=  Int32.MaxValue)
                                            builderInsertIWorkTime.SetValue(c => c.HitCount, workTime.HitCount);
                                        builderInsertIWorkTime.SetValue(c => c.PersentAvailability, workTime.PersentAvailability);
                                        builderInsertIWorkTime.SetValue(c => c.StartEmitting, workTime.StartEmitting);
                                        builderInsertIWorkTime.SetValue(c => c.StopEmitting, workTime.StopEmitting);
                                        scope.Executor.Execute(builderInsertIWorkTime);
                                    }
                                }
                                var spectrum = emitting.Spectrum;
                                if (spectrum != null)
                                {
                                    bool validationSpectrumResult = true;

                                    List<float> listLevelsdBmB = new List<float>();
                                    foreach (float levelsdBmB in spectrum.Levels_dBm)
                                    {
                                        if (levelsdBmB >= -200 && levelsdBmB <= 50)
                                            listLevelsdBmB.Add(levelsdBmB);
                                        else
                                            WriteLog("Incorrect value level", "ISpectrum", scope);
                                    }

                                    if (listLevelsdBmB.Count > 0)
                                        spectrum.Levels_dBm = listLevelsdBmB.ToArray();
                                    else
                                        validationSpectrumResult = false;

                                    if (!(spectrum.SpectrumStartFreq_MHz >= 0.009 && spectrum.SpectrumStartFreq_MHz <= 400000))
                                    {
                                        WriteLog("Incorrect value SpectrumStartFreq_MHz", "ISpectrumRaw", scope);
                                        validationSpectrumResult = false;
                                    }
                                    if (!(spectrum.SpectrumSteps_kHz >= 0.001 && spectrum.SpectrumSteps_kHz <= 1000000))
                                    {
                                        WriteLog("Incorrect value SpectrumSteps_kHz", "ISpectrumRaw", scope);
                                        validationSpectrumResult = false;
                                    }

                                    if (!(spectrum.T1 <= spectrum.MarkerIndex && spectrum.MarkerIndex <= spectrum.T2))
                                        WriteLog("Incorrect value MarkerIndex", "ISpectrumRaw", scope);
                                    if (!(spectrum.T1 >= 0 && spectrum.T1 <= spectrum.T2))
                                    {
                                        WriteLog("Incorrect value T1", "ISpectrumRaw", scope);
                                        validationSpectrumResult = false;
                                    }
                                    if (!(spectrum.T2 >= spectrum.T1 && spectrum.T2 <= spectrum.Levels_dBm.Length))
                                    {
                                        WriteLog("Incorrect value T2", "ISpectrumRaw", scope);
                                        validationSpectrumResult = false;
                                    }

                                    if (validationSpectrumResult)
                                    {

                                        var builderInsertISpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>().Insert();
                                        builderInsertISpectrum.SetValue(c => c.EMITTING.Id, valInsReferenceEmitting.Id);
                                        builderInsertISpectrum.SetValue(c => c.CorrectnessEstimations, spectrum.СorrectnessEstimations == true ? 1 : 0);
                                        builderInsertISpectrum.SetValue(c => c.Contravention, spectrum.Contravention == true ? 1 : 0);
                                        if (spectrum.Bandwidth_kHz >= 0 && spectrum.Bandwidth_kHz <= 1000000)
                                            builderInsertISpectrum.SetValue(c => c.Bandwidth_kHz, spectrum.Bandwidth_kHz);
                                        else
                                            WriteLog("Incorrect value Bandwidth_kHz", "ISpectrum", scope);
                                        builderInsertISpectrum.SetValue(c => c.MarkerIndex, spectrum.MarkerIndex);
                                        if (spectrum.SignalLevel_dBm >= -200 && spectrum.SignalLevel_dBm <= 50)
                                            builderInsertISpectrum.SetValue(c => c.SignalLevel_dBm, spectrum.SignalLevel_dBm);
                                        else
                                            WriteLog("Incorrect value SignalLevel_dBm", "ISpectrum", scope);
                                        builderInsertISpectrum.SetValue(c => c.SpectrumStartFreq_MHz, spectrum.SpectrumStartFreq_MHz);
                                        builderInsertISpectrum.SetValue(c => c.SpectrumSteps_kHz, spectrum.SpectrumSteps_kHz);
                                        builderInsertISpectrum.SetValue(c => c.T1, spectrum.T1);
                                        builderInsertISpectrum.SetValue(c => c.T2, spectrum.T2);
                                        if (spectrum.TraceCount >= 0 && spectrum.TraceCount <= 10000)
                                            builderInsertISpectrum.SetValue(c => c.TraceCount, spectrum.TraceCount);
                                        else
                                            WriteLog("Incorrect value TraceCount", "ISpectrum", scope);
                                        builderInsertISpectrum.SetValue(c => c.Levels_dBm, spectrum.Levels_dBm);
                                        scope.Executor.Execute<MD.ISpectrum_PK>(builderInsertISpectrum);
                                    }
                                }
                            }
                            if (emitting.SysInfos != null)
                            {
                                foreach (SignalingSysInfo sysInfo in emitting.SysInfos)
                                {
                                    var builderInsertSysInfo = this._dataLayer.GetBuilder<MD.ISignalingSysInfo>().Insert();
                                    builderInsertSysInfo.SetValue(c => c.EMITTING.Id, valInsReferenceEmitting.Id);
                                    builderInsertSysInfo.SetValue(c => c.BandWidth_Hz, sysInfo.BandWidth_Hz);
                                    builderInsertSysInfo.SetValue(c => c.BSIC, sysInfo.BSIC);
                                    builderInsertSysInfo.SetValue(c => c.ChannelNumber, sysInfo.ChannelNumber);
                                    builderInsertSysInfo.SetValue(c => c.CID, sysInfo.CID);
                                    builderInsertSysInfo.SetValue(c => c.CtoI, sysInfo.CtoI);
                                    builderInsertSysInfo.SetValue(c => c.Freq_Hz, sysInfo.Freq_Hz);
                                    builderInsertSysInfo.SetValue(c => c.LAC, sysInfo.LAC);
                                    builderInsertSysInfo.SetValue(c => c.Level_dBm, sysInfo.Level_dBm);
                                    builderInsertSysInfo.SetValue(c => c.MCC, sysInfo.MCC);
                                    builderInsertSysInfo.SetValue(c => c.MNC, sysInfo.MNC);
                                    builderInsertSysInfo.SetValue(c => c.Power, sysInfo.Power);
                                    builderInsertSysInfo.SetValue(c => c.RNC, sysInfo.RNC);
                                    builderInsertSysInfo.SetValue(c => c.Standard, sysInfo.Standard);
                                    var valInsSysInfo = scope.Executor.Execute<MD.ISignalingSysInfo_PK>(builderInsertSysInfo);
                                    if (valInsSysInfo.Id > 0 && sysInfo.WorkTimes != null)
                                    {
                                        foreach (WorkTime workTime in sysInfo.WorkTimes)
                                        {
                                            bool validationTimeResult = true;
                                            if (workTime.StartEmitting > workTime.StopEmitting)
                                            {
                                                WriteLog("StartEmitting must be less than StopEmitting", "ISignalingSysInfoWorkTime", scope);
                                                validationTimeResult = false;
                                            }
                                            if (!(workTime.PersentAvailability >= 0 && workTime.PersentAvailability <= 100))
                                            {
                                                WriteLog("Incorrect value PersentAvailability", "ISignalingSysInfoWorkTime", scope);
                                                validationTimeResult = false;
                                            }

                                            if (!validationTimeResult)
                                                continue;

                                            var builderInsertIWorkTime = this._dataLayer.GetBuilder<MD.ISignalingSysInfoWorkTime>().Insert();
                                            builderInsertIWorkTime.SetValue(c => c.SYSINFO.Id, valInsSysInfo.Id);
                                            if (workTime.HitCount >= 0 && workTime.HitCount <= Int32.MaxValue)
                                                builderInsertIWorkTime.SetValue(c => c.HitCount, workTime.HitCount);
                                            builderInsertIWorkTime.SetValue(c => c.PersentAvailability, workTime.PersentAvailability);
                                            builderInsertIWorkTime.SetValue(c => c.StartEmitting, workTime.StartEmitting);
                                            builderInsertIWorkTime.SetValue(c => c.StopEmitting, workTime.StopEmitting);
                                            scope.Executor.Execute(builderInsertIWorkTime);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                return false;
            }
        }
        private long InsertResStGeneral(StationMeasResult station, long valInsResMeasStation, GeneralMeasResult generalResult, IDataLayerScope scope)
        {
            var builderInsertResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().Insert();
            builderInsertResStGeneral.SetValue(c => c.Rbw, generalResult.RBW_kHz);
            builderInsertResStGeneral.SetValue(c => c.Vbw, generalResult.VBW_kHz);
            builderInsertResStGeneral.SetValue(c => c.CentralFrequencyMeas, generalResult.CentralFrequencyMeas_MHz);
            builderInsertResStGeneral.SetValue(c => c.CentralFrequency, generalResult.CentralFrequency_MHz);
            builderInsertResStGeneral.SetValue(c => c.DurationMeas, generalResult.MeasDuration_sec);
            if (generalResult.BandwidthResult != null)
            {
                var bandwidthResult = generalResult.BandwidthResult;
                if (bandwidthResult.MarkerIndex.HasValue && bandwidthResult.T1.HasValue && bandwidthResult.T2.HasValue)
                {
                    if (bandwidthResult.T1.Value >= 0 && bandwidthResult.T1.Value <= bandwidthResult.MarkerIndex.Value
                        && bandwidthResult.T2.Value >= bandwidthResult.MarkerIndex.Value && bandwidthResult.T2.Value <= 100000)
                    {
                        builderInsertResStGeneral.SetValue(c => c.MarkerIndex, bandwidthResult.MarkerIndex);
                        builderInsertResStGeneral.SetValue(c => c.T1, bandwidthResult.T1);
                        builderInsertResStGeneral.SetValue(c => c.T2, bandwidthResult.T2);

                    }
                    else
                    {
                        WriteLog("Incorrect values T1, T2 or M", "IResStGeneral", scope);
                    }
                }
                if (bandwidthResult.Bandwidth_kHz.HasValue && bandwidthResult.Bandwidth_kHz >= 1 && bandwidthResult.Bandwidth_kHz <= 100000)
                    builderInsertResStGeneral.SetValue(c => c.BW, bandwidthResult.Bandwidth_kHz);
                else WriteLog("Incorrect value of Bandwidth", "IResStGeneral", scope);
                if (bandwidthResult.TraceCount >= 1 && bandwidthResult.TraceCount <= 100000)
                {
                    WriteLog("Incorrect value TraceCount", "IResStGeneral", scope);
                }
                builderInsertResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                builderInsertResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations == true ? 1 : 0);
            }
            builderInsertResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
            builderInsertResStGeneral.SetValue(c => c.SpecrumStartFreq, generalResult.SpectrumStartFreq_MHz);
            builderInsertResStGeneral.SetValue(c => c.SpecrumSteps, generalResult.SpectrumSteps_kHz);
            builderInsertResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
            builderInsertResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
            builderInsertResStGeneral.SetValue(c => c.LevelsSpectrumdBm, station.GeneralResult.LevelsSpectrum_dBm);
            builderInsertResStGeneral.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation);
            
            var IDResGeneral = scope.Executor.Execute<MD.IResStGeneral_PK>(builderInsertResStGeneral);
            return IDResGeneral.Id;
        }
        private void InsertResSysInfo(StationMeasResult station, long IDResGeneral, IDataLayerScope scope)
        {
            if (station.GeneralResult.StationSysInfo != null)
            {
                var stationSysInfo = station.GeneralResult.StationSysInfo;
                var builderInsertResSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>().Insert();
                if (stationSysInfo.Location != null)
                {
                    var stationSysInfoLocation = stationSysInfo.Location;
                    builderInsertResSysInfo.SetValue(c => c.Agl, stationSysInfoLocation.AGL);
                    builderInsertResSysInfo.SetValue(c => c.Asl, stationSysInfoLocation.ASL);
                    builderInsertResSysInfo.SetValue(c => c.Lat, stationSysInfoLocation.Lat);
                    builderInsertResSysInfo.SetValue(c => c.Lon, stationSysInfoLocation.Lon);
                }
                builderInsertResSysInfo.SetValue(c => c.Bandwidth, stationSysInfo.BandWidth);
                builderInsertResSysInfo.SetValue(c => c.BaseId, stationSysInfo.BaseID);
                builderInsertResSysInfo.SetValue(c => c.Bsic, stationSysInfo.BSIC);
                builderInsertResSysInfo.SetValue(c => c.ChannelNumber, stationSysInfo.ChannelNumber);
                builderInsertResSysInfo.SetValue(c => c.Cid, stationSysInfo.CID);
                builderInsertResSysInfo.SetValue(c => c.Code, stationSysInfo.Code);
                builderInsertResSysInfo.SetValue(c => c.Ctoi, stationSysInfo.CtoI);
                builderInsertResSysInfo.SetValue(c => c.Eci, stationSysInfo.ECI);
                builderInsertResSysInfo.SetValue(c => c.Enodebid, stationSysInfo.eNodeBId);
                builderInsertResSysInfo.SetValue(c => c.Freq, stationSysInfo.Freq);
                builderInsertResSysInfo.SetValue(c => c.Icio, stationSysInfo.IcIo);
                builderInsertResSysInfo.SetValue(c => c.InbandPower, stationSysInfo.INBAND_POWER);
                builderInsertResSysInfo.SetValue(c => c.Iscp, stationSysInfo.ISCP);
                builderInsertResSysInfo.SetValue(c => c.Lac, stationSysInfo.LAC);
                builderInsertResSysInfo.SetValue(c => c.Mcc, stationSysInfo.MCC);
                builderInsertResSysInfo.SetValue(c => c.Mnc, stationSysInfo.MNC);
                builderInsertResSysInfo.SetValue(c => c.Nid, stationSysInfo.NID);
                builderInsertResSysInfo.SetValue(c => c.Pci, stationSysInfo.PCI);
                builderInsertResSysInfo.SetValue(c => c.Pn, stationSysInfo.PN);
                builderInsertResSysInfo.SetValue(c => c.Power, stationSysInfo.Power);
                builderInsertResSysInfo.SetValue(c => c.Ptotal, stationSysInfo.Ptotal);
                builderInsertResSysInfo.SetValue(c => c.Rnc, stationSysInfo.RNC);
                builderInsertResSysInfo.SetValue(c => c.Rscp, stationSysInfo.RSCP);
                builderInsertResSysInfo.SetValue(c => c.Rsrp, stationSysInfo.RSRP);
                builderInsertResSysInfo.SetValue(c => c.Rsrq, stationSysInfo.RSRQ);
                builderInsertResSysInfo.SetValue(c => c.Sc, stationSysInfo.SC);
                builderInsertResSysInfo.SetValue(c => c.Sid, stationSysInfo.SID);
                builderInsertResSysInfo.SetValue(c => c.Tac, stationSysInfo.TAC);
                builderInsertResSysInfo.SetValue(c => c.TypeCdmaevdo, stationSysInfo.TypeCDMAEVDO);
                builderInsertResSysInfo.SetValue(c => c.Ucid, stationSysInfo.UCID);
                builderInsertResSysInfo.SetValue(c => c.RES_STGENERAL.Id, IDResGeneral);
                var IDResSysInfoGeneral = scope.Executor.Execute<MD.IResSysInfo_PK>(builderInsertResSysInfo);
                if (IDResSysInfoGeneral.Id > 0 && stationSysInfo.InfoBlocks != null)
                {
                    foreach (StationSysInfoBlock blocks in stationSysInfo.InfoBlocks)
                    {
                        var builderInsertStationSysInfoBlock = this._dataLayer.GetBuilder<MD.IResSysInfoBlocks>().Insert();
                        builderInsertStationSysInfoBlock.SetValue(c => c.Data, blocks.Data);
                        builderInsertStationSysInfoBlock.SetValue(c => c.Type, blocks.Type);
                        builderInsertStationSysInfoBlock.SetValue(c => c.RES_SYS_INFO.Id, IDResSysInfoGeneral.Id);
                        scope.Executor.Execute<MD.IResSysInfoBlocks_PK>(builderInsertStationSysInfoBlock);
                    }
                }
            }
        }
        private void InsertResStMaskElement(StationMeasResult station, long IDResGeneral, IDataLayerScope scope)
        {
            if (station.GeneralResult.BWMask != null)
            {
                foreach (ElementsMask maskElem in station.GeneralResult.BWMask)
                {
                    if (maskElem.Level_dB.HasValue && maskElem.Level_dB.Value >= -300 && maskElem.Level_dB.Value <= 300
                        && maskElem.BW_kHz.HasValue && maskElem.BW_kHz.Value >= 1 && maskElem.BW_kHz.Value <= 200000)
                    {
                        var builderInsertmaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElement>().Insert();
                        builderInsertmaskElem.SetValue(c => c.Bw, maskElem.BW_kHz);
                        builderInsertmaskElem.SetValue(c => c.Level, maskElem.Level_dB);
                        builderInsertmaskElem.SetValue(c => c.RES_STGENERAL.Id, IDResGeneral);
                        scope.Executor.Execute(builderInsertmaskElem);
                    }
                    else WriteLog($"Incorrect value Level_dB: {maskElem.Level_dB} or BW_kHz: {maskElem.BW_kHz}", "InsertResStMaskElement", scope);
                }
            }
        }
        private void InsertResStLevelCar(StationMeasResult station, long valInsResMeasStation, IDataLayerScope scope)
        {
            if (station.LevelResults != null)
            {
                foreach (LevelMeasResult car in station.LevelResults)
                {
                    if (car.Level_dBm.HasValue && car.Level_dBm >= -150 && car.Level_dBm <= 20
                        &&
                        ((car.Level_dBmkVm.HasValue && car.Level_dBmkVm >= -10 && car.Level_dBmkVm <= 140) || (!car.Level_dBmkVm.HasValue)))
                    {
                        var builderInsertResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().Insert();
                        if (car.Location != null && this.ValidateGeoLocation<GeoLocation>(car.Location, "IResStLevelCar", scope))
                        {
                            builderInsertResStLevelCar.SetValue(c => c.Agl, car.Location.AGL);
                            builderInsertResStLevelCar.SetValue(c => c.Altitude, car.Location.ASL);
                            builderInsertResStLevelCar.SetValue(c => c.Lon, car.Location.Lon);
                            builderInsertResStLevelCar.SetValue(c => c.Lat, car.Location.Lat);
                        }
                        if (car.DifferenceTimeStamp_ns.HasValue && (car.DifferenceTimeStamp_ns < 0 && car.DifferenceTimeStamp_ns > 999999999))
                            WriteLog($"Incorrect value DifferenceTimeStamp_ns: {car.DifferenceTimeStamp_ns}", "IResStLevelCar", scope);
                        builderInsertResStLevelCar.SetValue(c => c.DifferenceTimeStamp, car.DifferenceTimeStamp_ns);
                        builderInsertResStLevelCar.SetValue(c => c.LevelDbm, car.Level_dBm);
                        builderInsertResStLevelCar.SetValue(c => c.LevelDbmkvm, car.Level_dBmkVm);
                        builderInsertResStLevelCar.SetValue(c => c.TimeOfMeasurements, car.MeasurementTime);
                        builderInsertResStLevelCar.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation);
                        scope.Executor.Execute(builderInsertResStLevelCar);
                    }
                    else WriteLog($"Incorrect value of Level_dBmkVm: {car.Level_dBmkVm} or Level_dBmkVm: {car.Level_dBmkVm}", "IResStLevelCar", scope);
                }
            }
        }
        private void InsertBearing(long valInsResMeasStation, StationMeasResult station, IDataLayerScope scope)
        {
            if (station.Bearings != null)
            {
                foreach (DirectionFindingData directionFindingData in station.Bearings)
                {
                    var builderInsertBearing = this._dataLayer.GetBuilder<MD.IBearing>().Insert();
                    builderInsertBearing.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation);
                    if (directionFindingData.Location != null && this.ValidateGeoLocation<GeoLocation>(directionFindingData.Location, "IBearing", scope))
                    {
                        builderInsertBearing.SetValue(c => c.Agl, directionFindingData.Location.AGL);
                        builderInsertBearing.SetValue(c => c.Asl, directionFindingData.Location.ASL);
                        builderInsertBearing.SetValue(c => c.Lon, directionFindingData.Location.Lon);
                        builderInsertBearing.SetValue(c => c.Lat, directionFindingData.Location.Lat);
                    }
                    builderInsertBearing.SetValue(c => c.Level_dBm, directionFindingData.Level_dBm);
                    builderInsertBearing.SetValue(c => c.Level_dBmkVm, directionFindingData.Level_dBmkVm);
                    builderInsertBearing.SetValue(c => c.MeasurementTime, directionFindingData.MeasurementTime);
                    builderInsertBearing.SetValue(c => c.Quality, directionFindingData.Quality);
                    builderInsertBearing.SetValue(c => c.AntennaAzimut, directionFindingData.AntennaAzimut);
                    builderInsertBearing.SetValue(c => c.Bandwidth_kHz, directionFindingData.Bandwidth_kHz);
                    builderInsertBearing.SetValue(c => c.Bearing, directionFindingData.Bearing);
                    builderInsertBearing.SetValue(c => c.CentralFrequency_MHz, directionFindingData.CentralFrequency_MHz);
                    scope.Executor.Execute(builderInsertBearing);
                }
            }
        }

        private bool ValidateGeoLocation<T>(T location, string tableName, IDataLayerScope scope)
            where T : GeoLocation
        {
            bool result = true;
            if (!(location.Lon >= -180 && location.Lon <= 180))
            {
                WriteLog($"Incorrect value Lon {location.Lon}", tableName, scope);
                return false;
            }
            if (!(location.Lat >= -90 && location.Lat <= 90))
            {
                WriteLog($"Incorrect value Lat {location.Lat}", tableName, scope);
                return false;
            }
            if (location.ASL < -1000 || location.ASL > 9000)
            {
                WriteLog($"Incorrect value Asl {location.ASL}", tableName, scope);
            }
            if (location.AGL < -100 || location.AGL > 500)
            {
                WriteLog($"Incorrect value Agl {location.AGL}", tableName, scope);
            }
            return result;
        }
        private void GetIds(string ResultId, string TaskId, out int subMeasTaskId, out int subMeasTaskStaId, out int sensorId, out int resultId, IDataLayerScope scope)
        {
            subMeasTaskId = -1; subMeasTaskStaId = -1; sensorId = -1; resultId = -1;
            if (ResultId != null)
            {
                string[] word = ResultId.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if ((word != null) && (word.Length == 5))
                {
                    subMeasTaskId = int.Parse(word[1]);
                    subMeasTaskStaId = int.Parse(word[2]);
                    sensorId = int.Parse(word[3]);
                    resultId = int.Parse(word[4]);
                }
                else
                {
                    WriteLog("Incorrect value ResultId: " + ResultId, scope);
                }
            }
        }
        private void WriteLog(string msg, IDataLayerScope scope)
        {
            WriteLog(msg, "", scope);
        }
        private void WriteLog(string msg, string tableName, IDataLayerScope scope)
        {
            var builderInsertLog = this._dataLayer.GetBuilder<MD.IValidationLogs>().Insert();
            builderInsertLog.SetValue(c => c.TableName, tableName);
            builderInsertLog.SetValue(c => c.When, DateTime.Now);
            builderInsertLog.SetValue(c => c.Info, msg);
            scope.Executor.Execute(builderInsertLog);
        }
    }
}
