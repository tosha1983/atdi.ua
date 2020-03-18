using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities.IeStation;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using System.Xml;
using System.Linq;



namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    public class LoadProtocols
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public LoadProtocols(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }


   

        public Protocols[] GetProtocolsByParameters(
                                 long? processId,
                                 string createdBy,
                                 DateTime? DateCreated,
                                 DateTime? DateStart,
                                 DateTime? DateStop,
                                 short? DateMeasDay,
                                 short? DateMeasMonth,
                                 short? DateMeasYear,
                                 double? freq,
                                 double? probability,
                                 string standard,
                                 string province,
                                 string ownerName,
                                 string permissionNumber,
                                 DateTime? permissionStart,
                                 DateTime? permissionStop)
        {
            var loadSynchroProcessData = new Utils(this._dataLayer, this._logger);
            var loadSensor = new LoadSensor(this._dataLayer, this._logger);
            var listProtocols = new List<Protocols>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetProtocolsByParametersMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

                var builderProtocols = this._dataLayer.GetBuilder<MD.IProtocols>().From();
                builderProtocols.Select(c => c.DateMeasDay,
                                        c => c.DateMeasMonth,
                                        c => c.DateMeasYear,
                                        c => c.DispersionLow,
                                        c => c.DispersionUp,
                                        c => c.Freq_MHz,
                                        c => c.GlobalSID,
                                        c => c.Id,
                                        c => c.Level_dBm,
                                        c => c.Percent,
                                        c => c.PermissionNumber,
                                        c => c.PermissionStart,
                                        c => c.PermissionStop,
                                        c => c.SensorLat,
                                        c => c.SensorLon,
                                        c => c.SensorName,
                                        c => c.SensorId,
                                        c => c.RadioControlBandWidth,
                                        c => c.RadioControlMeasFreq_MHz,
                                        c => c.STATION_EXTENDED.TableId,
                                        c => c.STATION_EXTENDED.TableName,
                                        c => c.STATION_EXTENDED.Address,
                                        c => c.STATION_EXTENDED.BandWidth,
                                        c => c.STATION_EXTENDED.DesigEmission,
                                        c => c.STATION_EXTENDED.Id,
                                        c => c.STATION_EXTENDED.Latitude,
                                        c => c.STATION_EXTENDED.Longitude,
                                        c => c.STATION_EXTENDED.OwnerName,
                                        c => c.STATION_EXTENDED.PermissionNumber,
                                        c => c.STATION_EXTENDED.PermissionStart,
                                        c => c.STATION_EXTENDED.PermissionStop,
                                        c => c.STATION_EXTENDED.Province,
                                        c => c.STATION_EXTENDED.Standard,
                                        c => c.STATION_EXTENDED.StandardName,

                                        c => c.STATION_EXTENDED.DocNum,
                                        c => c.STATION_EXTENDED.TestStartDate,
                                        c => c.STATION_EXTENDED.TestStopDate,
                                        c => c.STATION_EXTENDED.PermissionCancelDate,
                                        c => c.STATION_EXTENDED.StatusMeas,
                                        c => c.STATION_EXTENDED.CurentStatusStation,
                                        c => c.STATION_EXTENDED.StationTxFreq,
                                        c => c.STATION_EXTENDED.StationRxFreq,
                                        c => c.STATION_EXTENDED.StationChannel,
                                        c => c.STATION_EXTENDED.PermissionGlobalSID,
                                        c => c.SYNCHRO_PROCESS.Id
                                        );

                if (processId != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, processId);
                }

                if (freq != null)
                {
                    builderProtocols.Where(c => c.Freq_MHz, ConditionOperator.Equal, freq);
                }
                if (!string.IsNullOrEmpty(permissionNumber))
                {
                    builderProtocols.Where(c => c.PermissionNumber, ConditionOperator.Like, "%" + permissionNumber + "%");
                }
                if (permissionStart != null)
                {
                    builderProtocols.Where(c => c.PermissionStart, ConditionOperator.GreaterEqual, new DateTime(permissionStart.Value.Year, permissionStart.Value.Month, permissionStart.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.PermissionStart, ConditionOperator.LessEqual, new DateTime(permissionStart.Value.Year, permissionStart.Value.Month, permissionStart.Value.Day, 23, 59, 59, 999));
                }
                if (permissionStop != null)
                {
                    builderProtocols.Where(c => c.PermissionStop, ConditionOperator.GreaterEqual, new DateTime(permissionStop.Value.Year, permissionStop.Value.Month, permissionStop.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.PermissionStop, ConditionOperator.LessEqual, new DateTime(permissionStop.Value.Year, permissionStop.Value.Month, permissionStop.Value.Day, 23, 59, 59, 999));
                }
                if (DateMeasDay != null)
                {
                    builderProtocols.Where(c => c.DateMeasDay, ConditionOperator.Equal, DateMeasDay);
                }

                if (DateMeasMonth != null)
                {
                    builderProtocols.Where(c => c.DateMeasMonth, ConditionOperator.Equal, DateMeasMonth);
                }

                if (DateMeasYear != null)
                {
                    builderProtocols.Where(c => c.DateMeasYear, ConditionOperator.Equal, DateMeasYear);
                }

                if (DateCreated != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.CreatedDate, ConditionOperator.GreaterEqual, new DateTime(DateCreated.Value.Year, DateCreated.Value.Month, DateCreated.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.CreatedDate, ConditionOperator.LessEqual, new DateTime(DateCreated.Value.Year, DateCreated.Value.Month, DateCreated.Value.Day, 23, 59, 59, 999));
                }

                if (DateStart != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateStart, ConditionOperator.GreaterEqual, new DateTime(DateStart.Value.Year, DateStart.Value.Month, DateStart.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateStart, ConditionOperator.LessEqual, new DateTime(DateStart.Value.Year, DateStart.Value.Month, DateStart.Value.Day, 23, 59, 59, 999));
                }

                if (DateStop != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateEnd, ConditionOperator.GreaterEqual, new DateTime(DateStop.Value.Year, DateStop.Value.Month, DateStop.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateEnd, ConditionOperator.LessEqual, new DateTime(DateStop.Value.Year, DateStop.Value.Month, DateStop.Value.Day, 23, 59, 59, 999));
                }

                if (!string.IsNullOrEmpty(createdBy))
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.CreatedBy, ConditionOperator.Like, "%" + createdBy + "%");
                }

                if (!string.IsNullOrEmpty(standard))
                {
                    builderProtocols.Where(c => c.STATION_EXTENDED.Standard, ConditionOperator.Like, "%"+standard+ "%");
                }

                if (!string.IsNullOrEmpty(province))
                {
                    builderProtocols.Where(c => c.STATION_EXTENDED.Province, ConditionOperator.Like, "%" + province + "%");
                }

                if (!string.IsNullOrEmpty(ownerName))
                {
                    builderProtocols.Where(c => c.STATION_EXTENDED.OwnerName, ConditionOperator.Like, "%" + ownerName + "%");
                }

                if ((processId == null)
                    && (freq == null)
                    && (permissionStart == null)
                    && (permissionStop == null)
                    && (DateMeasMonth == null)
                    && (DateMeasYear == null)
                    && (DateMeasDay == null)
                    && string.IsNullOrEmpty(permissionNumber)
                    && (DateCreated == null)
                    && (DateStart == null)
                    && (DateStop == null)
                    && string.IsNullOrEmpty(createdBy)
                    && string.IsNullOrEmpty(standard)
                    && string.IsNullOrEmpty(province)
                    && string.IsNullOrEmpty(ownerName)
                    )
                {
                    builderProtocols.Where(c => c.Id, ConditionOperator.GreaterThan, 0);
                    builderProtocols.Where(c => c.DateMeasYear, ConditionOperator.IsNotNull);
                    builderProtocols.Where(c => c.DateMeasMonth, ConditionOperator.IsNotNull);
                    builderProtocols.Where(c => c.DateMeasDay, ConditionOperator.IsNotNull);
                    builderProtocols.OrderByDesc(c => c.DateMeasYear);
                    builderProtocols.OrderByDesc(c => c.DateMeasMonth);
                    builderProtocols.OrderByDesc(c => c.DateMeasDay);
                    builderProtocols.OnTop(200);
                }
                queryExecuter.Fetch(builderProtocols, readerProtocols =>
                {
                    while (readerProtocols.Read())
                    {
                        var protocols = new Protocols();
                        protocols.DataRefSpectrum = new DataRefSpectrum();
                        var dateMeas = new DateTime(readerProtocols.GetValue(c => c.DateMeasYear), readerProtocols.GetValue(c => c.DateMeasMonth), readerProtocols.GetValue(c => c.DateMeasDay));
                        protocols.DataRefSpectrum.DateMeas = dateMeas;
                        protocols.DataRefSpectrum.DispersionLow = readerProtocols.GetValue(c => c.DispersionLow);
                        protocols.DataRefSpectrum.DispersionUp = readerProtocols.GetValue(c => c.DispersionUp);
                        protocols.DataRefSpectrum.Freq_MHz = readerProtocols.GetValue(c => c.Freq_MHz);
                        protocols.DataRefSpectrum.GlobalSID = readerProtocols.GetValue(c => c.GlobalSID);
                        protocols.DataRefSpectrum.Level_dBm = readerProtocols.GetValue(c => c.Level_dBm);
                        protocols.DataRefSpectrum.Percent = readerProtocols.GetValue(c => c.Percent);
                        if (readerProtocols.GetValue(c => c.SensorId) != null)
                        {
                            protocols.DataRefSpectrum.SensorId = readerProtocols.GetValue(c => c.SensorId).Value;
                            protocols.Sensor = loadSensor.LoadBaseDateSensor(readerProtocols.GetValue(c => c.SensorId).Value);
                        }
                        protocols.DataRefSpectrum.TableId= readerProtocols.GetValue(c => c.STATION_EXTENDED.TableId);
                        protocols.DataRefSpectrum.TableName = readerProtocols.GetValue(c => c.STATION_EXTENDED.TableName);

                        if ((readerProtocols.GetValue(c => c.RadioControlBandWidth) != null) && (readerProtocols.GetValue(c => c.RadioControlMeasFreq_MHz) != null))
                        {
                            var radioControlParams = new RadioControlParams();
                            radioControlParams.RadioControlBandWidth = readerProtocols.GetValue(c => c.RadioControlBandWidth);
                            radioControlParams.RadioControlMeasFreq_MHz = readerProtocols.GetValue(c => c.RadioControlMeasFreq_MHz);
                            protocols.RadioControlParams = radioControlParams;
                        }

                        protocols.StationExtended = new StationExtended();
                        protocols.StationExtended.Address = readerProtocols.GetValue(c => c.STATION_EXTENDED.Address);
                        protocols.StationExtended.BandWidth = readerProtocols.GetValue(c => c.STATION_EXTENDED.BandWidth);
                        protocols.StationExtended.DesigEmission = readerProtocols.GetValue(c => c.STATION_EXTENDED.DesigEmission);
                        protocols.StationExtended.Id = readerProtocols.GetValue(c => c.STATION_EXTENDED.Id);
                        protocols.StationExtended.Location = new DataLocation();
                        protocols.StationExtended.Location.Latitude = readerProtocols.GetValue(c => c.STATION_EXTENDED.Latitude);
                        protocols.StationExtended.Location.Longitude = readerProtocols.GetValue(c => c.STATION_EXTENDED.Longitude);
                        protocols.StationExtended.OwnerName = readerProtocols.GetValue(c => c.STATION_EXTENDED.OwnerName);
                        protocols.StationExtended.PermissionNumber = readerProtocols.GetValue(c => c.STATION_EXTENDED.PermissionNumber);
                        protocols.StationExtended.PermissionStart = readerProtocols.GetValue(c => c.STATION_EXTENDED.PermissionStart);
                        protocols.StationExtended.PermissionStop = readerProtocols.GetValue(c => c.STATION_EXTENDED.PermissionStop);
                        protocols.StationExtended.Province = readerProtocols.GetValue(c => c.STATION_EXTENDED.Province);
                        protocols.StationExtended.Standard = readerProtocols.GetValue(c => c.STATION_EXTENDED.Standard);
                        protocols.StationExtended.StandardName = readerProtocols.GetValue(c => c.STATION_EXTENDED.StandardName);
                        protocols.StationExtended.TableId = readerProtocols.GetValue(c => c.STATION_EXTENDED.TableId);
                        protocols.StationExtended.TableName = readerProtocols.GetValue(c => c.STATION_EXTENDED.TableName);

                        protocols.StationExtended.DocNum = readerProtocols.GetValue(c => c.STATION_EXTENDED.DocNum);
                        protocols.StationExtended.StatusMeas = readerProtocols.GetValue(c => c.STATION_EXTENDED.StatusMeas);
                        protocols.StationExtended.CurentStatusStation = readerProtocols.GetValue(c => c.STATION_EXTENDED.CurentStatusStation);
                        protocols.StationExtended.PermissionGlobalSID = readerProtocols.GetValue(c => c.STATION_EXTENDED.PermissionGlobalSID);
                        protocols.StationExtended.TestStartDate = readerProtocols.GetValue(c => c.STATION_EXTENDED.TestStartDate);
                        protocols.StationExtended.TestStopDate = readerProtocols.GetValue(c => c.STATION_EXTENDED.TestStopDate);
                        protocols.StationExtended.PermissionCancelDate = readerProtocols.GetValue(c => c.STATION_EXTENDED.PermissionCancelDate);
                        protocols.StationExtended.StationTxFreq = readerProtocols.GetValue(c => c.STATION_EXTENDED.StationTxFreq);
                        protocols.StationExtended.StationRxFreq = readerProtocols.GetValue(c => c.STATION_EXTENDED.StationRxFreq);
                        protocols.StationExtended.StationChannel = readerProtocols.GetValue(c => c.STATION_EXTENDED.StationChannel);

                        protocols.DataSynchronizationProcess = loadSynchroProcessData.CurrentSynchronizationProcesByIds(readerProtocols.GetValue(c => c.SYNCHRO_PROCESS.Id));

                        


                        var builderLinkProtocolsWithEmittings = this._dataLayer.GetBuilder<MD.ILinkProtocolsWithEmittings>().From();
                        builderLinkProtocolsWithEmittings.Select(c => c.Bandwidth_kHz, c => c.Contravention, c => c.CorrectnessEstimations, c => c.CurentPower_dBm, c => c.Freq_kHz, c => c.Id, c => c.LevelsDistributionCount, c => c.LevelsDistributionLvl, c => c.Levels_dBm, c => c.Loss_dB, c => c.MarkerIndex, c => c.MeanDeviationFromReference, c => c.Probability, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.SignalLevel_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.T1, c => c.T2, c => c.TraceCount, c => c.TriggerDeviationFromReference, c => c.WorkTimeStart, c => c.WorkTimeStop);
                        builderLinkProtocolsWithEmittings.Where(c => c.PROTOCOLS.Id, ConditionOperator.Equal, readerProtocols.GetValue(c => c.Id));
                        builderLinkProtocolsWithEmittings.Where(c => c.PROTOCOLS.SensorId, ConditionOperator.Equal, readerProtocols.GetValue(c => c.SensorId));
                        if (probability != null)
                        {
                            builderLinkProtocolsWithEmittings.Where(c => c.Probability, ConditionOperator.Equal, probability);
                        }
                        queryExecuter.Fetch(builderLinkProtocolsWithEmittings, readerLinkProtocolsWithEmittings =>
                        {
                            while (readerLinkProtocolsWithEmittings.Read())
                            {
                                protocols.ProtocolsLinkedWithEmittings = new ProtocolsWithEmittings();
                                protocols.ProtocolsLinkedWithEmittings.Bandwidth_kHz = readerLinkProtocolsWithEmittings.GetValue(c => c.Bandwidth_kHz);
                                protocols.ProtocolsLinkedWithEmittings.Contravention = readerLinkProtocolsWithEmittings.GetValue(c => c.Contravention);
                                protocols.ProtocolsLinkedWithEmittings.CorrectnessEstimations = readerLinkProtocolsWithEmittings.GetValue(c => c.CorrectnessEstimations);
                                protocols.ProtocolsLinkedWithEmittings.CurentPower_dBm = readerLinkProtocolsWithEmittings.GetValue(c => c.CurentPower_dBm);
                                protocols.ProtocolsLinkedWithEmittings.Freq_kHz = readerLinkProtocolsWithEmittings.GetValue(c => c.Freq_kHz);
                                protocols.ProtocolsLinkedWithEmittings.Id = readerLinkProtocolsWithEmittings.GetValue(c => c.Id);
                                protocols.ProtocolsLinkedWithEmittings.Count = readerLinkProtocolsWithEmittings.GetValue(c => c.LevelsDistributionCount);
                                protocols.ProtocolsLinkedWithEmittings.Levels = readerLinkProtocolsWithEmittings.GetValue(c => c.LevelsDistributionLvl);
                                protocols.ProtocolsLinkedWithEmittings.Levels_dBm = readerLinkProtocolsWithEmittings.GetValue(c => c.Levels_dBm);
                                protocols.ProtocolsLinkedWithEmittings.Loss_dB = readerLinkProtocolsWithEmittings.GetValue(c => c.Loss_dB);
                                protocols.ProtocolsLinkedWithEmittings.MarkerIndex = readerLinkProtocolsWithEmittings.GetValue(c => c.MarkerIndex);
                                protocols.ProtocolsLinkedWithEmittings.MeanDeviationFromReference = readerLinkProtocolsWithEmittings.GetValue(c => c.MeanDeviationFromReference);
                                protocols.ProtocolsLinkedWithEmittings.Probability = readerLinkProtocolsWithEmittings.GetValue(c => c.Probability);
                                protocols.ProtocolsLinkedWithEmittings.ReferenceLevel_dBm = readerLinkProtocolsWithEmittings.GetValue(c => c.ReferenceLevel_dBm);
                                protocols.ProtocolsLinkedWithEmittings.RollOffFactor = readerLinkProtocolsWithEmittings.GetValue(c => c.RollOffFactor);
                                protocols.ProtocolsLinkedWithEmittings.SignalLevel_dBm = readerLinkProtocolsWithEmittings.GetValue(c => c.SignalLevel_dBm);
                                protocols.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz = readerLinkProtocolsWithEmittings.GetValue(c => c.SpectrumStartFreq_MHz);
                                protocols.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz = readerLinkProtocolsWithEmittings.GetValue(c => c.SpectrumSteps_kHz);
                                protocols.ProtocolsLinkedWithEmittings.StandardBW = readerLinkProtocolsWithEmittings.GetValue(c => c.StandardBW);
                                protocols.ProtocolsLinkedWithEmittings.StartFrequency_MHz = readerLinkProtocolsWithEmittings.GetValue(c => c.StartFrequency_MHz);
                                protocols.ProtocolsLinkedWithEmittings.StopFrequency_MHz = readerLinkProtocolsWithEmittings.GetValue(c => c.StopFrequency_MHz);
                                protocols.ProtocolsLinkedWithEmittings.T1 = readerLinkProtocolsWithEmittings.GetValue(c => c.T1);
                                protocols.ProtocolsLinkedWithEmittings.T2 = readerLinkProtocolsWithEmittings.GetValue(c => c.T2);
                                protocols.ProtocolsLinkedWithEmittings.TraceCount = readerLinkProtocolsWithEmittings.GetValue(c => c.TraceCount);
                                protocols.ProtocolsLinkedWithEmittings.TriggerDeviationFromReference = readerLinkProtocolsWithEmittings.GetValue(c => c.TriggerDeviationFromReference);
                                protocols.ProtocolsLinkedWithEmittings.WorkTimeStart = readerLinkProtocolsWithEmittings.GetValue(c => c.WorkTimeStart);
                                protocols.ProtocolsLinkedWithEmittings.WorkTimeStop = readerLinkProtocolsWithEmittings.GetValue(c => c.WorkTimeStop);
                            }
                            return true;
                        });

                        listProtocols.Add(protocols);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listProtocols.ToArray();
        }

        /// <summary>
        /// Основной метод, возвращающий массив объектов HeadProtocols
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="createdBy"></param>
        /// <param name="DateCreated"></param>
        /// <param name="DateStart"></param>
        /// <param name="DateStop"></param>
        /// <param name="DateMeasDay"></param>
        /// <param name="DateMeasMonth"></param>
        /// <param name="DateMeasYear"></param>
        /// <param name="freq"></param>
        /// <param name="probability"></param>
        /// <param name="standard"></param>
        /// <param name="province"></param>
        /// <param name="ownerName"></param>
        /// <param name="permissionNumber"></param>
        /// <param name="permissionStart"></param>
        /// <param name="permissionStop"></param>
        /// <returns></returns>
        public HeadProtocols[] GetDetailProtocolsByParameters(
                                long? processId,
                                string createdBy,
                                DateTime? DateCreated,
                                DateTime? DateStart,
                                DateTime? DateStop,
                                short? DateMeasDay,
                                short? DateMeasMonth,
                                short? DateMeasYear,
                                double? freq,
                                double? probability,
                                string standard,
                                string province,
                                string ownerName,
                                string permissionNumber,
                                DateTime? permissionStart,
                                DateTime? permissionStop)
        {
            var loadSynchroProcessData = new Utils(this._dataLayer, this._logger);
            var loadSensor = new LoadSensor(this._dataLayer, this._logger);
            var listHeadProtocols = new List<HeadProtocols>();
            var listDetailProtocols = new List<DetailProtocols>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetProtocolsByParametersMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

                var builderProtocols = this._dataLayer.GetBuilder<MD.IProtocols>().From();
                builderProtocols.Select(c => c.DateMeasDay,
                                        c => c.DateMeasMonth,
                                        c => c.DateMeasYear,
                                        c => c.DispersionLow,
                                        c => c.DispersionUp,
                                        c => c.Freq_MHz,
                                        c => c.GlobalSID,
                                        c => c.Id,
                                        c => c.Level_dBm,
                                        c => c.Percent,
                                        c => c.PermissionNumber,
                                        c => c.PermissionStart,
                                        c => c.PermissionStop,
                                        c => c.SensorLat,
                                        c => c.SensorLon,
                                        c => c.SensorName,
                                        c => c.SensorId,
                                        c => c.RadioControlBandWidth,
                                        c => c.RadioControlMeasFreq_MHz,
                                        c => c.STATION_EXTENDED.TableId,
                                        c => c.STATION_EXTENDED.TableName,
                                        c => c.STATION_EXTENDED.Address,
                                        c => c.STATION_EXTENDED.BandWidth,
                                        c => c.STATION_EXTENDED.DesigEmission,
                                        c => c.STATION_EXTENDED.Id,
                                        c => c.STATION_EXTENDED.Latitude,
                                        c => c.STATION_EXTENDED.Longitude,
                                        c => c.STATION_EXTENDED.OwnerName,
                                        c => c.STATION_EXTENDED.PermissionNumber,
                                        c => c.STATION_EXTENDED.PermissionStart,
                                        c => c.STATION_EXTENDED.PermissionStop,
                                        c => c.STATION_EXTENDED.Province,
                                        c => c.STATION_EXTENDED.Standard,
                                        c => c.STATION_EXTENDED.StandardName,
                                        c => c.STATION_EXTENDED.DocNum,
                                        c => c.STATION_EXTENDED.TestStartDate,
                                        c => c.STATION_EXTENDED.TestStopDate,
                                        c => c.STATION_EXTENDED.PermissionCancelDate,
                                        c => c.STATION_EXTENDED.StatusMeas,
                                        c => c.STATION_EXTENDED.CurentStatusStation,
                                        c => c.STATION_EXTENDED.StationTxFreq,
                                        c => c.STATION_EXTENDED.StationRxFreq,
                                        c => c.STATION_EXTENDED.StationChannel,
                                        c => c.STATION_EXTENDED.PermissionGlobalSID,
                                        c => c.SYNCHRO_PROCESS.Id
                                        );


                var dic = new Dictionary<string, string>();
                dic.Add("T", "Operating according to Test");
                dic.Add("A", "Operating according to License");
                dic.Add("U", "Transmitter operation not fixed");
                dic.Add("I", "Illegally operated transmitter");
                dic.Add("N", "New");
                

                if (processId != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, processId);
                }

                if (freq != null)
                {
                    builderProtocols.Where(c => c.Freq_MHz, ConditionOperator.Equal, freq);
                }
                if (!string.IsNullOrEmpty(permissionNumber))
                {
                    builderProtocols.Where(c => c.PermissionNumber, ConditionOperator.Like, "%" + permissionNumber + "%");
                }
                if (permissionStart != null)
                {
                    builderProtocols.Where(c => c.PermissionStart, ConditionOperator.GreaterEqual, new DateTime(permissionStart.Value.Year, permissionStart.Value.Month, permissionStart.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.PermissionStart, ConditionOperator.LessEqual, new DateTime(permissionStart.Value.Year, permissionStart.Value.Month, permissionStart.Value.Day, 23, 59, 59, 999));
                }
                if (permissionStop != null)
                {
                    builderProtocols.Where(c => c.PermissionStop, ConditionOperator.GreaterEqual, new DateTime(permissionStop.Value.Year, permissionStop.Value.Month, permissionStop.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.PermissionStop, ConditionOperator.LessEqual, new DateTime(permissionStop.Value.Year, permissionStop.Value.Month, permissionStop.Value.Day, 23, 59, 59, 999));
                }
                if (DateMeasDay != null)
                {
                    builderProtocols.Where(c => c.DateMeasDay, ConditionOperator.Equal, DateMeasDay);
                }

                if (DateMeasMonth != null)
                {
                    builderProtocols.Where(c => c.DateMeasMonth, ConditionOperator.Equal, DateMeasMonth);
                }

                if (DateMeasYear != null)
                {
                    builderProtocols.Where(c => c.DateMeasYear, ConditionOperator.Equal, DateMeasYear);
                }

                if (DateCreated != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.CreatedDate, ConditionOperator.GreaterEqual, new DateTime(DateCreated.Value.Year, DateCreated.Value.Month, DateCreated.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.CreatedDate, ConditionOperator.LessEqual, new DateTime(DateCreated.Value.Year, DateCreated.Value.Month, DateCreated.Value.Day, 23, 59, 59, 999));
                }

                if (DateStart != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateStart, ConditionOperator.GreaterEqual, new DateTime(DateStart.Value.Year, DateStart.Value.Month, DateStart.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateStart, ConditionOperator.LessEqual, new DateTime(DateStart.Value.Year, DateStart.Value.Month, DateStart.Value.Day, 23, 59, 59, 999));
                }

                if (DateStop != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateEnd, ConditionOperator.GreaterEqual, new DateTime(DateStop.Value.Year, DateStop.Value.Month, DateStop.Value.Day, 0, 0, 0, 1));
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateEnd, ConditionOperator.LessEqual, new DateTime(DateStop.Value.Year, DateStop.Value.Month, DateStop.Value.Day, 23, 59, 59, 999));
                }

                if (!string.IsNullOrEmpty(createdBy))
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.CreatedBy, ConditionOperator.Like, "%" + createdBy + "%");
                }

                if (!string.IsNullOrEmpty(standard))
                {
                    builderProtocols.Where(c => c.STATION_EXTENDED.Standard, ConditionOperator.Like, "%" + standard + "%");
                }

                if (!string.IsNullOrEmpty(province))
                {
                    builderProtocols.Where(c => c.STATION_EXTENDED.Province, ConditionOperator.Like, "%" + province + "%");
                }

                if (!string.IsNullOrEmpty(ownerName))
                {
                    builderProtocols.Where(c => c.STATION_EXTENDED.OwnerName, ConditionOperator.Like, "%" + ownerName + "%");
                }

                if ((processId == null)
                    && (freq == null)
                    && (permissionStart == null)
                    && (permissionStop == null)
                    && (DateMeasMonth == null)
                    && (DateMeasYear == null)
                    && (DateMeasDay == null)
                    && string.IsNullOrEmpty(permissionNumber)
                    && (DateCreated == null)
                    && (DateStart == null)
                    && (DateStop == null)
                    && string.IsNullOrEmpty(createdBy)
                    && string.IsNullOrEmpty(standard)
                    && string.IsNullOrEmpty(province)
                    && string.IsNullOrEmpty(ownerName)
                    )
                {
                    builderProtocols.Where(c => c.Id, ConditionOperator.GreaterThan, 0);
                    builderProtocols.Where(c => c.DateMeasYear, ConditionOperator.IsNotNull);
                    builderProtocols.Where(c => c.DateMeasMonth, ConditionOperator.IsNotNull);
                    builderProtocols.Where(c => c.DateMeasDay, ConditionOperator.IsNotNull);
                    builderProtocols.OrderByDesc(c => c.DateMeasYear);
                    builderProtocols.OrderByDesc(c => c.DateMeasMonth);
                    builderProtocols.OrderByDesc(c => c.DateMeasDay);
                    builderProtocols.OnTop(200);
                }
                queryExecuter.Fetch(builderProtocols, readerProtocols =>
                {
                    while (readerProtocols.Read())
                    {


                        var dateMeas = new DateTime(readerProtocols.GetValue(c => c.DateMeasYear), readerProtocols.GetValue(c => c.DateMeasMonth), readerProtocols.GetValue(c => c.DateMeasDay));
                        var process = loadSynchroProcessData.CurrentSynchronizationProcesByIds(readerProtocols.GetValue(c => c.SYNCHRO_PROCESS.Id));
                        var protocols = new DetailProtocols();

                        protocols.DateMeas = dateMeas;
                        protocols.GlobalSID = readerProtocols.GetValue(c => c.GlobalSID);
                        protocols.PermissionGlobalSID = readerProtocols.GetValue(c => c.STATION_EXTENDED.PermissionGlobalSID);
                        protocols.OwnerName = readerProtocols.GetValue(c => c.STATION_EXTENDED.OwnerName);
                        protocols.Address = readerProtocols.GetValue(c => c.STATION_EXTENDED.Address);
                        protocols.Latitude = readerProtocols.GetValue(c => c.STATION_EXTENDED.Latitude);
                        protocols.Longitude = readerProtocols.GetValue(c => c.STATION_EXTENDED.Longitude);
                        protocols.Id = readerProtocols.GetValue(c => c.Id);

                        var permissionNumberTemp = string.Empty;
                        if (readerProtocols.GetValue(c => c.PermissionNumber)==null)
                        {
                            if (readerProtocols.GetValue(c => c.STATION_EXTENDED.DocNum)!=null)
                            {
                                permissionNumberTemp = readerProtocols.GetValue(c => c.STATION_EXTENDED.DocNum);
                            }
                        }
                        else
                        {
                            permissionNumberTemp = readerProtocols.GetValue(c => c.PermissionNumber);
                        }

                        DateTime? permissionStartTemp = null;
                        if (readerProtocols.GetValue(c => c.PermissionStart) == null)
                        {
                            if (readerProtocols.GetValue(c => c.STATION_EXTENDED.TestStartDate) != null)
                            {
                                permissionStartTemp = readerProtocols.GetValue(c => c.STATION_EXTENDED.TestStartDate);
                            }
                        }
                        else
                        {
                            permissionStartTemp = readerProtocols.GetValue(c => c.PermissionStart);
                        }

                        DateTime? permissionStopTemp = null;
                        if (readerProtocols.GetValue(c => c.PermissionStop) == null)
                        {
                            if (readerProtocols.GetValue(c => c.STATION_EXTENDED.PermissionCancelDate) != null)
                            {
                                permissionStopTemp = readerProtocols.GetValue(c => c.STATION_EXTENDED.PermissionCancelDate);
                            }
                            else if (readerProtocols.GetValue(c => c.STATION_EXTENDED.TestStopDate) != null)
                            {
                                permissionStopTemp = readerProtocols.GetValue(c => c.STATION_EXTENDED.TestStopDate);
                            }
                        }
                        else
                        {
                            permissionStopTemp = readerProtocols.GetValue(c => c.PermissionStop);
                        }

                        if (string.IsNullOrEmpty(permissionNumberTemp))
                        {
                            var protocolId = readerProtocols.GetValue(c => c.Id);
                            var stationId = readerProtocols.GetValue(c => c.STATION_EXTENDED.Id);
                            this._logger.Warning(Contexts.ThisComponent, Categories.Processing, (EventText)$"For protocol.Id='{protocolId}' and associated station.Id='{stationId}' not found 'PermissionNumber'");
                        }

                        protocols.PermissionNumber = permissionNumberTemp;
                        protocols.PermissionStart = permissionStartTemp;
                        protocols.PermissionStop = permissionStopTemp;
                        var sensorData = loadSensor.LoadBaseDateSensor(readerProtocols.GetValue(c => c.SensorId).Value);
                        protocols.TitleSensor = sensorData.Title;
                        protocols.StandardName = readerProtocols.GetValue(c => c.STATION_EXTENDED.StandardName);
                        protocols.DateMeas_OnlyDate = dateMeas.Date;
                        protocols.DateMeas_OnlyTime = dateMeas.TimeOfDay;
                        protocols.DateCreated = process.DateCreated;
                        protocols.CreatedBy = process.CreatedBy;
                        //protocols.DurationMeasurement = 
                        //protocols.FieldStrength
                        protocols.Freq_MHz = readerProtocols.GetValue(c => c.Freq_MHz);
                        if ((readerProtocols.GetValue(c => c.RadioControlBandWidth) != null) && (readerProtocols.GetValue(c => c.RadioControlMeasFreq_MHz) != null))
                        {
                            protocols.RadioControlBandWidth_KHz = readerProtocols.GetValue(c => c.RadioControlBandWidth);
                            protocols.RadioControlMeasFreq_MHz = readerProtocols.GetValue(c => c.RadioControlMeasFreq_MHz);
                            protocols.RadioControlDeviationFreq_MHz = Math.Abs(protocols.RadioControlMeasFreq_MHz.Value - readerProtocols.GetValue(c => c.Freq_MHz));
                        }
                        if ((sensorData.Locations != null) && (sensorData.Locations.Length > 0))
                        {
                            protocols.SensorLatitude = sensorData.Locations[sensorData.Locations.Length - 1].Lat;
                            protocols.SensorLongitude = sensorData.Locations[sensorData.Locations.Length - 1].Lon;
                        }
                        protocols.SensorName = sensorData.Name;
                        protocols.Standard = readerProtocols.GetValue(c => c.STATION_EXTENDED.Standard);
                        var channels = readerProtocols.GetValue(c => c.STATION_EXTENDED.StationChannel);
                        if ((channels != null) && (channels.Length > 0))
                        {
                            protocols.StationChannel = string.Join(";", channels);
                        }
                        var txFreq = readerProtocols.GetValue(c => c.STATION_EXTENDED.StationTxFreq);
                        if ((txFreq != null) && (txFreq.Length > 0))
                        {
                            protocols.StationTxFreq = string.Join(";", txFreq);
                        }
                        protocols.StatusMeas = readerProtocols.GetValue(c => c.STATION_EXTENDED.StatusMeas);
                        if (protocols.StatusMeas != null)
                        {
                            protocols.StatusMeas = dic[protocols.StatusMeas];
                        }
                        protocols.BandWidth = readerProtocols.GetValue(c => c.STATION_EXTENDED.BandWidth);
                        protocols.CurentStatusStation = readerProtocols.GetValue(c => c.STATION_EXTENDED.CurentStatusStation);
                        protocols.Level_dBm = readerProtocols.GetValue(c => c.Level_dBm);


                        var builderLinkProtocolsWithEmittings = this._dataLayer.GetBuilder<MD.ILinkProtocolsWithEmittings>().From();
                        builderLinkProtocolsWithEmittings.Select(c => c.Bandwidth_kHz, c => c.Contravention, c => c.CorrectnessEstimations, c => c.CurentPower_dBm, c => c.Freq_kHz, c => c.Id, c => c.LevelsDistributionCount, c => c.LevelsDistributionLvl, c => c.Levels_dBm, c => c.Loss_dB, c => c.MarkerIndex, c => c.MeanDeviationFromReference, c => c.Probability, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.SignalLevel_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.T1, c => c.T2, c => c.TraceCount, c => c.TriggerDeviationFromReference, c => c.WorkTimeStart, c => c.WorkTimeStop);
                        builderLinkProtocolsWithEmittings.Where(c => c.PROTOCOLS.Id, ConditionOperator.Equal, readerProtocols.GetValue(c => c.Id));
                        builderLinkProtocolsWithEmittings.Where(c => c.PROTOCOLS.SensorId, ConditionOperator.Equal, readerProtocols.GetValue(c => c.SensorId));
                        if (probability != null)
                        {
                            builderLinkProtocolsWithEmittings.Where(c => c.Probability, ConditionOperator.Equal, probability);
                        }
                        queryExecuter.Fetch(builderLinkProtocolsWithEmittings, readerLinkProtocolsWithEmittings =>
                        {
                            while (readerLinkProtocolsWithEmittings.Read())
                            {
                                protocols.ProtocolsLinkedWithEmittings = new ProtocolsWithEmittings();
                                protocols.ProtocolsLinkedWithEmittings.Bandwidth_kHz = readerLinkProtocolsWithEmittings.GetValue(c => c.Bandwidth_kHz);
                                protocols.ProtocolsLinkedWithEmittings.Contravention = readerLinkProtocolsWithEmittings.GetValue(c => c.Contravention);
                                protocols.ProtocolsLinkedWithEmittings.CorrectnessEstimations = readerLinkProtocolsWithEmittings.GetValue(c => c.CorrectnessEstimations);
                                protocols.ProtocolsLinkedWithEmittings.CurentPower_dBm = readerLinkProtocolsWithEmittings.GetValue(c => c.CurentPower_dBm);
                                protocols.ProtocolsLinkedWithEmittings.Freq_kHz = readerLinkProtocolsWithEmittings.GetValue(c => c.Freq_kHz);
                                protocols.ProtocolsLinkedWithEmittings.Id = readerLinkProtocolsWithEmittings.GetValue(c => c.Id);
                                protocols.ProtocolsLinkedWithEmittings.Count = readerLinkProtocolsWithEmittings.GetValue(c => c.LevelsDistributionCount);
                                protocols.ProtocolsLinkedWithEmittings.Levels = readerLinkProtocolsWithEmittings.GetValue(c => c.LevelsDistributionLvl);
                                protocols.ProtocolsLinkedWithEmittings.Levels_dBm = readerLinkProtocolsWithEmittings.GetValue(c => c.Levels_dBm);
                                protocols.ProtocolsLinkedWithEmittings.Loss_dB = readerLinkProtocolsWithEmittings.GetValue(c => c.Loss_dB);
                                protocols.ProtocolsLinkedWithEmittings.MarkerIndex = readerLinkProtocolsWithEmittings.GetValue(c => c.MarkerIndex);
                                protocols.ProtocolsLinkedWithEmittings.MeanDeviationFromReference = readerLinkProtocolsWithEmittings.GetValue(c => c.MeanDeviationFromReference);
                                protocols.ProtocolsLinkedWithEmittings.Probability = readerLinkProtocolsWithEmittings.GetValue(c => c.Probability);
                                protocols.ProtocolsLinkedWithEmittings.ReferenceLevel_dBm = readerLinkProtocolsWithEmittings.GetValue(c => c.ReferenceLevel_dBm);
                                protocols.ProtocolsLinkedWithEmittings.RollOffFactor = readerLinkProtocolsWithEmittings.GetValue(c => c.RollOffFactor);
                                protocols.ProtocolsLinkedWithEmittings.SignalLevel_dBm = readerLinkProtocolsWithEmittings.GetValue(c => c.SignalLevel_dBm);
                                protocols.ProtocolsLinkedWithEmittings.SpectrumStartFreq_MHz = readerLinkProtocolsWithEmittings.GetValue(c => c.SpectrumStartFreq_MHz);
                                protocols.ProtocolsLinkedWithEmittings.SpectrumSteps_kHz = readerLinkProtocolsWithEmittings.GetValue(c => c.SpectrumSteps_kHz);
                                protocols.ProtocolsLinkedWithEmittings.StandardBW = readerLinkProtocolsWithEmittings.GetValue(c => c.StandardBW);
                                protocols.ProtocolsLinkedWithEmittings.StartFrequency_MHz = readerLinkProtocolsWithEmittings.GetValue(c => c.StartFrequency_MHz);
                                protocols.ProtocolsLinkedWithEmittings.StopFrequency_MHz = readerLinkProtocolsWithEmittings.GetValue(c => c.StopFrequency_MHz);
                                protocols.ProtocolsLinkedWithEmittings.T1 = readerLinkProtocolsWithEmittings.GetValue(c => c.T1);
                                protocols.ProtocolsLinkedWithEmittings.T2 = readerLinkProtocolsWithEmittings.GetValue(c => c.T2);
                                protocols.ProtocolsLinkedWithEmittings.TraceCount = readerLinkProtocolsWithEmittings.GetValue(c => c.TraceCount);
                                protocols.ProtocolsLinkedWithEmittings.TriggerDeviationFromReference = readerLinkProtocolsWithEmittings.GetValue(c => c.TriggerDeviationFromReference);
                                protocols.ProtocolsLinkedWithEmittings.WorkTimeStart = readerLinkProtocolsWithEmittings.GetValue(c => c.WorkTimeStart);
                                protocols.ProtocolsLinkedWithEmittings.WorkTimeStop = readerLinkProtocolsWithEmittings.GetValue(c => c.WorkTimeStop);
                                if ((readerLinkProtocolsWithEmittings.GetValue(c => c.WorkTimeStop) != null) && (readerLinkProtocolsWithEmittings.GetValue(c => c.WorkTimeStart) != null))
                                {
                                    protocols.DurationMeasurement = readerLinkProtocolsWithEmittings.GetValue(c => c.WorkTimeStop) - readerLinkProtocolsWithEmittings.GetValue(c => c.WorkTimeStart);
                                }
                            }
                            return true;
                        });
                        listDetailProtocols.Add(protocols);
                    }
                    return true;
                });

                for (int i = 0; i < listDetailProtocols.Count; i++)
                {
                    var detailProtocol = listDetailProtocols[i];
                    var protocolDataToGroup = new HeadProtocols();
                    protocolDataToGroup.Id = detailProtocol.Id;
                    protocolDataToGroup.PermissionGlobalSID = detailProtocol.PermissionGlobalSID;
                    protocolDataToGroup.OwnerName = detailProtocol.OwnerName;
                    protocolDataToGroup.Address = detailProtocol.Address;
                    protocolDataToGroup.PermissionNumber = detailProtocol.PermissionNumber;
                    protocolDataToGroup.PermissionStart = detailProtocol.PermissionStart;
                    protocolDataToGroup.PermissionStop = detailProtocol.PermissionStop;
                    protocolDataToGroup.Longitude = detailProtocol.Longitude;
                    protocolDataToGroup.Latitude = detailProtocol.Latitude;


                    if ((protocolDataToGroup.Longitude != null) && (protocolDataToGroup.Latitude != null) && (!string.IsNullOrEmpty(protocolDataToGroup.PermissionNumber)))
                    {
                        var findDataGroup = listHeadProtocols.Find(x =>  x.Id!= protocolDataToGroup.Id && x.Address == protocolDataToGroup.Address && x.OwnerName == protocolDataToGroup.OwnerName && x.PermissionGlobalSID == protocolDataToGroup.PermissionGlobalSID && x.PermissionNumber == protocolDataToGroup.PermissionNumber && x.PermissionStart == protocolDataToGroup.PermissionStart && x.PermissionStop == protocolDataToGroup.PermissionStop && (Math.Abs(x.Latitude.Value - protocolDataToGroup.Latitude.Value) < 0.0000001) && (Math.Abs(x.Longitude.Value - protocolDataToGroup.Longitude.Value) < 0.0000001));
                        if (findDataGroup == null)
                        {
                            listHeadProtocols.Add(protocolDataToGroup);
                        }
                    }
                    else if ((protocolDataToGroup.Longitude != null) && (protocolDataToGroup.Latitude != null) && (string.IsNullOrEmpty(protocolDataToGroup.PermissionNumber)))
                    {
                        var findDataGroup = listHeadProtocols.Find(x => x.Id != protocolDataToGroup.Id && (Math.Abs(x.Latitude.Value - protocolDataToGroup.Latitude.Value) < 0.0000001) && (Math.Abs(x.Longitude.Value - protocolDataToGroup.Longitude.Value) < 0.0000001));
                        if (findDataGroup == null)
                        {
                            listHeadProtocols.Add(protocolDataToGroup);
                        }
                    }
                    else
                    {
                        this._logger.Warning(Contexts.ThisComponent, Categories.Processing, (EventText)$"In the process of generate the list of protocols, a situation was discovered when the coordinates of the station were not determined!");
                    }
                }

                var orderByProtocolsOutput = from z in listHeadProtocols orderby z.PermissionGlobalSID, z.OwnerName, z.Address, z.PermissionNumber, z.PermissionStart, z.PermissionStop, z.Longitude, z.Latitude ascending select z;
                listHeadProtocols = orderByProtocolsOutput.ToList();
                var allDetailProtocolsForPermTemp = new List<DetailProtocols>();
                for (int i = 0; i < listHeadProtocols.Count; i++)
                {
                    var headProtocol = listHeadProtocols[i];
                    if (headProtocol!=null)
                    {
                        var allDetailProtocolsForPerm = new List<DetailProtocols>();

                        if ((headProtocol.Longitude != null) && (headProtocol.Latitude != null) && (!string.IsNullOrEmpty(headProtocol.PermissionNumber)))
                        {
                            allDetailProtocolsForPerm = listDetailProtocols.FindAll(x => x.Address == headProtocol.Address && x.OwnerName == headProtocol.OwnerName && x.PermissionGlobalSID == headProtocol.PermissionGlobalSID && x.PermissionNumber == headProtocol.PermissionNumber && x.PermissionStart == headProtocol.PermissionStart && x.PermissionStop == headProtocol.PermissionStop && (Math.Abs(x.Latitude.Value - headProtocol.Latitude.Value) < 0.0000001) && (Math.Abs(x.Longitude.Value - headProtocol.Longitude.Value) < 0.0000001));
                        }
                        else if ((headProtocol.Longitude != null) && (headProtocol.Latitude != null) && (string.IsNullOrEmpty(headProtocol.PermissionNumber)))
                        {
                            allDetailProtocolsForPerm = listDetailProtocols.FindAll(x => (Math.Abs(x.Latitude.Value - headProtocol.Latitude.Value) < 0.0000001) && (Math.Abs(x.Longitude.Value - headProtocol.Longitude.Value) < 0.0000001));
                        }
                          
                        if ((allDetailProtocolsForPerm!=null) && (allDetailProtocolsForPerm.Count>0))
                        {
                            headProtocol.Address = allDetailProtocolsForPerm[0].Address;
                            headProtocol.CreatedBy = allDetailProtocolsForPerm[0].CreatedBy;
                            headProtocol.DateCreated = allDetailProtocolsForPerm[0].DateCreated;
                            headProtocol.DateMeas = allDetailProtocolsForPerm[0].DateMeas;
                            headProtocol.GlobalSID= allDetailProtocolsForPerm[0].GlobalSID;
                            headProtocol.Latitude = allDetailProtocolsForPerm[0].Latitude;
                            headProtocol.Longitude = allDetailProtocolsForPerm[0].Longitude;
                            headProtocol.OwnerName = allDetailProtocolsForPerm[0].OwnerName;
                            headProtocol.PermissionGlobalSID = allDetailProtocolsForPerm[0].PermissionGlobalSID;
                            headProtocol.PermissionNumber = allDetailProtocolsForPerm[0].PermissionNumber;
                            headProtocol.PermissionStart = allDetailProtocolsForPerm[0].PermissionStart;
                            headProtocol.PermissionStop = allDetailProtocolsForPerm[0].PermissionStop;
                            headProtocol.StandardName = allDetailProtocolsForPerm[0].StandardName;
                            headProtocol.TitleSensor= allDetailProtocolsForPerm[0].TitleSensor;

                            var allDetailProtocol = new List<DetailProtocols>();
                            for (int j=0; j< allDetailProtocolsForPerm.Count; j++)
                            {
                                var fndProtocolTemp = allDetailProtocolsForPermTemp.Find(x => x.Id == allDetailProtocolsForPerm[j].Id);
                                if (fndProtocolTemp==null)
                                {
                                    allDetailProtocol.Add(allDetailProtocolsForPerm[j]);
                                    allDetailProtocolsForPermTemp.Add(allDetailProtocolsForPerm[j]);
                                }
                            }
                            headProtocol.DetailProtocols = allDetailProtocol.ToArray();
                        }
                    }
                }
                listHeadProtocols.RemoveAll(x => x.DetailProtocols == null || (x.DetailProtocols != null && x.DetailProtocols.Length == 0));
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listHeadProtocols.ToArray();
        }
    }
}


