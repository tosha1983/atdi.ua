using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;
using Atdi.DataModels.Api.EventSystem;
using DM = Atdi.DataModels.Sdrns.Device;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendMeasResultsDeviceBusEvent", SubscriberName = "SendMeasResultsSubscriber")]
    public class SendMeasResultsSubscriber : SubscriberBase<DM.MeasResults>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        public SendMeasResultsSubscriber(IMessagesSite messagesSite, ILogger logger, IDataLayer<EntityDataOrm> dataLayer) : base(messagesSite, logger)
        {
            this._dataLayer = dataLayer;
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.MeasResults deliveryObject)
        {
            try
            {
                this._logger.Verbouse(Contexts.ThisComponent, Categories.EventProcessing, Events.StartOperationWriting);

                bool validationResult = true;
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();


                //if (deliveryObject.Measurement == MeasurementType.MonitoringStations)
                //{
                //    validationResult = VaildateMeasResultMonitoringStations(deliveryObject);
                //    if (validationResult)
                //    {
                //        SaveMeasResultMonitoringStations(deliveryObject);
                //    }
                //}
                //if (deliveryObject.Measurement == MeasurementType.SpectrumOccupation)
                //{
                //    validationResult = VaildateMeasResultSpectrumOccupation(deliveryObject);
                //    if (validationResult)
                //    {
                //        SaveMeasResultSpectrumOccupation(deliveryObject);
                //    }
                //}
                //if (deliveryObject.Measurement == MeasurementType.Signaling)
                //{
                //    validationResult = VaildateMeasResultSignaling(deliveryObject);
                //    if (validationResult)
                //    {
                //        //if (SaveMeasResultSignaling(deliveryObject, out int newResMeasId, out int newResSensorId))
                //        //{
                //        //    DeleteOldMeasResultSignaling(deliveryObject, newResMeasId, newResSensorId);
                //        //}
                //    }
                //}
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.EventProcessing, e, this);
            }
            //throw new NotImplementedException($"Not handled result {deliveryObject.ResultId}");
        }
        //private bool VaildateMeasResultMonitoringStations(DM.MeasResults measResult)
        //{
        //    var result = true;

        //    if (string.IsNullOrEmpty(measResult.ResultId))
        //    {
        //        WriteLog("Undefined value ResultId", "IResMeas");
        //        result = false;
        //    }
        //    else if (measResult.ResultId.Length > 50)
        //        measResult.ResultId.SubString(50);

        //    if (string.IsNullOrEmpty(measResult.TaskId))
        //    {
        //        WriteLog("Undefined value TaskId", "IResMeas");
        //        result = false;
        //    }
        //    else if (measResult.TaskId.Length > 200)
        //        measResult.TaskId.SubString(200);

        //    if (!(measResult.ScansNumber >= 0 && measResult.ScansNumber <= 10000000))
        //        WriteLog("Incorrect value SwNumber", "IResMeas");

        //    #region Route

        //    foreach (var route in measResult.Routes)
        //    {
        //        bool validationResult = true;
        //        foreach (var routePoint in route.RoutePoints)
        //        {
        //            validationResult = this.ValidateGeoLocation<DM.RoutePoint>(routePoint, "IResRoutesRaw");

        //        }
        //    }


        //    var listRoutes = new List<DEV.Route>();
        //    var queryRoutes = this._dataLayer.GetBuilder<MD.IResRoutesRaw>()
        //    .From()
        //    .Select(c => c.Id, c => c.Lon, c => c.Lat, c => c.Agl, c => c.Asl, c => c.PointStayType, c => c.StartTime, c => c.FinishTime, c => c.RouteId)
        //    .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
        //    queryExecuter.Fetch(queryRoutes, reader =>
        //    {
        //        while (reader.Read())
        //        {
        //            bool validationResult = true;
        //            var route = new DEV.Route() { RouteId = reader.GetValue(c => c.RouteId) };

        //            var listRoutePoints = new List<DEV.RoutePoint>();
        //            var routePoint = new DEV.RoutePoint();

        //            var lon = reader.GetValue(c => c.Lon);
        //            var lat = reader.GetValue(c => c.Lat);
        //            if (lon.HasValue)
        //                routePoint.Lon = lon.Value;
        //            if (lat.HasValue)
        //                routePoint.Lat = lat.Value;
        //            routePoint.ASL = reader.GetValue(c => c.Asl);
        //            routePoint.AGL = reader.GetValue(c => c.Agl);

        //            validationResult = this.ValidateGeoLocation<DEV.RoutePoint>(routePoint, "IResRoutesRaw");

        //            if (Enum.TryParse(reader.GetValue(c => c.PointStayType), out DM.PointStayType pst))
        //                routePoint.PointStayType = pst;

        //            var startTime = reader.GetValue(c => c.StartTime);
        //            var finishTime = reader.GetValue(c => c.FinishTime);
        //            if (startTime.HasValue)
        //                routePoint.StartTime = startTime.Value;
        //            if (finishTime.HasValue)
        //                routePoint.FinishTime = finishTime.Value;

        //            if (routePoint.StartTime > routePoint.FinishTime)
        //            {
        //                WriteLog("StartTime must be less than FinishTime", "IResRoutesRaw");
        //            }

        //            if (validationResult)
        //            {
        //                listRoutePoints.Add(routePoint);
        //                route.RoutePoints = listRoutePoints.ToArray();
        //                listRoutes.Add(route);
        //            }
        //        }
        //        return true;
        //    });
        //    if (listRoutes.Count >= 0)
        //        measResult.Routes = listRoutes.ToArray();
        //    else
        //        result = false;

        //    #endregion

        //    #region StationMeasResult
        //    var listStationMeasResult = new List<DEV.StationMeasResult>();
        //    var queryMeasStation = this._dataLayer.GetBuilder<MD.IResMeasStaRaw>()
        //    .From()
        //    .Select(c => c.Id, c => c.StationId, c => c.MeasGlobalSID, c => c.SectorId, c => c.Status, c => c.Standard, c => c.GlobalSID)
        //    .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
        //    queryExecuter.Fetch(queryMeasStation, reader =>
        //    {
        //        while (reader.Read())
        //        {
        //            if (sensorId == -1)
        //            {
        //                var builderLinkResSensorRaw = this._dataLayer.GetBuilder<MD.ILinkResSensorRaw>().From();
        //                builderLinkResSensorRaw.Select(c => c.Id, c => c.SensorId);
        //                builderLinkResSensorRaw.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
        //                queryExecuter.Fetch(builderLinkResSensorRaw, readerLinkResSensorRaw =>
        //                {
        //                    while (readerLinkResSensorRaw.Read())
        //                    {
        //                        sensorId = readerLinkResSensorRaw.GetValue(c => c.SensorId);
        //                        break;
        //                    }
        //                    return true;
        //                });
        //            }

        //            var measStation = new DEV.StationMeasResult();
        //            var stationId = reader.GetValue(c => c.StationId);
        //            if (stationId.HasValue)
        //                measStation.StationId = stationId.Value.ToString().SubString(50);
        //            measStation.TaskGlobalSid = reader.GetValue(c => c.GlobalSID).SubString(50);
        //            measStation.RealGlobalSid = reader.GetValue(c => c.MeasGlobalSID).SubString(50);

        //            var sectorId = reader.GetValue(c => c.SectorId);
        //            if (sectorId.HasValue)
        //                measStation.SectorId = sectorId.Value.ToString().SubString(50);
        //            measStation.Status = reader.GetValue(c => c.Status).SubString(5);
        //            measStation.Standard = reader.GetValue(c => c.Standard).SubString(50);

        //            #region LevelMeasResult
        //            var listLevelMeasResult = new List<DEV.LevelMeasResult>();
        //            var queryLevelMeasResult = this._dataLayer.GetBuilder<MD.IResStLevelCarRaw>()
        //            .From()
        //            .Select(c => c.Id, c => c.LevelDbm, c => c.LevelDbmkvm, c => c.TimeOfMeasurements, c => c.DifferenceTimeStamp, c => c.Agl, c => c.Altitude, c => c.Lon, c => c.Lat)
        //            .Where(c => c.ResStationId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
        //            queryExecuter.Fetch(queryLevelMeasResult, readerLev =>
        //            {
        //                while (readerLev.Read())
        //                {
        //                    bool validationResult = true;
        //                    var levelMeasResult = new DEV.LevelMeasResult();
        //                    var geoLocation = new DM.GeoLocation();

        //                    var lon = readerLev.GetValue(c => c.Lon);
        //                    var lat = readerLev.GetValue(c => c.Lat);

        //                    if (lon.HasValue)
        //                        geoLocation.Lon = lon.Value;
        //                    if (lat.HasValue)
        //                        geoLocation.Lat = lat.Value;
        //                    geoLocation.ASL = readerLev.GetValue(c => c.Altitude);
        //                    geoLocation.AGL = readerLev.GetValue(c => c.Agl);

        //                    validationResult = this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IResStLevelCarRaw");
        //                    if (validationResult)
        //                        levelMeasResult.Location = geoLocation;

        //                    var levelDbm = readerLev.GetValue(c => c.LevelDbm);
        //                    if (levelDbm.HasValue && levelDbm >= -150 && levelDbm <= 20)
        //                        levelMeasResult.Level_dBm = levelDbm.Value;
        //                    else
        //                    {
        //                        if (levelDbm.HasValue)
        //                        {
        //                            WriteLog("Incorrect value LevelDbm", "IResStLevelCarRaw");
        //                            validationResult = false;
        //                        }
        //                    }

        //                    var levelDbmkvm = readerLev.GetValue(c => c.LevelDbmkvm);
        //                    if (levelDbmkvm.HasValue && levelDbmkvm >= -10 && levelDbmkvm <= 140)
        //                        levelMeasResult.Level_dBmkVm = levelDbmkvm.Value;
        //                    else
        //                    {
        //                        if (levelDbmkvm.HasValue)
        //                        {
        //                            WriteLog("Incorrect value LevelDbmkvm", "IResStLevelCarRaw");
        //                            validationResult = false;
        //                        }
        //                    }

        //                    var timeOfMeasurements = readerLev.GetValue(c => c.TimeOfMeasurements);
        //                    if (timeOfMeasurements.HasValue)
        //                        levelMeasResult.MeasurementTime = timeOfMeasurements.Value;
        //                    levelMeasResult.DifferenceTimeStamp_ns = readerLev.GetValue(c => c.DifferenceTimeStamp);

        //                    if (levelMeasResult.DifferenceTimeStamp_ns.HasValue && (levelMeasResult.DifferenceTimeStamp_ns < 0 && levelMeasResult.DifferenceTimeStamp_ns > 999999999))
        //                    {
        //                        WriteLog("Incorrect value DifferenceTimeStamp", "IResStLevelCarRaw");
        //                    }

        //                    if (validationResult)
        //                    {
        //                        listLevelMeasResult.Add(levelMeasResult);
        //                    }
        //                }
        //                return true;
        //            });

        //            measStation.LevelResults = listLevelMeasResult.ToArray();

        //            //var builderDelResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCarRaw>().Delete();
        //            //builderDelResStLevelCar.Where(c => c.ResStationId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
        //            //queryExecuter.Execute(builderDelResStLevelCar);
        //            #endregion

        //            #region DirectionFindingData

        //            var listFindingData = new List<DEV.DirectionFindingData>();
        //            var queryFinfdingData = this._dataLayer.GetBuilder<MD.IBearingRaw>()
        //            .From()
        //            .Select(c => c.Id, c => c.Agl, c => c.Asl, c => c.Lon, c => c.Lat, c => c.Agl, c => c.Level_dBm, c => c.Level_dBmkVm, c => c.MeasurementTime, c => c.Quality, c => c.AntennaAzimut, c => c.Bandwidth_kHz, c => c.Bearing, c => c.CentralFrequency_MHz)
        //            .Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
        //            queryExecuter.Fetch(queryFinfdingData, readerData =>
        //            {
        //                while (readerData.Read())
        //                {
        //                    var findingData = new DEV.DirectionFindingData();
        //                    var geoLocation = new DM.GeoLocation();

        //                    var lon = readerData.GetValue(c => c.Lon);
        //                    var lat = readerData.GetValue(c => c.Lat);
        //                    if (lon.HasValue)
        //                        geoLocation.Lon = lon.Value;
        //                    if (lat.HasValue)
        //                        geoLocation.Lat = lat.Value;
        //                    geoLocation.ASL = readerData.GetValue(c => c.Asl);
        //                    geoLocation.AGL = readerData.GetValue(c => c.Agl);

        //                    if (this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IBearingRaw"))
        //                        findingData.Location = geoLocation;

        //                    findingData.Level_dBm = readerData.GetValue(c => c.Level_dBm);
        //                    findingData.Level_dBmkVm = readerData.GetValue(c => c.Level_dBmkVm);
        //                    findingData.MeasurementTime = readerData.GetValue(c => c.MeasurementTime);
        //                    findingData.Quality = readerData.GetValue(c => c.Quality);
        //                    findingData.AntennaAzimut = readerData.GetValue(c => c.AntennaAzimut);
        //                    findingData.Bandwidth_kHz = readerData.GetValue(c => c.Bandwidth_kHz);
        //                    findingData.Bearing = readerData.GetValue(c => c.Bearing);
        //                    findingData.CentralFrequency_MHz = readerData.GetValue(c => c.CentralFrequency_MHz);

        //                    listFindingData.Add(findingData);
        //                }
        //                return true;
        //            });
        //            measStation.Bearings = listFindingData.ToArray();

        //            //var builderDelBearing = this._dataLayer.GetBuilder<MD.IBearingRaw>().Delete();
        //            //builderDelBearing.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
        //            //queryExecuter.Execute(builderDelBearing);
        //            #endregion

        //            #region GeneralMeasResult
        //            var queryGeneralMeasResult = this._dataLayer.GetBuilder<MD.IResStGeneralRaw>()
        //            .From()
        //            .Select(c => c.Id, c => c.CentralFrequency, c => c.CentralFrequencyMeas, c => c.OffsetFrequency, c => c.SpecrumStartFreq, c => c.SpecrumSteps, c => c.T1, c => c.T2, c => c.MarkerIndex, c => c.Correctnessestim, c => c.TraceCount, c => c.DurationMeas, c => c.TimeStartMeas, c => c.TimeFinishMeas, c => c.Rbw, c => c.Vbw, c => c.BW)
        //            .Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
        //            queryExecuter.Fetch(queryGeneralMeasResult, readerGeneralResult =>
        //            {
        //                bool removeGroup1 = false;
        //                while (readerGeneralResult.Read())
        //                {
        //                    var generalMeasResult = new DEV.GeneralMeasResult();

        //                    var centralFrequency = readerGeneralResult.GetValue(c => c.CentralFrequency);
        //                    if (centralFrequency.HasValue && centralFrequency >= 0.001 && centralFrequency <= 400000)
        //                        generalMeasResult.CentralFrequency_MHz = centralFrequency.Value;

        //                    var centralFrequencyMeas = readerGeneralResult.GetValue(c => c.CentralFrequencyMeas);
        //                    if (centralFrequencyMeas.HasValue && centralFrequencyMeas >= 0.001 && centralFrequencyMeas <= 400000)
        //                        generalMeasResult.CentralFrequencyMeas_MHz = centralFrequencyMeas.Value;

        //                    generalMeasResult.OffsetFrequency_mk = readerGeneralResult.GetValue(c => c.OffsetFrequency);

        //                    var specrumStartFreq = readerGeneralResult.GetValue(c => c.SpecrumStartFreq);
        //                    if (specrumStartFreq >= 0.001m && specrumStartFreq <= 400000m)
        //                        generalMeasResult.SpectrumStartFreq_MHz = specrumStartFreq;
        //                    else
        //                        removeGroup1 = true;

        //                    var specrumSteps = readerGeneralResult.GetValue(c => c.SpecrumSteps);

        //                    if (specrumSteps >= 0.001m && specrumSteps <= 100000m)
        //                        generalMeasResult.SpectrumSteps_kHz = specrumSteps;
        //                    else
        //                        removeGroup1 = true;

        //                    if (readerGeneralResult.GetValue(c => c.DurationMeas).HasValue)
        //                    {
        //                        generalMeasResult.MeasDuration_sec = readerGeneralResult.GetValue(c => c.DurationMeas).Value;
        //                    }
        //                    generalMeasResult.MeasStartTime = readerGeneralResult.GetValue(c => c.TimeStartMeas);
        //                    generalMeasResult.MeasFinishTime = readerGeneralResult.GetValue(c => c.TimeFinishMeas);
        //                    generalMeasResult.RBW_kHz = readerGeneralResult.GetValue(c => c.Rbw);
        //                    generalMeasResult.VBW_kHz = readerGeneralResult.GetValue(c => c.Vbw);

        //                    if (generalMeasResult.MeasStartTime > generalMeasResult.MeasFinishTime)
        //                    {
        //                        WriteLog("MeasStartTime must be less than MeasFinishTime", "IResStGeneralRaw");
        //                    }

        //                    if (removeGroup1)
        //                    {
        //                        generalMeasResult.SpectrumStartFreq_MHz = null;
        //                        generalMeasResult.SpectrumSteps_kHz = null;
        //                    }
        //                    else
        //                    {
        //                        #region BandwidthMeasResult
        //                        var bandwidthMeasResult = new DEV.BandwidthMeasResult();
        //                        bool isValidBandwith = true;

        //                        var markerIndex = readerGeneralResult.GetValue(c => c.MarkerIndex);
        //                        var t1 = readerGeneralResult.GetValue(c => c.T1);
        //                        var t2 = readerGeneralResult.GetValue(c => c.T2);

        //                        if (markerIndex.HasValue && t1.HasValue && t2.HasValue)
        //                        {
        //                            if (!(t1.Value >= 0 && t1.Value <= markerIndex.Value
        //                                && t2.Value >= markerIndex.Value && t2.Value <= 100000
        //                                && markerIndex.Value >= t1.Value && markerIndex.Value <= t2.Value))
        //                            {
        //                                isValidBandwith = false;
        //                            }
        //                        }
        //                        else
        //                            isValidBandwith = false;

        //                        if (isValidBandwith)
        //                        {
        //                            bandwidthMeasResult.T1 = t1.Value;
        //                            bandwidthMeasResult.T2 = t2.Value;
        //                            bandwidthMeasResult.Bandwidth_kHz = readerGeneralResult.GetValue(c => c.BW);
        //                            bandwidthMeasResult.MarkerIndex = markerIndex;

        //                            var correctnessestim = readerGeneralResult.GetValue(c => c.Correctnessestim);
        //                            if (correctnessestim.HasValue)
        //                                bandwidthMeasResult.СorrectnessEstimations = correctnessestim.Value == 1 ? true : false;
        //                            else
        //                                bandwidthMeasResult.СorrectnessEstimations = false;

        //                            var traceCount = readerGeneralResult.GetValue(c => c.TraceCount);
        //                            if (traceCount.HasValue && traceCount.Value >= 1 && traceCount.Value <= 100000)
        //                                bandwidthMeasResult.TraceCount = traceCount.Value;


        //                            generalMeasResult.BandwidthResult = bandwidthMeasResult;
        //                        }

        //                        #endregion

        //                        var listStLevelsSpect = new List<float>();
        //                        var queryStLevelsSpect = this._dataLayer.GetBuilder<MD.IResStLevelsSpectRaw>()
        //                        .From()
        //                        .Select(c => c.Id, c => c.LevelSpecrum)
        //                        .OrderByAsc(c => c.Id)
        //                        .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
        //                        queryExecuter.Fetch(queryStLevelsSpect, readerStLevelsSpect =>
        //                        {
        //                            while (readerStLevelsSpect.Read())
        //                            {
        //                                var levelSpecrum = readerStLevelsSpect.GetValue(c => c.LevelSpecrum);
        //                                if (levelSpecrum.HasValue)
        //                                {
        //                                    listStLevelsSpect.Add(levelSpecrum.Value);
        //                                }
        //                            }
        //                            return true;
        //                        });
        //                        generalMeasResult.LevelsSpectrum_dBm = listStLevelsSpect.ToArray();
        //                    }

        //                    //var builderDelResStLevels = this._dataLayer.GetBuilder<MD.IResStLevelsSpectRaw>().Delete();
        //                    //builderDelResStLevels.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
        //                    //queryExecuter.Execute(builderDelResStLevels);

        //                    #region MaskElement
        //                    var isValidElementMask = true;
        //                    var listElementsMask = new List<DEV.ElementsMask>();
        //                    var queryElementsMask = this._dataLayer.GetBuilder<MD.IResStMaskElementRaw>()
        //                    .From()
        //                    .Select(c => c.Id, c => c.Bw, c => c.Level)
        //                    .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
        //                    queryExecuter.Fetch(queryElementsMask, readerElementsMask =>
        //                    {
        //                        while (readerElementsMask.Read())
        //                        {
        //                            var elementMask = new DEV.ElementsMask();

        //                            var level = readerElementsMask.GetValue(c => c.Level);
        //                            if (level.HasValue && level.Value >= -300 && level.Value <= 300)
        //                                elementMask.Level_dB = level;
        //                            else
        //                                isValidElementMask = false;

        //                            var bw = readerElementsMask.GetValue(c => c.Bw);
        //                            if (bw.HasValue && bw.Value >= 1 && bw.Value <= 200000)
        //                                elementMask.BW_kHz = bw;
        //                            else
        //                                isValidElementMask = false;

        //                            if (isValidElementMask)
        //                                listElementsMask.Add(elementMask);
        //                        }
        //                        return true;
        //                    });
        //                    generalMeasResult.BWMask = listElementsMask.ToArray();

        //                    //var builderDelMaskElem = this._dataLayer.GetBuilder<MD.IResStMaskElementRaw>().Delete();
        //                    //builderDelMaskElem.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
        //                    //queryExecuter.Execute(builderDelMaskElem);
        //                    #endregion

        //                    #region StationSysInfo
        //                    var queryStationSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfoRaw>()
        //                    .From()
        //                    .Select(c => c.Id, c => c.Agl, c => c.Asl, c => c.Bandwidth, c => c.BaseId, c => c.Bsic, c => c.ChannelNumber, c => c.Cid, c => c.Code, c => c.Ctoi, c => c.Eci, c => c.Enodebid, c => c.Freq, c => c.Icio, c => c.InbandPower, c => c.Iscp, c => c.Lac)
        //                    .Select(c => c.Lat, c => c.Lon, c => c.Mcc, c => c.Mnc, c => c.Nid, c => c.Pci, c => c.Pn, c => c.Power, c => c.Ptotal, c => c.Rnc, c => c.Rscp, c => c.Rsrp, c => c.Rsrq, c => c.Sc, c => c.Sid, c => c.Tac, c => c.TypeCdmaevdo, c => c.Ucid)
        //                    .Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
        //                    queryExecuter.Fetch(queryStationSysInfo, readerStationSysInfo =>
        //                    {
        //                        while (readerStationSysInfo.Read())
        //                        {
        //                            var stationSysInfo = new DEV.StationSysInfo();
        //                            var location = new DM.GeoLocation();

        //                            var lat = readerStationSysInfo.GetValue(c => c.Lat);
        //                            var lon = readerStationSysInfo.GetValue(c => c.Lon);
        //                            if (lat.HasValue)
        //                                location.Lat = lat.Value;
        //                            if (lon.HasValue)
        //                                location.Lon = lon.Value;
        //                            location.AGL = readerStationSysInfo.GetValue(c => c.Agl);
        //                            location.ASL = readerStationSysInfo.GetValue(c => c.Asl);

        //                            if (this.ValidateGeoLocation<DM.GeoLocation>(location, "IResSysInfoRaw"))
        //                                stationSysInfo.Location = location;

        //                            stationSysInfo.Freq = readerStationSysInfo.GetValue(c => c.Freq);
        //                            stationSysInfo.BandWidth = readerStationSysInfo.GetValue(c => c.Bandwidth);
        //                            stationSysInfo.RSRP = readerStationSysInfo.GetValue(c => c.Rsrp);
        //                            stationSysInfo.RSRQ = readerStationSysInfo.GetValue(c => c.Rsrq);
        //                            stationSysInfo.INBAND_POWER = readerStationSysInfo.GetValue(c => c.InbandPower);
        //                            stationSysInfo.MCC = readerStationSysInfo.GetValue(c => c.Mcc);
        //                            stationSysInfo.MNC = readerStationSysInfo.GetValue(c => c.Mnc);
        //                            stationSysInfo.TAC = readerStationSysInfo.GetValue(c => c.Tac);
        //                            stationSysInfo.eNodeBId = readerStationSysInfo.GetValue(c => c.Enodebid);
        //                            stationSysInfo.CID = readerStationSysInfo.GetValue(c => c.Cid);
        //                            stationSysInfo.ECI = readerStationSysInfo.GetValue(c => c.Eci);
        //                            stationSysInfo.PCI = readerStationSysInfo.GetValue(c => c.Pci);
        //                            stationSysInfo.BSIC = readerStationSysInfo.GetValue(c => c.Bsic);
        //                            stationSysInfo.LAC = readerStationSysInfo.GetValue(c => c.Lac);
        //                            stationSysInfo.Power = readerStationSysInfo.GetValue(c => c.Power);
        //                            stationSysInfo.CtoI = readerStationSysInfo.GetValue(c => c.Ctoi);
        //                            stationSysInfo.SC = readerStationSysInfo.GetValue(c => c.Sc);
        //                            stationSysInfo.UCID = readerStationSysInfo.GetValue(c => c.Ucid);
        //                            stationSysInfo.RNC = readerStationSysInfo.GetValue(c => c.Rnc);
        //                            stationSysInfo.Ptotal = readerStationSysInfo.GetValue(c => c.Ptotal);
        //                            stationSysInfo.RSCP = readerStationSysInfo.GetValue(c => c.Rscp);
        //                            stationSysInfo.ISCP = readerStationSysInfo.GetValue(c => c.Iscp);
        //                            stationSysInfo.Code = readerStationSysInfo.GetValue(c => c.Code);
        //                            stationSysInfo.IcIo = readerStationSysInfo.GetValue(c => c.Icio);
        //                            stationSysInfo.ChannelNumber = readerStationSysInfo.GetValue(c => c.ChannelNumber);
        //                            stationSysInfo.TypeCDMAEVDO = readerStationSysInfo.GetValue(c => c.TypeCdmaevdo);
        //                            stationSysInfo.SID = readerStationSysInfo.GetValue(c => c.Sid);
        //                            stationSysInfo.NID = readerStationSysInfo.GetValue(c => c.Nid);
        //                            stationSysInfo.PN = readerStationSysInfo.GetValue(c => c.Pn);
        //                            stationSysInfo.BaseID = readerStationSysInfo.GetValue(c => c.BaseId);

        //                            var listStationSysInfoBls = new List<DEV.StationSysInfoBlock>();
        //                            var queryStationSysInfoBls = this._dataLayer.GetBuilder<MD.IResSysInfoBlsRaw>()
        //                            .From()
        //                            .Select(c => c.Id, c => c.BinData, c => c.Type)
        //                            .Where(c => c.ResSysInfoId, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
        //                            queryExecuter.Fetch(queryStationSysInfoBls, readerStationSysInfoBls =>
        //                            {
        //                                while (readerStationSysInfoBls.Read())
        //                                {
        //                                    var stationSysInfoBls = new DEV.StationSysInfoBlock()
        //                                    {
        //                                        Data = BinaryDecoder.Deserialize<string>(readerStationSysInfoBls.GetValue(c => c.BinData)),
        //                                        Type = readerStationSysInfoBls.GetValue(c => c.Type)
        //                                    };
        //                                    listStationSysInfoBls.Add(stationSysInfoBls);
        //                                }

        //                                //var builderDelResSysInfoBls = this._dataLayer.GetBuilder<MD.IResSysInfoBlsRaw>().Delete();
        //                                //builderDelResSysInfoBls.Where(c => c.ResSysInfoId, ConditionOperator.Equal, readerStationSysInfo.GetValue(c => c.Id));
        //                                //queryExecuter.Execute(builderDelResSysInfoBls);

        //                                return true;
        //                            });

        //                            stationSysInfo.InfoBlocks = listStationSysInfoBls.ToArray();
        //                            generalMeasResult.StationSysInfo = stationSysInfo;
        //                        }
        //                        return true;
        //                    });

        //                    //var builderDelResSysInfo = this._dataLayer.GetBuilder<MD.IResSysInfoRaw>().Delete();
        //                    //builderDelResSysInfo.Where(c => c.ResStGeneralId, ConditionOperator.Equal, readerGeneralResult.GetValue(c => c.Id));
        //                    //queryExecuter.Execute(builderDelResSysInfo);
        //                    #endregion

        //                    measStation.GeneralResult = generalMeasResult;
        //                }
        //                return true;
        //            });

        //            #endregion

        //            listStationMeasResult.Add(measStation);

        //            //var builderDelLinkResSensor = this._dataLayer.GetBuilder<MD.ILinkResSensorRaw>().Delete();
        //            //builderDelLinkResSensor.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
        //            //queryExecuter.Execute(builderDelLinkResSensor);

        //            //var builderDelResGeneral = this._dataLayer.GetBuilder<MD.IResStGeneralRaw>().Delete();
        //            //builderDelResGeneral.Where(c => c.ResMeasStaId, ConditionOperator.Equal, reader.GetValue(c => c.Id));
        //            //queryExecuter.Execute(builderDelResGeneral);
        //        }

        //        return true;
        //    });

        //    //var builderDelStation = this._dataLayer.GetBuilder<MD.IResMeasStaRaw>().Delete();
        //    //builderDelStation.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
        //    //queryExecuter.Execute(builderDelStation);


        //    measResult.SensorId = sensorId;

        //    if (listStationMeasResult.Count > 0)
        //        measResult.StationResults = listStationMeasResult.ToArray();
        //    else
        //        result = false;
        //    return result;
        //    #endregion
        //}
        //private bool SaveMeasResultMonitoringStations(DM.MeasResults measResult)
        //{
        //    return true;
        //}
        //private bool VaildateMeasResultSpectrumOccupation(DM.MeasResults measResult)
        //{
        //    var result = true;
        //    var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

        //    if (string.IsNullOrEmpty(measResult.ResultId))
        //    {
        //        WriteLog("Undefined value ResultId", "IResMeasRaw");
        //        result = false;
        //    }
        //    else if (measResult.ResultId.Length > 50)
        //        measResult.ResultId.SubString(50);

        //    if (string.IsNullOrEmpty(measResult.TaskId))
        //    {
        //        WriteLog("Undefined value TaskId", "IResMeasRaw");
        //        result = false;
        //    }
        //    else if (measResult.TaskId.Length > 200)
        //        measResult.TaskId.SubString(200);

        //    if (measResult.Status.Length > 5)
        //        measResult.Status = "";

        //    if (!(measResult.SwNumber >= 0 && measResult.SwNumber <= 10000))
        //        WriteLog("Incorrect value SwNumber", "IResMeasRaw");

        //    if (measResult.StartTime > measResult.StopTime)
        //        WriteLog("StartTime must be less than StopTime", "IResMeasRaw");

        //    var geoLocation = new DM.GeoLocation();
        //    var queryLoc = this._dataLayer.GetBuilder<MD.IResLocSensorRaw>()
        //    .From()
        //    .Select(c => c.Id, c => c.Lon, c => c.Lat, c => c.Agl, c => c.Asl)
        //    .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
        //    queryExecuter.Fetch(queryLoc, reader =>
        //    {
        //        while (reader.Read())
        //        {
        //            var lon = reader.GetValue(c => c.Lon);
        //            var lat = reader.GetValue(c => c.Lat);

        //            if (lon.HasValue)
        //                geoLocation.Lon = lon.Value;
        //            if (lat.HasValue)
        //                geoLocation.Lat = lat.Value;
        //            geoLocation.ASL = reader.GetValue(c => c.Asl);
        //            geoLocation.AGL = reader.GetValue(c => c.Agl);
        //        }
        //        return true;
        //    });

        //    //foreach (var route in measResult.Routes)
        //    //{
        //    //    bool validationResult = true;
        //    //    foreach (var routePoint in route.RoutePoints)
        //    //    {
        //    //        validationResult = this.ValidateGeoLocation<DM.RoutePoint>(routePoint, "IResRoutesRaw");

        //    //    }
        //    //}


        //    if (this.ValidateGeoLocation<DM.GeoLocation>(geoLocation, "IResMeasRaw"))
        //        measResult.Location = geoLocation;

        //    //var builderDelLocSensor = this._dataLayer.GetBuilder<MD.IResLocSensorRaw>().Delete();
        //    //builderDelLocSensor.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
        //    //queryExecuter.Execute(builderDelLocSensor);


        //    #region FrequencySample
        //    var listFrequencySample = new List<DEV.FrequencySample>();
        //    var queryFrequencySample = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>()
        //    .From()
        //    .Select(c => c.Id, c => c.Freq_MHz, c => c.Level_dBm, c => c.Level_dBmkVm, c => c.LevelMin_dBm, c => c.LevelMax_dBm, c => c.OccupationPt)
        //    .Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
        //    queryExecuter.Fetch(queryFrequencySample, reader =>
        //    {
        //        while (reader.Read())
        //        {
        //            bool validationResult = true;
        //            var freqSample = new DEV.FrequencySample();

        //            var freq_MHz = reader.GetValue(c => c.Freq_MHz);
        //            if (freq_MHz.HasValue && freq_MHz >= 0 && freq_MHz.Value <= 400000)
        //                freqSample.Freq_MHz = freq_MHz.Value;
        //            else
        //            {
        //                WriteLog("Incorrect value Freq_MHz", "IFreqSampleRaw");
        //                validationResult = false;
        //            }

        //            var occupationPt = reader.GetValue(c => c.OccupationPt);
        //            if (occupationPt.HasValue && occupationPt >= 0 && occupationPt.Value <= 100)
        //                freqSample.Occupation_Pt = occupationPt.Value;
        //            else
        //            {
        //                WriteLog("Incorrect value Freq_MHz", "IFreqSampleRaw");
        //                validationResult = false;
        //            }

        //            if (validationResult)
        //            {
        //                var level_dBm = reader.GetValue(c => c.Level_dBm);
        //                if (level_dBm.HasValue && level_dBm.Value >= -150 && level_dBm.Value <= 20)
        //                    freqSample.Level_dBm = level_dBm.Value;

        //                var level_dBmkVm = reader.GetValue(c => c.Level_dBmkVm);
        //                if (level_dBmkVm.HasValue && level_dBmkVm.Value >= 10 && level_dBmkVm.Value <= 140)
        //                    freqSample.Level_dBmkVm = level_dBmkVm.Value;

        //                var levelMin_dBm = reader.GetValue(c => c.LevelMin_dBm);
        //                if (levelMin_dBm.HasValue && levelMin_dBm.Value >= -120 && levelMin_dBm.Value <= 20)
        //                    freqSample.LevelMin_dBm = levelMin_dBm.Value;

        //                var levelMax_dBm = reader.GetValue(c => c.LevelMax_dBm);
        //                if (levelMax_dBm.HasValue && levelMax_dBm.Value >= -120 && levelMax_dBm.Value <= 20)
        //                    freqSample.LevelMax_dBm = levelMax_dBm.Value;

        //                listFrequencySample.Add(freqSample);
        //            }
        //        }
        //        return true;
        //    });

        //    //var builderDelFreqSample = this._dataLayer.GetBuilder<MD.IFreqSampleRaw>().Delete();
        //    //builderDelFreqSample.Where(c => c.ResMeasId, ConditionOperator.Equal, resultId);
        //    //queryExecuter.Execute(builderDelFreqSample);

        //    if (listFrequencySample.Count > 0)
        //        measResult.FrequencySamples = listFrequencySample.ToArray();
        //    else
        //        return false;

        //    return result;
        //    #endregion
        //}
        //private bool SaveMeasResultSpectrumOccupation(DM.MeasResults measResult)
        //{
        //    var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
        //    try
        //    {
        //        queryExecuter.BeginTransaction();

        //        int subMeasTaskId = -1; int subMeasTaskStaId = -1; int sensorId = -1; int resultId = -1;
        //        GetIds(measResult.ResultId, measResult.TaskId, out subMeasTaskId, out subMeasTaskStaId, out sensorId, out resultId);

        //        int valInsResMeas = 0;
        //        var builderInsertIResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Insert();
        //        builderInsertIResMeas.SetValue(c => c.MeasResultSID, resultId.ToString());
        //        builderInsertIResMeas.SetValue(c => c.MeasTaskId, measResult.TaskId);
        //        builderInsertIResMeas.SetValue(c => c.TimeMeas, measResult.Measured);
        //        builderInsertIResMeas.SetValue(c => c.Status, measResult.Status);
        //        builderInsertIResMeas.SetValue(c => c.StartTime, measResult.StartTime);
        //        builderInsertIResMeas.SetValue(c => c.StopTime, measResult.StopTime);
        //        builderInsertIResMeas.SetValue(c => c.ScansNumber, measResult.ScansNumber);
        //        builderInsertIResMeas.SetValue(c => c.TypeMeasurements, measResult.Measurement.ToString());
        //        builderInsertIResMeas.SetValue(c => c.MeasSubTaskId, subMeasTaskId);
        //        builderInsertIResMeas.SetValue(c => c.MeasSubTaskStationId, subMeasTaskStaId);
        //        builderInsertIResMeas.SetValue(c => c.SensorId, sensorId);
        //        builderInsertIResMeas.Select(c => c.Id);
        //        queryExecuter.ExecuteAndFetch(builderInsertIResMeas, reader =>
        //        {
        //            var res = reader.Read();
        //            if (res)
        //            {
        //                valInsResMeas = reader.GetValue(c => c.Id);
        //            }
        //            return res;
        //        });

        //        if (valInsResMeas > 0)
        //        {
        //            if (measResult.FrequencySamples != null)
        //            {
        //                var lstIns = new IQueryInsertStatement<MD.IResLevels>[measResult.FrequencySamples.Length];
        //                for (int i = 0; i < measResult.FrequencySamples.Length; i++)
        //                {
        //                    var item = measResult.FrequencySamples[i];

        //                    var builderInsertResLevels = this._dataLayer.GetBuilder<MD.IResLevels>().Insert();
        //                    builderInsertResLevels.SetValue(c => c.FreqMeas, item.Freq_MHz);
        //                    builderInsertResLevels.SetValue(c => c.VMMaxLvl, item.LevelMax_dBm);
        //                    builderInsertResLevels.SetValue(c => c.VMinLvl, item.LevelMin_dBm);
        //                    builderInsertResLevels.SetValue(c => c.ValueLvl, item.Level_dBm);
        //                    builderInsertResLevels.SetValue(c => c.ValueSpect, item.Level_dBmkVm);
        //                    builderInsertResLevels.SetValue(c => c.OccupancySpect, item.Occupation_Pt);
        //                    builderInsertResLevels.SetValue(c => c.ResMeasId, valInsResMeas);
        //                    builderInsertResLevels.Select(c => c.Id);
        //                    lstIns[i] = builderInsertResLevels;
        //                }
        //                queryExecuter.ExecuteAndFetch(lstIns, reader =>
        //                {
        //                    return true;
        //                });
        //            }

        //            var builderInsertResLocSensorMeas = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().Insert();
        //            builderInsertResLocSensorMeas.SetValue(c => c.Agl, measResult.Location.AGL);
        //            builderInsertResLocSensorMeas.SetValue(c => c.Asl, measResult.Location.ASL);
        //            builderInsertResLocSensorMeas.SetValue(c => c.Lon, measResult.Location.Lon);
        //            builderInsertResLocSensorMeas.SetValue(c => c.Lat, measResult.Location.Lat);
        //            builderInsertResLocSensorMeas.SetValue(c => c.ResMeasId, valInsResMeas);
        //            builderInsertResLocSensorMeas.Select(c => c.Id);
        //            queryExecuter.Execute(builderInsertResLocSensorMeas);
        //        }

        //        queryExecuter.CommitTransaction();
        //        return true;
        //    }
        //    catch (Exception exp)
        //    {
        //        _logger.Exception(Contexts.ThisComponent, exp);
        //        queryExecuter.RollbackTransaction();
        //        return false;
        //    }
        //}
        //private bool VaildateMeasResultSignaling(DM.MeasResults measResult)
        //{
        //    return true;
        //}
        //private bool SaveMeasResultSignaling(DM.MeasResults measResult)
        //{
        //    return true;
        //}
        //private bool DeleteOldMeasResultSignaling(DM.MeasResults measResult)
        //{
        //    return true;
        //}
        //private void GetIds(string ResultId, string TaskId, out int subMeasTaskId, out int subMeasTaskStaId, out int sensorId, out int resultId)
        //{
        //    subMeasTaskId = -1; subMeasTaskStaId = -1; sensorId = -1; resultId = -1;
        //    if (ResultId != null)
        //    {
        //        string[] word = ResultId.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        //        if ((word != null) && (word.Length == 5))
        //        {
        //            subMeasTaskId = int.Parse(word[1]);
        //            subMeasTaskStaId = int.Parse(word[2]);
        //            sensorId = int.Parse(word[3]);
        //            resultId = int.Parse(word[4]);
        //        }
        //        else
        //        {
        //            int SubTaskIdTemp = -1;
        //            int SubTaskStationIdTemp = -1;
        //            int SensorIdTemp = -1;
        //            int taskId = -1;
        //            if (int.TryParse(TaskId, out taskId))
        //            {
        //                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
        //                var builderFromIMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().From();
        //                builderFromIMeasSubTaskSta.Select(c => c.Id, c => c.MEASSUBTASK.Id, c => c.SensorId, c => c.MEASSUBTASK.TimeStart);
        //                builderFromIMeasSubTaskSta.Where(c => c.MEASSUBTASK.MEASTASK.Id, ConditionOperator.Equal, taskId);
        //                builderFromIMeasSubTaskSta.OrderByDesc(c => c.MEASSUBTASK.TimeStart);
        //                //queryExecuter.Fetch(builderFromIMeasSubTaskSta, reader =>
        //                //{
        //                //    while (reader.Read())
        //                //    {
        //                //        SubTaskIdTemp = reader.GetValue(c => c.MEASSUBTASK.Id);
        //                //        SubTaskStationIdTemp = reader.GetValue(c => c.Id);
        //                //        if (reader.GetValue(c => c.SensorId).HasValue)
        //                //        {
        //                //            SensorIdTemp = reader.GetValue(c => c.SensorId).Value;
        //                //        }
        //                //        break;
        //                //    }
        //                //    return true;
        //                //});
        //                subMeasTaskId = SubTaskIdTemp;
        //                subMeasTaskStaId = SubTaskStationIdTemp;
        //                sensorId = SensorIdTemp;
        //            }
        //        }
        //    }
        //}
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
