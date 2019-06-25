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
        public SendMeasResultsSubscriber(IEventEmitter eventEmitter, ISdrnMessagePublisher messagePublisher, IMessagesSite messagesSite, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger) : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.MeasResults deliveryObject)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                var  status = SdrnMessageHandlingStatus.Unprocessed;
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
                        isSuccessProcessed = SaveMeasResultSignaling(deliveryObject, out long newResMeasId, out int newResSensorId);
                        //    validationResult = VaildateMeasResultSignaling(deliveryObject);
                        //    if (validationResult)
                        //    {
                        //        //if (SaveMeasResultSignaling(deliveryObject))
                        //        //{
                        //        //    DeleteOldMeasResultSignaling(deliveryObject, newResMeasId, newResSensorId);
                        //        //}
                        //    }
                    }


                    isSuccessProcessed = true;
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    status = SdrnMessageHandlingStatus.Error;
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
        private bool SaveMeasResultSpectrumOccupation(DM.MeasResults measResult)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
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

                queryExecuter.BeginTransaction();

                int subMeasTaskId = -1; int subMeasTaskStaId = -1; int sensorId = -1; int resultId = -1;
                GetIds(measResult.ResultId, measResult.TaskId, out subMeasTaskId, out subMeasTaskStaId, out sensorId, out resultId);

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
                builderInsertIResMeas.SetValue(c => c.MeasSubTaskId, subMeasTaskId);
                builderInsertIResMeas.SetValue(c => c.MeasSubTaskStationId, subMeasTaskStaId);
                builderInsertIResMeas.SetValue(c => c.SensorId, sensorId);
                builderInsertIResMeas.Select(c => c.Id);
                queryExecuter.ExecuteAndFetch(builderInsertIResMeas, reader =>
                {
                    var res = reader.Read();
                    if (res)
                    {
                        valInsResMeas = reader.GetValue(c => c.Id);
                    }
                    return res;
                });

                if (valInsResMeas > 0)
                {
                    if (measResult.FrequencySamples != null)
                    {
                        bool validationResult = true;
                        var lstIns = new List<IQueryInsertStatement<MD.IResLevels>>();
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
                            builderInsertResLevels.SetValue(c => c.ResMeasId, valInsResMeas);
                            builderInsertResLevels.Select(c => c.Id);
                            if (validationResult)
                            {
                                lstIns.Add(builderInsertResLevels);
                            }
                        }
                        queryExecuter.ExecuteAndFetch(lstIns.ToArray(), reader =>
                        {
                            return true;
                        });
                    }

                    if (this.ValidateGeoLocation(measResult.Location, "IResMeas"))
                    {
                        var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                        builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                        builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                        builderInsertResLocSensorMeas.SetValue(c => c.ResMeasId, valInsResMeas);
                        builderInsertResLocSensorMeas.Select(c => c.Id);
                        queryExecuter.Execute(builderInsertResLocSensorMeas);
                    }
                }

                queryExecuter.CommitTransaction();
                return true;
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                queryExecuter.RollbackTransaction();
                return false;
            }
        }
        private bool SaveMeasResultMonitoringStations(DM.MeasResults measResult)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
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

                var listStationIdsTemp = new List<long>();
                var dic = new Dictionary<string, string>();
                queryExecuter.BeginTransaction();

                long idResMeas = 0;
                bool isMerge = false;
                double? diffDates = null;

                int subMeasTaskId = -1; int subMeasTaskStaId = -1; int sensorId = -1; int resultId = -1;
                GetIds(measResult.ResultId, measResult.TaskId, out subMeasTaskId, out subMeasTaskStaId, out sensorId, out resultId);

                var builderResMeasSearch = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                builderResMeasSearch.Select(c => c.Id, c => c.TimeMeas, c => c.DataRank);
                builderResMeasSearch.OrderByAsc(c => c.Id);
                builderResMeasSearch.Where(c => c.MeasTaskId, ConditionOperator.Equal, measResult.TaskId);
                builderResMeasSearch.Where(c => c.Status, ConditionOperator.Equal, "N");
                builderResMeasSearch.Where(c => c.TimeMeas, ConditionOperator.Between, new DateTime?[] { measResult.Measured.AddHours(-1), measResult.Measured.AddHours(1) });
                queryExecuter.Fetch(builderResMeasSearch, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        var dataRank = readerResMeas.GetValue(c => c.DataRank);
                        if (!dataRank.HasValue || dataRank.Value == measResult.SwNumber)
                        {
                            var timeMeas = readerResMeas.GetValue(c => c.TimeMeas);
                            if (diffDates == null || diffDates > Math.Abs((timeMeas.Value - measResult.Measured).TotalMilliseconds))
                            {
                                diffDates = Math.Abs((timeMeas.Value - measResult.Measured).TotalMilliseconds);
                                idResMeas = readerResMeas.GetValue(c => c.Id);
                                isMerge = true;
                            }
                        }
                    }
                    return true;
                });

                if (isMerge)
                {
                    var builderUpdateResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Update();
                    builderUpdateResMeas.SetValue(c => c.MeasResultSID, resultId != -1 ? resultId.ToString() : measResult.ResultId);
                    builderUpdateResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                    builderUpdateResMeas.SetValue(c => c.Status, measResult.Status);
                    builderUpdateResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                    builderUpdateResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                    builderUpdateResMeas.SetValue(c => c.DataRank, measResult.SwNumber);
                    builderUpdateResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                    builderUpdateResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                    builderUpdateResMeas.Where(c => c.Id, ConditionOperator.Equal, idResMeas);
                    queryExecuter.Execute(builderUpdateResMeas);
                }
                else
                {
                    var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                    builderInsertIResMeas.SetValue(c => c.MeasResultSID, resultId != -1 ? resultId.ToString() : measResult.ResultId);
                    builderInsertIResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                    builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                    builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                    builderInsertIResMeas.SetValue(c => c.DataRank, measResult.SwNumber);
                    builderInsertIResMeas.SetValue(c => c.MeasSubTaskId, subMeasTaskId);
                    builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                    builderInsertIResMeas.SetValue(c => c.MeasSubTaskStationId, subMeasTaskStaId);
                    builderInsertIResMeas.SetValue(c => c.SensorId, measResult.SensorId != null ? measResult.SensorId : sensorId);
                    builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                    builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                    builderInsertIResMeas.Select(c => c.Id);
                    queryExecuter.ExecuteAndFetch(builderInsertIResMeas, reader =>
                    {
                        var res = reader.Read();
                        if (res)
                            idResMeas = reader.GetValue(c => c.Id);
                        return res;
                    });
                }

                if (idResMeas > 0)
                {
                    var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Insert();
                    builderInsertLinkResSensor.SetValue(c => c.ResMeasStaId, idResMeas);
                    builderInsertLinkResSensor.SetValue(c => c.SensorId, measResult.SensorId);
                    builderInsertLinkResSensor.Select(c => c.Id);
                    queryExecuter.ExecuteAndFetch(builderInsertLinkResSensor, reader => { return true; });
                }

                if (measResult.Routes != null)
                {
                    foreach (Route route in measResult.Routes)
                    {
                        if (route.RoutePoints != null)
                        {
                            var lstIns = new List<IQueryInsertStatement<MD.IResRoutes>>();
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
                                builderInsertroutePoints.SetValue(c => c.ResMeasId, idResMeas);
                                builderInsertroutePoints.Select(c => c.Id);
                                lstIns.Add(builderInsertroutePoints);
                            }
                            queryExecuter.ExecuteAndFetch(lstIns.ToArray(), reader => { return true; });
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

                        if (isMerge)
                        {
                            long idMeasResultStation = 0;
                            long idMeasResultGeneral = 0;
                            DateTime? measStartTime = null;
                            bool isMergeStation = false;
                            DateTime? startTime = null;
                            DateTime? finishTime = null;

                            if ((!string.IsNullOrEmpty(station.RealGlobalSid)) && !dic.ContainsKey(station.RealGlobalSid + @"/|\" + station.Standard))
                            {
                                dic.Add(station.RealGlobalSid + @"/|\" + station.Standard, "");

                                var builderResMeasStationSearch = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                                builderResMeasStationSearch.Select(c => c.Id);
                                builderResMeasStationSearch.Where(c => c.MeasGlobalSID, ConditionOperator.Equal, station.RealGlobalSid);
                                builderResMeasStationSearch.Where(c => c.Standard, ConditionOperator.Equal, station.Standard);
                                queryExecuter.Fetch(builderResMeasStationSearch, readerResMeasStation =>
                                {
                                    while (readerResMeasStation.Read())
                                    {
                                        var builderGeneralResultSearch = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                                        builderGeneralResultSearch.Select(c => c.CentralFrequency, c => c.CentralFrequencyMeas, c => c.TimeStartMeas, c => c.TimeFinishMeas, c => c.Id);
                                        builderGeneralResultSearch.Where(c => c.ResMeasStaId, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                                        queryExecuter.Fetch(builderGeneralResultSearch, readerGeneralResult =>
                                        {
                                            int itemCount = station.GeneralResult.LevelsSpectrum_dBm.Length;
                                            while (readerGeneralResult.Read())
                                            {
                                                var centralFrequency = readerGeneralResult.GetValue(c => c.CentralFrequency);
                                                var centralFrequencyMeas = readerGeneralResult.GetValue(c => c.CentralFrequencyMeas);
                                                var timeStartMeas = readerGeneralResult.GetValue(c => c.TimeStartMeas);

                                                if ((centralFrequency.HasValue && station.GeneralResult.CentralFrequency_MHz.HasValue && centralFrequency.Value == station.GeneralResult.CentralFrequency_MHz.Value)
                                                    || (centralFrequencyMeas.HasValue && station.GeneralResult.CentralFrequencyMeas_MHz.HasValue && Math.Abs(centralFrequencyMeas.Value - station.GeneralResult.CentralFrequencyMeas_MHz.Value) <= 0.005)
                                                    || (!centralFrequency.HasValue && !station.GeneralResult.CentralFrequency_MHz.HasValue && !centralFrequencyMeas.HasValue && !station.GeneralResult.CentralFrequencyMeas_MHz.HasValue))
                                                {
                                                    if (!measStartTime.HasValue || measStartTime.Value > timeStartMeas || idMeasResultStation == 0)
                                                    {
                                                        if (itemCount == 0 || station.GeneralResult == null)
                                                        {
                                                            idMeasResultStation = readerResMeasStation.GetValue(c => c.Id);
                                                            startTime = timeStartMeas;
                                                            finishTime = readerGeneralResult.GetValue(c => c.TimeFinishMeas);
                                                            idMeasResultGeneral = readerGeneralResult.GetValue(c => c.Id);
                                                            isMergeStation = true;
                                                        }
                                                    }
                                                }
                                            }
                                            return true;
                                        });
                                    }
                                    return true;
                                });
                            }

                            if (isMergeStation)
                            {
                                InsertResStLevelCar(queryExecuter, station, idMeasResultStation);
                                InsertBearing(queryExecuter, idMeasResultStation, station);

                                bool isUpdate = false;

                                var builderUpdateMeasResult = this._dataLayer.GetBuilder<MD.IResMeasStation>().Update();
                                if (!string.IsNullOrEmpty(station.StationId) && long.TryParse(station.StationId, out long Idstation))
                                {
                                    builderUpdateMeasResult.SetValue(c => c.StationId, Idstation);
                                    isUpdate = true;
                                }
                                if (!string.IsNullOrEmpty(station.SectorId) && long.TryParse(station.SectorId, out long IdSector))
                                {
                                    builderUpdateMeasResult.SetValue(c => c.SectorId, IdSector);
                                    isUpdate = true;
                                }
                                if (!string.IsNullOrEmpty(station.TaskGlobalSid))
                                {
                                    builderUpdateMeasResult.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                                    isUpdate = true;
                                }

                                if (!string.IsNullOrEmpty(station.Status))
                                {
                                    builderUpdateMeasResult.SetValue(c => c.Status, station.Status);
                                    isUpdate = true;
                                }

                                if (isUpdate)
                                {
                                    builderUpdateMeasResult.Where(c => c.Id, ConditionOperator.Equal, idMeasResultStation);
                                    queryExecuter.Execute(builderUpdateMeasResult);
                                    isUpdate = false;
                                }

                                var generalResult = station.GeneralResult;
                                if (generalResult != null)
                                {
                                    var builderUpdateResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().Update();
                                    if (generalResult.RBW_kHz.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.Rbw, generalResult.RBW_kHz);
                                    if (generalResult.VBW_kHz.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.Vbw, generalResult.VBW_kHz);
                                    if (generalResult.CentralFrequencyMeas_MHz.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.CentralFrequencyMeas, generalResult.CentralFrequencyMeas_MHz);
                                    if (generalResult.CentralFrequency_MHz.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.CentralFrequency, generalResult.CentralFrequency_MHz);
                                    if (generalResult.MeasDuration_sec.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.DurationMeas, generalResult.MeasDuration_sec);
                                    if (generalResult.BandwidthResult != null)
                                    {
                                        var bandwidthResult = generalResult.BandwidthResult;
                                        if (bandwidthResult.Bandwidth_kHz.HasValue)
                                            builderUpdateResStGeneral.SetValue(c => c.BW, bandwidthResult.Bandwidth_kHz);
                                        if (bandwidthResult.MarkerIndex.HasValue)
                                            builderUpdateResStGeneral.SetValue(c => c.MarkerIndex, bandwidthResult.MarkerIndex);
                                        if (bandwidthResult.T1.HasValue)
                                            builderUpdateResStGeneral.SetValue(c => c.T1, bandwidthResult.T1);
                                        if (bandwidthResult.T2.HasValue)
                                            builderUpdateResStGeneral.SetValue(c => c.T2, bandwidthResult.T2);
                                        builderUpdateResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                                        if (bandwidthResult.СorrectnessEstimations.HasValue)
                                            builderUpdateResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations == true ? 1 : 0);
                                    }
                                    if (generalResult.OffsetFrequency_mk.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
                                    if (generalResult.SpectrumStartFreq_MHz.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.SpecrumStartFreq, (double?)generalResult.SpectrumStartFreq_MHz);
                                    if (generalResult.SpectrumSteps_kHz.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.SpecrumSteps, (double?)generalResult.SpectrumSteps_kHz);
                                    if (generalResult.MeasStartTime.HasValue && startTime.HasValue && generalResult.MeasStartTime.Value < startTime)
                                        builderUpdateResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
                                    if (generalResult.MeasFinishTime.HasValue && finishTime.HasValue && generalResult.MeasFinishTime.Value > finishTime)
                                        builderUpdateResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
                                    builderUpdateResStGeneral.SetValue(c => c.ResMeasStaId, idMeasResultStation);
                                    builderUpdateResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, idMeasResultStation);
                                    queryExecuter.Execute(builderUpdateResStGeneral);
                                }

                                InsertResStMaskElement(queryExecuter, station, idMeasResultGeneral, true);
                                if (station.GeneralResult.StationSysInfo != null)
                                {
                                    var builderDelSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>().Delete();
                                    builderDelSysInfo.Where(c => c.ResStGeneralId, ConditionOperator.Equal, idMeasResultGeneral);
                                    queryExecuter.Execute(builderDelSysInfo);

                                    InsertResSysInfo(queryExecuter, station, idMeasResultGeneral, -1);
                                }
                            }
                            else
                            {
                                var listStationIds = new List<long>();
                                long valInsResMeasStation = 0;

                                var builderResMeasStationSearch = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                                builderResMeasStationSearch.Select(c => c.Id);
                                builderResMeasStationSearch.Where(c => c.MeasGlobalSID, ConditionOperator.Equal, station.RealGlobalSid);
                                builderResMeasStationSearch.Where(c => c.Standard, ConditionOperator.Equal, station.Standard);
                                queryExecuter.Fetch(builderResMeasStationSearch, readerResMeasStation =>
                                {
                                    while (readerResMeasStation.Read())
                                    {
                                        valInsResMeasStation = readerResMeasStation.GetValue(c => c.Id);
                                        if (!listStationIdsTemp.Contains(valInsResMeasStation))
                                        {
                                            listStationIds.Add(valInsResMeasStation);
                                            listStationIdsTemp.Add(valInsResMeasStation);
                                        }
                                    }
                                    return true;
                                });

                                if (listStationIds.Count == 0)
                                {
                                    var builderInsertResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().Insert();
                                    builderInsertResMeasStation.SetValue(c => c.Status, station.Status);
                                    builderInsertResMeasStation.SetValue(c => c.MeasGlobalSID, station.RealGlobalSid);
                                    builderInsertResMeasStation.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                                    builderInsertResMeasStation.SetValue(c => c.ResMeasId, idResMeas);
                                    builderInsertResMeasStation.SetValue(c => c.Standard, station.Standard);
                                    if (int.TryParse(station.StationId, out int Idstation))
                                        builderInsertResMeasStation.SetValue(c => c.StationId, Idstation);
                                    if (int.TryParse(station.SectorId, out int IdSector))
                                        builderInsertResMeasStation.SetValue(c => c.SectorId, IdSector);
                                    builderInsertResMeasStation.Select(c => c.Id);
                                    queryExecuter.ExecuteAndFetch(builderInsertResMeasStation, reader =>
                                    {
                                        var res = reader.Read();
                                        if (res)
                                        {
                                            valInsResMeasStation = reader.GetValue(c => c.Id);
                                            if (!listStationIdsTemp.Contains(valInsResMeasStation))
                                            {
                                                listStationIds.Add(valInsResMeasStation);
                                                listStationIdsTemp.Add(valInsResMeasStation);
                                            }
                                        }
                                        return res;
                                    });
                                }

                                if (listStationIds.Count > 0)
                                {
                                    for (int p = 0; p < listStationIds.Count; p++)
                                    {
                                        valInsResMeasStation = listStationIds[p];
                                        long idLinkRes = -1;

                                        var builderLinkResSensorRaw = this._dataLayer.GetBuilder<MD.ILinkResSensor>().From();
                                        builderLinkResSensorRaw.Select(c => c.Id);
                                        builderLinkResSensorRaw.Where(c => c.ResMeasStaId, ConditionOperator.Equal, valInsResMeasStation);
                                        builderLinkResSensorRaw.Where(c => c.SensorId, ConditionOperator.Equal, measResult.SensorId);
                                        queryExecuter.Fetch(builderLinkResSensorRaw, readerLinkResSensorRaw =>
                                        {
                                            while (readerLinkResSensorRaw.Read())
                                            {
                                                idLinkRes = readerLinkResSensorRaw.GetValue(c => c.Id);
                                                break;
                                            }
                                            return true;
                                        });

                                        if (idLinkRes == -1)
                                        {
                                            var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Insert();
                                            builderInsertLinkResSensor.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                            builderInsertLinkResSensor.SetValue(c => c.SensorId, measResult.SensorId);
                                            builderInsertLinkResSensor.Select(c => c.Id);
                                            queryExecuter.ExecuteAndFetch(builderInsertLinkResSensor, reader =>
                                            {
                                                var res = reader.Read();
                                                if (res)
                                                {
                                                    idLinkRes = reader.GetValue(c => c.Id);
                                                }
                                                return res;
                                            });
                                        }

                                        var generalResult = station.GeneralResult;
                                        if (generalResult != null)
                                        {
                                            long IDResGeneral = -1;
                                            var builderIResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                                            builderIResStGeneral.Select(c => c.Id);
                                            builderIResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, valInsResMeasStation);
                                            queryExecuter.Fetch(builderIResStGeneral, readerResStGeneral =>
                                            {
                                                while (readerResStGeneral.Read())
                                                {
                                                    IDResGeneral = readerResStGeneral.GetValue(c => c.Id);
                                                    break;
                                                }
                                                return true;
                                            });
                                            if (IDResGeneral == -1)
                                            {
                                                IDResGeneral = InsertResStGeneral(queryExecuter, station, valInsResMeasStation, generalResult, IDResGeneral);

                                                if (IDResGeneral > -1)
                                                {
                                                    long IDResSysInfoGeneral = -1;
                                                    var builderResSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>().From();
                                                    builderResSysInfo.Select(c => c.Id);
                                                    builderResSysInfo.Where(c => c.ResStGeneralId, ConditionOperator.Equal, IDResGeneral);
                                                    queryExecuter.Fetch(builderResSysInfo, readerIResSysInfo =>
                                                    {
                                                        while (readerIResSysInfo.Read())
                                                        {
                                                            IDResSysInfoGeneral = readerIResSysInfo.GetValue(c => c.Id);
                                                            break;
                                                        }
                                                        return true;
                                                    });

                                                    IDResSysInfoGeneral = InsertResSysInfo(queryExecuter, station, IDResGeneral, IDResSysInfoGeneral);
                                                    InsertResStMaskElement(queryExecuter, station, IDResGeneral, false);
                                                    InsertResStLevelCar(queryExecuter, station, valInsResMeasStation);
                                                    InsertBearing(queryExecuter, valInsResMeasStation, station);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            long valInsResMeasStation = 0;
                            var builderInsertResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().Insert();
                            builderInsertResMeasStation.SetValue(c => c.Status, station.Status);
                            builderInsertResMeasStation.SetValue(c => c.MeasGlobalSID, station.RealGlobalSid);
                            builderInsertResMeasStation.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                            builderInsertResMeasStation.SetValue(c => c.ResMeasId, idResMeas);
                            builderInsertResMeasStation.SetValue(c => c.Standard, station.Standard);
                            if (int.TryParse(station.StationId, out int Idstation))
                                builderInsertResMeasStation.SetValue(c => c.StationId, Idstation);
                            if (int.TryParse(station.SectorId, out int IdSector))
                                builderInsertResMeasStation.SetValue(c => c.SectorId, IdSector);
                            builderInsertResMeasStation.Select(c => c.Id);
                            queryExecuter.ExecuteAndFetch(builderInsertResMeasStation, reader =>
                            {
                                var res = reader.Read();
                                if (res)
                                    valInsResMeasStation = reader.GetValue(c => c.Id);
                                return res;
                            });

                            if (valInsResMeasStation > 0)
                            {
                                long idLinkRes = -1;
                                var builderLinkResSensorRaw = this._dataLayer.GetBuilder<MD.ILinkResSensor>().From();
                                builderLinkResSensorRaw.Select(c => c.Id);
                                builderLinkResSensorRaw.Where(c => c.ResMeasStaId, ConditionOperator.Equal, valInsResMeasStation);
                                builderLinkResSensorRaw.Where(c => c.SensorId, ConditionOperator.Equal, measResult.SensorId);
                                queryExecuter.Fetch(builderLinkResSensorRaw, readerLinkResSensorRaw =>
                                {
                                    while (readerLinkResSensorRaw.Read())
                                    {
                                        idLinkRes = readerLinkResSensorRaw.GetValue(c => c.Id);
                                        break;
                                    }
                                    return true;
                                });

                                if (idLinkRes == -1)
                                {
                                    var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Insert();
                                    builderInsertLinkResSensor.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                    builderInsertLinkResSensor.SetValue(c => c.SensorId, measResult.SensorId);
                                    builderInsertLinkResSensor.Select(c => c.Id);
                                    queryExecuter.ExecuteAndFetch(builderInsertLinkResSensor, reader =>
                                    {
                                        var res = reader.Read();
                                        if (res)
                                            idLinkRes = reader.GetValue(c => c.Id);
                                        return res;
                                    });
                                }

                                var generalResult = station.GeneralResult;
                                if (generalResult != null)
                                {
                                    long IDResGeneral = -1;
                                    var builderIResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                                    builderIResStGeneral.Select(c => c.Id);
                                    builderIResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, valInsResMeasStation);
                                    queryExecuter.Fetch(builderIResStGeneral, readerResStGeneral =>
                                    {
                                        while (readerResStGeneral.Read())
                                        {
                                            IDResGeneral = readerResStGeneral.GetValue(c => c.Id);
                                            break;
                                        }
                                        return true;
                                    });

                                    if (IDResGeneral == -1)
                                    {
                                        IDResGeneral = InsertResStGeneral(queryExecuter, station, valInsResMeasStation, generalResult, IDResGeneral);

                                        if (IDResGeneral > -1)
                                        {
                                            long IDResSysInfoGeneral = -1;
                                            var builderResSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>().From();
                                            builderResSysInfo.Select(c => c.Id);
                                            builderResSysInfo.Where(c => c.ResStGeneralId, ConditionOperator.Equal, IDResGeneral);
                                            queryExecuter.Fetch(builderResSysInfo, readerIResSysInfo =>
                                            {
                                                while (readerIResSysInfo.Read())
                                                {
                                                    IDResSysInfoGeneral = readerIResSysInfo.GetValue(c => c.Id);
                                                    break;
                                                }
                                                return true;
                                            });
                                            InsertResSysInfo(queryExecuter, station, IDResGeneral, IDResSysInfoGeneral);
                                            InsertResStMaskElement(queryExecuter, station, IDResGeneral, false);
                                            InsertResStLevelCar(queryExecuter, station, valInsResMeasStation);
                                            InsertBearing(queryExecuter, valInsResMeasStation, station);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                queryExecuter.CommitTransaction();
                return true;
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                queryExecuter.RollbackTransaction();
                return false;
            }
        }
        private bool SaveMeasResultSignaling(DM.MeasResults measResult, out long ResMeasId, out int ResSensorId)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            long valInsResMeas = 0;
            int sensorId = -1;
            try
            {
                queryExecuter.BeginTransaction();

                int subMeasTaskId = -1; int subMeasTaskStaId = -1; int resultId = -1;
                GetIds(measResult.ResultId, measResult.TaskId, out subMeasTaskId, out subMeasTaskStaId, out sensorId, out resultId);

                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, resultId.ToString());
                builderInsertIResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.SetValue(c => c.MeasSubTaskId, subMeasTaskId);
                builderInsertIResMeas.SetValue(c => c.MeasSubTaskStationId, subMeasTaskStaId);
                builderInsertIResMeas.SetValue(c => c.SensorId, sensorId);
                builderInsertIResMeas.Select(c => c.Id);
                queryExecuter.ExecuteAndFetch(builderInsertIResMeas, reader =>
                {
                    var res = reader.Read();
                    if (res)
                    {
                        valInsResMeas = reader.GetValue(c => c.Id);
                    }

                    return res;
                });

                if (valInsResMeas > 0)
                {
                    var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                    builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                    builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                    builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                    builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                    builderInsertResLocSensorMeas.SetValue(c => c.ResMeasId, valInsResMeas);
                    builderInsertResLocSensorMeas.Select(c => c.Id);
                    queryExecuter.Execute(builderInsertResLocSensorMeas);

                    if (measResult.RefLevels != null)
                    {
                        long valInsReferenceLevels = 0;
                        var refLevels = measResult.RefLevels;
                        var builderInsertReferenceLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Insert();
                        builderInsertReferenceLevels.SetValue(c => c.StartFrequency_Hz, refLevels.StartFrequency_Hz);
                        builderInsertReferenceLevels.SetValue(c => c.StepFrequency_Hz, refLevels.StepFrequency_Hz);
                        if (refLevels.levels != null)
                        {
                            builderInsertReferenceLevels.SetValue(c => c.RefLevels, refLevels.levels);
                        }
                        builderInsertReferenceLevels.SetValue(c => c.ResMeasId, valInsResMeas);
                        builderInsertReferenceLevels.Select(c => c.Id);
                        queryExecuter
                        .ExecuteAndFetch(builderInsertReferenceLevels, readerReferenceLevels =>
                        {
                            var res = readerReferenceLevels.Read();
                            if (res)
                            {
                                valInsReferenceLevels = readerReferenceLevels.GetValue(c => c.Id);
                            }
                            return true;
                        });
                    }
                    if (measResult.Emittings != null)
                    {
                        foreach (Emitting emitting in measResult.Emittings)
                        {
                            long valInsReferenceEmitting = 0;
                            var builderInsertEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Insert();
                            builderInsertEmitting.SetValue(c => c.CurentPower_dBm, emitting.CurentPower_dBm);
                            builderInsertEmitting.SetValue(c => c.MeanDeviationFromReference, emitting.MeanDeviationFromReference);
                            builderInsertEmitting.SetValue(c => c.ReferenceLevel_dBm, emitting.ReferenceLevel_dBm);
                            builderInsertEmitting.SetValue(c => c.ResMeasId, valInsResMeas);
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
                            builderInsertEmitting.Select(c => c.Id);
                            queryExecuter.ExecuteAndFetch(builderInsertEmitting, readerEmitting =>
                            {
                                var res = readerEmitting.Read();
                                if (res)
                                    valInsReferenceEmitting = readerEmitting.GetValue(c => c.Id);
                                return true;
                            });

                            if (valInsReferenceEmitting > 0)
                            {
                                var workTimes = emitting.WorkTimes;
                                if (workTimes != null)
                                {
                                    var lstInsWorkTime = new IQueryInsertStatement<MD.IWorkTime>[workTimes.Length];
                                    for (int r = 0; r < workTimes.Length; r++)
                                    {
                                        var builderInsertIWorkTime = this._dataLayer.GetBuilder<MD.IWorkTime>().Insert();
                                        builderInsertIWorkTime.SetValue(c => c.EmittingId, valInsReferenceEmitting);
                                        builderInsertIWorkTime.SetValue(c => c.HitCount, workTimes[r].HitCount);
                                        builderInsertIWorkTime.SetValue(c => c.PersentAvailability, workTimes[r].PersentAvailability);
                                        builderInsertIWorkTime.SetValue(c => c.StartEmitting, workTimes[r].StartEmitting);
                                        builderInsertIWorkTime.SetValue(c => c.StopEmitting, workTimes[r].StopEmitting);
                                        builderInsertIWorkTime.Select(c => c.Id);
                                        lstInsWorkTime[r] = builderInsertIWorkTime;
                                    }
                                    queryExecuter.ExecuteAndFetch(lstInsWorkTime, readerWorkTime =>
                                    {
                                        return true;
                                    });
                                }

                                var spectrum = emitting.Spectrum;
                                if (spectrum != null)
                                {
                                    long valInsSpectrum = 0;
                                    var builderInsertISpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>().Insert();
                                    builderInsertISpectrum.SetValue(c => c.EmittingId, valInsReferenceEmitting);
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
                                    builderInsertISpectrum.Select(c => c.Id);
                                    queryExecuter.ExecuteAndFetch(builderInsertISpectrum, readerISpectrum =>
                                    {
                                        var resSpectrum = readerISpectrum.Read();
                                        if (resSpectrum)
                                        {
                                            valInsSpectrum = readerISpectrum.GetValue(c => c.Id);
                                        }
                                        return true;
                                    });
                                }
                            }

                        }
                    }
                }

            ResMeasId = valInsResMeas; ResSensorId = sensorId;
            queryExecuter.CommitTransaction();
            return true;
            }
            catch (Exception exp)
            {
                ResMeasId = valInsResMeas; ResSensorId = sensorId;
                _logger.Exception(Contexts.ThisComponent, exp);
                queryExecuter.RollbackTransaction();
                return false;
            }
        }
        private long InsertResStGeneral(IQueryExecutor queryExecuter, StationMeasResult station, long valInsResMeasStation, GeneralMeasResult generalResult, long IDResGeneral)
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
            builderInsertResStGeneral.SetValue(c => c.SpecrumStartFreq, (double?)generalResult.SpectrumStartFreq_MHz);
            builderInsertResStGeneral.SetValue(c => c.SpecrumSteps, (double?)generalResult.SpectrumSteps_kHz);
            builderInsertResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
            builderInsertResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
            builderInsertResStGeneral.SetValue(c => c.LevelsSpectrumdBm, station.GeneralResult.LevelsSpectrum_dBm);
            builderInsertResStGeneral.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
            builderInsertResStGeneral.Select(c => c.Id);
            queryExecuter.ExecuteAndFetch(builderInsertResStGeneral, reader =>
            {
                var res = reader.Read();
                if (res)
                    IDResGeneral = reader.GetValue(c => c.Id);
                return res;
            });
            return IDResGeneral;
        }
        private long InsertResSysInfo(IQueryExecutor queryExecuter, StationMeasResult station, long IDResGeneral, long IDResSysInfoGeneral)
        {
            if (IDResSysInfoGeneral == -1 && station.GeneralResult.StationSysInfo != null)
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
                builderInsertResSysInfo.SetValue(c => c.ResStGeneralId, IDResGeneral);
                builderInsertResSysInfo.Select(c => c.Id);
                queryExecuter.ExecuteAndFetch(builderInsertResSysInfo, reader =>
                {
                    var res = reader.Read();
                    if (res)
                        IDResSysInfoGeneral = reader.GetValue(c => c.Id);
                    return res;
                });
                if (IDResSysInfoGeneral > -1 && stationSysInfo.InfoBlocks != null)
                {
                    foreach (StationSysInfoBlock blocks in stationSysInfo.InfoBlocks)
                    {
                        long IDResSysInfoBlocks = -1;
                        var builderInsertStationSysInfoBlock = this._dataLayer.GetBuilder<MD.IResSysInfoBlocks>().Insert();
                        builderInsertStationSysInfoBlock.SetValue(c => c.Data, blocks.Data);
                        builderInsertStationSysInfoBlock.SetValue(c => c.Type, blocks.Type);
                        builderInsertStationSysInfoBlock.SetValue(c => c.ResSysInfoId, IDResSysInfoGeneral);
                        builderInsertStationSysInfoBlock.Select(c => c.Id);
                        queryExecuter.ExecuteAndFetch(builderInsertStationSysInfoBlock, reader =>
                        {
                            var res = reader.Read();
                            if (res)
                                IDResSysInfoBlocks = reader.GetValue(c => c.Id);
                            return res;
                        });
                    }
                }
            }
            return IDResSysInfoGeneral;
        }
        private void InsertResStMaskElement(IQueryExecutor queryExecuter, StationMeasResult station, long IDResGeneral, bool isMergeStation)
        {
            if (station.GeneralResult.BWMask != null && station.GeneralResult.BWMask.Length > 0)
            {
                var lstIns = new List<IQueryInsertStatement<MD.IResStMaskElement>>();
                foreach (ElementsMask maskElem in station.GeneralResult.BWMask)
                {
                    if (maskElem.Level_dB.HasValue  && maskElem.Level_dB.Value >= -300 && maskElem.Level_dB.Value <= 300
                        && maskElem.BW_kHz.HasValue && maskElem.BW_kHz.Value >= 1 && maskElem.BW_kHz.Value <= 200000)
                    {
                        var builderInsertmaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElement>().Insert();
                        builderInsertmaskElem.SetValue(c => c.Bw, maskElem.BW_kHz);
                        builderInsertmaskElem.SetValue(c => c.Level, maskElem.Level_dB);
                        builderInsertmaskElem.SetValue(c => c.ResStGeneralId, IDResGeneral);
                        builderInsertmaskElem.Select(c => c.Id);
                        lstIns.Add(builderInsertmaskElem);
                    }
                }
                if (lstIns.Count > 0)
                {
                    if (isMergeStation)
                    {
                        var builderDelMaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElement>().Delete();
                        builderDelMaskElem.Where(c => c.ResStGeneralId, ConditionOperator.Equal, IDResGeneral);
                        queryExecuter.Execute(builderDelMaskElem);
                    }
                    queryExecuter.ExecuteAndFetch(lstIns.ToArray(), reader => { return true; });
                }
            }
        }
        private void InsertResStLevelCar(IQueryExecutor queryExecuter, StationMeasResult station, long valInsResMeasStation)
        {
            if (station.LevelResults != null)
            {
                var lstIns = new List<IQueryInsertStatement<MD.IResStLevelCar>>();
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
                        builderInsertResStLevelCar.SetValue(c => c.ResStationId, valInsResMeasStation);
                        builderInsertResStLevelCar.Select(c => c.Id);
                        lstIns.Add(builderInsertResStLevelCar);
                    }
                }
                if (lstIns.Count > 0)
                    queryExecuter.ExecuteAndFetch(lstIns.ToArray(), reader => { return true; });
            }
        }
        private void InsertBearing(IQueryExecutor queryExecuter, long valInsResMeasStation, StationMeasResult station)
        {
            if (station.Bearings != null)
            {
                var lstInsBearingRaw = new List<IQueryInsertStatement<MD.IBearing>>();
                foreach (DirectionFindingData directionFindingData in station.Bearings)
                {
                    var builderInsertBearing = this._dataLayer.GetBuilder<MD.IBearing>().Insert();
                    builderInsertBearing.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
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
                    builderInsertBearing.Select(c => c.Id);
                    lstInsBearingRaw.Add(builderInsertBearing);
                }
                if (lstInsBearingRaw.Count > 0)
                    queryExecuter.ExecuteAndFetch(lstInsBearingRaw.ToArray(), reader => { return true; });
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
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var builderInsertLog = this._dataLayer.GetBuilder<MD.ILogs>().Insert();
            builderInsertLog.SetValue(c => c.TableName, tableName);
            builderInsertLog.SetValue(c => c.When, DateTime.Now);
            builderInsertLog.SetValue(c => c.Who, "");
            builderInsertLog.SetValue(c => c.Lcount, 1);
            builderInsertLog.SetValue(c => c.Info, msg);
            builderInsertLog.SetValue(c => c.Event, "");
            builderInsertLog.Select(c => c.Id);
            queryExecuter.Execute(builderInsertLog);
        }
    }
}
