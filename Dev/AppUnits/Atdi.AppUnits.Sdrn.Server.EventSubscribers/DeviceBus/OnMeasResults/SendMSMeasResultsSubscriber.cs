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
    [SubscriptionEvent(EventName = "OnMSMeasResultsDeviceBusEvent", SubscriberName = "SendMSMeasResultsSubscriber")]
    public class SendMSMeasResultsSubscriber : SubscriberBase<DM.MeasResults>
    {
        class HandleContext
        {
            public long messageId;
            public long resMeasId = 0;
            public string sensorName;
            public string sensorTechId;
            public IDataLayerScope scope;
            public DM.MeasResults measResult;
        }
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IStatistics _statistics;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;

        private readonly IDataCache<string, long> _verifiedSubTaskSensorIdentityCache;
        private readonly IDataCache<string, long> _sensorIdentityCache;
        private readonly IDataCache<string, long> _measResultStationIdentityCache;
        private readonly IDataCache<string, long> _measResultIdentityCache;


        private readonly IStatisticCounter _monitoringStationsCounter;

        public SendMSMeasResultsSubscriber(
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

            this._verifiedSubTaskSensorIdentityCache = cacheSite.Ensure(DataCaches.VerifiedSubTaskSensorIdentity);
            this._sensorIdentityCache = cacheSite.Ensure(DataCaches.SensorIdentity);
            this._measResultStationIdentityCache = cacheSite.Ensure(DataCaches.MeasResultStationIdentity);
            this._measResultIdentityCache = cacheSite.Ensure(DataCaches.MeasResultIdentity);

            if (this._statistics != null)
            {
                this._monitoringStationsCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsMonitoringStations);
            }

        }

        protected override void Handle(string sensorName, string sensorTechId, DM.MeasResults deliveryObject, long messageId)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                if (deliveryObject.Measurement != MeasurementType.MonitoringStations)
                {
                    throw new InvalidOperationException("Incorrect MeasurementType. Expected is MonitoringStations");
                }

                var status = SdrnMessageHandlingStatus.Unprocessed;
                bool isSuccessProcessed = false;
                var reasonFailure = "";
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        var context = new HandleContext()
                        {
                            messageId = messageId,
                            resMeasId = 0,
                            sensorName = sensorName,
                            sensorTechId = sensorTechId,
                            scope = scope,
                            measResult = deliveryObject
                        };

                        this._monitoringStationsCounter?.Increment();
                        this.SaveMeasResultMonitoringStations(context);
                    }
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
        private bool SaveMeasResultMonitoringStations(HandleContext context)
        {
            try
            {
                var measResult = context.measResult;

                if (string.IsNullOrEmpty(measResult.TaskId))
                {
                    WriteLog("Undefined value TaskId", "IResMeas", context);
                    return false;
                }
                if (string.IsNullOrEmpty(measResult.ResultId))
                {
                    WriteLog("Undefined value ResultId", "IResMeas", context);
                    return false;
                }
                if (measResult.StationResults == null || measResult.StationResults.Length == 0)
                {
                    WriteLog("Undefined values StationResults[]", "IResMeas", context);
                    return false;
                }
                if (!(measResult.SwNumber >= 0 && measResult.SwNumber <= 10000))
                {
                    WriteLog("Incorrect value SwNumber", "IResMeas", context);
                }

                measResult.TaskId = measResult.TaskId.SubString(200);
                measResult.ResultId = measResult.ResultId.SubString(50);

                if ((measResult.Status != null) && (measResult.Status.Length > 5))
                {
                    measResult.Status = "";
                }

                var subSubTaskSensorId = EnsureSubTaskSensorId(context.sensorName, context.sensorTechId, measResult.Measurement, measResult.TaskId, measResult.Measured, context.scope);
                context.resMeasId = this.EnsureMeasResultMonitoring(subSubTaskSensorId, context);

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
                                    WriteLog("StartTime must be less than FinishTime", "IResRoutes", context);

                                if (this.ValidateGeoLocation<RoutePoint>(routePoint, "IResRoutes", context))
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
                                builderInsertroutePoints.SetValue(c => c.RES_MEAS.Id, context.resMeasId);
                                context.scope.Executor.Execute(builderInsertroutePoints);
                            }
                        }
                    }
                }

                for (int i = 0; i < measResult.StationResults.Length; i++)
                {
                    var station = measResult.StationResults[i];
                    var generalResult = station.GeneralResult;
                    if (generalResult == null)
                    {
                        ///TODO: Записать влог и игнорироват запись - перейти на следующую
                        WriteLog($"({i}) GeneralResult is empty", "IResStGeneral", context);
                        continue;
                    }

                    if (!generalResult.CentralFrequency_MHz.HasValue
                        || generalResult.CentralFrequency_MHz.Value < 0.001 || generalResult.CentralFrequency_MHz.Value > 400000)
                        generalResult.CentralFrequency_MHz = null;
                    if (!generalResult.CentralFrequencyMeas_MHz.HasValue
                        || generalResult.CentralFrequencyMeas_MHz.Value < 0.001 || generalResult.CentralFrequencyMeas_MHz.Value > 400000)
                        generalResult.CentralFrequencyMeas_MHz = null;

                    ///TODO: Записать влог и игнорироват запись - перейти на следующую
                    var stationFrequency = generalResult.CentralFrequency_MHz ?? generalResult.CentralFrequencyMeas_MHz;
                    if (stationFrequency == null)
                    {
                        ///TODO: Записать влог и игнорироват запись - перейти на следующую
                        WriteLog($"({i}) CentralFrequency and CentralFrequencyMeas are empty", "IResStGeneral", context);
                        continue;
                    }

                    station.StationId = station.StationId.SubString(50);
                    station.TaskGlobalSid = station.TaskGlobalSid.SubString(50);
                    station.RealGlobalSid = station.RealGlobalSid.SubString(50);
                    station.SectorId = station.SectorId.SubString(50);
                    station.Status = station.Status.SubString(5);
                    station.Standard = station.Standard.SubString(10);

                    if (!generalResult.SpectrumStartFreq_MHz.HasValue || generalResult.SpectrumStartFreq_MHz.Value < 0.001m || generalResult.SpectrumStartFreq_MHz.Value > 400000
                        || !generalResult.SpectrumSteps_kHz.HasValue || generalResult.SpectrumSteps_kHz.Value < 0.001m || generalResult.SpectrumSteps_kHz.Value > 100000)
                    {
                        generalResult.SpectrumStartFreq_MHz = null;
                        generalResult.SpectrumSteps_kHz = null;
                        generalResult.LevelsSpectrum_dBm = null;
                        generalResult.BandwidthResult = null;
                    }
                    if (generalResult.MeasStartTime > generalResult.MeasFinishTime)
                    {
                        WriteLog($"({i}) MeasStartTime must be less than MeasFinishTime", "IResStGeneral", context);
                    }

                    var clientFrequency = Convert.ToDecimal(Math.Round(stationFrequency.Value, 3));
                    var resMeasStationId = this.EnsureMeasResultStation(context.resMeasId, station, clientFrequency, context);


                    if (generalResult.LevelsSpectrum_dBm != null
                        && generalResult.LevelsSpectrum_dBm.Length > 0)
                    {
                        var resGeneralId = InsertResStGeneral(resMeasStationId, generalResult, i, context);
                        InsertResSysInfo(station, resGeneralId, context);
                        InsertResStMaskElement(generalResult, resGeneralId, context);
                    }

                    InsertResStLevelCar(resMeasStationId, station.LevelResults, i, context);
                    InsertBearing(resMeasStationId, station, context);
                }

                return true;
            }
            catch (Exception exp)
            {
                _logger.Exception(Contexts.ThisComponent, exp);
                return false;
            }
        }
        private long InsertResStGeneral(long resMeasStationId, GeneralMeasResult generalResult, int index, HandleContext context)
        {
            if (!generalResult.RBW_kHz.HasValue || generalResult.RBW_kHz.Value < 0.001 || generalResult.RBW_kHz.Value > 100000)
                generalResult.RBW_kHz = null;
            if (!generalResult.VBW_kHz.HasValue || generalResult.VBW_kHz.Value < 0.001 || generalResult.VBW_kHz.Value > 100000)
                generalResult.VBW_kHz = null;

            var builderInsertResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().Insert();
            builderInsertResStGeneral.SetValue(c => c.RES_MEAS_STATION.Id, resMeasStationId);
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
                        WriteLog($"({index}) Incorrect values T1, T2 or M", "IResStGeneral", context);
                    }
                }
                if (bandwidthResult.Bandwidth_kHz.HasValue && bandwidthResult.Bandwidth_kHz >= 1 && bandwidthResult.Bandwidth_kHz <= 100000)
                {
                    builderInsertResStGeneral.SetValue(c => c.BW, bandwidthResult.Bandwidth_kHz);
                }
                else
                {
                    WriteLog($"({index}) Incorrect value of Bandwidth", "IResStGeneral", context);
                }

                if (bandwidthResult.TraceCount < 1 || bandwidthResult.TraceCount > 100000)
                {
                    WriteLog($"({index}) Incorrect value TraceCount", "IResStGeneral", context);
                }
                builderInsertResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                builderInsertResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations == true ? 1 : 0);
            }
            builderInsertResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
            builderInsertResStGeneral.SetValue(c => c.SpecrumStartFreq, generalResult.SpectrumStartFreq_MHz);
            builderInsertResStGeneral.SetValue(c => c.SpecrumSteps, generalResult.SpectrumSteps_kHz);
            builderInsertResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
            builderInsertResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
            builderInsertResStGeneral.SetValue(c => c.LevelsSpectrumdBm, generalResult.LevelsSpectrum_dBm);

            var pk = context.scope.Executor.Execute<MD.IResStGeneral_PK>(builderInsertResStGeneral);
            return pk.Id;
        }
        private void InsertResSysInfo(StationMeasResult station, long IDResGeneral, HandleContext context)
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
                var IDResSysInfoGeneral = context.scope.Executor.Execute<MD.IResSysInfo_PK>(builderInsertResSysInfo);
                if (IDResSysInfoGeneral.Id > 0 && stationSysInfo.InfoBlocks != null)
                {
                    foreach (StationSysInfoBlock blocks in stationSysInfo.InfoBlocks)
                    {
                        var builderInsertStationSysInfoBlock = this._dataLayer.GetBuilder<MD.IResSysInfoBlocks>().Insert();
                        builderInsertStationSysInfoBlock.SetValue(c => c.Data, blocks.Data);
                        builderInsertStationSysInfoBlock.SetValue(c => c.Type, blocks.Type);
                        builderInsertStationSysInfoBlock.SetValue(c => c.RES_SYS_INFO.Id, IDResSysInfoGeneral.Id);
                        context.scope.Executor.Execute(builderInsertStationSysInfoBlock);
                    }
                }
            }
        }

        private void InsertResStMaskElement(GeneralMeasResult generalResult, long IDResGeneral, HandleContext context)
        {
            if (generalResult.BWMask != null)
            {
                foreach (ElementsMask maskElem in generalResult.BWMask)
                {
                    if (maskElem.Level_dB.HasValue && maskElem.Level_dB.Value >= -300 && maskElem.Level_dB.Value <= 300
                        && maskElem.BW_kHz.HasValue && maskElem.BW_kHz.Value >= 1 && maskElem.BW_kHz.Value <= 200000)
                    {
                        var builderInsertmaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElement>().Insert();
                        builderInsertmaskElem.SetValue(c => c.Bw, maskElem.BW_kHz);
                        builderInsertmaskElem.SetValue(c => c.Level, maskElem.Level_dB);
                        builderInsertmaskElem.SetValue(c => c.RES_STGENERAL.Id, IDResGeneral);
                        context.scope.Executor.Execute(builderInsertmaskElem);
                    }
                    else WriteLog($"Incorrect value Level_dB: {maskElem.Level_dB} or BW_kHz: {maskElem.BW_kHz}", "InsertResStMaskElement", context);
                }
            }
        }
        private void InsertResStLevelCar(long valInsResMeasStation, LevelMeasResult[] levelResults, int index, HandleContext context)
        {
            if (levelResults != null)
            {
                foreach (LevelMeasResult car in levelResults)
                {
                    if (car.Level_dBm.HasValue && car.Level_dBm >= -150 && car.Level_dBm <= 20
                        &&
                        ((car.Level_dBmkVm.HasValue && car.Level_dBmkVm >= -10 && car.Level_dBmkVm <= 140) || (!car.Level_dBmkVm.HasValue)))
                    {
                        var builderInsertResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().Insert();
                        if (car.Location != null && this.ValidateGeoLocation<GeoLocation>(car.Location, "IResStLevelCar", context))
                        {
                            builderInsertResStLevelCar.SetValue(c => c.Agl, car.Location.AGL);
                            builderInsertResStLevelCar.SetValue(c => c.Altitude, car.Location.ASL);
                            builderInsertResStLevelCar.SetValue(c => c.Lon, car.Location.Lon);
                            builderInsertResStLevelCar.SetValue(c => c.Lat, car.Location.Lat);
                        }
                        if (car.DifferenceTimeStamp_ns.HasValue && (car.DifferenceTimeStamp_ns < 0 && car.DifferenceTimeStamp_ns > 999999999))
                        {
                            WriteLog($"({index}) Incorrect value DifferenceTimeStamp_ns: {car.DifferenceTimeStamp_ns}", "IResStLevelCar", context);
                        }

                        builderInsertResStLevelCar.SetValue(c => c.DifferenceTimeStamp, car.DifferenceTimeStamp_ns);
                        builderInsertResStLevelCar.SetValue(c => c.LevelDbm, car.Level_dBm);
                        builderInsertResStLevelCar.SetValue(c => c.LevelDbmkvm, car.Level_dBmkVm);
                        builderInsertResStLevelCar.SetValue(c => c.TimeOfMeasurements, car.MeasurementTime);
                        builderInsertResStLevelCar.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation);
                        context.scope.Executor.Execute(builderInsertResStLevelCar);
                    }
                    else
                    {
                        WriteLog($"({index}) Incorrect value of Level_dBmkVm: {car.Level_dBmkVm} or Level_dBmkVm: {car.Level_dBmkVm}", "IResStLevelCar", context);
                    }
                }
            }
        }
        private void InsertBearing(long valInsResMeasStation, StationMeasResult station, HandleContext context)
        {
            if (station.Bearings != null)
            {
                foreach (DirectionFindingData directionFindingData in station.Bearings)
                {
                    var builderInsertBearing = this._dataLayer.GetBuilder<MD.IBearing>().Insert();
                    builderInsertBearing.SetValue(c => c.RES_MEAS_STATION.Id, valInsResMeasStation);
                    if (directionFindingData.Location != null && this.ValidateGeoLocation<GeoLocation>(directionFindingData.Location, "IBearing", context))
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
                    context.scope.Executor.Execute(builderInsertBearing);
                }
            }
        }

        private bool ValidateGeoLocation<T>(T location, string tableName, HandleContext context)
            where T : GeoLocation
        {
            bool result = true;
            if (!(location.Lon >= -180 && location.Lon <= 180))
            {
                WriteLog($"Incorrect value Lon {location.Lon}", tableName, context);
                return false;
            }
            if (!(location.Lat >= -90 && location.Lat <= 90))
            {
                WriteLog($"Incorrect value Lat {location.Lat}", tableName, context);
                return false;
            }
            if (location.ASL < -1000 || location.ASL > 9000)
            {
                WriteLog($"Incorrect value Asl {location.ASL}", tableName, context);
            }
            if (location.AGL < -100 || location.AGL > 500)
            {
                WriteLog($"Incorrect value Agl {location.AGL}", tableName, context);
            }
            return result;
        }
        private void WriteLog(string msg, string tableName, HandleContext context)
        {
            var builderInsertLog = this._dataLayer.GetBuilder<MD.IValidationLogs>().Insert();
            builderInsertLog.SetValue(c => c.TableName, tableName);
            builderInsertLog.SetValue(c => c.When, DateTime.Now);
            builderInsertLog.SetValue(c => c.Info, msg);
            builderInsertLog.SetValue(c => c.MESSAGE.Id, context.messageId);
            builderInsertLog.SetValue(c => c.RES_MEAS.Id, context.resMeasId);
            context.scope.Executor.Execute(builderInsertLog);
        }
        private long EnsureMeasResultMonitoring(long subTaskSensorId, HandleContext context)
        {
            var date = context.measResult.Measured;
            var key = $"subTaskSensorId: {subTaskSensorId}, year: {date.Year}, month: {date.Month}, day: {date.Day}";

            // поиск в кеше
            if (_measResultIdentityCache.TryGet(key, out long measResultId))
            {
                return measResultId;
            }

            // поиск в хранилище
            if (this.TryGetMeasResultFromStorage(date, subTaskSensorId, context, out measResultId))
            {
                _measResultIdentityCache.Set(key, measResultId);
                return measResultId;
            }

            // нужно создать таск
            measResultId = this.CreateMeasResult(date, subTaskSensorId, context);
            _measResultIdentityCache.Set(key, measResultId);

            return measResultId;
        }
        private bool TryGetMeasResultFromStorage(DateTime date, long subTaskSensorId, HandleContext context, out long measResultId)
        {
            var query = _dataLayer.GetBuilder<MD.IResMeas>()
                .From()
                .OnTop(1)
                .Select(c => c.Id)
                .Where(c => c.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId)
                .Where(c => c.TimeMeas, ConditionOperator.Between, date.Date, new DateTime(date.Year, date.Month, date.Day, 23, 59, 59));

            var id = default(long);
            var result = context.scope.Executor.ExecuteAndFetch(query, reader =>
            {
                var readState = reader.Read();
                if (readState)
                {
                    id = reader.GetValue(c => c.Id);
                }
                return readState;
            });

            measResultId = id;
            return result;
        }
        private long CreateMeasResult(DateTime date, long subTaskSensorId, HandleContext context)
        {
            var measResult = context.measResult;
            var query = _dataLayer.GetBuilder<MD.IResMeas>()
                .Insert()
                .SetValue(c => c.MeasResultSID, measResult.ResultId)
                .SetValue(c => c.Status, measResult.Status)
                .SetValue(c => c.TimeMeas, measResult.Measured.Date)
                .SetValue(c => c.DataRank, measResult.SwNumber)
                .SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString())
                .SetValue(c => c.SUBTASK_SENSOR.Id, subTaskSensorId)
                .SetValue(c => c.StartTime, measResult.StartTime)
                .SetValue(c => c.StopTime, measResult.StopTime);

            var pk = context.scope.Executor.Execute<MD.IResMeas_PK>(query);
            if (pk == null)
            {
                throw new InvalidOperationException($"Cannot create meas result by date '{date}'");
            }

            return pk.Id;
        }

        private long EnsureSubTaskSensorId(string sensorName, string techId, MeasurementType measurement, string clientTaskId, DateTime measDate, IDataLayerScope scope)
        {
            // формат токена SDRN.FieldName.LongValue
            var tokens = clientTaskId.Split('.');

            if (tokens.Length == 3 && "SDRN".Equals(tokens[0]))
            {
                if (!"SubTaskSensorId".Equals(tokens[1], StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Incorrect SDRN Task ID token '{clientTaskId}'");
                }

                if (!long.TryParse(tokens[2], out long subTaskSensorId))
                {
                    throw new InvalidOperationException($"Incorrect Task ID value '{clientTaskId}'");
                }
                var key = $"sensorName: {sensorName}, techId: {techId}, subTaskSensorId: {subTaskSensorId}";

                if (_verifiedSubTaskSensorIdentityCache.TryGet(key, out long data))
                {
                    return data;
                }
                var sensorId = this.EnsureSensor(sensorName, techId, scope);

                if (!this.ExistsSubTaskSensorFromStorage(subTaskSensorId, sensorId, scope))
                {
                    throw new InvalidOperationException($"A SubTaskSensor entry not found in storage by ID #{subTaskSensorId} and SensorName '{sensorName}' and Tech ID '{techId}'");
                }

                _verifiedSubTaskSensorIdentityCache.Set(key, subTaskSensorId);
                return subTaskSensorId;
            }
            throw new InvalidOperationException($"Incorrect Task ID value '{clientTaskId}'");
        }
        private bool ExistsSubTaskSensorFromStorage(long subTaskSensorId, long sensorId, IDataLayerScope scope)
        {
            var query = _dataLayer.GetBuilder<MD.ISubTaskSensor>()
                .From()
                .OnTop(1)
                .Select(c => c.Id)
                .Where(c => c.Id, ConditionOperator.Equal, subTaskSensorId)
                .Where(c => c.SENSOR.Id, ConditionOperator.Equal, sensorId);

            return scope.Executor.ExecuteAndFetch(query, reader =>
            {
                return reader.Read();
            });
        }
        private long EnsureSensor(string sensorName, string techId, IDataLayerScope scope)
        {
            var key = $"name: {sensorName}, techId: {techId}";

            // поиск в кеше
            if (_sensorIdentityCache.TryGet(key, out long sensorId))
            {
                return sensorId;
            }

            // поиск в хранилище
            if (this.TryGetSensorFromStorage(sensorName, techId, scope, out sensorId))
            {
                _sensorIdentityCache.Set(key, sensorId);
                return sensorId;
            }

            throw new InvalidOperationException($"Not found sensor in storage by name {sensorName} and TechID {techId}");
        }
        private bool TryGetSensorFromStorage(string sensorName, string techId, IDataLayerScope scope, out long sensorId)
        {
            var query = _dataLayer.GetBuilder<MD.ISensor>()
                .From()
                .OnTop(1)
                .Select(c => c.Id)
                .Where(c => c.Name, ConditionOperator.Equal, sensorName)
                .Where(c => c.TechId, ConditionOperator.Equal, techId);

            var id = default(long);
            var result = scope.Executor.ExecuteAndFetch(query, reader =>
            {
                var readState = reader.Read();
                if (readState)
                {
                    id = reader.GetValue(c => c.Id);
                }
                return readState;
            });

            sensorId = id;
            return result;
        }
        private long EnsureMeasResultStation(long measResultId, StationMeasResult clientStation, decimal clientFrequency, HandleContext context)
        {
            var key = $"measResultId: {measResultId}, GlobalSID: {clientStation.TaskGlobalSid}, MeasGlobalSID : {clientStation.RealGlobalSid}, Frequency: {clientFrequency}";

            // поиск в кеше
            if (_measResultStationIdentityCache.TryGet(key, out long measResultStationId))
            {
                return measResultStationId;
            }

            // поиск в хранилище
            if (this.TryGetMeasResultStationFromStorage(measResultId, clientStation, clientFrequency, context, out measResultStationId))
            {
                _measResultStationIdentityCache.Set(key, measResultStationId);
                return measResultStationId;
            }

            // нужно создать таск
            measResultStationId = this.CreateMeasResultStation(measResultId, clientStation, clientFrequency, context);
            _measResultStationIdentityCache.Set(key, measResultStationId);

            return measResultStationId;
        }

        private long CreateMeasResultStation(long measResultId, StationMeasResult clientStation, decimal clientFrequency, HandleContext context)
        {
            var statement = this._dataLayer.GetBuilder<MD.IResMeasStation>().Insert();
            statement.SetValue(c => c.RES_MEAS.Id, measResultId);
            statement.SetValue(c => c.Status, clientStation.Status);
            statement.SetValue(c => c.MeasGlobalSID, clientStation.RealGlobalSid);
            statement.SetValue(c => c.GlobalSID, clientStation.TaskGlobalSid);
            statement.SetValue(c => c.Frequency, clientFrequency);
            statement.SetValue(c => c.Standard, clientStation.Standard);

            if (int.TryParse(clientStation.StationId, out int Idstation))
            {
                statement.SetValue(c => c.ClientStationCode, Idstation);
            }
            if (int.TryParse(clientStation.SectorId, out int IdSector))
            {
                statement.SetValue(c => c.ClientSectorCode, IdSector);
            }

            var pk = context.scope.Executor.Execute<MD.IResMeasStation_PK>(statement);
            if (pk == null)
            {
                throw new InvalidOperationException($"Cannot create meas result station by Meas Result ID #{measResultId} and  MeasGlobalSID is '{clientStation.RealGlobalSid}' and  GlobalSID = '{clientStation.TaskGlobalSid}' and Frequency #{clientFrequency}");
            }

            var sensorId = this.EnsureSensor(context.sensorName, context.sensorTechId, context.scope);

            var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>()
                .Insert()
                .SetValue(c => c.RES_MEAS_STATION.Id, pk.Id)
                .SetValue(c => c.SENSOR.Id, sensorId);
            context.scope.Executor.Execute(builderInsertLinkResSensor);

            return pk.Id;
        }

        private bool TryGetMeasResultStationFromStorage(long measResultId, StationMeasResult clientStation, decimal clientFrequency, HandleContext context, out long measResultStationId)
        {
            var query = _dataLayer.GetBuilder<MD.IResMeasStation>()
                .From()
                .OnTop(1)
                .Select(c => c.Id)
                .Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, measResultId)
                .Where(c => c.Frequency, ConditionOperator.Equal, clientFrequency);
            if (clientStation.TaskGlobalSid != null)
            {
                query.Where(c => c.GlobalSID, ConditionOperator.Equal, clientStation.TaskGlobalSid);
            }
            else
            {
                query.Where(c => c.GlobalSID, ConditionOperator.IsNull);
            }
            if (clientStation.RealGlobalSid != null)
            {
                query.Where(c => c.MeasGlobalSID, ConditionOperator.Equal, clientStation.RealGlobalSid);
            }
            else
            {
                query.Where(c => c.MeasGlobalSID, ConditionOperator.IsNull);
            }


            var id = default(long);
            var result = context.scope.Executor.ExecuteAndFetch(query, reader =>
            {
                var readState = reader.Read();
                if (readState)
                {
                    id = reader.GetValue(c => c.Id);
                }
                return readState;
            });

            measResultStationId = id;
            return result;
        }
    }
}
