using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server;
using DM = Atdi.DataModels.Sdrns;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using Atdi.Modules.Sdrn.Server.Events;
using Atdi.Common;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnReceivedNewSOResult", SubscriberName = "SubscriberSendMeasResultsProcess")]
    public class OnReceivedNewSOResult : IEventSubscriber<OnReceivedNewSOResultEvent>
    {
        private readonly ILogger _logger;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;

        public OnReceivedNewSOResult(ISdrnMessagePublisher messagePublisher, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger)
        {
            this._logger = logger;
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
        }
        public void Notify(OnReceivedNewSOResultEvent @event)
        {
            try
            {
                bool validationResult = true;
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var measResult = new DEV.MeasResults();
                var queryResMeas = this._dataLayer.GetBuilder<MD.IResMeasRaw>()
                .From()
                .Select(c => c.Id, c => c.MeasResultSID, c => c.MeasTaskId, c => c.TimeMeas, c => c.Status, c => c.TypeMeasurements, c => c.DataRank, c => c.ScansNumber, c => c.StartTime, c => c.StopTime)
                .Where(c => c.Id, ConditionOperator.Equal, @event.ResultId);
                queryExecuter.Fetch(queryResMeas, reader =>
                {
                    var result = reader.Read();
                    if (result)
                    {
                        DM.MeasurementType measurement;
                        if (!Enum.TryParse(reader.GetValue(c => c.TypeMeasurements), out measurement))
                            measurement = DM.MeasurementType.MonitoringStations;

                        measResult.Measurement = measurement;
                        measResult.ResultId = reader.GetValue(c => c.MeasResultSID);
                        measResult.TaskId = reader.GetValue(c => c.MeasTaskId);
                        if (reader.GetValue(c => c.TimeMeas).HasValue)
                            measResult.Measured = reader.GetValue(c => c.TimeMeas).Value;
                        measResult.Status = reader.GetValue(c => c.Status);
                        if (reader.GetValue(c => c.DataRank).HasValue)
                            measResult.SwNumber = reader.GetValue(c => c.DataRank).Value;
                        if (reader.GetValue(c => c.ScansNumber).HasValue)
                            measResult.ScansNumber = reader.GetValue(c => c.ScansNumber).Value;
                        if (reader.GetValue(c => c.StartTime).HasValue)
                            measResult.StartTime = reader.GetValue(c => c.StartTime).Value;
                        if (reader.GetValue(c => c.StopTime).HasValue)
                            measResult.StopTime = reader.GetValue(c => c.StopTime).Value;
                    }
                    return result;
                });

                var builderDelMeas = this._dataLayer.GetBuilder<MD.IResMeasRaw>().Delete();
                builderDelMeas.Where(c => c.Id, ConditionOperator.Equal, @event.ResultId);
                queryExecuter.Execute(builderDelMeas);

                if (measResult.Measurement == DM.MeasurementType.MonitoringStations)
                {
                    validationResult = VaildateMeasResultMonitoringStations(ref measResult, @event.ResultId);
                    if (validationResult)
                    {
                        SaveMeasResultMonitoringStations(measResult);
                    }
                }
                if (measResult.Measurement == DM.MeasurementType.SpectrumOccupation)
                {
                    validationResult = VaildateMeasResultSpectrumOccupation(ref measResult, @event.ResultId);
                    if (validationResult)
                    {
                        SaveMeasResultSpectrumOccupation(measResult);
                    }
                }
                if (measResult.Measurement == DM.MeasurementType.Signaling)
                {
                    validationResult = VaildateMeasResultSignaling(ref measResult, @event.ResultId);
                    if (validationResult)
                    {
                        SaveMeasResultSignaling(measResult);
                    }
                }

            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
            }
        }
        private bool VaildateMeasResultMonitoringStations(ref DEV.MeasResults measResult, int resultId)
        {
            var result = true;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

            if (string.IsNullOrEmpty(measResult.ResultId))
            {
                WriteLog("Undefined value ResultId", "IResMeasRaw");
                result = false;
            }
            else if (measResult.ResultId.Length > 50)
                measResult.ResultId.SubString(50);

            if (string.IsNullOrEmpty(measResult.TaskId))
            {
                WriteLog("Undefined value TaskId", "IResMeasRaw");
                result = false;
            }
            else if (measResult.TaskId.Length > 200)
                measResult.TaskId.SubString(200);

            if (!(measResult.ScansNumber >= 1 && measResult.ScansNumber <= 10000000))
                WriteLog("Incorrect value SwNumber", "IResMeasRaw");

            #region Route
            var listRoutes = new List<DEV.Route>();
            var queryRoutes = this._dataLayer.GetBuilder<MD.IResRoutesRaw>()
            .From()
            .Select(c => c.Id, c => c.Lon, c => c.Lat, c => c.Agl, c => c.Asl, c => c.PointStayType, c => c.StartTime, c => c.FinishTime, c => c.RouteId)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Fetch(queryRoutes, reader =>
            {
                while (reader.Read())
                {
                    bool validationResult = true;
                    var route = new DEV.Route();
                    route.RouteId = reader.GetValue(c => c.RouteId);

                    var listRoutePoints = new List<DEV.RoutePoint>();
                    var routePoint = new DEV.RoutePoint();

                    if (reader.GetValue(c => c.Lon).HasValue)
                        routePoint.Lon = reader.GetValue(c => c.Lon).Value;
                    if (reader.GetValue(c => c.Lat).HasValue)
                        routePoint.Lat = reader.GetValue(c => c.Lat).Value;
                    routePoint.ASL = reader.GetValue(c => c.Asl);
                    routePoint.AGL = reader.GetValue(c => c.Agl);

                    validationResult = this.ValidateGeoLocation<DEV.RoutePoint>(routePoint, "IResRoutesRaw");

                    DM.PointStayType pst;
                    if (Enum.TryParse(reader.GetValue(c => c.PointStayType), out pst))
                        routePoint.PointStayType = pst;
                    if (reader.GetValue(c => c.StartTime).HasValue)
                        routePoint.StartTime = reader.GetValue(c => c.StartTime).Value;
                    if (reader.GetValue(c => c.FinishTime).HasValue)
                        routePoint.FinishTime = reader.GetValue(c => c.FinishTime).Value;

                    if (routePoint.StartTime > routePoint.FinishTime)
                    {
                        WriteLog("StartTime must be less than FinishTime", "IResRoutesRaw");
                    }

                    if (validationResult)
                    {
                        listRoutePoints.Add(routePoint);
                        route.RoutePoints = listRoutePoints.ToArray();
                        listRoutes.Add(route);
                    }
                }
                return true;
            });
            if (listRoutes.Count > 0)
                measResult.Routes = listRoutes.ToArray();
            else
                result = false;

            var builderDelRoute = this._dataLayer.GetBuilder<MD.IResRoutesRaw>().Delete();
            builderDelRoute.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Execute(builderDelRoute);

            #endregion

            #region StationMeasResult
            var listStationMeasResult = new List<DEV.StationMeasResult>();
            var queryMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStaRaw>()
            .From()
            .Select(c => c.Id, c => c.StationId, c => c.MeasGlobalSID, c => c.SectorId, c => c.Status, c => c.Standard, c => c.GlobalSID)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Fetch(queryMeasStation, reader =>
            {
                while (reader.Read())
                {
                    var measStation = new DEV.StationMeasResult();
                    if (reader.GetValue(c => c.StationId).HasValue)
                        measStation.StationId = reader.GetValue(c => c.StationId).Value.ToString().SubString(50);
                    measStation.TaskGlobalSid = reader.GetValue(c => c.GlobalSID).SubString(50);
                    measStation.RealGlobalSid = reader.GetValue(c => c.MeasGlobalSID).SubString(50);
                    if (reader.GetValue(c => c.SectorId).HasValue)
                        measStation.SectorId = reader.GetValue(c => c.SectorId).Value.ToString().SubString(50);
                    measStation.Status = reader.GetValue(c => c.Status).SubString(5);
                    measStation.Standard = reader.GetValue(c => c.Standard).SubString(50);

                    #region LevelMeasResult
                    var listLevelMeasResult = new List<DEV.LevelMeasResult>();
                    var queryLevelMeasResult = this._dataLayer.GetBuilder<MD.IResStLevelCarRaw>()
                    .From()
                    .Select(c => c.Id, c => c.LevelDbm, c => c.LevelDbmkvm, c => c.TimeOfMeasurements, c => c.DifferenceTimeStamp, c => c.Agl, c => c.Altitude, c => c.Lon, c => c.Lat)
                    .Where(c => c.ResStationId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(queryLevelMeasResult, readerLev =>
                    {
                        while (readerLev.Read())
                        {
                            bool validationResult = true;
                            var levelMeasResult = new DEV.LevelMeasResult();
                            var geoLocation = new DM.GeoLocation();

                            if (readerLev.GetValue(c => c.Lon).HasValue)
                                geoLocation.Lon = readerLev.GetValue(c => c.Lon).Value;
                            if (readerLev.GetValue(c => c.Lat).HasValue)
                                geoLocation.Lat = readerLev.GetValue(c => c.Lat).Value;
                            geoLocation.ASL = readerLev.GetValue(c => c.Altitude);
                            geoLocation.AGL = readerLev.GetValue(c => c.Agl);

                            validationResult = this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IResStLevelCarRaw");
                            if (validationResult)
                                levelMeasResult.Location = geoLocation;

                            if (readerLev.GetValue(c => c.LevelDbm).HasValue && readerLev.GetValue(c => c.LevelDbm) >= -150 && readerLev.GetValue(c => c.LevelDbm) <= 20)
                                levelMeasResult.Level_dBm = readerLev.GetValue(c => c.LevelDbm).Value;
                            else
                            {
                                WriteLog("Incorrect value LevelDbm", "IResStLevelCarRaw");
                                validationResult = false;
                            }

                            if (readerLev.GetValue(c => c.LevelDbmkvm).HasValue && readerLev.GetValue(c => c.LevelDbmkvm) >= -10 && readerLev.GetValue(c => c.LevelDbmkvm) <= 140)
                                levelMeasResult.Level_dBmkVm = readerLev.GetValue(c => c.LevelDbmkvm).Value;
                            else
                            {
                                WriteLog("Incorrect value LevelDbmkvm", "IResStLevelCarRaw");
                                validationResult = false;
                            }

                            if (readerLev.GetValue(c => c.TimeOfMeasurements).HasValue)
                                levelMeasResult.MeasurementTime = readerLev.GetValue(c => c.TimeOfMeasurements).Value;
                            levelMeasResult.DifferenceTimeStamp_ns = readerLev.GetValue(c => c.DifferenceTimeStamp);

                            if (levelMeasResult.DifferenceTimeStamp_ns.HasValue && (levelMeasResult.DifferenceTimeStamp_ns < 0 && levelMeasResult.DifferenceTimeStamp_ns > 999999999))
                            {
                                WriteLog("Incorrect value DifferenceTimeStamp", "IResStLevelCarRaw");
                            }

                            if (validationResult)
                            {
                                listLevelMeasResult.Add(levelMeasResult);
                            }
                        }
                        return true;
                    });

                    measStation.LevelResults = listLevelMeasResult.ToArray();

                    var builderDelResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCarRaw>().Delete();
                    builderDelResStLevelCar.Where(c => c.ResStationId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Execute(builderDelResStLevelCar);
                    #endregion

                    #region DirectionFindingData

                    var listFindingData = new List<DEV.DirectionFindingData>();
                    var queryFinfdingData = this._dataLayer.GetBuilder<MD.IBearingRaw>()
                    .From()
                    .Select(c => c.Id, c => c.Agl, c => c.Asl, c => c.Lon, c => c.Lat, c => c.Agl, c => c.Level_dBm, c => c.Level_dBmkVm, c => c.MeasurementTime, c => c.Quality, c => c.AntennaAzimut, c => c.Bandwidth_kHz, c => c.Bearing, c => c.CentralFrequency_MHz)
                    .Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(queryFinfdingData, readerData =>
                    {
                        while (readerData.Read())
                        {
                            var findingData = new DEV.DirectionFindingData();
                            var geoLocation = new DM.GeoLocation();

                            if (readerData.GetValue(c => c.Lon).HasValue)
                                geoLocation.Lon = readerData.GetValue(c => c.Lon).Value;
                            if (readerData.GetValue(c => c.Lat).HasValue)
                                geoLocation.Lat = readerData.GetValue(c => c.Lat).Value;
                            geoLocation.ASL = readerData.GetValue(c => c.Asl);
                            geoLocation.AGL = readerData.GetValue(c => c.Agl);

                            if (this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IBearingRaw"))
                                findingData.Location = geoLocation;

                            findingData.Level_dBm = readerData.GetValue(c => c.Level_dBm);
                            findingData.Level_dBmkVm = readerData.GetValue(c => c.Level_dBmkVm);
                            findingData.MeasurementTime = readerData.GetValue(c => c.MeasurementTime);
                            findingData.Quality = readerData.GetValue(c => c.Quality);
                            findingData.AntennaAzimut = readerData.GetValue(c => c.AntennaAzimut);
                            findingData.Bandwidth_kHz = readerData.GetValue(c => c.Bandwidth_kHz);
                            findingData.Bearing = readerData.GetValue(c => c.Bearing);
                            findingData.CentralFrequency_MHz = readerData.GetValue(c => c.CentralFrequency_MHz);

                            listFindingData.Add(findingData);
                        }
                        return true;
                    });
                    measStation.Bearings = listFindingData.ToArray();

                    var builderDelBearing = this._dataLayer.GetBuilder<MD.IBearingRaw>().Delete();
                    builderDelBearing.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Execute(builderDelBearing);
                    #endregion

                    #region GeneralMeasResult
                    var queryGeneralMeasResult = this._dataLayer.GetBuilder<MD.IResStGeneralRaw>()
                    .From()
                    .Select(c => c.Id, c => c.CentralFrequency, c => c.CentralFrequencyMeas, c => c.OffsetFrequency, c => c.SpecrumStartFreq, c => c.SpecrumSteps, c => c.T1, c => c.T2, c => c.MarkerIndex, c => c.Correctnessestim, c => c.TraceCount, c => c.DurationMeas, c => c.TimeStartMeas, c => c.TimeFinishMeas)
                    .Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(queryGeneralMeasResult, readerGeneralResult =>
                    {
                        bool removeGroup1 = false;
                        while (readerGeneralResult.Read())
                        {
                            var generalMeasResult = new DEV.GeneralMeasResult();
                            if (readerGeneralResult.GetValue(c => c.CentralFrequency).HasValue && readerGeneralResult.GetValue(c => c.CentralFrequency) >= 0.001 && readerGeneralResult.GetValue(c => c.CentralFrequency) <= 400000)
                                generalMeasResult.CentralFrequency_MHz = readerGeneralResult.GetValue(c => c.CentralFrequency).Value;
                            if (readerGeneralResult.GetValue(c => c.CentralFrequencyMeas).HasValue && readerGeneralResult.GetValue(c => c.CentralFrequencyMeas) >= 0.001 && readerGeneralResult.GetValue(c => c.CentralFrequencyMeas) <= 400000)
                                generalMeasResult.CentralFrequencyMeas_MHz = readerGeneralResult.GetValue(c => c.CentralFrequencyMeas).Value;
                            generalMeasResult.OffsetFrequency_mk = readerGeneralResult.GetValue(c => c.OffsetFrequency);
                            if (readerGeneralResult.GetValue(c => c.SpecrumStartFreq) >= 0.001 && readerGeneralResult.GetValue(c => c.SpecrumStartFreq) <= 400000)
                                generalMeasResult.SpectrumStartFreq_MHz = (decimal)readerGeneralResult.GetValue(c => c.SpecrumStartFreq);
                            else
                                removeGroup1 = true;
                            if (readerGeneralResult.GetValue(c => c.SpecrumSteps) >= 0.001 && readerGeneralResult.GetValue(c => c.SpecrumSteps) <= 100000)
                                generalMeasResult.SpectrumSteps_kHz = (decimal)readerGeneralResult.GetValue(c => c.SpecrumSteps);
                            else
                                removeGroup1 = true;
                            generalMeasResult.MeasDuration_sec = readerGeneralResult.GetValue(c => c.DurationMeas).Value;
                            generalMeasResult.MeasStartTime = readerGeneralResult.GetValue(c => c.TimeStartMeas);
                            generalMeasResult.MeasFinishTime = readerGeneralResult.GetValue(c => c.TimeFinishMeas);
                            if (generalMeasResult.MeasStartTime > generalMeasResult.MeasFinishTime)
                            {
                                WriteLog("MeasStartTime must be less than MeasFinishTime", "IResStGeneralRaw");
                            }

                            if (removeGroup1)
                            {
                                generalMeasResult.SpectrumStartFreq_MHz = null;
                                generalMeasResult.SpectrumSteps_kHz = null;
                            }
                            else
                            {
                                #region BandwidthMeasResult
                                var bandwidthMeasResult = new DEV.BandwidthMeasResult();
                                bool isValidBandwith = true;
                                if (readerGeneralResult.GetValue(c => c.MarkerIndex).HasValue && readerGeneralResult.GetValue(c => c.T1).HasValue && readerGeneralResult.GetValue(c => c.T2).HasValue)
                                {
                                    if (!(readerGeneralResult.GetValue(c => c.T1).Value >= 0 && readerGeneralResult.GetValue(c => c.T1).Value <= readerGeneralResult.GetValue(c => c.MarkerIndex).Value
                                        && readerGeneralResult.GetValue(c => c.T2).Value >= readerGeneralResult.GetValue(c => c.MarkerIndex).Value && readerGeneralResult.GetValue(c => c.T2).Value <= 100000
                                        && readerGeneralResult.GetValue(c => c.MarkerIndex).Value >= readerGeneralResult.GetValue(c => c.T1).Value && readerGeneralResult.GetValue(c => c.MarkerIndex) <= readerGeneralResult.GetValue(c => c.T2).Value))
                                    {
                                        isValidBandwith = false;
                                    }
                                }
                                else
                                    isValidBandwith = false;
                                bandwidthMeasResult.T1 = readerGeneralResult.GetValue(c => c.T1);
                                bandwidthMeasResult.T2 = readerGeneralResult.GetValue(c => c.T2);
                                bandwidthMeasResult.MarkerIndex = readerGeneralResult.GetValue(c => c.MarkerIndex);
                                bandwidthMeasResult.СorrectnessEstimations = readerGeneralResult.GetValue(c => c.Correctnessestim);
                                if (readerGeneralResult.GetValue(c => c.TraceCount).HasValue && readerGeneralResult.GetValue(c => c.TraceCount).Value >= 1 && readerGeneralResult.GetValue(c => c.TraceCount).Value <= 100000)
                                    bandwidthMeasResult.TraceCount = readerGeneralResult.GetValue(c => c.TraceCount).Value;

                                if (isValidBandwith)
                                    generalMeasResult.BandwidthResult = bandwidthMeasResult;
                                #endregion

                                var listStLevelsSpect = new List<float>();
                                var queryStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpectRaw>()
                                .From()
                                .Select(c => c.Id, c => c.LevelSpecrum)
                                .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                                queryExecuter.Fetch(queryStLevelsSpect, readerStLevelsSpect =>
                                {
                                    while (readerStLevelsSpect.Read())
                                    {
                                        if (readerStLevelsSpect.GetValue(c => c.LevelSpecrum).HasValue)
                                        {
                                            listStLevelsSpect.Add((float)readerStLevelsSpect.GetValue(c => c.LevelSpecrum).Value);
                                        }
                                    }
                                    return true;
                                });
                                generalMeasResult.LevelsSpectrum_dBm = listStLevelsSpect.ToArray();
                            }

                            var builderDelResStLevels = this._dataLayer.GetBuilder<MD.IResStLevelsSpectRaw>().Delete();
                            builderDelResStLevels.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                            queryExecuter.Execute(builderDelResStLevels);

                            #region MaskElement
                            var isValidElementMask = true;
                            var listElementsMask = new List<DEV.ElementsMask>();
                            var queryElementsMask = this._dataLayer.GetBuilder<MD.IResStMaskElementRaw>()
                            .From()
                            .Select(c => c.Id, c => c.Bw, c => c.Level)
                            .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                            queryExecuter.Fetch(queryElementsMask, readerElementsMask =>
                            {
                                while (readerElementsMask.Read())
                                {
                                    var elementMask = new DEV.ElementsMask();

                                    if (readerElementsMask.GetValue(c => c.Level).HasValue && readerElementsMask.GetValue(c => c.Level).Value >= -300 && readerElementsMask.GetValue(c => c.Level).Value <= 300)
                                        elementMask.Level_dB = readerElementsMask.GetValue(c => c.Level);
                                    else
                                        isValidElementMask = false;

                                    if (readerElementsMask.GetValue(c => c.Bw).HasValue && readerElementsMask.GetValue(c => c.Bw).Value >= 1 && readerElementsMask.GetValue(c => c.Bw).Value <= 200000)
                                        elementMask.BW_kHz = readerElementsMask.GetValue(c => c.Bw);
                                    else
                                        isValidElementMask = false;

                                    if (isValidElementMask)
                                        listElementsMask.Add(elementMask);
                                }
                                return true;
                            });
                            generalMeasResult.BWMask = listElementsMask.ToArray();

                            var builderDelMaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElementRaw>().Delete();
                            builderDelMaskElem.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                            queryExecuter.Execute(builderDelMaskElem);
                            #endregion

                            #region StationSysInfo
                            var queryStationSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfoRaw>()
                            .From()
                            .Select(c => c.Id, c => c.Agl, c => c.Asl, c => c.Bandwidth, c => c.BaseId, c => c.Bsic, c => c.ChannelNumber, c => c.Cid, c => c.Code, c => c.Ctoi, c => c.Eci, c => c.Enodebid, c => c.Freq, c => c.Icio, c => c.InbandPower, c => c.Iscp, c => c.Lac)
                            .Select(c => c.Lat, c => c.Lon, c => c.Mcc, c => c.Mnc, c => c.Nid, c => c.Pci, c => c.Pn, c => c.Power, c => c.Ptotal, c => c.Rnc, c => c.Rscp, c => c.Rsrp, c => c.Rsrq, c => c.Sc, c => c.Sid, c => c.Tac, c => c.TypeCdmaevdo, c => c.Ucid)
                            .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                            queryExecuter.Fetch(queryStationSysInfo, readerStationSysInfo =>
                            {
                                while (readerStationSysInfo.Read())
                                {
                                    var stationSysInfo = new DEV.StationSysInfo();
                                    var location = new DM.GeoLocation();

                                    if (readerStationSysInfo.GetValue(c => c.Lat).HasValue)
                                        location.Lat = readerStationSysInfo.GetValue(c => c.Lat).Value;
                                    if (readerStationSysInfo.GetValue(c => c.Lon).HasValue)
                                        location.Lon = readerStationSysInfo.GetValue(c => c.Lon).Value;
                                    location.AGL = readerStationSysInfo.GetValue(c => c.Agl);
                                    location.ASL = readerStationSysInfo.GetValue(c => c.Asl);

                                    if (this.ValidateGeoLocation<DM.GeoLocation>(location, "IResSysInfoRaw"))
                                        stationSysInfo.Location = location;

                                    stationSysInfo.Freq = readerStationSysInfo.GetValue(c => c.Freq);
                                    stationSysInfo.BandWidth = readerStationSysInfo.GetValue(c => c.Bandwidth);
                                    stationSysInfo.RSRP = readerStationSysInfo.GetValue(c => c.Rsrp);
                                    stationSysInfo.RSRQ = readerStationSysInfo.GetValue(c => c.Rsrq);
                                    stationSysInfo.INBAND_POWER = readerStationSysInfo.GetValue(c => c.InbandPower);
                                    stationSysInfo.MCC = readerStationSysInfo.GetValue(c => c.Mcc);
                                    stationSysInfo.MNC = readerStationSysInfo.GetValue(c => c.Mnc);
                                    stationSysInfo.TAC = readerStationSysInfo.GetValue(c => c.Tac);
                                    stationSysInfo.eNodeBId = readerStationSysInfo.GetValue(c => c.Enodebid);
                                    stationSysInfo.CID = readerStationSysInfo.GetValue(c => c.Cid);
                                    stationSysInfo.ECI = readerStationSysInfo.GetValue(c => c.Eci);
                                    stationSysInfo.PCI = readerStationSysInfo.GetValue(c => c.Pci);
                                    stationSysInfo.BSIC = readerStationSysInfo.GetValue(c => c.Bsic);
                                    stationSysInfo.LAC = readerStationSysInfo.GetValue(c => c.Lac);
                                    stationSysInfo.Power = readerStationSysInfo.GetValue(c => c.Power);
                                    stationSysInfo.CtoI = readerStationSysInfo.GetValue(c => c.Ctoi);
                                    stationSysInfo.SC = readerStationSysInfo.GetValue(c => c.Sc);
                                    stationSysInfo.UCID = readerStationSysInfo.GetValue(c => c.Ucid);
                                    stationSysInfo.RNC = readerStationSysInfo.GetValue(c => c.Rnc);
                                    stationSysInfo.Ptotal = readerStationSysInfo.GetValue(c => c.Ptotal);
                                    stationSysInfo.RSCP = readerStationSysInfo.GetValue(c => c.Rscp);
                                    stationSysInfo.ISCP = readerStationSysInfo.GetValue(c => c.Iscp);
                                    stationSysInfo.Code = readerStationSysInfo.GetValue(c => c.Code);
                                    stationSysInfo.IcIo = readerStationSysInfo.GetValue(c => c.Icio);
                                    stationSysInfo.ChannelNumber = readerStationSysInfo.GetValue(c => c.ChannelNumber);
                                    stationSysInfo.TypeCDMAEVDO = readerStationSysInfo.GetValue(c => c.TypeCdmaevdo);
                                    stationSysInfo.SID = readerStationSysInfo.GetValue(c => c.Sid);
                                    stationSysInfo.NID = readerStationSysInfo.GetValue(c => c.Nid);
                                    stationSysInfo.PN = readerStationSysInfo.GetValue(c => c.Pn);
                                    stationSysInfo.BaseID = readerStationSysInfo.GetValue(c => c.BaseId);

                                    var listStationSysInfoBls = new List<DEV.StationSysInfoBlock>();
                                    var queryStationSysInfoBls = this._dataLayer.GetBuilder<MD.IResSysInfoBlsRaw>()
                                    .From()
                                    .Select(c => c.Id, c => c.Data, c => c.Type)
                                    .Where(c => c.ResSysInfoId, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
                                    queryExecuter.Fetch(queryStationSysInfoBls, readerStationSysInfoBls =>
                                    {
                                        while (readerStationSysInfoBls.Read())
                                        {
                                            var stationSysInfoBls = new DEV.StationSysInfoBlock();
                                            stationSysInfoBls.Data = readerStationSysInfoBls.GetValue(c => c.Data);
                                            stationSysInfoBls.Type = readerStationSysInfoBls.GetValue(c => c.Type);

                                            listStationSysInfoBls.Add(stationSysInfoBls);
                                        }

                                        var builderDelResSysInfoBls = this._dataLayer.GetBuilder<MD.IResSysInfoBlsRaw>().Delete();
                                        builderDelResSysInfoBls.Where(c => c.ResSysInfoId, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
                                        queryExecuter.Execute(builderDelResSysInfoBls);

                                        return true;
                                    });

                                    stationSysInfo.InfoBlocks = listStationSysInfoBls.ToArray();
                                    generalMeasResult.StationSysInfo = stationSysInfo;
                                }
                                return true;
                            });

                            var builderDelResSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfoRaw>().Delete();
                            builderDelResSysInfo.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                            queryExecuter.Execute(builderDelResSysInfo);
                            #endregion

                            measStation.GeneralResult = generalMeasResult;
                        }
                        return true;
                    });

                    #endregion

                    listStationMeasResult.Add(measStation);

                    var builderDelLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensorRaw>().Delete();
                    builderDelLinkResSensor.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Execute(builderDelLinkResSensor);

                    var builderDelResGeneral = this._dataLayer.GetBuilder<MD.IResStGeneralRaw>().Delete();
                    builderDelResGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Execute(builderDelResGeneral);
                }

                return true;
            });

            var builderDelStation = this._dataLayer.GetBuilder<MD.IResMeasStaRaw>().Delete();
            builderDelStation.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Execute(builderDelStation);

            if (listStationMeasResult.Count > 0)
                measResult.StationResults = listStationMeasResult.ToArray();
            else
                result = false;

            return result;

            #endregion
        }
        private bool SaveMeasResultMonitoringStations(DEV.MeasResults measResult)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            try
            {
                queryExecuter.BeginTransaction();

                int idResMeas = 0;
                bool isMerge = false;
                double? diffDates = null;

                var builderResMeasSearch = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                builderResMeasSearch.Select(c => c.Id, c => c.MeasResultSID, c => c.MeasTaskId, c => c.Status, c => c.TimeMeas, c => c.DataRank);
                builderResMeasSearch.OrderByAsc(c => c.Id);
                builderResMeasSearch.Where(c => c.MeasTaskId, ConditionOperator.Equal, measResult.TaskId);
                builderResMeasSearch.Where(c => c.Status, ConditionOperator.IsNull);
                builderResMeasSearch.Where(c => c.TimeMeas, ConditionOperator.Between, new DateTime?[] { measResult.Measured.AddHours(-1), measResult.Measured.AddHours(1) });
                queryExecuter.Fetch(builderResMeasSearch, readerResMeas =>
                {
                    while (readerResMeas.Read())
                    {
                        if (!readerResMeas.GetValue(c => c.DataRank).HasValue || readerResMeas.GetValue(c => c.DataRank).Value == measResult.SwNumber)
                        {
                            if (diffDates == null || diffDates > Math.Abs((readerResMeas.GetValue(c => c.TimeMeas).Value - measResult.Measured).TotalMilliseconds))
                            {
                                diffDates = Math.Abs((readerResMeas.GetValue(c => c.TimeMeas).Value - measResult.Measured).TotalMilliseconds);
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
                    builderUpdateResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                    builderUpdateResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                    builderUpdateResMeas.SetValue(c => c.Status, measResult.Status);
                    builderUpdateResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                    builderUpdateResMeas.SetValue(c => c.DataRank, measResult.SwNumber);
                    builderUpdateResMeas.Where(c => c.Id, ConditionOperator.Equal, idResMeas);
                    queryExecuter.Execute(builderUpdateResMeas);
                }
                else
                {
                    var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                    builderInsertIResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                    builderInsertIResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                    builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                    builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                    builderInsertIResMeas.SetValue(c => c.DataRank, measResult.SwNumber);
                    builderInsertIResMeas.Select(c => c.Id);
                    queryExecuter.ExecuteAndFetch(builderInsertIResMeas, reader =>
                    {
                        var res = reader.Read();
                        if (res)
                            idResMeas = reader.GetValue(c => c.Id);
                        return res;
                    });
                }

                if (measResult.Routes != null)
                {
                    foreach (DEV.Route route in measResult.Routes)
                    {
                        if (route.RoutePoints != null)
                        {
                            var lstIns = new IQueryInsertStatement<MD.IResRoutes>[route.RoutePoints.Length];
                            for (int j = 0; j < route.RoutePoints.Length; j++)
                            {
                                var routePoint = route.RoutePoints[j];
                                var builderInsertroutePoints = this._dataLayer.GetBuilder<MD.IResRoutes>().Insert();
                                builderInsertroutePoints.SetValue(c => c.Agl, routePoint.AGL);
                                builderInsertroutePoints.SetValue(c => c.Asl, routePoint.ASL);
                                builderInsertroutePoints.SetValue(c => c.FinishTime, routePoint.FinishTime);
                                builderInsertroutePoints.SetValue(c => c.StartTime, routePoint.StartTime);
                                builderInsertroutePoints.SetValue(c => c.RouteId, route.RouteId);
                                builderInsertroutePoints.SetValue(c => c.PointStayType, routePoint.PointStayType.ToString());
                                builderInsertroutePoints.SetValue(c => c.Lat, routePoint.Lat);
                                builderInsertroutePoints.SetValue(c => c.Lon, routePoint.Lon);
                                builderInsertroutePoints.SetValue(c => c.ResMeasId, idResMeas);
                                builderInsertroutePoints.Select(c => c.Id);
                                lstIns[j] = builderInsertroutePoints;
                            }
                            queryExecuter.ExecuteAndFetch(lstIns, reader =>
                            {
                                return true;
                            });
                        }
                    }
                }
                if (measResult.StationResults != null)
                {
                    if (isMerge)
                    {
                        for (int n = 0; n < measResult.StationResults.Length; n++)
                        {
                            DEV.StationMeasResult station = measResult.StationResults[n];
                            int idMeasResultStation = 0;
                            int idMeasResultGeneral = 0;
                            DateTime? measStartTime = null;
                            bool isMergeStation = false;
                            DateTime? startTime = null;
                            DateTime? finishTime = null;

                            var builderResMeasStationSearch = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                            builderResMeasStationSearch.Select(c => c.Id);
                            builderResMeasStationSearch.Where(c => c.MeasGlobalSID, ConditionOperator.Equal, station.RealGlobalSid);
                            builderResMeasStationSearch.Where(c => c.Standard, ConditionOperator.Equal, station.Standard);
                            queryExecuter.Fetch(builderResMeasSearch, readerResMeasStation =>
                            {
                                while (readerResMeasStation.Read())
                                {
                                    var builderGeneralResultSearch = this._dataLayer.GetBuilder<MD.IResStGeneral>().From();
                                    builderGeneralResultSearch.Select(c => c.CentralFrequency, c => c.CentralFrequencyMeas, c => c.TimeStartMeas, c => c.TimeFinishMeas, c => c.Id);
                                    builderGeneralResultSearch.Where(c => c.ResMeasStaId, ConditionOperator.Equal, readerResMeasStation.GetValue(c => c.Id));
                                    queryExecuter.Fetch(builderGeneralResultSearch, readerGeneralResult =>
                                    {
                                        int itemCount = 0;
                                        while (readerGeneralResult.Read())
                                        {
                                            var queryStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpect>()
                                            .From()
                                            .Select(c => c.Id, c => c.LevelSpecrum)
                                            .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                                            queryExecuter.Fetch(queryStLevelsSpect, readerStLevelsSpect =>
                                            {
                                                while (readerStLevelsSpect.Read())
                                                    itemCount++;
                                                return true;
                                            });


                                            if ((readerGeneralResult.GetValue(c => c.CentralFrequency).HasValue && station.GeneralResult.CentralFrequency_MHz.HasValue && readerGeneralResult.GetValue(c => c.CentralFrequency).Value == station.GeneralResult.CentralFrequency_MHz.Value)
                                                || (readerGeneralResult.GetValue(c => c.CentralFrequencyMeas).HasValue && station.GeneralResult.CentralFrequencyMeas_MHz.HasValue && Math.Abs(readerGeneralResult.GetValue(c => c.CentralFrequencyMeas).Value - station.GeneralResult.CentralFrequencyMeas_MHz.Value) <= 0.005)
                                                || (!readerGeneralResult.GetValue(c => c.CentralFrequency).HasValue && !station.GeneralResult.CentralFrequency_MHz.HasValue && !readerGeneralResult.GetValue(c => c.CentralFrequencyMeas).HasValue && !station.GeneralResult.CentralFrequencyMeas_MHz.HasValue))
                                            {
                                                if (!measStartTime.HasValue || measStartTime.Value > readerGeneralResult.GetValue(c => c.TimeStartMeas) || idMeasResultStation == 0)
                                                {
                                                    if (itemCount == 0 || station.GeneralResult == null)
                                                    {
                                                        idMeasResultStation = readerResMeasStation.GetValue(c => c.Id);
                                                        startTime = readerGeneralResult.GetValue(c => c.TimeStartMeas);
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

                            if (isMergeStation)
                            {
                                if (station.LevelResults != null)
                                {
                                    if (station.LevelResults.Length > 0)
                                    {
                                        var lstIns = new IQueryInsertStatement<MD.IResStLevelCar>[station.LevelResults.Length];
                                        for (int l = 0; l < station.LevelResults.Length; l++)
                                        {
                                            DEV.LevelMeasResult car = station.LevelResults[l];
                                            var builderInsertResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().Insert();
                                            if (car.Location != null)
                                            {
                                                builderInsertResStLevelCar.SetValue(c => c.Agl, car.Location.AGL);
                                                builderInsertResStLevelCar.SetValue(c => c.Altitude, car.Location.ASL);
                                                builderInsertResStLevelCar.SetValue(c => c.Lon, car.Location.Lon);
                                                builderInsertResStLevelCar.SetValue(c => c.Lat, car.Location.Lat);
                                            }
                                            builderInsertResStLevelCar.SetValue(c => c.DifferenceTimeStamp, car.DifferenceTimeStamp_ns);
                                            builderInsertResStLevelCar.SetValue(c => c.LevelDbm, car.Level_dBm);
                                            builderInsertResStLevelCar.SetValue(c => c.LevelDbmkvm, car.Level_dBmkVm);
                                            builderInsertResStLevelCar.SetValue(c => c.TimeOfMeasurements, car.MeasurementTime);

                                            if (station.GeneralResult != null)
                                            {
                                                var generalResults = station.GeneralResult;

                                                builderInsertResStLevelCar.SetValue(c => c.CentralFrequency, generalResults.CentralFrequency_MHz);
                                                if (generalResults.BandwidthResult != null)
                                                {
                                                    builderInsertResStLevelCar.SetValue(c => c.Bw, generalResults.BandwidthResult.Bandwidth_kHz);
                                                }
                                            }
                                            builderInsertResStLevelCar.SetValue(c => c.ResStationId, idMeasResultStation);
                                            builderInsertResStLevelCar.Select(c => c.Id);
                                            lstIns[l] = builderInsertResStLevelCar;

                                        }
                                        queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                        {
                                            return true;
                                        });
                                    }
                                }

                                if (station.Bearings != null)
                                {
                                    if (station.Bearings.Length > 0)
                                    {
                                        var listBearings = station.Bearings;
                                        var lstInsBearingRaw = new IQueryInsertStatement<MD.IBearingRaw>[listBearings.Length];
                                        for (int p = 0; p < listBearings.Length; p++)
                                        {
                                            DEV.DirectionFindingData directionFindingData = listBearings[p];
                                            var builderInsertBearingRaw = this._dataLayer.GetBuilder<MD.IBearingRaw>().Insert();
                                            builderInsertBearingRaw.SetValue(c => c.ResMeasStaId, idMeasResultStation);
                                            if (directionFindingData.Location != null)
                                            {
                                                builderInsertBearingRaw.SetValue(c => c.Agl, directionFindingData.Location.AGL);
                                                builderInsertBearingRaw.SetValue(c => c.Asl, directionFindingData.Location.ASL);
                                                builderInsertBearingRaw.SetValue(c => c.Lon, directionFindingData.Location.Lon);
                                                builderInsertBearingRaw.SetValue(c => c.Lat, directionFindingData.Location.Lat);
                                            }

                                            builderInsertBearingRaw.SetValue(c => c.Level_dBm, directionFindingData.Level_dBm);
                                            builderInsertBearingRaw.SetValue(c => c.Level_dBmkVm, directionFindingData.Level_dBmkVm);
                                            builderInsertBearingRaw.SetValue(c => c.MeasurementTime, directionFindingData.MeasurementTime);
                                            builderInsertBearingRaw.SetValue(c => c.Quality, directionFindingData.Quality);
                                            builderInsertBearingRaw.SetValue(c => c.AntennaAzimut, directionFindingData.AntennaAzimut);
                                            builderInsertBearingRaw.SetValue(c => c.Bandwidth_kHz, directionFindingData.Bandwidth_kHz);
                                            builderInsertBearingRaw.SetValue(c => c.Bearing, directionFindingData.Bearing);
                                            builderInsertBearingRaw.SetValue(c => c.CentralFrequency_MHz, directionFindingData.CentralFrequency_MHz);
                                            builderInsertBearingRaw.Select(c => c.Id);
                                            lstInsBearingRaw[p] = builderInsertBearingRaw;
                                        }

                                        queryExecuter.ExecuteAndFetch(lstInsBearingRaw, reader =>
                                        {
                                            return true;
                                        });
                                    }
                                }

                                int Idstation; int IdSector;
                                var builderUpdateMeasResult = this._dataLayer.GetBuilder<MD.IResMeasStation>().Update();
                                if (!string.IsNullOrEmpty(station.StationId) && int.TryParse(station.StationId, out Idstation))
                                    builderUpdateMeasResult.SetValue(c => c.StationId, Idstation);
                                if (!string.IsNullOrEmpty(station.SectorId) && int.TryParse(station.SectorId, out IdSector))
                                    builderUpdateMeasResult.SetValue(c => c.SectorId, IdSector);
                                if (!string.IsNullOrEmpty(station.TaskGlobalSid))
                                    builderUpdateMeasResult.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                                if (!string.IsNullOrEmpty(station.Status))
                                    builderUpdateMeasResult.SetValue(c => c.Status, station.Status);
                                builderUpdateMeasResult.Where(c => c.Id, ConditionOperator.Equal, idMeasResultStation);
                                queryExecuter.Execute(builderUpdateMeasResult);

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
                                        if (bandwidthResult.MarkerIndex.HasValue)
                                            builderUpdateResStGeneral.SetValue(c => c.MarkerIndex, bandwidthResult.MarkerIndex);
                                        if (bandwidthResult.T1.HasValue)
                                            builderUpdateResStGeneral.SetValue(c => c.T1, bandwidthResult.T1);
                                        if (bandwidthResult.T2.HasValue)
                                            builderUpdateResStGeneral.SetValue(c => c.T2, bandwidthResult.T2);
                                        builderUpdateResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                                        if (bandwidthResult.СorrectnessEstimations.HasValue)
                                            builderUpdateResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations);
                                    }
                                    if (generalResult.OffsetFrequency_mk.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
                                    if (generalResult.SpectrumStartFreq_MHz.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.SpecrumStartFreq, Convert.ToDouble(generalResult.SpectrumStartFreq_MHz));
                                    if (generalResult.SpectrumSteps_kHz.HasValue)
                                        builderUpdateResStGeneral.SetValue(c => c.SpecrumSteps, Convert.ToDouble(generalResult.SpectrumSteps_kHz));
                                    if (generalResult.MeasStartTime.HasValue && startTime.HasValue && generalResult.MeasStartTime.Value < startTime)
                                        builderUpdateResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
                                    if (generalResult.MeasFinishTime.HasValue && finishTime.HasValue && generalResult.MeasFinishTime.Value > finishTime)
                                        builderUpdateResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
                                    builderUpdateResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, idMeasResultStation);
                                    queryExecuter.Execute(builderUpdateResStGeneral);
                                }

                                if (station.GeneralResult.BWMask != null)
                                {
                                    if (station.GeneralResult.BWMask.Length > 0)
                                    {
                                        var builderDelMaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElement>().Delete();
                                        builderDelMaskElem.Where(c => c.ResStGeneralId, ConditionOperator.Equal, idMeasResultGeneral);
                                        queryExecuter.Execute(builderDelMaskElem);

                                        var lstIns = new IQueryInsertStatement<MD.IResStMaskElement>[station.GeneralResult.BWMask.Length];
                                        for (int l = 0; l < station.GeneralResult.BWMask.Length; l++)
                                        {
                                            DEV.ElementsMask maskElem = station.GeneralResult.BWMask[l];
                                            var builderInsertmaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElement>().Insert();
                                            builderInsertmaskElem.SetValue(c => c.Bw, maskElem.BW_kHz);
                                            builderInsertmaskElem.SetValue(c => c.Level, maskElem.Level_dB);
                                            builderInsertmaskElem.SetValue(c => c.ResStGeneralId, idMeasResultGeneral);
                                            builderInsertmaskElem.Select(c => c.Id);
                                            lstIns[l] = builderInsertmaskElem;
                                        }
                                        queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                        {
                                            return true;
                                        });
                                    }
                                }

                                if (station.GeneralResult.StationSysInfo != null)
                                {
                                    var builderDelSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>().Delete();
                                    builderDelSysInfo.Where(c => c.ResStGeneralId, ConditionOperator.Equal, idMeasResultGeneral);
                                    queryExecuter.Execute(builderDelSysInfo);

                                    var stationSysInfo = station.GeneralResult.StationSysInfo;
                                    int IDResSysInfoGeneral = -1;
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
                                    builderInsertResSysInfo.SetValue(c => c.ResStGeneralId, idMeasResultGeneral);
                                    builderInsertResSysInfo.Select(c => c.Id);

                                    queryExecuter
                                    .ExecuteAndFetch(builderInsertResSysInfo, reader =>
                                    {
                                        var res = reader.Read();
                                        if (res)
                                        {
                                            IDResSysInfoGeneral = reader.GetValue(c => c.Id);
                                        }
                                        return res;
                                    });


                                    if (IDResSysInfoGeneral > -1)
                                    {
                                        if (stationSysInfo.InfoBlocks != null)
                                        {
                                            foreach (DEV.StationSysInfoBlock blocks in stationSysInfo.InfoBlocks)
                                            {
                                                int IDResSysInfoBlocks = -1;
                                                var builderInsertStationSysInfoBlock = this._dataLayer.GetBuilder<MD.IResSysInfoBlocks>().Insert();
                                                builderInsertStationSysInfoBlock.SetValue(c => c.Data, blocks.Data);
                                                builderInsertStationSysInfoBlock.SetValue(c => c.Type, blocks.Type);
                                                builderInsertStationSysInfoBlock.SetValue(c => c.ResSysInfoId, IDResSysInfoGeneral);
                                                builderInsertStationSysInfoBlock.Select(c => c.Id);
                                                queryExecuter
                                                .ExecuteAndFetch(builderInsertStationSysInfoBlock, reader =>
                                                {
                                                    var res = reader.Read();
                                                    if (res)
                                                    {
                                                        IDResSysInfoBlocks = reader.GetValue(c => c.Id);
                                                    }
                                                    return res;
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int n = 0; n < measResult.StationResults.Length; n++)
                        {
                            int valInsResMeasStation = 0;
                            int Idstation; int IdSector;
                            DEV.StationMeasResult station = measResult.StationResults[n];
                            var builderInsertResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().Insert();
                            builderInsertResMeasStation.SetValue(c => c.Status, station.Status);
                            builderInsertResMeasStation.SetValue(c => c.MeasGlobalSID, station.RealGlobalSid);
                            builderInsertResMeasStation.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                            builderInsertResMeasStation.SetValue(c => c.ResMeasId, idResMeas);
                            builderInsertResMeasStation.SetValue(c => c.Standard, station.Standard);
                            if (int.TryParse(station.StationId, out Idstation))
                            {
                                builderInsertResMeasStation.SetValue(c => c.StationId, Idstation);
                            }
                            if (int.TryParse(station.SectorId, out IdSector))
                            {
                                builderInsertResMeasStation.SetValue(c => c.SectorId, IdSector);
                            }
                            builderInsertResMeasStation.Select(c => c.Id);

                            queryExecuter
                           .ExecuteAndFetch(builderInsertResMeasStation, reader =>
                           {
                               var res = reader.Read();
                               if (res)
                               {
                                   valInsResMeasStation = reader.GetValue(c => c.Id);
                               }
                               return res;
                           });


                            if (valInsResMeasStation > 0)
                            {
                                int StationId;
                                int idLinkRes = -1;
                                var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Insert();
                                builderInsertLinkResSensor.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                if (int.TryParse(station.StationId, out StationId))
                                {
                                    builderInsertLinkResSensor.SetValue(c => c.SensorId, StationId);
                                }
                                builderInsertLinkResSensor.Select(c => c.Id);
                                queryExecuter
                                .ExecuteAndFetch(builderInsertLinkResSensor, reader =>
                                {
                                    var res = reader.Read();
                                    if (res)
                                    {
                                        idLinkRes = reader.GetValue(c => c.Id);
                                    }
                                    return res;
                                });


                                var generalResult = station.GeneralResult;
                                if (generalResult != null)
                                {
                                    int IDResGeneral = -1;
                                    var builderInsertResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().Insert();
                                    builderInsertResStGeneral.SetValue(c => c.Rbw, generalResult.RBW_kHz);
                                    builderInsertResStGeneral.SetValue(c => c.Vbw, generalResult.VBW_kHz);
                                    builderInsertResStGeneral.SetValue(c => c.CentralFrequencyMeas, generalResult.CentralFrequencyMeas_MHz);
                                    builderInsertResStGeneral.SetValue(c => c.CentralFrequency, generalResult.CentralFrequency_MHz);
                                    builderInsertResStGeneral.SetValue(c => c.DurationMeas, generalResult.MeasDuration_sec);
                                    if (generalResult.BandwidthResult != null)
                                    {
                                        var bandwidthResult = generalResult.BandwidthResult;
                                        builderInsertResStGeneral.SetValue(c => c.MarkerIndex, bandwidthResult.MarkerIndex);
                                        builderInsertResStGeneral.SetValue(c => c.T1, bandwidthResult.T1);
                                        builderInsertResStGeneral.SetValue(c => c.T2, bandwidthResult.T2);
                                        builderInsertResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                                        builderInsertResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations);
                                    }
                                    builderInsertResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
                                    builderInsertResStGeneral.SetValue(c => c.SpecrumStartFreq, Convert.ToDouble(generalResult.SpectrumStartFreq_MHz));
                                    builderInsertResStGeneral.SetValue(c => c.SpecrumSteps, Convert.ToDouble(generalResult.SpectrumSteps_kHz));
                                    builderInsertResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
                                    builderInsertResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
                                    builderInsertResStGeneral.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                    builderInsertResStGeneral.Select(c => c.Id);
                                    queryExecuter
                                    .ExecuteAndFetch(builderInsertResStGeneral, reader =>
                                    {
                                        var res = reader.Read();
                                        if (res)
                                        {
                                            IDResGeneral = reader.GetValue(c => c.Id);
                                        }
                                        return res;
                                    });


                                    if (IDResGeneral > -1)
                                    {
                                        if (station.GeneralResult.StationSysInfo != null)
                                        {
                                            var stationSysInfo = station.GeneralResult.StationSysInfo;
                                            int IDResSysInfoGeneral = -1;
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

                                            queryExecuter
                                            .ExecuteAndFetch(builderInsertResSysInfo, reader =>
                                            {
                                                var res = reader.Read();
                                                if (res)
                                                {
                                                    IDResSysInfoGeneral = reader.GetValue(c => c.Id);
                                                }
                                                return res;
                                            });


                                            if (IDResSysInfoGeneral > -1)
                                            {
                                                if (stationSysInfo.InfoBlocks != null)
                                                {
                                                    foreach (DEV.StationSysInfoBlock blocks in stationSysInfo.InfoBlocks)
                                                    {
                                                        int IDResSysInfoBlocks = -1;
                                                        var builderInsertStationSysInfoBlock = this._dataLayer.GetBuilder<MD.IResSysInfoBlocks>().Insert();
                                                        builderInsertStationSysInfoBlock.SetValue(c => c.Data, blocks.Data);
                                                        builderInsertStationSysInfoBlock.SetValue(c => c.Type, blocks.Type);
                                                        builderInsertStationSysInfoBlock.SetValue(c => c.ResSysInfoId, IDResSysInfoGeneral);
                                                        builderInsertStationSysInfoBlock.Select(c => c.Id);
                                                        queryExecuter
                                                        .ExecuteAndFetch(builderInsertStationSysInfoBlock, reader =>
                                                        {
                                                            var res = reader.Read();
                                                            if (res)
                                                            {
                                                                IDResSysInfoBlocks = reader.GetValue(c => c.Id);
                                                            }
                                                            return res;
                                                        });
                                                    }
                                                }
                                            }
                                        }

                                        if (station.GeneralResult.BWMask != null)
                                        {
                                            if (station.GeneralResult.BWMask.Length > 0)
                                            {
                                                var lstIns = new IQueryInsertStatement<MD.IResStMaskElement>[station.GeneralResult.BWMask.Length];
                                                for (int l = 0; l < station.GeneralResult.BWMask.Length; l++)
                                                {
                                                    DEV.ElementsMask maskElem = station.GeneralResult.BWMask[l];
                                                    var builderInsertmaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElement>().Insert();
                                                    builderInsertmaskElem.SetValue(c => c.Bw, maskElem.BW_kHz);
                                                    builderInsertmaskElem.SetValue(c => c.Level, maskElem.Level_dB);
                                                    builderInsertmaskElem.SetValue(c => c.ResStGeneralId, IDResGeneral);
                                                    builderInsertmaskElem.Select(c => c.Id);
                                                    lstIns[l] = builderInsertmaskElem;
                                                }
                                                queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                                {
                                                    return true;
                                                });
                                            }
                                        }

                                        if (station.GeneralResult.LevelsSpectrum_dBm != null)
                                        {
                                            if (station.GeneralResult.LevelsSpectrum_dBm.Length > 0)
                                            {
                                                var lstIns = new IQueryInsertStatement<MD.IResStLevelsSpect>[station.GeneralResult.LevelsSpectrum_dBm.Length];
                                                for (int l = 0; l < station.GeneralResult.LevelsSpectrum_dBm.Length; l++)
                                                {
                                                    double lvl = station.GeneralResult.LevelsSpectrum_dBm[l];
                                                    var builderInsertResStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpect>().Insert();
                                                    builderInsertResStLevelsSpect.SetValue(c => c.LevelSpecrum, lvl);
                                                    builderInsertResStLevelsSpect.SetValue(c => c.ResStGeneralId, IDResGeneral);
                                                    builderInsertResStLevelsSpect.Select(c => c.Id);
                                                    lstIns[l] = builderInsertResStLevelsSpect;
                                                }
                                                queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                                {
                                                    return true;
                                                });
                                            }
                                        }


                                        if (station.LevelResults != null)
                                        {
                                            if (station.LevelResults.Length > 0)
                                            {
                                                var lstIns = new IQueryInsertStatement<MD.IResStLevelCar>[station.LevelResults.Length];
                                                for (int l = 0; l < station.LevelResults.Length; l++)
                                                {
                                                    DEV.LevelMeasResult car = station.LevelResults[l];
                                                    var builderInsertResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().Insert();
                                                    if (car.Location != null)
                                                    {
                                                        builderInsertResStLevelCar.SetValue(c => c.Agl, car.Location.AGL);
                                                        builderInsertResStLevelCar.SetValue(c => c.Altitude, car.Location.ASL);
                                                        builderInsertResStLevelCar.SetValue(c => c.Lon, car.Location.Lon);
                                                        builderInsertResStLevelCar.SetValue(c => c.Lat, car.Location.Lat);
                                                    }
                                                    builderInsertResStLevelCar.SetValue(c => c.DifferenceTimeStamp, car.DifferenceTimeStamp_ns);
                                                    builderInsertResStLevelCar.SetValue(c => c.LevelDbm, car.Level_dBm);
                                                    builderInsertResStLevelCar.SetValue(c => c.LevelDbmkvm, car.Level_dBmkVm);
                                                    builderInsertResStLevelCar.SetValue(c => c.TimeOfMeasurements, car.MeasurementTime);

                                                    if (station.GeneralResult != null)
                                                    {
                                                        var generalResults = station.GeneralResult;
                                                        builderInsertResStLevelCar.SetValue(c => c.CentralFrequency, generalResults.CentralFrequency_MHz);
                                                        if (generalResults.BandwidthResult != null)
                                                        {
                                                            builderInsertResStLevelCar.SetValue(c => c.Bw, generalResults.BandwidthResult.Bandwidth_kHz);
                                                        }
                                                    }
                                                    builderInsertResStLevelCar.SetValue(c => c.ResStationId, valInsResMeasStation);
                                                    builderInsertResStLevelCar.Select(c => c.Id);
                                                    lstIns[l] = builderInsertResStLevelCar;

                                                }
                                                queryExecuter.ExecuteAndFetch(lstIns, reader =>
                                                {
                                                    return true;
                                                });
                                            }
                                        }

                                        if (station.Bearings != null)
                                        {
                                            if (station.Bearings.Length > 0)
                                            {
                                                var listBearings = station.Bearings;
                                                var lstInsBearingRaw = new IQueryInsertStatement<MD.IBearingRaw>[listBearings.Length];
                                                for (int p = 0; p < listBearings.Length; p++)
                                                {
                                                    DEV.DirectionFindingData directionFindingData = listBearings[p];
                                                    var builderInsertBearingRaw = this._dataLayer.GetBuilder<MD.IBearingRaw>().Insert();
                                                    builderInsertBearingRaw.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                                    if (directionFindingData.Location != null)
                                                    {
                                                        builderInsertBearingRaw.SetValue(c => c.Agl, directionFindingData.Location.AGL);
                                                        builderInsertBearingRaw.SetValue(c => c.Asl, directionFindingData.Location.ASL);
                                                        builderInsertBearingRaw.SetValue(c => c.Lon, directionFindingData.Location.Lon);
                                                        builderInsertBearingRaw.SetValue(c => c.Lat, directionFindingData.Location.Lat);
                                                    }

                                                    builderInsertBearingRaw.SetValue(c => c.Level_dBm, directionFindingData.Level_dBm);
                                                    builderInsertBearingRaw.SetValue(c => c.Level_dBmkVm, directionFindingData.Level_dBmkVm);
                                                    builderInsertBearingRaw.SetValue(c => c.MeasurementTime, directionFindingData.MeasurementTime);
                                                    builderInsertBearingRaw.SetValue(c => c.Quality, directionFindingData.Quality);
                                                    builderInsertBearingRaw.SetValue(c => c.AntennaAzimut, directionFindingData.AntennaAzimut);
                                                    builderInsertBearingRaw.SetValue(c => c.Bandwidth_kHz, directionFindingData.Bandwidth_kHz);
                                                    builderInsertBearingRaw.SetValue(c => c.Bearing, directionFindingData.Bearing);
                                                    builderInsertBearingRaw.SetValue(c => c.CentralFrequency_MHz, directionFindingData.CentralFrequency_MHz);
                                                    builderInsertBearingRaw.Select(c => c.Id);
                                                    lstInsBearingRaw[p] = builderInsertBearingRaw;
                                                }

                                                queryExecuter.ExecuteAndFetch(lstInsBearingRaw, reader =>
                                                {
                                                    return true;
                                                });
                                            }
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
        private bool VaildateMeasResultSpectrumOccupation(ref DEV.MeasResults measResult, int resultId)
        {
            var result = true;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

            if (string.IsNullOrEmpty(measResult.ResultId))
            {
                WriteLog("Undefined value ResultId", "IResMeasRaw");
                result = false;
            }
            else if (measResult.ResultId.Length > 50)
                measResult.ResultId.SubString(50);

            if (string.IsNullOrEmpty(measResult.TaskId))
            {
                WriteLog("Undefined value TaskId", "IResMeasRaw");
                result = false;
            }
            else if (measResult.TaskId.Length > 200)
                measResult.TaskId.SubString(200);

            if (measResult.Status.Length > 5)
                measResult.Status = "";

            if (!(measResult.SwNumber >= 0 && measResult.SwNumber <= 10000))
                WriteLog("Incorrect value SwNumber", "IResMeasRaw");

            if (measResult.StartTime > measResult.StopTime)
                WriteLog("StartTime must be less than StopTime", "IResMeasRaw");

            var geoLocation = new DM.GeoLocation();
            var queryLoc = this._dataLayer.GetBuilder<MD.IResLocSensorRaw>()
            .From()
            .Select(c => c.Id, c => c.Lon, c => c.Lat, c => c.Agl, c => c.Asl)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Fetch(queryLoc, reader =>
            {
                while (reader.Read())
                {
                    if (reader.GetValue(c => c.Lon).HasValue)
                        geoLocation.Lon = reader.GetValue(c => c.Lon).Value;
                    if (reader.GetValue(c => c.Lat).HasValue)
                        geoLocation.Lat = reader.GetValue(c => c.Lat).Value;
                    geoLocation.ASL = reader.GetValue(c => c.Asl);
                    geoLocation.AGL = reader.GetValue(c => c.Agl);
                }
                return true;
            });

            if (this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IResMeasRaw"))
                measResult.Location = geoLocation;

            var builderDelLocSensor = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Delete();
            builderDelLocSensor.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Execute(builderDelLocSensor);


            #region FrequencySample
            var listFrequencySample = new List<DEV.FrequencySample>();
            var queryFrequencySample = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>()
            .From()
            .Select(c => c.Id, c => c.Freq_MHz, c => c.Level_dBm, c => c.Level_dBmkVm, c => c.LevelMin_dBm, c => c.LevelMax_dBm, c => c.OccupationPt)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Fetch(queryFrequencySample, reader =>
            {
                while (reader.Read())
                {
                    bool validationResult = true;
                    var freqSample = new DEV.FrequencySample();

                    if (reader.GetValue(c => c.Freq_MHz).HasValue && reader.GetValue(c => c.Freq_MHz) >= 0 && reader.GetValue(c => c.Freq_MHz).Value <= 400000)
                        freqSample.Freq_MHz = (float)reader.GetValue(c => c.Freq_MHz).Value;
                    else
                    {
                        WriteLog("Incorrect value Freq_MHz", "IFreqSampleRaw");
                        validationResult = false;
                    }
                    if (reader.GetValue(c => c.OccupationPt).HasValue && reader.GetValue(c => c.OccupationPt) >= 0 && reader.GetValue(c => c.OccupationPt).Value <= 100)
                        freqSample.Occupation_Pt = (float)reader.GetValue(c => c.OccupationPt).Value;
                    else
                    {
                        WriteLog("Incorrect value Freq_MHz", "IFreqSampleRaw");
                        validationResult = false;
                    }
                    if (reader.GetValue(c => c.Level_dBm).HasValue && reader.GetValue(c => c.Level_dBm).Value >= -150 && reader.GetValue(c => c.Level_dBm).Value <= 20)
                        freqSample.Level_dBm = (float)reader.GetValue(c => c.Level_dBm).Value;
                    if (reader.GetValue(c => c.Level_dBmkVm).HasValue && reader.GetValue(c => c.Level_dBmkVm).Value >= 10 && reader.GetValue(c => c.Level_dBmkVm).Value <= 140)
                        freqSample.Level_dBmkVm = (float)reader.GetValue(c => c.Level_dBmkVm).Value;
                    if (reader.GetValue(c => c.LevelMin_dBm).HasValue && reader.GetValue(c => c.LevelMin_dBm).Value >= -120 && reader.GetValue(c => c.LevelMin_dBm).Value <= 20)
                        freqSample.LevelMin_dBm = (float)reader.GetValue(c => c.LevelMin_dBm).Value;
                    if (reader.GetValue(c => c.LevelMax_dBm).HasValue && reader.GetValue(c => c.LevelMax_dBm).Value >= -120 && reader.GetValue(c => c.LevelMax_dBm).Value <= 20)
                        freqSample.LevelMax_dBm = (float)reader.GetValue(c => c.LevelMax_dBm).Value;

                    if (validationResult)
                        listFrequencySample.Add(freqSample);
                }
                return true;
            });

            var builderDelFreqSample = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>().Delete();
            builderDelFreqSample.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Execute(builderDelFreqSample);

            if (listFrequencySample.Count > 0)
                measResult.FrequencySamples = listFrequencySample.ToArray();
            else
                return false;

            return result;
            #endregion
        }
        private bool SaveMeasResultSpectrumOccupation(DEV.MeasResults measResult)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            try
            {
                queryExecuter.BeginTransaction();

                int valInsResMeas = 0;
                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                builderInsertIResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.Select(c => c.Id);
                queryExecuter.ExecuteAndFetch(builderInsertIResMeas, reader =>
                {
                    var res = reader.Read();
                    if (res)
                        valInsResMeas = reader.GetValue(c => c.Id);
                    return res;
                });

                if (valInsResMeas > 0)
                {
                    if (measResult.FrequencySamples != null)
                    {
                        var lstIns = new IQueryInsertStatement<MD.IFreqSampleRaw>[measResult.FrequencySamples.Length];
                        for (int i = 0; i < measResult.FrequencySamples.Length; i++)
                        {
                            var item = measResult.FrequencySamples[i];
                            var builderInsertFreqSampleRaw = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>().Insert();
                            builderInsertFreqSampleRaw.SetValue(c => c.Freq_MHz, item.Freq_MHz);
                            builderInsertFreqSampleRaw.SetValue(c => c.LevelMax_dBm, item.LevelMax_dBm);
                            builderInsertFreqSampleRaw.SetValue(c => c.LevelMin_dBm, item.LevelMin_dBm);
                            builderInsertFreqSampleRaw.SetValue(c => c.Level_dBm, item.Level_dBm);
                            builderInsertFreqSampleRaw.SetValue(c => c.Level_dBmkVm, item.Level_dBmkVm);
                            builderInsertFreqSampleRaw.SetValue(c => c.OccupationPt, item.Occupation_Pt);
                            builderInsertFreqSampleRaw.SetValue(c => c.ResMeasId, valInsResMeas);
                            builderInsertFreqSampleRaw.Select(c => c.Id);
                            lstIns[i] = builderInsertFreqSampleRaw;
                        }
                        queryExecuter.ExecuteAndFetch(lstIns, reader =>
                        {
                            return true;
                        });
                    }

                    var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
                    builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
                    builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
                    builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
                    builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
                    builderInsertResLocSensorMeas.SetValue(c => c.ResMeasId, valInsResMeas);
                    builderInsertResLocSensorMeas.Select(c => c.Id);
                    queryExecuter.Execute(builderInsertResLocSensorMeas);
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
        private bool VaildateMeasResultSignaling(ref DEV.MeasResults measResult, int resultId)
        {
            var result = true;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

            if (string.IsNullOrEmpty(measResult.ResultId))
            {
                WriteLog("Undefined value ResultId", "IResMeasRaw");
                result = false;
            }
            else if (measResult.ResultId.Length > 50)
                measResult.ResultId.SubString(50);

            if (string.IsNullOrEmpty(measResult.TaskId))
            {
                WriteLog("Undefined value TaskId", "IResMeasRaw");
                result = false;
            }
            else if (measResult.TaskId.Length > 200)
                measResult.TaskId.SubString(200);

            if (measResult.Status.Length > 5)
                measResult.Status = "";

            if (measResult.StartTime > measResult.StopTime)
                WriteLog("StartTime must be less than StopTime", "IResMeasRaw");

            if (!(measResult.ScansNumber >= 1 && measResult.ScansNumber <= 10000000))
                WriteLog("Incorrect value SwNumber", "IResMeasRaw");

            var geoLocation = new DM.GeoLocation();
            var queryLoc = this._dataLayer.GetBuilder<MD.IResLocSensorRaw>()
            .From()
            .Select(c => c.Id, c => c.Lon, c => c.Lat, c => c.Agl, c => c.Asl)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Fetch(queryLoc, reader =>
            {
                while (reader.Read())
                {
                    if (reader.GetValue(c => c.Lon).HasValue)
                        geoLocation.Lon = reader.GetValue(c => c.Lon).Value;
                    if (reader.GetValue(c => c.Lat).HasValue)
                        geoLocation.Lat = reader.GetValue(c => c.Lat).Value;
                    geoLocation.ASL = reader.GetValue(c => c.Asl);
                    geoLocation.AGL = reader.GetValue(c => c.Agl);
                }
                return true;
            });

            if (this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IResMeasRaw"))
                measResult.Location = geoLocation;

            var builderDelLocSensor = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Delete();
            builderDelLocSensor.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Execute(builderDelLocSensor);

            #region Emittings
            var listEmitting = new List<DEV.Emitting>();
            var queryEmitting = this._dataLayer.GetBuilder<MD.IEmittingRaw>()
            .From()
            .Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.TriggerDeviationFromReference)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Fetch(queryEmitting, reader =>
            {
                while (reader.Read())
                {
                    bool validationResult = true;
                    var emitting = new DEV.Emitting();

                    if (reader.GetValue(c => c.StartFrequency_MHz).HasValue && reader.GetValue(c => c.StartFrequency_MHz) >= 0.009 && reader.GetValue(c => c.StartFrequency_MHz).Value <= 400000)
                    {
                        WriteLog("Incorrect value StartFrequency_MHz", "IEmittingRaw");
                        validationResult = false;
                    }
                    if (reader.GetValue(c => c.StopFrequency_MHz).HasValue && reader.GetValue(c => c.StopFrequency_MHz) >= 0.009 && reader.GetValue(c => c.StopFrequency_MHz).Value <= 400000)
                    {
                        WriteLog("Incorrect value StopFrequency_MHz", "IEmittingRaw");
                        validationResult = false;
                    }
                    if (reader.GetValue(c => c.StartFrequency_MHz).HasValue && reader.GetValue(c => c.StopFrequency_MHz).HasValue && reader.GetValue(c => c.StartFrequency_MHz).Value > reader.GetValue(c => c.StopFrequency_MHz).Value)
                    {
                        WriteLog("StartFrequency_MHz must be less than StopFrequency_MHz", "IEmittingRaw");
                        validationResult = false;
                    }
                    if (reader.GetValue(c => c.StartFrequency_MHz).HasValue)
                        emitting.StartFrequency_MHz = reader.GetValue(c => c.StartFrequency_MHz).Value;
                    if (reader.GetValue(c => c.StopFrequency_MHz).HasValue)
                        emitting.StopFrequency_MHz = reader.GetValue(c => c.StopFrequency_MHz).Value;
                    if (reader.GetValue(c => c.CurentPower_dBm).HasValue && reader.GetValue(c => c.CurentPower_dBm).Value >= -200 && reader.GetValue(c => c.CurentPower_dBm).Value <= 50)
                        emitting.CurentPower_dBm = reader.GetValue(c => c.CurentPower_dBm).Value;
                    if (reader.GetValue(c => c.ReferenceLevel_dBm).HasValue && reader.GetValue(c => c.ReferenceLevel_dBm).Value >= -200 && reader.GetValue(c => c.ReferenceLevel_dBm).Value <= 50)
                        emitting.ReferenceLevel_dBm = reader.GetValue(c => c.ReferenceLevel_dBm).Value;
                    if (reader.GetValue(c => c.MeanDeviationFromReference).HasValue && reader.GetValue(c => c.MeanDeviationFromReference).Value >= 0 && reader.GetValue(c => c.MeanDeviationFromReference).Value <= 1)
                        emitting.MeanDeviationFromReference = reader.GetValue(c => c.MeanDeviationFromReference).Value;
                    if (reader.GetValue(c => c.TriggerDeviationFromReference).HasValue && reader.GetValue(c => c.TriggerDeviationFromReference).Value >= 0 && reader.GetValue(c => c.TriggerDeviationFromReference).Value <= 1)
                        emitting.TriggerDeviationFromReference = reader.GetValue(c => c.TriggerDeviationFromReference).Value;

                    var emittingParam = new DEV.EmittingParameters();

                    if (reader.GetValue(c => c.RollOffFactor).HasValue && reader.GetValue(c => c.RollOffFactor).Value >= 0 && reader.GetValue(c => c.RollOffFactor).Value <= 2.5)
                        emittingParam.RollOffFactor = reader.GetValue(c => c.RollOffFactor).Value;
                    if (reader.GetValue(c => c.StandardBW).HasValue && reader.GetValue(c => c.StandardBW).Value >= 0 && reader.GetValue(c => c.StandardBW).Value <= 1000000)
                    {
                        emittingParam.StandardBW = reader.GetValue(c => c.StandardBW).Value;
                        emitting.EmittingParameters = emittingParam;
                    }
                    else
                        WriteLog("Incorrect value StandardBW", "IEmittingRaw");

                    #region WorkTime
                    var listTime = new List<DEV.WorkTime>();
                    var queryTime = this._dataLayer.GetBuilder<MD.IWorkTimeRaw>()
                    .From()
                    .Select(c => c.Id, c => c.StartEmitting, c => c.StopEmitting, c => c.HitCount, c => c.PersentAvailability)
                    .Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(queryTime, readerTime =>
                    {
                        while (readerTime.Read())
                        {
                            bool validationTimeResult = true;
                            var workTime = new DEV.WorkTime();

                            if (readerTime.GetValue(c => c.StartEmitting).HasValue && readerTime.GetValue(c => c.StopEmitting).HasValue && readerTime.GetValue(c => c.StartEmitting).Value > readerTime.GetValue(c => c.StopEmitting).Value)
                            {
                                WriteLog("StartEmitting must be less than StopEmitting", "IWorkTimeRaw");
                                validationTimeResult = false;
                            }
                            if (readerTime.GetValue(c => c.StartEmitting).HasValue)
                                workTime.StartEmitting = readerTime.GetValue(c => c.StartEmitting).Value;
                            if (readerTime.GetValue(c => c.StopEmitting).HasValue)
                                workTime.StopEmitting = readerTime.GetValue(c => c.StopEmitting).Value;
                            if (readerTime.GetValue(c => c.HitCount).HasValue && readerTime.GetValue(c => c.HitCount).Value >= 0 && readerTime.GetValue(c => c.HitCount).Value <= Int32.MaxValue)
                                workTime.HitCount = readerTime.GetValue(c => c.HitCount).Value;
                            if (readerTime.GetValue(c => c.PersentAvailability).HasValue && readerTime.GetValue(c => c.PersentAvailability).Value >= 0 && readerTime.GetValue(c => c.PersentAvailability).Value <= 100)
                                workTime.PersentAvailability = (float)readerTime.GetValue(c => c.PersentAvailability).Value;
                            else
                            {
                                WriteLog("Incorrect value PersentAvailability", "IWorkTimeRaw");
                                validationTimeResult = false;
                            }
                            if (validationTimeResult)
                                listTime.Add(workTime);
                        }
                        return true;
                    });
                    if (listTime.Count > 0)
                        emitting.WorkTimes = listTime.ToArray();

                    var builderDelTime = this._dataLayer.GetBuilder<MD.IWorkTimeRaw>().Delete();
                    builderDelTime.Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Execute(builderDelTime);

                    #endregion

                    #region SignalMask
                    var listLoss_dB = new List<float>();
                    var listFreq_kHz = new List<double>();
                    var querySignalMask = this._dataLayer.GetBuilder<MD.ISignalMaskRaw>()
                    .From()
                    .Select(c => c.Id, c => c.Loss_dB, c => c.Freq_kHz)
                    .Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(querySignalMask, readerSignalMask =>
                    {
                        while (readerSignalMask.Read())
                        {
                            if (readerSignalMask.GetValue(c => c.Loss_dB).HasValue && readerSignalMask.GetValue(c => c.Loss_dB).Value >= -100 && readerSignalMask.GetValue(c => c.Loss_dB).Value <= 500)
                                listLoss_dB.Add((float)readerSignalMask.GetValue(c => c.Loss_dB).Value);
                            else
                                WriteLog("Incorrect value Loss_dB", "ISignalMaskRaw");
                            if (readerSignalMask.GetValue(c => c.Freq_kHz).HasValue && readerSignalMask.GetValue(c => c.Freq_kHz).Value >= -1000000 && readerSignalMask.GetValue(c => c.Freq_kHz).Value <= 1000000)
                                listFreq_kHz.Add(readerSignalMask.GetValue(c => c.Freq_kHz).Value);
                            else
                                WriteLog("Incorrect value Freq_kHz", "ISignalMaskRaw");
                        }
                        return true;
                    });

                    var signalMask = new DEV.SignalMask();
                    if (listLoss_dB.Count > 0)
                        signalMask.Loss_dB = listLoss_dB.ToArray();
                    if (listFreq_kHz.Count > 0)
                        signalMask.Freq_kHz = listFreq_kHz.ToArray();

                    emitting.SignalMask = signalMask;

                    var builderSignalDel = this._dataLayer.GetBuilder<MD.ISignalMaskRaw>().Delete();
                    builderSignalDel.Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Execute(builderSignalDel);

                    #endregion

                    #region LevelDistribution
                    var listLevel = new List<int>();
                    var listCount = new List<int>();
                    bool validationLevelResult = true;
                    var queryLevelDist = this._dataLayer.GetBuilder<MD.ILevelsDistributionRaw>()
                    .From()
                    .Select(c => c.Id, c => c.level, c => c.count)
                    .Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(queryLevelDist, readerLevelDist =>
                    {
                        while (readerLevelDist.Read())
                        {
                            if (readerLevelDist.GetValue(c => c.level).HasValue && readerLevelDist.GetValue(c => c.level).Value >= -200 && readerLevelDist.GetValue(c => c.level).Value <= 100)
                                validationLevelResult = true;
                            else
                            {
                                validationLevelResult = false;
                                WriteLog("Incorrect value readerLevelDist", "ILevelsDistributionRaw");
                            }

                            if (readerLevelDist.GetValue(c => c.count).HasValue && readerLevelDist.GetValue(c => c.count).Value >= 0 && readerLevelDist.GetValue(c => c.count).Value <= Int32.MaxValue)
                                validationLevelResult = true;
                            else
                            {
                                validationLevelResult = false;
                                WriteLog("Incorrect value readerLevelDist", "ILevelsDistributionRaw");
                            }

                            if (validationLevelResult)
                            {
                                listLevel.Add(readerLevelDist.GetValue(c => c.level).Value);
                                listCount.Add(readerLevelDist.GetValue(c => c.count).Value);
                            }
                        }
                        return true;
                    });

                    var levelDist = new DEV.LevelsDistribution();
                    if (listLevel.Count > 0)
                        levelDist.Levels = listLevel.ToArray();
                    if (listCount.Count > 0)
                        levelDist.Count = listCount.ToArray();

                    emitting.LevelsDistribution = levelDist;

                    var builderLevelDist = this._dataLayer.GetBuilder<MD.ILevelsDistributionRaw>().Delete();
                    builderLevelDist.Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Execute(builderLevelDist);

                    #endregion

                    #region Spectrum
                    bool validationSpectrumResult = true;
                    var spectrum = new DEV.Spectrum();
                    var listLevelsdBm = new List<float>();
                    var querySpectrum = this._dataLayer.GetBuilder<MD.ISpectrumRaw>()
                    .From()
                    .Select(c => c.Id, c => c.Bandwidth_kHz, c => c.CorrectnessEstimations, c => c.MarkerIndex, c => c.SignalLevel_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.T1, c => c.T2, c => c.TraceCount)
                    .Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(querySpectrum, readerSpectrum =>
                    {
                        while (readerSpectrum.Read())
                        {
                            #region LevelsdBm
                            var queryLevelsdBm = this._dataLayer.GetBuilder<MD.IDetailSpectrumLevelsRaw>()
                            .From()
                            .Select(c => c.Id, c => c.level)
                            .Where(c => c.SpectrumId, ConditionOperator.Equal, readerSpectrum.GetValue(c => c.Id));
                            queryExecuter.Fetch(queryLevelsdBm, readerLevelsdBm =>
                            {
                                while (readerLevelsdBm.Read())
                                {
                                    if (readerLevelsdBm.GetValue(c => c.level).HasValue && readerLevelsdBm.GetValue(c => c.level).Value >= -200 && readerLevelsdBm.GetValue(c => c.level).Value <= 50)
                                        listLevelsdBm.Add((float)readerLevelsdBm.GetValue(c => c.level).Value);
                                    else
                                        WriteLog("Incorrect value level", "IDetailSpectrumLevelsRaw");
                                }
                                return true;
                            });

                            if (listLevelsdBm.Count > 0)
                                spectrum.Levels_dBm = listLevelsdBm.ToArray();
                            else
                                validationSpectrumResult = false;

                            var builderLevelsdBm = this._dataLayer.GetBuilder<MD.IDetailSpectrumLevelsRaw>().Delete();
                            builderLevelsdBm.Where(c => c.SpectrumId, ConditionOperator.Equal, readerSpectrum.GetValue(c => c.Id));
                            queryExecuter.Execute(builderLevelsdBm);

                            #endregion

                            if (readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).HasValue && readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).Value >= 0.009 && readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).Value <= 400000)
                                spectrum.SpectrumStartFreq_MHz = readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz).Value;
                            else
                            {
                                WriteLog("Incorrect value SpectrumStartFreq_MHz", "ISpectrumRaw");
                                validationSpectrumResult = false;
                            }

                            if (readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).HasValue && readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).Value >= 0.001 && readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).Value <= 1000000)
                                spectrum.SpectrumSteps_kHz = readerSpectrum.GetValue(c => c.SpectrumSteps_kHz).Value;
                            else
                            {
                                WriteLog("Incorrect value SpectrumSteps_kHz", "ISpectrumRaw");
                                validationSpectrumResult = false;
                            }

                            if (readerSpectrum.GetValue(c => c.Bandwidth_kHz).HasValue && readerSpectrum.GetValue(c => c.Bandwidth_kHz).Value >= 0 && readerSpectrum.GetValue(c => c.Bandwidth_kHz).Value <= 1000000)
                                spectrum.Bandwidth_kHz = readerSpectrum.GetValue(c => c.Bandwidth_kHz).Value;
                            else
                                WriteLog("Incorrect value Bandwidth_kHz", "ISpectrumRaw");

                            if (readerSpectrum.GetValue(c => c.TraceCount).HasValue && readerSpectrum.GetValue(c => c.TraceCount).Value >= 0 && readerSpectrum.GetValue(c => c.TraceCount).Value <= 10000)
                                spectrum.TraceCount = readerSpectrum.GetValue(c => c.TraceCount).Value;
                            else
                                WriteLog("Incorrect value TraceCount", "ISpectrumRaw");

                            if (readerSpectrum.GetValue(c => c.SignalLevel_dBm).HasValue && readerSpectrum.GetValue(c => c.SignalLevel_dBm).Value >= -200 && readerSpectrum.GetValue(c => c.SignalLevel_dBm).Value <= 50)
                                spectrum.SignalLevel_dBm = (float)readerSpectrum.GetValue(c => c.SignalLevel_dBm).Value;
                            else
                                WriteLog("Incorrect value SignalLevel_dBm", "ISpectrumRaw");

                            if (readerSpectrum.GetValue(c => c.T1).HasValue && readerSpectrum.GetValue(c => c.T2).HasValue && readerSpectrum.GetValue(c => c.MarkerIndex).HasValue
                                && readerSpectrum.GetValue(c => c.T1).Value <= readerSpectrum.GetValue(c => c.MarkerIndex).Value && readerSpectrum.GetValue(c => c.MarkerIndex).Value >= readerSpectrum.GetValue(c => c.T2).Value)
                                spectrum.MarkerIndex = readerSpectrum.GetValue(c => c.MarkerIndex).Value;
                            else
                                WriteLog("Incorrect value MarkerIndex", "ISpectrumRaw");

                            if (readerSpectrum.GetValue(c => c.T1).HasValue && readerSpectrum.GetValue(c => c.T2).HasValue && readerSpectrum.GetValue(c => c.T1).Value >= 0 && readerSpectrum.GetValue(c => c.T1).Value <= readerSpectrum.GetValue(c => c.T2).Value)
                                spectrum.T1 = readerSpectrum.GetValue(c => c.T1).Value;
                            else
                            {
                                WriteLog("Incorrect value T1", "ISpectrumRaw");
                                validationSpectrumResult = false;
                            }

                            if (readerSpectrum.GetValue(c => c.T1).HasValue && readerSpectrum.GetValue(c => c.T2).HasValue && readerSpectrum.GetValue(c => c.T2).Value >= readerSpectrum.GetValue(c => c.T1).Value && readerSpectrum.GetValue(c => c.T2).Value <= spectrum.Levels_dBm.Length)
                                spectrum.T2 = readerSpectrum.GetValue(c => c.T2).Value;
                            else
                            {
                                WriteLog("Incorrect value T2", "ISpectrumRaw");
                                validationSpectrumResult = false;
                            }
                        }
                        return true;
                    });

                    if (validationSpectrumResult)
                    {
                        spectrum.Levels_dBm = listLevelsdBm.ToArray();
                        emitting.Spectrum = spectrum;
                    }

                    var builderSpectrumDel = this._dataLayer.GetBuilder<MD.ISpectrumRaw>().Delete();
                    builderSpectrumDel.Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Execute(builderSpectrumDel);

                    #endregion

                    if (validationResult)
                        listEmitting.Add(emitting);
                }
                return true;
            });

            var builderDelEmitting = this._dataLayer.GetBuilder<MD.IEmittingRaw>().Delete();
            builderDelEmitting.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Execute(builderDelEmitting);

            if (listEmitting.Count > 0)
                measResult.Emittings = listEmitting.ToArray();
            else
                return false;

            #endregion

            #region ReferenceLevels
            bool validationLevelsResult = true;
            var level = new DEV.ReferenceLevels();
            var listLevels = new List<float>();
            var queryLevels = this._dataLayer.GetBuilder<MD.IReferenceLevelsRaw>()
            .From()
            .Select(c => c.Id, c => c.StartFrequency_Hz, c => c.StepFrequency_Hz)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Fetch(queryLevels, readerLevels =>
            {
                while (readerLevels.Read())
                {
                    if (readerLevels.GetValue(c => c.StartFrequency_Hz).HasValue && readerLevels.GetValue(c => c.StartFrequency_Hz).Value >= 9000 && readerLevels.GetValue(c => c.StartFrequency_Hz).Value <= 400000000000)
                        level.StartFrequency_Hz = readerLevels.GetValue(c => c.StartFrequency_Hz).Value;
                    else
                    {
                        validationLevelsResult = false;
                        WriteLog("Incorrect value StartFrequency_Hz", "IReferenceLevelsRaw");
                    }

                    if (readerLevels.GetValue(c => c.StepFrequency_Hz).HasValue && readerLevels.GetValue(c => c.StepFrequency_Hz).Value >= 1 && readerLevels.GetValue(c => c.StepFrequency_Hz).Value <= 1000000000)
                        level.StepFrequency_Hz = readerLevels.GetValue(c => c.StepFrequency_Hz).Value;
                    else
                    {
                        validationLevelsResult = false;
                        WriteLog("Incorrect value StepFrequency_Hz", "IReferenceLevelsRaw");
                    }

                    var queryLevelsDet = this._dataLayer.GetBuilder<MD.IDetailReferenceLevelsRaw>()
                    .From()
                    .Select(c => c.Id, c => c.level)
                    .Where(c => c.ReferenceLevelId, ConditionOperator.Equal, readerLevels.GetValue(c => c.Id));
                    queryExecuter.Fetch(queryLevelsDet, readerLevelsDet =>
                    {
                        while (readerLevelsDet.Read())
                        {
                            if (readerLevelsDet.GetValue(c => c.level).HasValue && readerLevelsDet.GetValue(c => c.level).Value >= -200 && readerLevelsDet.GetValue(c => c.level).Value <= 50)
                                listLevels.Add((float)readerLevelsDet.GetValue(c => c.level).Value);
                            else
                                WriteLog("Incorrect value Level", "IDetailReferenceLevelsRaw");
                        }
                        return true;
                    });

                    if (listLevels.Count > 0)
                        level.levels = listLevels.ToArray();
                    else
                        validationLevelsResult = false;

                    var builderDelLevelsDet = this._dataLayer.GetBuilder<MD.IDetailReferenceLevelsRaw>().Delete();
                    builderDelLevelsDet.Where(c => c.ReferenceLevelId, ConditionOperator.Equal, readerLevels.GetValue(c => c.Id));
                    queryExecuter.Execute(builderDelLevelsDet);
                }
                return true;
            });

            if (validationLevelsResult)
                measResult.RefLevels = level;

            var builderLevelDel = this._dataLayer.GetBuilder<MD.IReferenceLevelsRaw>().Delete();
            builderLevelDel.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Execute(builderLevelDel);

            #endregion

            return result;
        }
        private bool SaveMeasResultSignaling(DEV.MeasResults measResult)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            try
            {
                queryExecuter.BeginTransaction();

                int valInsResMeas = 0;
                var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
                builderInsertIResMeas.SetValue(c => c.MeasResultSID, measResult.ResultId);
                builderInsertIResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
                builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
                builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
                builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
                builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
                builderInsertIResMeas.Select(c => c.Id);
                queryExecuter.ExecuteAndFetch(builderInsertIResMeas, reader =>
                {
                    var res = reader.Read();
                    if (res)
                        valInsResMeas = reader.GetValue(c => c.Id);
                    return res;
                });

                if (valInsResMeas > 0)
                {
                    if (measResult.RefLevels != null)
                    {
                        int valInsReferenceLevels = 0;
                        var refLevels = measResult.RefLevels;
                        var builderInsertReferenceLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Insert();
                        builderInsertReferenceLevels.SetValue(c => c.StartFrequency_Hz, refLevels.StartFrequency_Hz);
                        builderInsertReferenceLevels.SetValue(c => c.StepFrequency_Hz, refLevels.StepFrequency_Hz);
                        builderInsertReferenceLevels.SetValue(c => c.ResMeasId, valInsResMeas);
                        builderInsertReferenceLevels.Select(c => c.Id);
                        queryExecuter
                        .ExecuteAndFetch(builderInsertReferenceLevels, readerReferenceLevels =>
                        {
                            var res = readerReferenceLevels.Read();
                            if (res)
                            {
                                valInsReferenceLevels = readerReferenceLevels.GetValue(c => c.Id);
                                if (valInsReferenceLevels > 0)
                                {
                                    var lstInslevels = new IQueryInsertStatement<MD.IDetailReferenceLevels>[refLevels.levels.Length];
                                    for (int l = 0; l < refLevels.levels.Length; l++)
                                    {
                                        var lvl = refLevels.levels[l];

                                        var builderInsertDetailReferenceLevels = this._dataLayer.GetBuilder<MD.IDetailReferenceLevels>().Insert();
                                        builderInsertDetailReferenceLevels.SetValue(c => c.level, lvl);
                                        builderInsertDetailReferenceLevels.SetValue(c => c.ReferenceLevelId, valInsReferenceLevels);
                                        builderInsertDetailReferenceLevels.Select(c => c.Id);
                                        lstInslevels[l] = builderInsertDetailReferenceLevels;
                                    }
                                    queryExecuter.ExecuteAndFetch(lstInslevels, readerDetailReferenceLevels =>
                                    {
                                        return true;
                                    });
                                }

                            }
                            return true;
                        });
                    }
                    if (measResult.Emittings != null)
                    {
                        var emittings = measResult.Emittings;
                        for (int l = 0; l < emittings.Length; l++)
                        {
                            int valInsReferenceEmitting = 0;
                            var builderInsertEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Insert();
                            builderInsertEmitting.SetValue(c => c.CurentPower_dBm, emittings[l].CurentPower_dBm);
                            builderInsertEmitting.SetValue(c => c.MeanDeviationFromReference, emittings[l].MeanDeviationFromReference);
                            builderInsertEmitting.SetValue(c => c.ReferenceLevel_dBm, emittings[l].ReferenceLevel_dBm);
                            builderInsertEmitting.SetValue(c => c.ResMeasId, valInsResMeas);
                            if (emittings[l].EmittingParameters != null)
                            {
                                builderInsertEmitting.SetValue(c => c.RollOffFactor, emittings[l].EmittingParameters.RollOffFactor);
                                builderInsertEmitting.SetValue(c => c.StandardBW, emittings[l].EmittingParameters.StandardBW);
                            }
                            builderInsertEmitting.SetValue(c => c.StartFrequency_MHz, emittings[l].StartFrequency_MHz);
                            builderInsertEmitting.SetValue(c => c.StopFrequency_MHz, emittings[l].StopFrequency_MHz);
                            builderInsertEmitting.SetValue(c => c.TriggerDeviationFromReference, emittings[l].TriggerDeviationFromReference);
                            builderInsertEmitting.Select(c => c.Id);
                            queryExecuter
                            .ExecuteAndFetch(builderInsertEmitting, readerEmitting =>
                            {
                                var res = readerEmitting.Read();
                                if (res)
                                {
                                    valInsReferenceEmitting = readerEmitting.GetValue(c => c.Id);
                                    if (valInsReferenceEmitting > 0)
                                    {
                                        var workTimes = emittings[l].WorkTimes;
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

                                        var spectrum = emittings[l].Spectrum;
                                        if (spectrum != null)
                                        {
                                            int valInsSpectrum = 0;

                                            var builderInsertISpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>().Insert();
                                            builderInsertISpectrum.SetValue(c => c.EmittingId, valInsReferenceEmitting);
                                            builderInsertISpectrum.SetValue(c => c.CorrectnessEstimations, spectrum.СorrectnessEstimations);
                                            builderInsertISpectrum.SetValue(c => c.Bandwidth_kHz, spectrum.Bandwidth_kHz);
                                            builderInsertISpectrum.SetValue(c => c.MarkerIndex, spectrum.MarkerIndex);
                                            builderInsertISpectrum.SetValue(c => c.SignalLevel_dBm, spectrum.SignalLevel_dBm);
                                            builderInsertISpectrum.SetValue(c => c.SpectrumStartFreq_MHz, spectrum.SpectrumStartFreq_MHz);
                                            builderInsertISpectrum.SetValue(c => c.SpectrumSteps_kHz, spectrum.SpectrumSteps_kHz);
                                            builderInsertISpectrum.SetValue(c => c.T1, spectrum.T1);
                                            builderInsertISpectrum.SetValue(c => c.T2, spectrum.T2);
                                            builderInsertISpectrum.SetValue(c => c.TraceCount, spectrum.TraceCount);
                                            builderInsertISpectrum.Select(c => c.Id);
                                            queryExecuter
                                            .ExecuteAndFetch(builderInsertISpectrum, readerISpectrum =>
                                            {
                                                var resSpectrum = readerISpectrum.Read();
                                                if (resSpectrum)
                                                {
                                                    valInsSpectrum = readerISpectrum.GetValue(c => c.Id);
                                                    if (valInsSpectrum > 0)
                                                    {
                                                        var lstInsLevels_dBm = new IQueryInsertStatement<MD.IDetailSpectrumLevels>[spectrum.Levels_dBm.Length];
                                                        for (int k = 0; k < spectrum.Levels_dBm.Length; k++)
                                                        {
                                                            var level_dBm = spectrum.Levels_dBm[k];

                                                            var builderInsertIDetailReferenceLevels = this._dataLayer.GetBuilder<MD.IDetailSpectrumLevels>().Insert();
                                                            builderInsertIDetailReferenceLevels.SetValue(c => c.level, level_dBm);
                                                            builderInsertIDetailReferenceLevels.SetValue(c => c.SpectrumId, valInsSpectrum);
                                                            builderInsertIDetailReferenceLevels.Select(c => c.Id);
                                                            lstInsLevels_dBm[k] = builderInsertIDetailReferenceLevels;
                                                        }
                                                        queryExecuter.ExecuteAndFetch(lstInsLevels_dBm, readerDetailSpectrumLevels =>
                                                        {
                                                            return true;
                                                        });
                                                    }
                                                }
                                                return true;
                                            });
                                        }

                                        var signalMask = emittings[l].SignalMask;
                                        if (signalMask != null)
                                        {
                                            var lstInsSignalMask = new IQueryInsertStatement<MD.ISignalMask>[signalMask.Freq_kHz.Length];
                                            for (int k = 0; k < signalMask.Freq_kHz.Length; k++)
                                            {
                                                var freq_kH = signalMask.Freq_kHz[k];
                                                var loss_dB = signalMask.Loss_dB[k];

                                                var builderInsertSignalMask = this._dataLayer.GetBuilder<MD.ISignalMask>().Insert();
                                                builderInsertSignalMask.SetValue(c => c.Freq_kHz, freq_kH);
                                                builderInsertSignalMask.SetValue(c => c.Loss_dB, loss_dB);
                                                builderInsertSignalMask.SetValue(c => c.EmittingId, valInsReferenceEmitting);
                                                builderInsertSignalMask.Select(c => c.Id);
                                                lstInsSignalMask[k] = builderInsertSignalMask;
                                            }
                                            queryExecuter.ExecuteAndFetch(lstInsSignalMask, readerSignalMask =>
                                            {
                                                return true;
                                            });
                                        }

                                        var levelsDistribution = emittings[l].LevelsDistribution;
                                        if (levelsDistribution != null)
                                        {
                                            var lstInsLevelsDistribution = new IQueryInsertStatement<MD.ILevelsDistribution>[levelsDistribution.Levels.Length];
                                            for (int k = 0; k < levelsDistribution.Levels.Length; k++)
                                            {
                                                var lvl = levelsDistribution.Levels[k];
                                                var count = levelsDistribution.Count[k];
                                                var builderInsertLevelsDistribution = this._dataLayer.GetBuilder<MD.ILevelsDistribution>().Insert();
                                                builderInsertLevelsDistribution.SetValue(c => c.level, lvl);
                                                builderInsertLevelsDistribution.SetValue(c => c.count, count);
                                                builderInsertLevelsDistribution.SetValue(c => c.EmittingId, valInsReferenceEmitting);
                                                builderInsertLevelsDistribution.Select(c => c.Id);
                                                lstInsLevelsDistribution[k] = builderInsertLevelsDistribution;
                                            }
                                            queryExecuter.ExecuteAndFetch(lstInsLevelsDistribution, readerLevelsDistribution =>
                                            {
                                                return true;
                                            });


                                        }
                                    }
                                }
                                return true;
                            });
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

        private bool ValidateGeoLocation<T>(T location, string tableName)
            where T : DM.GeoLocation
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