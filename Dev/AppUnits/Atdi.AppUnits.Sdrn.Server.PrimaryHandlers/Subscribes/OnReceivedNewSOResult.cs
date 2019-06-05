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
                this._logger.Verbouse(Contexts.PrimaryHandler, Categories.OnReceivedNewSOResultEvent, Events.StartOperationWriting);
                bool validationResult = true;
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var measResult = new DEV.MeasResults();
                var queryResMeas = this._dataLayer.GetBuilder<MD.IResMeasRaw>()
                .From()
                .Select(c => c.Id, c => c.MeasResultSID, c => c.MeasTaskId, c => c.TimeMeas, c => c.Status, c => c.TypeMeasurements, c => c.DataRank, c => c.ScansNumber, c => c.StartTime, c => c.StopTime, c => c.SensorId)
                .Where(c => c.Id, ConditionOperator.Equal, @event.ResultId);
                queryExecuter.Fetch(queryResMeas, reader =>
                {
                    var result = reader.Read();
                    if (result)
                    {
                        if ((!Enum.TryParse(reader.GetValue(c => c.TypeMeasurements), out DM.MeasurementType measurement)) || (string.IsNullOrEmpty(reader.GetValue(c => c.TypeMeasurements))))
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
                        if (reader.GetValue(c => c.SensorId).HasValue)
                            measResult.SensorId = reader.GetValue(c => c.SensorId).Value;
                    }
                    return result;
                });

                //var builderDelMeas = this._dataLayer.GetBuilder<MD.IResMeasRaw>().Delete();
                //builderDelMeas.Where(c => c.Id, ConditionOperator.Equal, @event.ResultId);
                //queryExecuter.Execute(builderDelMeas);

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
                        if (SaveMeasResultSignaling(measResult, out int newResMeasId, out int newResSensorId))
                        {
                            DeleteOldMeasResultSignaling(measResult, newResMeasId, newResSensorId);
                        }
                    }
                }
                this._logger.Verbouse(Contexts.PrimaryHandler, Categories.OnReceivedNewSOResultEvent, Events.EndOperationWriting);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
            }
        }
        private bool VaildateMeasResultMonitoringStations(ref DEV.MeasResults measResult, int resultId)
        {
            int? sensorId = -1;
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

            if (!(measResult.ScansNumber >= 0 && measResult.ScansNumber <= 10000000))
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
                    var route = new DEV.Route() { RouteId = reader.GetValue(c => c.RouteId) };

                    var listRoutePoints = new List<DEV.RoutePoint>();
                    var routePoint = new DEV.RoutePoint();

                    var lon = reader.GetValue(c => c.Lon);
                    var lat = reader.GetValue(c => c.Lat);
                    if (lon.HasValue)
                        routePoint.Lon = lon.Value;
                    if (lat.HasValue)
                        routePoint.Lat = lat.Value;
                    routePoint.ASL = reader.GetValue(c => c.Asl);
                    routePoint.AGL = reader.GetValue(c => c.Agl);

                    validationResult = this.ValidateGeoLocation<DEV.RoutePoint>(routePoint, "IResRoutesRaw");

                    if (Enum.TryParse(reader.GetValue(c => c.PointStayType), out DM.PointStayType pst))
                        routePoint.PointStayType = pst;

                    var startTime = reader.GetValue(c => c.StartTime);
                    var finishTime = reader.GetValue(c => c.FinishTime);
                    if (startTime.HasValue)
                        routePoint.StartTime = startTime.Value;
                    if (finishTime.HasValue)
                        routePoint.FinishTime = finishTime.Value;

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
            if (listRoutes.Count >= 0)
                measResult.Routes = listRoutes.ToArray();
            else
                result = false;

            //var builderDelRoute = this._dataLayer.GetBuilder<MD.IResRoutesRaw>().Delete();
            //builderDelRoute.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            //queryExecuter.Execute(builderDelRoute);

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
                    if (sensorId == -1)
                    {
                        var builderLinkResSensorRaw = this._dataLayer.GetBuilder<MD.ILinkResSensorRaw>().From();
                        builderLinkResSensorRaw.Select(c => c.Id, c => c.SensorId);
                        builderLinkResSensorRaw.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c=>c.Id));
                        queryExecuter.Fetch(builderLinkResSensorRaw, readerLinkResSensorRaw =>
                        {
                            while (readerLinkResSensorRaw.Read())
                            {
                                sensorId = readerLinkResSensorRaw.GetValue(c => c.SensorId);
                                break;
                            }
                            return true;
                        });
                    }

                    var measStation = new DEV.StationMeasResult();
                    var stationId = reader.GetValue(c => c.StationId);
                    if (stationId.HasValue)
                        measStation.StationId = stationId.Value.ToString().SubString(50);
                    measStation.TaskGlobalSid = reader.GetValue(c => c.GlobalSID).SubString(50);
                    measStation.RealGlobalSid = reader.GetValue(c => c.MeasGlobalSID).SubString(50);

                    var sectorId = reader.GetValue(c => c.SectorId);
                    if (sectorId.HasValue)
                        measStation.SectorId = sectorId.Value.ToString().SubString(50);
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

                            var lon = readerLev.GetValue(c => c.Lon);
                            var lat = readerLev.GetValue(c => c.Lat);

                            if (lon.HasValue)
                                geoLocation.Lon = lon.Value;
                            if (lat.HasValue)
                                geoLocation.Lat = lat.Value;
                            geoLocation.ASL = readerLev.GetValue(c => c.Altitude);
                            geoLocation.AGL = readerLev.GetValue(c => c.Agl);

                            validationResult = this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IResStLevelCarRaw");
                            if (validationResult)
                                levelMeasResult.Location = geoLocation;

                            var levelDbm = readerLev.GetValue(c => c.LevelDbm);
                            if (levelDbm.HasValue && levelDbm >= -150 && levelDbm <= 20)
                                levelMeasResult.Level_dBm = levelDbm.Value;
                            else
                            {
                                if (levelDbm.HasValue)
                                {
                                    WriteLog("Incorrect value LevelDbm", "IResStLevelCarRaw");
                                    validationResult = false;
                                }
                            }

                            var levelDbmkvm = readerLev.GetValue(c => c.LevelDbmkvm);
                            if (levelDbmkvm.HasValue && levelDbmkvm >= -10 && levelDbmkvm <= 140)
                                levelMeasResult.Level_dBmkVm = levelDbmkvm.Value;
                            else
                            {
                                if (levelDbmkvm.HasValue)
                                {
                                    WriteLog("Incorrect value LevelDbmkvm", "IResStLevelCarRaw");
                                    validationResult = false;
                                }
                            }

                            var timeOfMeasurements = readerLev.GetValue(c => c.TimeOfMeasurements);
                            if (timeOfMeasurements.HasValue)
                                levelMeasResult.MeasurementTime = timeOfMeasurements.Value;
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

                    //var builderDelResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCarRaw>().Delete();
                    //builderDelResStLevelCar.Where(c => c.ResStationId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    //queryExecuter.Execute(builderDelResStLevelCar);
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

                            var lon = readerData.GetValue(c => c.Lon);
                            var lat = readerData.GetValue(c => c.Lat);  
                            if (lon.HasValue)
                                geoLocation.Lon = lon.Value;
                            if (lat.HasValue)
                                geoLocation.Lat = lat.Value;
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

                    //var builderDelBearing = this._dataLayer.GetBuilder<MD.IBearingRaw>().Delete();
                    //builderDelBearing.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    //queryExecuter.Execute(builderDelBearing);
                    #endregion

                    #region GeneralMeasResult
                    var queryGeneralMeasResult = this._dataLayer.GetBuilder<MD.IResStGeneralRaw>()
                    .From()
                    .Select(c => c.Id, c => c.CentralFrequency, c => c.CentralFrequencyMeas, c => c.OffsetFrequency, c => c.SpecrumStartFreq, c => c.SpecrumSteps, c => c.T1, c => c.T2, c => c.MarkerIndex, c => c.Correctnessestim, c => c.TraceCount, c => c.DurationMeas, c => c.TimeStartMeas, c => c.TimeFinishMeas, c => c.Rbw,  c => c.Vbw, c => c.BW)
                    .Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(queryGeneralMeasResult, readerGeneralResult =>
                    {
                        bool removeGroup1 = false;
                        while (readerGeneralResult.Read())
                        {
                            var generalMeasResult = new DEV.GeneralMeasResult();

                            var centralFrequency = readerGeneralResult.GetValue(c => c.CentralFrequency);
                            if (centralFrequency.HasValue && centralFrequency >= 0.001 && centralFrequency <= 400000)
                                generalMeasResult.CentralFrequency_MHz = centralFrequency.Value;

                            var centralFrequencyMeas = readerGeneralResult.GetValue(c => c.CentralFrequencyMeas);
                            if (centralFrequencyMeas.HasValue && centralFrequencyMeas >= 0.001 && centralFrequencyMeas <= 400000)
                                generalMeasResult.CentralFrequencyMeas_MHz = centralFrequencyMeas.Value;

                            generalMeasResult.OffsetFrequency_mk = readerGeneralResult.GetValue(c => c.OffsetFrequency);

                            var specrumStartFreq = readerGeneralResult.GetValue(c => c.SpecrumStartFreq);
                            if (specrumStartFreq >= 0.001m && specrumStartFreq <= 400000m)
                                generalMeasResult.SpectrumStartFreq_MHz = specrumStartFreq;
                            else
                                removeGroup1 = true;

                            var specrumSteps = readerGeneralResult.GetValue(c => c.SpecrumSteps);

                            if (specrumSteps >= 0.001m && specrumSteps <= 100000m)
                                generalMeasResult.SpectrumSteps_kHz = specrumSteps;
                            else
                                removeGroup1 = true;

                            if (readerGeneralResult.GetValue(c => c.DurationMeas).HasValue)
                            {
                                generalMeasResult.MeasDuration_sec = readerGeneralResult.GetValue(c => c.DurationMeas).Value;
                            }
                            generalMeasResult.MeasStartTime = readerGeneralResult.GetValue(c => c.TimeStartMeas);
                            generalMeasResult.MeasFinishTime = readerGeneralResult.GetValue(c => c.TimeFinishMeas);
                            generalMeasResult.RBW_kHz = readerGeneralResult.GetValue(c => c.Rbw);
                            generalMeasResult.VBW_kHz = readerGeneralResult.GetValue(c => c.Vbw);

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

                                var markerIndex = readerGeneralResult.GetValue(c => c.MarkerIndex);
                                var t1 = readerGeneralResult.GetValue(c => c.T1);
                                var t2 = readerGeneralResult.GetValue(c => c.T2);

                                if (markerIndex.HasValue && t1.HasValue && t2.HasValue)
                                {
                                    if (!(t1.Value >= 0 && t1.Value <= markerIndex.Value
                                        && t2.Value >= markerIndex.Value && t2.Value <= 100000
                                        && markerIndex.Value >= t1.Value && markerIndex.Value <= t2.Value))
                                    {
                                        isValidBandwith = false;
                                    }
                                }
                                else
                                    isValidBandwith = false;

                                if (isValidBandwith)
                                {
                                    bandwidthMeasResult.T1 = t1.Value;
                                    bandwidthMeasResult.T2 = t2.Value;
                                    bandwidthMeasResult.Bandwidth_kHz = readerGeneralResult.GetValue(c => c.BW);
                                    bandwidthMeasResult.MarkerIndex = markerIndex;

                                    var correctnessestim = readerGeneralResult.GetValue(c => c.Correctnessestim);
                                    if (correctnessestim.HasValue)
                                        bandwidthMeasResult.СorrectnessEstimations = correctnessestim.Value == 1 ? true : false;
                                    else
                                        bandwidthMeasResult.СorrectnessEstimations = false;

                                    var traceCount = readerGeneralResult.GetValue(c => c.TraceCount);
                                    if (traceCount.HasValue && traceCount.Value >= 1 && traceCount.Value <= 100000)
                                        bandwidthMeasResult.TraceCount = traceCount.Value;


                                    generalMeasResult.BandwidthResult = bandwidthMeasResult;
                                }

                                #endregion

                                var listStLevelsSpect = new List<float>();
                                var queryStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpectRaw>()
                                .From()
                                .Select(c => c.Id, c => c.LevelSpecrum)
                                .OrderByAsc(c => c.Id)
                                .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                                queryExecuter.Fetch(queryStLevelsSpect, readerStLevelsSpect =>
                                {
                                    while (readerStLevelsSpect.Read())
                                    {
                                        var levelSpecrum = readerStLevelsSpect.GetValue(c => c.LevelSpecrum);
                                        if (levelSpecrum.HasValue)
                                        {
                                            listStLevelsSpect.Add(levelSpecrum.Value);
                                        }
                                    }
                                    return true;
                                });
                                generalMeasResult.LevelsSpectrum_dBm = listStLevelsSpect.ToArray();
                            }

                            //var builderDelResStLevels = this._dataLayer.GetBuilder<MD.IResStLevelsSpectRaw>().Delete();
                            //builderDelResStLevels.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                            //queryExecuter.Execute(builderDelResStLevels);

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

                                    var level = readerElementsMask.GetValue(c => c.Level);
                                    if (level.HasValue && level.Value >= -300 && level.Value <= 300)
                                        elementMask.Level_dB = level;
                                    else
                                        isValidElementMask = false;

                                    var bw = readerElementsMask.GetValue(c => c.Bw);
                                    if (bw.HasValue && bw.Value >= 1 && bw.Value <= 200000)
                                        elementMask.BW_kHz = bw;
                                    else
                                        isValidElementMask = false;

                                    if (isValidElementMask)
                                        listElementsMask.Add(elementMask);
                                }
                                return true;
                            });
                            generalMeasResult.BWMask = listElementsMask.ToArray();

                            //var builderDelMaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElementRaw>().Delete();
                            //builderDelMaskElem.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                            //queryExecuter.Execute(builderDelMaskElem);
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

                                    var lat = readerStationSysInfo.GetValue(c => c.Lat);
                                    var lon = readerStationSysInfo.GetValue(c => c.Lon);
                                    if (lat.HasValue)
                                        location.Lat = lat.Value;
                                    if (lon.HasValue)
                                        location.Lon = lon.Value;
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
                                    .Select(c => c.Id, c => c.BinData, c => c.Type)
                                    .Where(c => c.ResSysInfoId, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
                                    queryExecuter.Fetch(queryStationSysInfoBls, readerStationSysInfoBls =>
                                    {
                                        while (readerStationSysInfoBls.Read())
                                        {
                                            var stationSysInfoBls = new DEV.StationSysInfoBlock()
                                            {
                                                Data = BinaryDecoder.Deserialize<string>(readerStationSysInfoBls.GetValue(c => c.BinData)),
                                                Type = readerStationSysInfoBls.GetValue(c => c.Type)
                                            };
                                            listStationSysInfoBls.Add(stationSysInfoBls);
                                        }

                                        //var builderDelResSysInfoBls = this._dataLayer.GetBuilder<MD.IResSysInfoBlsRaw>().Delete();
                                        //builderDelResSysInfoBls.Where(c => c.ResSysInfoId, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
                                        //queryExecuter.Execute(builderDelResSysInfoBls);

                                        return true;
                                    });

                                    stationSysInfo.InfoBlocks = listStationSysInfoBls.ToArray();
                                    generalMeasResult.StationSysInfo = stationSysInfo;
                                }
                                return true;
                            });

                            //var builderDelResSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfoRaw>().Delete();
                            //builderDelResSysInfo.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
                            //queryExecuter.Execute(builderDelResSysInfo);
                            #endregion

                            measStation.GeneralResult = generalMeasResult;
                        }
                        return true;
                    });

                    #endregion

                    listStationMeasResult.Add(measStation);

                    //var builderDelLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensorRaw>().Delete();
                    //builderDelLinkResSensor.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    //queryExecuter.Execute(builderDelLinkResSensor);

                    //var builderDelResGeneral = this._dataLayer.GetBuilder<MD.IResStGeneralRaw>().Delete();
                    //builderDelResGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    //queryExecuter.Execute(builderDelResGeneral);
                }

                return true;
            });

            //var builderDelStation = this._dataLayer.GetBuilder<MD.IResMeasStaRaw>().Delete();
            //builderDelStation.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            //queryExecuter.Execute(builderDelStation);
       

            measResult.SensorId = sensorId;

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
                var listStationIdsTemp = new List<int>();
                //var dic = new List<KeyValuePair<string, string>>();
                var dic = new Dictionary<string, string>();
                queryExecuter.BeginTransaction();

                int idResMeas = 0;
                bool isMerge = false;
                double? diffDates = null;

                int subMeasTaskId = -1; int subMeasTaskStaId = -1; int sensorId = -1; int resultId = -1;
                GetIds(measResult.ResultId, measResult.TaskId, out subMeasTaskId, out subMeasTaskStaId, out sensorId, out resultId);

                var builderResMeasSearch = this._dataLayer.GetBuilder<MD.IResMeas>().From();
                builderResMeasSearch.Select(c => c.Id, c => c.TimeMeas, c => c.DataRank);
                builderResMeasSearch.OrderByAsc(c => c.Id);
                builderResMeasSearch.Where(c => c.MeasTaskId, ConditionOperator.Equal, measResult.TaskId);
                //builderResMeasSearch.Where(c => c.Status, ConditionOperator.IsNull);
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
                    builderUpdateResMeas.SetValue(c => c.MeasResultSID, resultId!=-1 ? resultId.ToString() : measResult.ResultId);
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
                    builderInsertIResMeas.SetValue(c => c.SensorId, measResult.SensorId!=null ? measResult.SensorId : sensorId);
                    builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
                    builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
                    builderInsertIResMeas.Select(c => c.Id);
                    queryExecuter.ExecuteAndFetch(builderInsertIResMeas, reader =>
                    {
                        var res = reader.Read();
                        if (res)
                        {
                            idResMeas = reader.GetValue(c => c.Id);
                        }
                        return res;
                    });

                    
                }

                if (idResMeas>0)
                {
                    var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Insert();
                    builderInsertLinkResSensor.SetValue(c => c.ResMeasStaId, idResMeas);
                    builderInsertLinkResSensor.SetValue(c => c.SensorId, measResult.SensorId);
                    builderInsertLinkResSensor.Select(c => c.Id);
                    queryExecuter
                    .ExecuteAndFetch(builderInsertLinkResSensor, reader =>
                    {
                        return true;
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

                            //if ((!string.IsNullOrEmpty(station.RealGlobalSid)) && (dic.Find(x=>x.Key==station.RealGlobalSid && x.Value==station.Standard).Key == null))
                            if ((!string.IsNullOrEmpty(station.RealGlobalSid)) && !dic.ContainsKey(station.RealGlobalSid + @"/|\" + station.Standard))
                            {
                                //dic.Add(new KeyValuePair<string, string>(station.RealGlobalSid, station.Standard));
                                dic.Add(station.RealGlobalSid + @"/|\" + station.Standard, "");

                                var builderResMeasStationSearch = this._dataLayer.GetBuilder<MD.IResMeasStation>().From();
                                builderResMeasStationSearch.Select(c => c.Id);
                                builderResMeasStationSearch.Where(c => c.MeasGlobalSID, ConditionOperator.Equal, station.RealGlobalSid);
                                builderResMeasStationSearch.Where(c => c.Standard, ConditionOperator.Equal, station.Standard);
                                queryExecuter.Fetch(builderResMeasStationSearch, readerResMeasStation =>
                                {
                                    while (readerResMeasStation.Read())
                                    {
                                        //if (!listStationIdsTemp.Contains(readerResMeasStation.GetValue(c => c.Id)))
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
                                    }
                                    return true;
                                });
                            }

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

                                bool isUpdate = false;

                                var builderUpdateMeasResult = this._dataLayer.GetBuilder<MD.IResMeasStation>().Update();
                                if (!string.IsNullOrEmpty(station.StationId) && int.TryParse(station.StationId, out int Idstation))
                                {
                                    builderUpdateMeasResult.SetValue(c => c.StationId, Idstation);
                                    isUpdate = true;
                                }
                                if (!string.IsNullOrEmpty(station.SectorId) && int.TryParse(station.SectorId, out int IdSector))
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
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.Rbw, generalResult.RBW_kHz);
                                        isUpdate = true;
                                    }
                                    if (generalResult.VBW_kHz.HasValue)
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.Vbw, generalResult.VBW_kHz);
                                        isUpdate = true;
                                    }
                                    if (generalResult.CentralFrequencyMeas_MHz.HasValue)
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.CentralFrequencyMeas, generalResult.CentralFrequencyMeas_MHz);
                                        isUpdate = true;
                                    }
                                    if (generalResult.CentralFrequency_MHz.HasValue)
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.CentralFrequency, generalResult.CentralFrequency_MHz);
                                        isUpdate = true;
                                    }
                                    if (generalResult.MeasDuration_sec.HasValue)
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.DurationMeas, generalResult.MeasDuration_sec);
                                        isUpdate = true;
                                    }
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
                                        isUpdate = true;
                                    }
                                    if (generalResult.OffsetFrequency_mk.HasValue)
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
                                        isUpdate = true;
                                    }
                                    if (generalResult.SpectrumStartFreq_MHz.HasValue)
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.SpecrumStartFreq, generalResult.SpectrumStartFreq_MHz);
                                        isUpdate = true;
                                    }
                                    if (generalResult.SpectrumSteps_kHz.HasValue)
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.SpecrumSteps, generalResult.SpectrumSteps_kHz);
                                        isUpdate = true;
                                    }
                                    if (generalResult.MeasStartTime.HasValue && startTime.HasValue && generalResult.MeasStartTime.Value < startTime)
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.TimeStartMeas, generalResult.MeasStartTime);
                                        isUpdate = true;
                                    }
                                    if (generalResult.MeasFinishTime.HasValue && finishTime.HasValue && generalResult.MeasFinishTime.Value > finishTime)
                                    {
                                        builderUpdateResStGeneral.SetValue(c => c.TimeFinishMeas, generalResult.MeasFinishTime);
                                        isUpdate = true;
                                    }
                                    if (isUpdate)
                                    {
                                        builderUpdateResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, idMeasResultStation);
                                        queryExecuter.Execute(builderUpdateResStGeneral);
                                        isUpdate = false;
                                    }
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
                                                builderInsertStationSysInfoBlock.SetValue(c => c.BinData, BinaryDecoder.ObjectToByteArray(blocks.Data));
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
                            else
                            {
                                var listStationIds = new List<int>();
                                int valInsResMeasStation = 0;

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
                                    station = measResult.StationResults[n];
                                    var builderInsertResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().Insert();
                                    builderInsertResMeasStation.SetValue(c => c.Status, station.Status);
                                    builderInsertResMeasStation.SetValue(c => c.MeasGlobalSID, station.RealGlobalSid);
                                    builderInsertResMeasStation.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                                    builderInsertResMeasStation.SetValue(c => c.ResMeasId, idResMeas);
                                    builderInsertResMeasStation.SetValue(c => c.Standard, station.Standard);
                                    if (int.TryParse(station.StationId, out int Idstation))
                                    {
                                        builderInsertResMeasStation.SetValue(c => c.StationId, Idstation);
                                    }
                                    if (int.TryParse(station.SectorId, out int IdSector))
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
                                        int idLinkRes = -1;

                                        //if (int.TryParse(station.StationId, out int StationId))
                                        //{
                                        //var builderILinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Delete();
                                        //builderILinkResSensor.Where(c => c.ResMeasStaId, ConditionOperator.Equal, valInsResMeasStation);
                                        //builderILinkResSensor.Where(c => c.SensorId, ConditionOperator.Equal, StationId);
                                        //queryExecuter.Execute(builderILinkResSensor);
                                        //}

                                        //if (int.TryParse(station.StationId, out int StationId))
                                        //{
                                        var builderLinkResSensorRaw = this._dataLayer.GetBuilder<MD.ILinkResSensor>().From();
                                        builderLinkResSensorRaw.Select(c => c.Id);
                                        builderLinkResSensorRaw.Where(c => c.ResMeasStaId, ConditionOperator.Equal, valInsResMeasStation);
                                        //builderLinkResSensorRaw.Where(c => c.SensorId, ConditionOperator.Equal, StationId);
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
                                        //}

                                        if (idLinkRes == -1)
                                        {
                                            var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Insert();
                                            builderInsertLinkResSensor.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                            builderInsertLinkResSensor.SetValue(c => c.SensorId, measResult.SensorId);
                                            //builderInsertLinkResSensor.SetValue(c => c.SensorId, StationId);
                                            //if (int.TryParse(station.StationId, out StationId))
                                            //{
                                            //builderInsertLinkResSensor.SetValue(c => c.SensorId, StationId);
                                            //}
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
                                        }

                                        var generalResult = station.GeneralResult;
                                        if (generalResult != null)
                                        {
                                            //var builderIResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().Delete();
                                            //builderIResStGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, valInsResMeasStation);
                                            //queryExecuter.Execute(builderIResStGeneral);

                                            int IDResGeneral = -1;

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



                                                var builderInsertResStGeneral = this._dataLayer.GetBuilder<MD.IResStGeneral>().Insert();
                                                builderInsertResStGeneral.SetValue(c => c.Rbw, generalResult.RBW_kHz);
                                                builderInsertResStGeneral.SetValue(c => c.Vbw, generalResult.VBW_kHz);
                                                builderInsertResStGeneral.SetValue(c => c.CentralFrequencyMeas, generalResult.CentralFrequencyMeas_MHz);
                                                builderInsertResStGeneral.SetValue(c => c.CentralFrequency, generalResult.CentralFrequency_MHz);
                                                builderInsertResStGeneral.SetValue(c => c.DurationMeas, generalResult.MeasDuration_sec);
                                                if (generalResult.BandwidthResult != null)
                                                {
                                                    var bandwidthResult = generalResult.BandwidthResult;
                                                    builderInsertResStGeneral.SetValue(c => c.BW, bandwidthResult.Bandwidth_kHz);
                                                    builderInsertResStGeneral.SetValue(c => c.MarkerIndex, bandwidthResult.MarkerIndex);
                                                    builderInsertResStGeneral.SetValue(c => c.T1, bandwidthResult.T1);
                                                    builderInsertResStGeneral.SetValue(c => c.T2, bandwidthResult.T2);
                                                    builderInsertResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                                                    builderInsertResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations == true ? 1 : 0);
                                                }
                                                builderInsertResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
                                                builderInsertResStGeneral.SetValue(c => c.SpecrumStartFreq, generalResult.SpectrumStartFreq_MHz);
                                                builderInsertResStGeneral.SetValue(c => c.SpecrumSteps, generalResult.SpectrumSteps_kHz);
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

                                                    //var builderDelSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfo>().Delete();
                                                    //builderDelSysInfo.Where(c => c.ResStGeneralId, ConditionOperator.Equal, IDResGeneral);
                                                    //queryExecuter.Execute(builderDelSysInfo);
                                                    int IDResSysInfoGeneral = -1;
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

                                                    if (IDResSysInfoGeneral == -1)
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
                                                                        builderInsertStationSysInfoBlock.SetValue(c => c.BinData, BinaryDecoder.ObjectToByteArray(blocks.Data));
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
                                                                var lvl = station.GeneralResult.LevelsSpectrum_dBm[l];
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
                                                            for (int v = 0; v < listBearings.Length; v++)
                                                            {
                                                                DEV.DirectionFindingData directionFindingData = listBearings[v];
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
                        }
                    }
                    else
                    {
                        for (int n = 0; n < measResult.StationResults.Length; n++)
                        {
                            int valInsResMeasStation = 0;
                            DEV.StationMeasResult station = measResult.StationResults[n];
                            var builderInsertResMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStation>().Insert();
                            builderInsertResMeasStation.SetValue(c => c.Status, station.Status);
                            builderInsertResMeasStation.SetValue(c => c.MeasGlobalSID, station.RealGlobalSid);
                            builderInsertResMeasStation.SetValue(c => c.GlobalSID, station.TaskGlobalSid);
                            builderInsertResMeasStation.SetValue(c => c.ResMeasId, idResMeas);
                            builderInsertResMeasStation.SetValue(c => c.Standard, station.Standard);
                            if (int.TryParse(station.StationId, out int Idstation))
                            {
                                builderInsertResMeasStation.SetValue(c => c.StationId, Idstation);
                            }
                            if (int.TryParse(station.SectorId, out int IdSector))
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
                                int idLinkRes = -1;

                                //if (int.TryParse(station.StationId, out int StationId))
                                {
                                    var builderLinkResSensorRaw = this._dataLayer.GetBuilder<MD.ILinkResSensor>().From();
                                    builderLinkResSensorRaw.Select(c => c.Id);
                                    builderLinkResSensorRaw.Where(c => c.ResMeasStaId, ConditionOperator.Equal, valInsResMeasStation);
                                    //builderLinkResSensorRaw.Where(c => c.SensorId, ConditionOperator.Equal, StationId);
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
                                }

                                if (idLinkRes == -1)
                                {
                                    var builderInsertLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensor>().Insert();
                                    builderInsertLinkResSensor.SetValue(c => c.ResMeasStaId, valInsResMeasStation);
                                    builderInsertLinkResSensor.SetValue(c => c.SensorId, measResult.SensorId);
                                    //if (int.TryParse(station.StationId, out int StationId))
                                    //{
                                    //builderInsertLinkResSensor.SetValue(c => c.SensorId, StationId);
                                    //}
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
                                }


                                var generalResult = station.GeneralResult;
                                if (generalResult != null)
                                {
                                    int IDResGeneral = -1;

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
                                            builderInsertResStGeneral.SetValue(c => c.BW, bandwidthResult.Bandwidth_kHz);
                                            builderInsertResStGeneral.SetValue(c => c.TraceCount, bandwidthResult.TraceCount);
                                            builderInsertResStGeneral.SetValue(c => c.Correctnessestim, bandwidthResult.СorrectnessEstimations == true ? 1 : 0);
                                        }
                                        builderInsertResStGeneral.SetValue(c => c.OffsetFrequency, generalResult.OffsetFrequency_mk);
                                        builderInsertResStGeneral.SetValue(c => c.SpecrumStartFreq, generalResult.SpectrumStartFreq_MHz);
                                        builderInsertResStGeneral.SetValue(c => c.SpecrumSteps, generalResult.SpectrumSteps_kHz);
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
                                            int IDResSysInfoGeneral = -1;
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

                                            if (IDResSysInfoGeneral == -1)
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
                                                                builderInsertStationSysInfoBlock.SetValue(c => c.BinData, BinaryDecoder.ObjectToByteArray(blocks.Data));
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
                                                        var lvl = station.GeneralResult.LevelsSpectrum_dBm[l];
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
                    var lon = reader.GetValue(c => c.Lon);
                    var lat = reader.GetValue(c => c.Lat);

                    if (lon.HasValue)
                        geoLocation.Lon = lon.Value;
                    if (lat.HasValue)
                        geoLocation.Lat = lat.Value;
                    geoLocation.ASL = reader.GetValue(c => c.Asl);
                    geoLocation.AGL = reader.GetValue(c => c.Agl);
                }
                return true;
            });

            if (this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IResMeasRaw"))
                measResult.Location = geoLocation;

            //var builderDelLocSensor = this._dataLayer.GetBuilder<MD.IResLocSensorRaw>().Delete();
            //builderDelLocSensor.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            //queryExecuter.Execute(builderDelLocSensor);


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

                    var freq_MHz = reader.GetValue(c => c.Freq_MHz);
                    if (freq_MHz.HasValue && freq_MHz >= 0 && freq_MHz.Value <= 400000)
                        freqSample.Freq_MHz = freq_MHz.Value;
                    else
                    {
                        WriteLog("Incorrect value Freq_MHz", "IFreqSampleRaw");
                        validationResult = false;
                    }

                    var occupationPt = reader.GetValue(c => c.OccupationPt);
                    if (occupationPt.HasValue && occupationPt >= 0 && occupationPt.Value <= 100)
                        freqSample.Occupation_Pt = occupationPt.Value;
                    else
                    {
                        WriteLog("Incorrect value Freq_MHz", "IFreqSampleRaw");
                        validationResult = false;
                    }

                    if (validationResult)
                    {
                        var level_dBm = reader.GetValue(c => c.Level_dBm);
                        if (level_dBm.HasValue && level_dBm.Value >= -150 && level_dBm.Value <= 20)
                            freqSample.Level_dBm = level_dBm.Value;

                        var level_dBmkVm = reader.GetValue(c => c.Level_dBmkVm);
                        if (level_dBmkVm.HasValue && level_dBmkVm.Value >= 10 && level_dBmkVm.Value <= 140)
                            freqSample.Level_dBmkVm = level_dBmkVm.Value;

                        var levelMin_dBm = reader.GetValue(c => c.LevelMin_dBm);
                        if (levelMin_dBm.HasValue && levelMin_dBm.Value >= -120 && levelMin_dBm.Value <= 20)
                            freqSample.LevelMin_dBm = levelMin_dBm.Value;

                        var levelMax_dBm = reader.GetValue(c => c.LevelMax_dBm);
                        if (levelMax_dBm.HasValue && levelMax_dBm.Value >= -120 && levelMax_dBm.Value <= 20)
                            freqSample.LevelMax_dBm = levelMax_dBm.Value;

                        listFrequencySample.Add(freqSample);
                    }
                }
                return true;
            });

            //var builderDelFreqSample = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>().Delete();
            //builderDelFreqSample.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            //queryExecuter.Execute(builderDelFreqSample);

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

                int subMeasTaskId = -1; int subMeasTaskStaId = -1; int sensorId = -1; int resultId = -1;
                GetIds(measResult.ResultId, measResult.TaskId, out subMeasTaskId, out subMeasTaskStaId, out sensorId, out resultId);

                int valInsResMeas = 0;
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
                        var lstIns = new IQueryInsertStatement<MD.IResLevels>[measResult.FrequencySamples.Length];
                        for (int i = 0; i < measResult.FrequencySamples.Length; i++)
                        {
                            var item = measResult.FrequencySamples[i];

                            var builderInsertResLevels = this._dataLayer.GetBuilder<MD.IResLevels>().Insert();
                            builderInsertResLevels.SetValue(c => c.FreqMeas, item.Freq_MHz);
                            builderInsertResLevels.SetValue(c => c.VMMaxLvl, item.LevelMax_dBm);
                            builderInsertResLevels.SetValue(c => c.VMinLvl, item.LevelMin_dBm);
                            builderInsertResLevels.SetValue(c => c.ValueLvl, item.Level_dBm);
                            builderInsertResLevels.SetValue(c => c.ValueSpect, item.Level_dBmkVm);
                            builderInsertResLevels.SetValue(c => c.OccupancySpect, item.Occupation_Pt);
                            builderInsertResLevels.SetValue(c => c.ResMeasId, valInsResMeas);
                            builderInsertResLevels.Select(c => c.Id);
                            lstIns[i] = builderInsertResLevels;
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
                    var lon = reader.GetValue(c => c.Lon);
                    var lat = reader.GetValue(c => c.Lat);

                    if (lon.HasValue)
                        geoLocation.Lon = lon.Value;
                    if (lat.HasValue)
                        geoLocation.Lat = lat.Value;
                    geoLocation.ASL = reader.GetValue(c => c.Asl);
                    geoLocation.AGL = reader.GetValue(c => c.Agl);
                }
                return true;
            });

            if (this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IResMeasRaw"))
                measResult.Location = geoLocation;

            //var builderDelLocSensor = this._dataLayer.GetBuilder<MD.IResLocSensorRaw>().Delete();
            //builderDelLocSensor.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            //queryExecuter.Execute(builderDelLocSensor);

            #region Emittings
            var listEmitting = new List<DEV.Emitting>();
            var queryEmitting = this._dataLayer.GetBuilder<MD.IEmittingRaw>()
            .From()
            .Select(c => c.Id, c => c.CurentPower_dBm, c => c.MeanDeviationFromReference, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.TriggerDeviationFromReference, c => c.LevelsDistribution, c =>c.SensorId)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Fetch(queryEmitting, reader =>
            {
                while (reader.Read())
                {
                    bool validationResult = true;
                    var listLevel = new List<int>();
                    var listCount = new List<int>();
                    bool validationLevelResult = true;
                    var emitting = new DEV.Emitting();

                    var startFrequency_MHz = reader.GetValue(c => c.StartFrequency_MHz);
                    var stopFrequency_MHz = reader.GetValue(c => c.StopFrequency_MHz);
                    if ((startFrequency_MHz.HasValue && !(startFrequency_MHz >= 0.009 && startFrequency_MHz.Value <= 400000)) || (!startFrequency_MHz.HasValue))
                    {
                        WriteLog("Incorrect value StartFrequency_MHz", "IEmittingRaw");
                        validationResult = false;
                    }
                    if ((stopFrequency_MHz.HasValue && !(stopFrequency_MHz >= 0.009 && stopFrequency_MHz.Value <= 400000)) || (!stopFrequency_MHz.HasValue))
                    {
                        WriteLog("Incorrect value StopFrequency_MHz", "IEmittingRaw");
                        validationResult = false;
                    }
                    if (startFrequency_MHz.HasValue && stopFrequency_MHz.HasValue && startFrequency_MHz.Value > stopFrequency_MHz.Value)
                    {
                        WriteLog("StartFrequency_MHz must be less than StopFrequency_MHz", "IEmittingRaw");
                        validationResult = false;
                    }

                    if (!validationResult)
                        continue;

                    if (startFrequency_MHz.HasValue)
                        emitting.StartFrequency_MHz = startFrequency_MHz.Value;
                    if (stopFrequency_MHz.HasValue)
                        emitting.StopFrequency_MHz = stopFrequency_MHz.Value;

                    var curentPower_dBm = reader.GetValue(c => c.CurentPower_dBm);
                    if (curentPower_dBm.HasValue && curentPower_dBm.Value >= -200 && curentPower_dBm.Value <= 50)
                        emitting.CurentPower_dBm = curentPower_dBm.Value;

                    var referenceLevel_dBm = reader.GetValue(c => c.ReferenceLevel_dBm);
                    if (referenceLevel_dBm.HasValue && referenceLevel_dBm.Value >= -200 && referenceLevel_dBm.Value <= 50)
                        emitting.ReferenceLevel_dBm = referenceLevel_dBm.Value;

                    var meanDeviationFromReference = reader.GetValue(c => c.MeanDeviationFromReference);
                    if (meanDeviationFromReference.HasValue && meanDeviationFromReference.Value >= 0 && meanDeviationFromReference.Value <= 1)
                        emitting.MeanDeviationFromReference = meanDeviationFromReference.Value;

                    var triggerDeviationFromReference = reader.GetValue(c => c.TriggerDeviationFromReference);
                    if (triggerDeviationFromReference.HasValue && triggerDeviationFromReference.Value >= 0 && triggerDeviationFromReference.Value <= 1)
                        emitting.TriggerDeviationFromReference = triggerDeviationFromReference.Value;

                    if (reader.GetValue(c => c.SensorId).HasValue)
                        emitting.SensorId = reader.GetValue(c => c.SensorId).Value;

                    var levelsDistribution = reader.GetValue(c => c.LevelsDistribution);

                    if (levelsDistribution != null)
                    {
                        var objLevelsDistribution = BinaryDecoder.Deserialize<string>(levelsDistribution);
                        if (!string.IsNullOrEmpty(objLevelsDistribution))
                        {
                            var wrds = objLevelsDistribution.Split(new char[] { ';'}, StringSplitOptions.RemoveEmptyEntries);
                            if ((wrds!=null) && (wrds.Length>0))
                            {
                                for (int h=0; h< wrds.Length; h++)
                                {
                                    var oneString = wrds[h];
                                    if (!string.IsNullOrEmpty(oneString))
                                    {
                                        var wrdLevelCount = oneString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                        if ((wrdLevelCount != null) && (wrdLevelCount.Length == 2))
                                        {
                                            if ((int.TryParse(wrdLevelCount[0], out int levelValue)) && (int.TryParse(wrdLevelCount[1], out int countValue)))
                                            {
                                                if (levelValue >= -200 && levelValue <= 100)
                                                    validationLevelResult = true;
                                                else
                                                {
                                                    validationLevelResult = false;
                                                    WriteLog("Incorrect value readerLevelDist", "ILevelsDistributionRaw");
                                                }

                                                if (countValue >= 0 && countValue <= Int32.MaxValue)
                                                    validationLevelResult = true;
                                                else
                                                {
                                                    validationLevelResult = false;
                                                    WriteLog("Incorrect value readerLevelDist", "ILevelsDistributionRaw");
                                                }

                                                if (validationLevelResult)
                                                {
                                                    listLevel.Add(levelValue);
                                                    listCount.Add(countValue);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var emittingParam = new DEV.EmittingParameters();

                    var rollOffFactor = reader.GetValue(c => c.RollOffFactor);
                    if (rollOffFactor.HasValue && rollOffFactor.Value >= 0 && rollOffFactor.Value <= 2.5)
                        emittingParam.RollOffFactor = rollOffFactor.Value;

                    var standardBW = reader.GetValue(c => c.StandardBW);
                    if (standardBW.HasValue && standardBW.Value >= 0 && standardBW.Value <= 1000000)
                    {
                        emittingParam.StandardBW = standardBW.Value;
                        emitting.EmittingParameters = emittingParam;
                    }
                    else if (standardBW.HasValue && !(standardBW.Value >= 0 && standardBW.Value <= 1000000))
                    {
                        WriteLog("Incorrect value StandardBW", "IEmittingRaw");
                    }

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

                            var startEmitting = readerTime.GetValue(c => c.StartEmitting);
                            var stopEmitting = readerTime.GetValue(c => c.StopEmitting);

                            if (startEmitting.HasValue && stopEmitting.HasValue && startEmitting.Value > stopEmitting.Value)
                            {
                                WriteLog("StartEmitting must be less than StopEmitting", "IWorkTimeRaw");
                                validationTimeResult = false;
                            }

                            if (!validationTimeResult)
                                continue;

                            if (startEmitting.HasValue)
                                workTime.StartEmitting = startEmitting.Value;
                            if (stopEmitting.HasValue)
                                workTime.StopEmitting = stopEmitting.Value;

                            var hitCount = readerTime.GetValue(c => c.HitCount);
                            if (hitCount.HasValue && hitCount.Value >= 0 && hitCount.Value <= Int32.MaxValue)
                                workTime.HitCount = hitCount.Value;

                            var persentAvailability = readerTime.GetValue(c => c.PersentAvailability);
                            if (persentAvailability.HasValue && persentAvailability.Value >= 0 && persentAvailability.Value <= 100)
                                workTime.PersentAvailability = persentAvailability.Value;
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

                    //var builderDelTime = this._dataLayer.GetBuilder<MD.IWorkTimeRaw>().Delete();
                    //builderDelTime.Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    //queryExecuter.Execute(builderDelTime);

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
                            var loss_dB = readerSignalMask.GetValue(c => c.Loss_dB);
                            var freq_kHz = readerSignalMask.GetValue(c => c.Freq_kHz);

                            if (loss_dB.HasValue && loss_dB.Value >= -100 && loss_dB.Value <= 500)
                                listLoss_dB.Add(loss_dB.Value);
                            else
                                WriteLog("Incorrect value Loss_dB", "ISignalMaskRaw");
                            if (freq_kHz.HasValue && freq_kHz.Value >= -1000000 && freq_kHz.Value <= 1000000)
                                listFreq_kHz.Add(freq_kHz.Value);
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

                    //var builderSignalDel = this._dataLayer.GetBuilder<MD.ISignalMaskRaw>().Delete();
                    //builderSignalDel.Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    //queryExecuter.Execute(builderSignalDel);

                    #endregion

                    #region LevelDistribution

                    var levelDist = new DEV.LevelsDistribution();
                    if (listLevel.Count > 0)
                        levelDist.Levels = listLevel.ToArray();
                    if (listCount.Count > 0)
                        levelDist.Count = listCount.ToArray();

                    emitting.LevelsDistribution = levelDist;

                    #endregion

                    #region Spectrum
                    bool validationSpectrumResult = true;
                    var spectrum = new DEV.Spectrum();
                    var listLevelsdBm = new List<float>();
                    var querySpectrum = this._dataLayer.GetBuilder<MD.ISpectrumRaw>()
                    .From()
                    .Select(c => c.Id, c => c.Bandwidth_kHz, c => c.CorrectnessEstimations, c => c.MarkerIndex, c => c.SignalLevel_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.T1, c => c.T2, c => c.TraceCount, c => c.LevelsdBm, c => c.Contravention)
                    .Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    queryExecuter.Fetch(querySpectrum, readerSpectrum =>
                    {
                        while (readerSpectrum.Read())
                        {
                            #region LevelsdBm

                            var levelsdBmB = readerSpectrum.GetValue(c => c.LevelsdBm);
                            if (levelsdBmB != null)
                            {
                                object levelsdBm = BinaryDecoder.Deserialize<float[]>(levelsdBmB);
                                if (levelsdBm != null)
                                {
                                    var lvldBms = levelsdBm as float[];
                                    for (int k=0; k< lvldBms.Length; k++)
                                    {
                                        if (lvldBms[k] >= -200 && lvldBms[k] <= 50)
                                            listLevelsdBm.Add((float)lvldBms[k]);
                                        else
                                            WriteLog("Incorrect value level", "ISpectrumRaw");
                                    }
                                }
                            }

                            if (listLevelsdBm.Count > 0)
                                spectrum.Levels_dBm = listLevelsdBm.ToArray();
                            else
                                validationSpectrumResult = false;

                            #endregion
                            var correctnessEstimations = readerSpectrum.GetValue(c => c.CorrectnessEstimations);
                            if (correctnessEstimations.HasValue)
                                spectrum.СorrectnessEstimations = correctnessEstimations.Value == 1 ? true : false;

                            var contravention = readerSpectrum.GetValue(c => c.Contravention);
                            if (contravention.HasValue)
                                spectrum.Contravention = contravention.Value == 1 ? true : false;

                            var spectrumStartFreq_MHz = readerSpectrum.GetValue(c => c.SpectrumStartFreq_MHz);
                            if (spectrumStartFreq_MHz.HasValue && spectrumStartFreq_MHz.Value >= 0.009 && spectrumStartFreq_MHz.Value <= 400000)
                                spectrum.SpectrumStartFreq_MHz = spectrumStartFreq_MHz.Value;
                            else
                            {
                                WriteLog("Incorrect value SpectrumStartFreq_MHz", "ISpectrumRaw");
                                validationSpectrumResult = false;
                            }

                            var spectrumSteps_kHz = readerSpectrum.GetValue(c => c.SpectrumSteps_kHz);
                            if (spectrumSteps_kHz.HasValue && spectrumSteps_kHz.Value >= 0.001 && spectrumSteps_kHz.Value <= 1000000)
                                spectrum.SpectrumSteps_kHz = spectrumSteps_kHz.Value;
                            else
                            {
                                WriteLog("Incorrect value SpectrumSteps_kHz", "ISpectrumRaw");
                                validationSpectrumResult = false;
                            }

                            var bandwidth_kHz = readerSpectrum.GetValue(c => c.Bandwidth_kHz);
                            if (bandwidth_kHz.HasValue && bandwidth_kHz.Value >= 0 && bandwidth_kHz.Value <= 1000000)
                                spectrum.Bandwidth_kHz = bandwidth_kHz.Value;
                            else
                                WriteLog("Incorrect value Bandwidth_kHz", "ISpectrumRaw");

                            var traceCount = readerSpectrum.GetValue(c => c.TraceCount);
                            if (traceCount.HasValue && traceCount.Value >= 0 && traceCount.Value <= 10000)
                                spectrum.TraceCount = traceCount.Value;
                            else
                                WriteLog("Incorrect value TraceCount", "ISpectrumRaw");

                            var signalLevel_dBm = readerSpectrum.GetValue(c => c.SignalLevel_dBm);
                            if (signalLevel_dBm.HasValue && signalLevel_dBm.Value >= -200 && signalLevel_dBm.Value <= 50)
                                spectrum.SignalLevel_dBm = signalLevel_dBm.Value;
                            else
                                WriteLog("Incorrect value SignalLevel_dBm", "ISpectrumRaw");

                            var t1 = readerSpectrum.GetValue(c => c.T1);
                            var t2 = readerSpectrum.GetValue(c => c.T2);
                            var markerIndex = readerSpectrum.GetValue(c => c.MarkerIndex);

                            if (t1.HasValue && t2.HasValue && markerIndex.HasValue && t1.Value <= markerIndex.Value && markerIndex.Value <= t2.Value)
                                spectrum.MarkerIndex = markerIndex.Value;
                            else
                                WriteLog("Incorrect value MarkerIndex", "ISpectrumRaw");

                            if (t1.HasValue && t2.HasValue && t1.Value >= 0 && t1.Value <= t2.Value)
                                spectrum.T1 = t1.Value;
                            else
                            {
                                WriteLog("Incorrect value T1", "ISpectrumRaw");
                                validationSpectrumResult = false;
                            }

                            if (t1.HasValue && t2.HasValue && t2.Value >= t1.Value && t2.Value <= spectrum.Levels_dBm.Length)
                                spectrum.T2 = t2.Value;
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

                    //var builderSpectrumDel = this._dataLayer.GetBuilder<MD.ISpectrumRaw>().Delete();
                    //builderSpectrumDel.Where(c => c.EmittingId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
                    //queryExecuter.Execute(builderSpectrumDel);

                    #endregion

                    listEmitting.Add(emitting);
                }
                return true;
            });

            //var builderDelEmitting = this._dataLayer.GetBuilder<MD.IEmittingRaw>().Delete();
            //builderDelEmitting.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            //queryExecuter.Execute(builderDelEmitting);

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
            .Select(c => c.Id, c => c.StartFrequency_Hz, c => c.StepFrequency_Hz, c => c.ReferenceLevels)
            .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            queryExecuter.Fetch(queryLevels, readerLevels =>
            {
                while (readerLevels.Read())
                {
                    var startFrequency_Hz = readerLevels.GetValue(c => c.StartFrequency_Hz);
                    if (startFrequency_Hz.HasValue && startFrequency_Hz.Value >= 9000 && startFrequency_Hz.Value <= 400000000000)
                        level.StartFrequency_Hz = startFrequency_Hz.Value;
                    else
                    {
                        validationLevelsResult = false;
                        WriteLog("Incorrect value StartFrequency_Hz", "IReferenceLevelsRaw");
                    }

                    var stepFrequency_Hz = readerLevels.GetValue(c => c.StepFrequency_Hz);
                    if (stepFrequency_Hz.HasValue && stepFrequency_Hz.Value >= 1 && stepFrequency_Hz.Value <= 1000000000)
                        level.StepFrequency_Hz = stepFrequency_Hz.Value;
                    else
                    {
                        validationLevelsResult = false;
                        WriteLog("Incorrect value StepFrequency_Hz", "IReferenceLevelsRaw");
                    }

                    object refLevels = BinaryDecoder.Deserialize<float[]>(readerLevels.GetValue(c => c.ReferenceLevels));
                    if (refLevels != null)
                    {
                        var lvls = refLevels as float[];
                        for (int k = 0; k < lvls.Length; k++)
                        {
                            if (lvls[k] >= -200 && lvls[k] <= 50)
                                listLevels.Add(lvls[k]);
                            else
                                WriteLog("Incorrect value Level", "IDetailReferenceLevelsRaw");
                        }
                    }


                    if (listLevels.Count > 0)
                        level.levels = listLevels.ToArray();
                    else
                        validationLevelsResult = false;
                    
                }
                return true;
            });

            if (validationLevelsResult)
                measResult.RefLevels = level;

            //var builderLevelDel = this._dataLayer.GetBuilder<MD.IReferenceLevelsRaw>().Delete();
            //builderLevelDel.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
            //queryExecuter.Execute(builderLevelDel);

            #endregion

            return result;
        }
        private bool SaveMeasResultSignaling(DEV.MeasResults measResult, out int ResMeasId, out int ResSensorId)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            int valInsResMeas = 0;
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
                        int valInsReferenceLevels = 0;
                        var refLevels = measResult.RefLevels;
                        var builderInsertReferenceLevels = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Insert();
                        builderInsertReferenceLevels.SetValue(c => c.StartFrequency_Hz, refLevels.StartFrequency_Hz);
                        builderInsertReferenceLevels.SetValue(c => c.StepFrequency_Hz, refLevels.StepFrequency_Hz);
                        if (refLevels.levels != null)
                        {
                            builderInsertReferenceLevels.SetValue(c => c.ReferenceLevels, BinaryDecoder.ObjectToByteArray(refLevels.levels));
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
                        var emittings = measResult.Emittings;
                        for (int l = 0; l < emittings.Length; l++)
                        {
                            int valInsReferenceEmitting = 0;
                            var builderInsertEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Insert();
                            builderInsertEmitting.SetValue(c => c.CurentPower_dBm, emittings[l].CurentPower_dBm);
                            builderInsertEmitting.SetValue(c => c.MeanDeviationFromReference, emittings[l].MeanDeviationFromReference);
                            builderInsertEmitting.SetValue(c => c.ReferenceLevel_dBm, emittings[l].ReferenceLevel_dBm);
                            builderInsertEmitting.SetValue(c => c.ResMeasId, valInsResMeas);
                            builderInsertEmitting.SetValue(c => c.SensorId, emittings[l].SensorId);
                            if (emittings[l].EmittingParameters != null)
                            {
                                builderInsertEmitting.SetValue(c => c.RollOffFactor, emittings[l].EmittingParameters.RollOffFactor);
                                builderInsertEmitting.SetValue(c => c.StandardBW, emittings[l].EmittingParameters.StandardBW);
                            }
                            builderInsertEmitting.SetValue(c => c.StartFrequency_MHz, emittings[l].StartFrequency_MHz);
                            builderInsertEmitting.SetValue(c => c.StopFrequency_MHz, emittings[l].StopFrequency_MHz);
                            builderInsertEmitting.SetValue(c => c.TriggerDeviationFromReference, emittings[l].TriggerDeviationFromReference);
                            var levelsDistribution = emittings[l].LevelsDistribution;
                            if (levelsDistribution != null)
                            {
                                var outListStrings = new List<string>();
                                for (int p = 0; p < levelsDistribution.Levels.Length; p++)
                                {
                                    outListStrings.Add(string.Format("{0} {1}", levelsDistribution.Levels[p], levelsDistribution.Count[p]));
                                }
                                var outString = string.Join(";", outListStrings);
                                builderInsertEmitting.SetValue(c => c.LevelsDistribution, BinaryDecoder.ObjectToByteArray(outString));
                            }
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
                                            builderInsertISpectrum.SetValue(c => c.CorrectnessEstimations, spectrum.СorrectnessEstimations==true ? 1: 0);
                                            builderInsertISpectrum.SetValue(c => c.Contravention, spectrum.Contravention == true ? 1 : 0);
                                            builderInsertISpectrum.SetValue(c => c.Bandwidth_kHz, spectrum.Bandwidth_kHz);
                                            builderInsertISpectrum.SetValue(c => c.MarkerIndex, spectrum.MarkerIndex);
                                            builderInsertISpectrum.SetValue(c => c.SignalLevel_dBm, spectrum.SignalLevel_dBm);
                                            builderInsertISpectrum.SetValue(c => c.SpectrumStartFreq_MHz, spectrum.SpectrumStartFreq_MHz);
                                            builderInsertISpectrum.SetValue(c => c.SpectrumSteps_kHz, spectrum.SpectrumSteps_kHz);
                                            builderInsertISpectrum.SetValue(c => c.T1, spectrum.T1);
                                            builderInsertISpectrum.SetValue(c => c.T2, spectrum.T2);
                                            builderInsertISpectrum.SetValue(c => c.TraceCount, spectrum.TraceCount);
                                            if (spectrum.Levels_dBm != null)
                                            {
                                                builderInsertISpectrum.SetValue(c => c.LevelsdBm, BinaryDecoder.ObjectToByteArray(spectrum.Levels_dBm));
                                            }

                                            builderInsertISpectrum.Select(c => c.Id);
                                            queryExecuter
                                            .ExecuteAndFetch(builderInsertISpectrum, readerISpectrum =>
                                            {
                                                var resSpectrum = readerISpectrum.Read();
                                                if (resSpectrum)
                                                {
                                                    valInsSpectrum = readerISpectrum.GetValue(c => c.Id);
                                                }
                                                return true;
                                            });
                                        }

                                        var signalMask = emittings[l].SignalMask;
                                        if (signalMask != null)
                                        {
                                            if (signalMask.Freq_kHz != null)
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
                                        }

                                   }
                                }
                                return true;
                            });
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
        private bool DeleteOldMeasResultSignaling(DEV.MeasResults measResult, int ResMeasId, int ResSensorId)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            try
            {
                queryExecuter.BeginTransaction();

                bool isCopyRefLev = false;

                var queryResMeas = this._dataLayer.GetBuilder<MD.IResMeas>()
                .From()
                .Select(c => c.Id, c => c.TimeMeas)
                .Where(c => c.MeasTaskId, ConditionOperator.Equal, measResult.TaskId)
                .Where(c => c.SensorId, ConditionOperator.Equal, ResSensorId)
                .Where(c => c.Id, ConditionOperator.NotEqual, ResMeasId)
                .OrderByDesc(c => c.TimeMeas);
                queryExecuter.Fetch(queryResMeas, reader =>
                {
                    while (reader.Read())
                    {
                        int ResOldMeasId = reader.GetValue(c => c.Id);
                        DateTime? timeMeas = reader.GetValue(c => c.TimeMeas);

                        if (timeMeas.HasValue && measResult.Measured.Year == timeMeas.Value.Year && measResult.Measured.Month == timeMeas.Value.Month && measResult.Measured.Day == timeMeas.Value.Day && measResult.Location != null)
                        {
                            var queryLoc = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>()
                            .From()
                            .Select(c => c.Id, c => c.Lon, c => c.Lat)
                            .Where(c => c.ResMeasId, ConditionOperator.Equal, ResOldMeasId);
                            queryExecuter.Fetch(queryLoc, readerLoc =>
                            {
                                while (readerLoc.Read())
                                {
                                    double? lon = readerLoc.GetValue(c => c.Lon);
                                    double? lat = readerLoc.GetValue(c => c.Lat);

                                    if (lon.HasValue && lat.HasValue && Math.Abs(measResult.Location.Lon - lon.Value) <= 0.0004 && Math.Abs(measResult.Location.Lat - lat.Value) <= 0.0004)
                                    {
                                        if (measResult.RefLevels == null)
                                        {
                                            if (!isCopyRefLev)
                                            {
                                                var builderUpdateRefLev = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Update();
                                                builderUpdateRefLev.SetValue(c => c.ResMeasId, ResMeasId);
                                                builderUpdateRefLev.Where(c => c.ResMeasId, ConditionOperator.Equal, ResOldMeasId);

                                                if (queryExecuter.Execute(builderUpdateRefLev) > 0)
                                                    isCopyRefLev = true;
                                            }
                                        }

                                        var builderLevelDel = this._dataLayer.GetBuilder<MD.IReferenceLevels>().Delete();
                                        builderLevelDel.Where(c => c.ResMeasId, ConditionOperator.Equal, ResOldMeasId);
                                        queryExecuter.Execute(builderLevelDel);

                                        var builderDelLocSensor = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Delete();
                                        builderDelLocSensor.Where(c => c.ResMeasId, ConditionOperator.Equal, ResOldMeasId);
                                        queryExecuter.Execute(builderDelLocSensor);

                                        var queryEmitting = this._dataLayer.GetBuilder<MD.IEmittingRaw>()
                                        .From()
                                        .Select(c => c.Id)
                                        .Where(c => c.ResMeasId, ConditionOperator.Equal, ResOldMeasId);
                                        queryExecuter.Fetch(queryEmitting, readerEmitt =>
                                        {
                                            while (readerEmitt.Read())
                                            {
                                                var builderDelTime = this._dataLayer.GetBuilder<MD.IWorkTime>().Delete();
                                                builderDelTime.Where(c => c.EmittingId, ConditionOperator.Equal, readerEmitt.GetValue(c => c.Id));
                                                queryExecuter.Execute(builderDelTime);

                                                var builderSignalDel = this._dataLayer.GetBuilder<MD.ISignalMask>().Delete();
                                                builderSignalDel.Where(c => c.EmittingId, ConditionOperator.Equal, readerEmitt.GetValue(c => c.Id));
                                                queryExecuter.Execute(builderSignalDel);

                                                var builderSpectrumDel = this._dataLayer.GetBuilder<MD.ISpectrum>().Delete();
                                                builderSpectrumDel.Where(c => c.EmittingId, ConditionOperator.Equal, readerEmitt.GetValue(c => c.Id));
                                                queryExecuter.Execute(builderSpectrumDel);
                                            }
                                            return true;
                                        });

                                        var builderDelEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Delete();
                                        builderDelEmitting.Where(c => c.ResMeasId, ConditionOperator.Equal, ResOldMeasId);
                                        queryExecuter.Execute(builderDelEmitting);

                                        var builderResMeasDel = this._dataLayer.GetBuilder<MD.IResMeas>().Delete();
                                        builderResMeasDel.Where(c => c.Id, ConditionOperator.Equal, ResOldMeasId);
                                        queryExecuter.Execute(builderResMeasDel);

                                    }
                                }
                                return true;
                            });
                        }
                    }
                    return true;
                });

              

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
                    int SubTaskIdTemp = -1;
                    int SubTaskStationIdTemp = -1;
                    int SensorIdTemp = -1;
                    int taskId = -1;
                    if (int.TryParse(TaskId, out taskId))
                    {
                        var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                        var builderFromIMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().From();
                        builderFromIMeasSubTaskSta.Select(c => c.Id, c => c.MEASSUBTASK.Id, c => c.SensorId, c => c.MEASSUBTASK.TimeStart);
                        builderFromIMeasSubTaskSta.Where(c => c.MEASSUBTASK.MEASTASK.Id, ConditionOperator.Equal, taskId);
                        builderFromIMeasSubTaskSta.OrderByDesc(c => c.MEASSUBTASK.TimeStart);
                        queryExecuter.Fetch(builderFromIMeasSubTaskSta, reader =>
                        {
                            while (reader.Read())
                            {
                                SubTaskIdTemp = reader.GetValue(c => c.MEASSUBTASK.Id);
                                SubTaskStationIdTemp = reader.GetValue(c => c.Id);
                                if (reader.GetValue(c => c.SensorId).HasValue)
                                {
                                    SensorIdTemp = reader.GetValue(c => c.SensorId).Value;
                                }
                                break;
                            }
                            return true;
                        });
                        subMeasTaskId = SubTaskIdTemp;
                        subMeasTaskStaId = SubTaskStationIdTemp;
                        sensorId = SensorIdTemp;
                    }
                }
            }
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