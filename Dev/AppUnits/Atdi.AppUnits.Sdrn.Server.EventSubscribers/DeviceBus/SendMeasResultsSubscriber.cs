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

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendMeasResultsDeviceBusEvent", SubscriberName = "SendMeasResultsSubscriber")]
    public class SendMeasResultsSubscriber : SubscriberBase<DM.MeasResults>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;
        public SendMeasResultsSubscriber(IEventEmitter eventEmitter, ISdrnMessagePublisher messagePublisher, IMessagesSite messagesSite, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger) : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.MeasResults deliveryObject)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                //var  status = SdrnMessageHandlingStatus.Unprocessed;
                bool isSuccessProcessed=false;
                var reasonFailure = "";
                try
                {
                    if (deliveryObject.Measurement == MeasurementType.SpectrumOccupation)
                    {
                        isSuccessProcessed = SaveMeasResultSpectrumOccupation(deliveryObject);
                    }
                    if (deliveryObject.Measurement == MeasurementType.MonitoringStations)
                    {
                        isSuccessProcessed = SaveMeasResultMonitoringStations(deliveryObject);
                    }
                    if (deliveryObject.Measurement == MeasurementType.Signaling)
                    {
                        isSuccessProcessed = SaveMeasResultSignaling(deliveryObject);
                    }
                    isSuccessProcessed = true;
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    //status = SdrnMessageHandlingStatus.Error;
                    reasonFailure = e.StackTrace;
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи о обновлении данных о сенсоре
                    //var deviceCommandResult = new DeviceCommand
                    //{
                    //    EquipmentTechId = sensorTechId,
                    //    SensorName = sensorName,
                    //    SdrnServer = this._environment.ServerInstance,
                    //    Command = "SendMeasResultsConfirmed",
                    //    CommandId = "SendCommand",
                    //    CustTxt1 = "Success"
                    //};

                    //if (status == SdrnMessageHandlingStatus.Error)
                    //{
                    //    deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"" + reasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    //}
                    //else if (isSuccessProcessed)
                    //{
                    //    deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Success\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    //}
                    //else
                    //{
                    //    deviceCommandResult.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + deliveryObject.ResultId + "\"", "\"Message\"", "\"" + reasonFailure + "\"", "\"DateCreated\"", "\"" + DateTime.Now.ToString("dd.MM.yyyyTHH:mm:ss") + "\"") + " }";
                    //}
                    //var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DeviceCommand>();
                    //envelop.SensorName = sensorName;
                    //envelop.SensorTechId = sensorTechId;
                    //envelop.DeliveryObject = deviceCommandResult;
                    //_messagePublisher.Send(envelop);
                }

            }
        }
        private bool SaveMeasResultSpectrumOccupation(DM.MeasResults measResult)
        {
            try
            {
                if (string.IsNullOrEmpty(measResult.ResultId))
                {
                    WriteLog("Undefined value ResultId", "IResMeas");
                    return false;
                }
                else if (measResult.ResultId.Length > 50)
                    measResult.ResultId.SubString(50);

                if (string.IsNullOrEmpty(measResult.TaskId))
                {
                    WriteLog("Undefined value TaskId", "IResMeas");
                    return false;
                }
                else if (measResult.TaskId.Length > 200)
                    measResult.TaskId.SubString(200);

                if (measResult.Status.Length > 5)
                    measResult.Status = "";

                if (!(measResult.SwNumber >= 0 && measResult.SwNumber <= 10000))
                    WriteLog("Incorrect value SwNumber", "IResMeas");

                if (!(measResult.ScansNumber >= 0 && measResult.ScansNumber <= 10000000))
                    WriteLog("Incorrect value ScansNumber", "IResMeas");

                if (measResult.StartTime > measResult.StopTime)
                    WriteLog("StartTime must be less than StopTime", "IResMeas");

                GetIds(measResult.ResultId, measResult.TaskId, out int subMeasTaskId, out int subMeasTaskStaId, out int sensorId, out int resultId);
                long valInsResMeas = 0;
                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, resultId.ToString());
                builderInsertIResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.SetValue(c => c.MEAS_SUB_TASK.Id, subMeasTaskId);
                builderInsertIResMeas.SetValue(c => c.MEAS_SUB_TASK_STATION.Id, subMeasTaskStaId);
                builderInsertIResMeas.SetValue(c => c.SENSOR.Id, sensorId);
                var pk = this._queryExecutor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);
                valInsResMeas = pk.Id;

                if (valInsResMeas > 0)
                {
                    if (measResult.FrequencySamples != null)
                    {
                        bool validationResult = true;
                        foreach (var freqSample in measResult.FrequencySamples)
                        {
                            if (freqSample.Occupation_Pt < 0 && freqSample.Occupation_Pt >= 100)
                            {
                                validationResult = false;
                                WriteLog("Incorrect value Occupation_Pt", "IFreqSample");
                            }
                            if (freqSample.Freq_MHz < 0 && freqSample.Freq_MHz >= 400000)
                            {
                                validationResult = false;
                                WriteLog("Incorrect value Freq_MHz", "IFreqSample");
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
                                this._queryExecutor.Execute(builderInsertResLevels);
                        }
                    }
                    if (this.ValidateGeoLocation(measResult.Location, "IResMeas"))
                    {
                        var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                        builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                        builderInsertResLocSensorMeas.SetValue(c => c.RES_MEAS.Id, valInsResMeas);
                        this._queryExecutor.Execute(builderInsertResLocSensorMeas);
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
        private bool SaveMeasResultMonitoringStations(DM.MeasResults measResult)
        {
            try
            {
                if (string.IsNullOrEmpty(measResult.ResultId))
                {
                    WriteLog("Undefined value ResultId", "IResMeas");
                    return false;
                }
                else if (measResult.ResultId.Length > 50)
                    measResult.ResultId.SubString(50);

                if (string.IsNullOrEmpty(measResult.TaskId))
                {
                    WriteLog("Undefined value TaskId", "IResMeas");
                    return false;
                }
                else if (measResult.TaskId.Length > 200)
                    measResult.TaskId.SubString(200);

                if (measResult.Status.Length > 5)
                    measResult.Status = "";

                if (!(measResult.SwNumber >= 0 && measResult.SwNumber <= 10000))
                    WriteLog("Incorrect value SwNumber", "IResMeas");

                if (measResult.StationResults == null || measResult.StationResults.Length == 0)
                {
                    WriteLog("Undefined values StationResults[]", "IResMeas");
                    return false;
                }
                if (measResult.Routes == null || measResult.Routes.Length == 0)
                {
                    WriteLog("Undefined values Routes[]", "IResMeas");
                    return false;
                }

                GetIds(measResult.ResultId, measResult.TaskId, out int subMeasTaskId, out int subMeasTaskStaId, out int sensorId, out int resultId);
                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, resultId != -1 ? resultId.ToString() : measResult.ResultId);
                builderInsertIResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.DataRank, measResult.SwNumber);
                builderInsertIResMeas.SetValue(c => c.MEAS_SUB_TASK.Id, subMeasTaskId);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.SetValue(c => c.MEAS_SUB_TASK_STATION.Id, subMeasTaskStaId);
                builderInsertIResMeas.SetValue(c => c.SENSOR.Id, measResult.SensorId != null ? (long)measResult.SensorId : (long)sensorId);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                var idResMeas = this._queryExecutor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);

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
                                    WriteLog("StartTime must be less than FinishTime", "IResRoutesRaw");

                                if (this.ValidateGeoLocation<RoutePoint>(routePoint, "IResRoutes"))
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
                                this._queryExecutor.Execute(builderInsertroutePoints);
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
                            WriteLog("MeasStartTime must be less than MeasFinishTime", "IResStGeneralRaw");
                        }

                        var builderInsertResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().Insert();
                        builderInsertResMeasStation.SetValue(c => c.Status, station.Status);
                        builderInsertResMeasStation.SetValue(c => c.MeasGlobalSID, station.RealGlobalSid);
                        builderInsertResMeasStation.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                        builderInsertResMeasStation.SetValue(c => c.RES_MEAS.Id, idResMeas.Id);
                        builderInsertResMeasStation.SetValue(c => c.Standard, station.Standard);
                        if (int.TryParse(station.StationId, out int Idstation))
                            builderInsertResMeasStation.SetValue(c => c.STATION.Id, Idstation);
                        if (int.TryParse(station.SectorId, out int IdSector))
                            builderInsertResMeasStation.SetValue(c => c.SECTOR.Id, IdSector);
                        builderInsertResMeasStation.Select(c => c.Id);
                        var valInsResMeasStation = this._queryExecutor.Execute<MD.IResMeasStation_PK>(builderInsertResMeasStation);

                        if (valInsResMeasStation.Id > 0)
                        {
                            var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Insert();
                            builderInsertLinkResSensor.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation.Id);
                            builderInsertLinkResSensor.SetValue(c => c.SENSOR.Id, (long)measResult.SensorId);
                            builderInsertLinkResSensor.Select(c => c.Id);
                            this._queryExecutor.Execute<MD.ILinkResSensor_PK>(builderInsertLinkResSensor);

                            var generalResult = station.GeneralResult;
                            if (generalResult != null)
                            {
                                var IDResGeneral = InsertResStGeneral(station, valInsResMeasStation.Id, generalResult);
                                if (IDResGeneral > 0)
                                {
                                    InsertResSysInfo(station, IDResGeneral);
                                    InsertResStMaskElement(station, IDResGeneral);
                                    InsertResStLevelCar(station, valInsResMeasStation.Id);
                                    InsertBearing(valInsResMeasStation.Id, station);
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
        private bool SaveMeasResultSignaling(DM.MeasResults measResult)
        {
            try
            {
                GetIds(measResult.ResultId, measResult.TaskId, out int subMeasTaskId, out int subMeasTaskStaId, out int sensorId, out int resultId);

                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, resultId.ToString());
                builderInsertIResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.SetValue(c => c.MEAS_SUB_TASK.Id, subMeasTaskId);
                builderInsertIResMeas.SetValue(c => c.MEAS_SUB_TASK_STATION.Id, subMeasTaskStaId);
                builderInsertIResMeas.SetValue(c => c.SENSOR.Id, sensorId);
                var valInsResMeas = this._queryExecutor.Execute<MD.IResMeas_PK>(builderInsertIResMeas);
                if (valInsResMeas.Id > 0)
                {
                    var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                    builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                    builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                    builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                    builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                    builderInsertResLocSensorMeas.SetValue(c => c.RES_MEAS.Id, valInsResMeas.Id);
                    this._queryExecutor.Execute(builderInsertResLocSensorMeas);

                    if (measResult.RefLevels != null)
                    {
                        var refLevels = measResult.RefLevels;
                        var builderInsertReferenceLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Insert();
                        builderInsertReferenceLevels.SetValue(c => c.StartFrequency_Hz, refLevels.StartFrequency_Hz);
                        builderInsertReferenceLevels.SetValue(c => c.StepFrequency_Hz, refLevels.StepFrequency_Hz);
                        if (refLevels.levels != null)
                        {
                            builderInsertReferenceLevels.SetValue(c => c.RefLevels, refLevels.levels);
                        }
                        builderInsertReferenceLevels.SetValue(c => c.RES_MEAS.Id, valInsResMeas.Id);
                        this._queryExecutor.Execute<MD.IReferenceLevels_PK>(builderInsertReferenceLevels);
                    }
                    if (measResult.Emittings != null)
                    {
                        foreach (Emitting emitting in measResult.Emittings)
                        {
                            var builderInsertEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Insert();
                            builderInsertEmitting.SetValue(c => c.CurentPower_dBm, emitting.CurentPower_dBm);
                            builderInsertEmitting.SetValue(c => c.MeanDeviationFromReference, emitting.MeanDeviationFromReference);
                            builderInsertEmitting.SetValue(c => c.ReferenceLevel_dBm, emitting.ReferenceLevel_dBm);
                            builderInsertEmitting.SetValue(c => c.RES_MEAS.Id, valInsResMeas.Id);
                            builderInsertEmitting.SetValue(c => c.SensorId, emitting.SensorId);
                            if (emitting.EmittingParameters != null)
                            {
                                builderInsertEmitting.SetValue(c => c.RollOffFactor, emitting.EmittingParameters.RollOffFactor);
                                builderInsertEmitting.SetValue(c => c.StandardBW, emitting.EmittingParameters.StandardBW);
                            }
                            builderInsertEmitting.SetValue(c => c.StartFrequency_MHz, emitting.StartFrequency_MHz);
                            builderInsertEmitting.SetValue(c => c.StopFrequency_MHz, emitting.StopFrequency_MHz);
                            builderInsertEmitting.SetValue(c => c.TriggerDeviationFromReference, emitting.TriggerDeviationFromReference);
                            var levelsDistribution = emitting.LevelsDistribution;
                            if (levelsDistribution != null)
                            {
                                builderInsertEmitting.SetValue(c => c.LevelsDistributionCount, levelsDistribution.Count);
                                builderInsertEmitting.SetValue(c => c.LevelsDistributionLvl, levelsDistribution.Levels);
                            }
                            builderInsertEmitting.SetValue(c => c.Loss_dB, emitting.SignalMask.Loss_dB);
                            builderInsertEmitting.SetValue(c => c.Freq_kHz, emitting.SignalMask.Freq_kHz);
                            var valInsReferenceEmitting = this._queryExecutor.Execute<MD.IEmitting_PK>(builderInsertEmitting);

                            if (valInsReferenceEmitting.Id > 0)
                            {
                                var workTimes = emitting.WorkTimes;
                                if (workTimes != null)
                                {
                                    foreach (WorkTime workTime in workTimes)
                                    {
                                        var builderInsertIWorkTime = this._dataLayer.GetBuilder<MD.IWorkTime>().Insert();
                                        builderInsertIWorkTime.SetValue(c => c.EmittingId, valInsReferenceEmitting.Id);
                                        builderInsertIWorkTime.SetValue(c => c.HitCount, workTime.HitCount);
                                        builderInsertIWorkTime.SetValue(c => c.PersentAvailability, workTime.PersentAvailability);
                                        builderInsertIWorkTime.SetValue(c => c.StartEmitting, workTime.StartEmitting);
                                        builderInsertIWorkTime.SetValue(c => c.StopEmitting, workTime.StopEmitting);
                                        this._queryExecutor.Execute(builderInsertIWorkTime);
                                    }
                                }
                                var spectrum = emitting.Spectrum;
                                if (spectrum != null)
                                {
                                    var builderInsertISpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>().Insert();
                                    builderInsertISpectrum.SetValue(c => c.EMITTING.Id, valInsReferenceEmitting.Id);
                                    builderInsertISpectrum.SetValue(c => c.CorrectnessEstimations, spectrum.СorrectnessEstimations == true ? 1 : 0);
                                    builderInsertISpectrum.SetValue(c => c.Contravention, spectrum.Contravention == true ? 1 : 0);
                                    builderInsertISpectrum.SetValue(c => c.Bandwidth_kHz, spectrum.Bandwidth_kHz);
                                    builderInsertISpectrum.SetValue(c => c.MarkerIndex, spectrum.MarkerIndex);
                                    builderInsertISpectrum.SetValue(c => c.SignalLevel_dBm, spectrum.SignalLevel_dBm);
                                    builderInsertISpectrum.SetValue(c => c.SpectrumStartFreq_MHz, spectrum.SpectrumStartFreq_MHz);
                                    builderInsertISpectrum.SetValue(c => c.SpectrumSteps_kHz, spectrum.SpectrumSteps_kHz);
                                    builderInsertISpectrum.SetValue(c => c.T1, spectrum.T1);
                                    builderInsertISpectrum.SetValue(c => c.T2, spectrum.T2);
                                    builderInsertISpectrum.SetValue(c => c.TraceCount, spectrum.TraceCount);
                                    builderInsertISpectrum.SetValue(c => c.Levels_dBm, spectrum.Levels_dBm);
                                    this._queryExecutor.Execute<MD.ISpectrum_PK>(builderInsertISpectrum);
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
        private long InsertResStGeneral(StationMeasResult station, long valInsResMeasStation, GeneralMeasResult generalResult)
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
                    if (!(bandwidthResult.T1.Value >= 0 && bandwidthResult.T1.Value <= bandwidthResult.MarkerIndex.Value
                        && bandwidthResult.T2.Value >= bandwidthResult.MarkerIndex.Value && bandwidthResult.T2.Value <= 100000
                        && bandwidthResult.MarkerIndex.Value >= bandwidthResult.T1.Value && bandwidthResult.MarkerIndex.Value <= bandwidthResult.T2.Value))
                    {
                        if (bandwidthResult.Bandwidth_kHz.HasValue && bandwidthResult.Bandwidth_kHz >= 1 && bandwidthResult.Bandwidth_kHz <= 100000)
                            builderInsertResStGeneral.SetValue(c => c.BW, bandwidthResult.Bandwidth_kHz);
                        builderInsertResStGeneral.SetValue(c => c.MarkerIndex, bandwidthResult.MarkerIndex);
                        builderInsertResStGeneral.SetValue(c => c.T1, bandwidthResult.T1);
                        builderInsertResStGeneral.SetValue(c => c.T2, bandwidthResult.T2);
                        if (bandwidthResult.TraceCount >= 1 && bandwidthResult.TraceCount <= 100000)
                        {
                            WriteLog("Incorrect value TraceCount", "IResStGeneral");
                        }
                        builderInsertResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                        builderInsertResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations == true ? 1 : 0);
                    }
                }
            }
            builderInsertResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
            builderInsertResStGeneral.SetValue(c => c.SpecrumStartFreq, generalResult.SpectrumStartFreq_MHz);
            builderInsertResStGeneral.SetValue(c => c.SpecrumSteps, generalResult.SpectrumSteps_kHz);
            builderInsertResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
            builderInsertResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
            builderInsertResStGeneral.SetValue(c => c.LevelsSpectrumdBm, station.GeneralResult.LevelsSpectrum_dBm);
            builderInsertResStGeneral.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation);
            builderInsertResStGeneral.Select(c => c.Id);
            var IDResGeneral = this._queryExecutor.Execute<MD.IResStGeneral_PK>(builderInsertResStGeneral);
            return IDResGeneral.Id;
        }
        private void InsertResSysInfo(StationMeasResult station, long IDResGeneral)
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
                var IDResSysInfoGeneral = this._queryExecutor.Execute<MD.IResSysInfo_PK>(builderInsertResSysInfo);
                if (IDResSysInfoGeneral.Id > 0 && stationSysInfo.InfoBlocks != null)
                {
                    foreach (StationSysInfoBlock blocks in stationSysInfo.InfoBlocks)
                    {
                        var builderInsertStationSysInfoBlock = this._dataLayer.GetBuilder<MD.IResSysInfoBlocks>().Insert();
                        builderInsertStationSysInfoBlock.SetValue(c => c.Data, blocks.Data);
                        builderInsertStationSysInfoBlock.SetValue(c => c.Type, blocks.Type);
                        builderInsertStationSysInfoBlock.SetValue(c => c.RES_SYS_INFO.Id, IDResSysInfoGeneral.Id);
                        this._queryExecutor.Execute<MD.IResSysInfoBlocks_PK>(builderInsertStationSysInfoBlock);
                    }
                }
            }
        }
        private void InsertResStMaskElement(StationMeasResult station, long IDResGeneral)
        {
            if (station.GeneralResult.BWMask != null)
            {
                foreach (ElementsMask maskElem in station.GeneralResult.BWMask)
                {
                    if (maskElem.Level_dB.HasValue  && maskElem.Level_dB.Value >= -300 && maskElem.Level_dB.Value <= 300
                        && maskElem.BW_kHz.HasValue && maskElem.BW_kHz.Value >= 1 && maskElem.BW_kHz.Value <= 200000)
                    {
                        var builderInsertmaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElement>().Insert();
                        builderInsertmaskElem.SetValue(c => c.Bw, maskElem.BW_kHz);
                        builderInsertmaskElem.SetValue(c => c.Level, maskElem.Level_dB);
                        builderInsertmaskElem.SetValue(c => c.RES_STGENERAL.Id, IDResGeneral);
                        this._queryExecutor.Execute(builderInsertmaskElem);
                    }
                }
            }
        }
        private void InsertResStLevelCar(StationMeasResult station, long valInsResMeasStation)
        {
            if (station.LevelResults != null)
            {
                foreach (LevelMeasResult car in station.LevelResults)
                {
                    if (car.Level_dBm.HasValue && car.Level_dBm >= -150 && car.Level_dBm <= 20
                        && car.Level_dBmkVm.HasValue && car.Level_dBmkVm >= -10 && car.Level_dBmkVm <= 140)
                    {
                        var builderInsertResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().Insert();
                        if (car.Location != null && this.ValidateGeoLocation<GeoLocation>(car.Location, "IResStLevelCar"))
                        {
                            builderInsertResStLevelCar.SetValue(c => c.Agl, car.Location.AGL);
                            builderInsertResStLevelCar.SetValue(c => c.Altitude, car.Location.ASL);
                            builderInsertResStLevelCar.SetValue(c => c.Lon, car.Location.Lon);
                            builderInsertResStLevelCar.SetValue(c => c.Lat, car.Location.Lat);
                        }
                        if (car.DifferenceTimeStamp_ns.HasValue && (car.DifferenceTimeStamp_ns < 0 && car.DifferenceTimeStamp_ns > 999999999))
                            WriteLog("Incorrect value DifferenceTimeStamp_ns", "IResStLevelCar");
                        builderInsertResStLevelCar.SetValue(c => c.DifferenceTimeStamp, car.DifferenceTimeStamp_ns);
                        builderInsertResStLevelCar.SetValue(c => c.LevelDbm, car.Level_dBm);
                        builderInsertResStLevelCar.SetValue(c => c.LevelDbmkvm, car.Level_dBmkVm);
                        builderInsertResStLevelCar.SetValue(c => c.TimeOfMeasurements, car.MeasurementTime);
                        builderInsertResStLevelCar.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation);
                        this._queryExecutor.Execute(builderInsertResStLevelCar);
                    }
                }
            }
        }
        private void InsertBearing(long valInsResMeasStation, StationMeasResult station)
        {
            if (station.Bearings != null)
            {
                foreach (DirectionFindingData directionFindingData in station.Bearings)
                {
                    var builderInsertBearing = this._dataLayer.GetBuilder<MD.IBearing>().Insert();
                    builderInsertBearing.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation);
                    if (directionFindingData.Location != null && this.ValidateGeoLocation<GeoLocation>(directionFindingData.Location, "IBearing"))
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
                    this._queryExecutor.Execute(builderInsertBearing);
                }
            }
        }

        private bool ValidateGeoLocation<T>(T location, string tableName)
            where T : GeoLocation
        {
            bool result = true;
            if (!(location.Lon >= -180 && location.Lon <= 180))
            {
                WriteLog("Incorrect value Lon", tableName);
                return false;
            }
            if (!(location.Lat >= -90 && location.Lat <= 90))
            {
                WriteLog("Incorrect value Lat", tableName);
                return false;
            }
            if (location.ASL < -1000 || location.ASL > 9000)
            {
                WriteLog("Incorrect value Asl", tableName);
            }
            if (location.AGL < -100 || location.AGL > 500)
            {
                WriteLog("Incorrect value Agl", tableName);
            }
            return result;
        }
        private void GetIds(string ResultId, string TaskId, out int subMeasTaskId, out int subMeasTaskStaId, out int sensorId, out int resultId)
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
                    WriteLog("Incorrect value ResultId: " + ResultId);
                }
            }
        }
        private void WriteLog(string msg)
        {
            WriteLog(msg, "");
        }
        private void WriteLog(string msg, string tableName)
        {
            using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
            {
                var builderInsertLog = this._dataLayer.GetBuilder<MD.IValidationLogs>().Insert();
                builderInsertLog.SetValue(c => c.TableName, tableName);
                builderInsertLog.SetValue(c => c.When, DateTime.Now);
                builderInsertLog.SetValue(c => c.Who, "");
                builderInsertLog.SetValue(c => c.Lcount, 1);
                builderInsertLog.SetValue(c => c.Info, msg);
                builderInsertLog.SetValue(c => c.Event, "");
                builderInsertLog.Select(c => c.Id);
                scope.Executor.Execute(builderInsertLog);
            } 
        }
    }
}
