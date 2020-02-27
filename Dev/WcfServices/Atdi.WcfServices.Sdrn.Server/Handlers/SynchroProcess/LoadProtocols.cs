using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Xml;
using System.Linq;


namespace Atdi.WcfServices.Sdrn.Server
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
                                 long? protocolId,
                                 string createdBy,
                                 DateTime? DateCreated,
                                 DateTime? DateStart,
                                 DateTime? DateStop,
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
            var loadSynchroProcessData = new LoadSynchroProcessData(this._dataLayer, this._logger);
            var loadSensor = new LoadSensor(this._dataLayer, this._logger);
            var listProtocols = new List<Protocols>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetProtocolsByParametersMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();

                var builderProtocols = this._dataLayer.GetBuilder<MD.IProtocols>().From();
                builderProtocols.Select(c => c.DateMeas,
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
                                        c => c.SYNCHRO_PROCESS.Id

                                        );
                
                if (protocolId != null)
                {
                    builderProtocols.Where(c => c.Id, ConditionOperator.Equal, protocolId);
                }

                if (freq != null)
                {
                    builderProtocols.Where(c => c.Freq_MHz, ConditionOperator.Equal, freq);
                }
                if (!string.IsNullOrEmpty(permissionNumber))
                {
                    builderProtocols.Where(c => c.PermissionNumber, ConditionOperator.Equal, permissionNumber);
                }
                if (permissionStart != null)
                {
                    builderProtocols.Where(c => c.PermissionStart, ConditionOperator.Equal, permissionStart);
                }
                if (permissionStop != null)
                {
                    builderProtocols.Where(c => c.PermissionStop, ConditionOperator.Equal, permissionStop);
                }
                if (DateMeas != null)
                {
                    builderProtocols.Where(c => c.DateMeas, ConditionOperator.Equal, DateMeas);
                }

                if (DateCreated != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.CreatedDate, ConditionOperator.Equal, DateCreated);
                }

                if (DateStart != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateStart, ConditionOperator.LessEqual, DateStart);
                }

                if (DateStop != null)
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.DateEnd, ConditionOperator.GreaterEqual, DateStop);
                }

                if (!string.IsNullOrEmpty(createdBy))
                {
                    builderProtocols.Where(c => c.SYNCHRO_PROCESS.CreatedBy, ConditionOperator.Equal, createdBy);
                }

                if (!string.IsNullOrEmpty(standard))
                {
                    builderProtocols.Where(c => c.STATION_EXTENDED.Standard, ConditionOperator.Equal, standard);
                }

                if (!string.IsNullOrEmpty(province))
                {
                    builderProtocols.Where(c => c.STATION_EXTENDED.Province, ConditionOperator.Equal, province);
                }

                if (!string.IsNullOrEmpty(ownerName))
                {
                    builderProtocols.Where(c => c.STATION_EXTENDED.OwnerName, ConditionOperator.Equal, ownerName);
                }

                if ((protocolId == null) && (freq == null) && (permissionStart == null) && (permissionStop == null) && (DateMeas == null) && string.IsNullOrEmpty(permissionNumber))
                {
                    throw new Exception("");
                }
                queryExecuter.Fetch(builderProtocols, readerProtocols =>
                {
                    while (readerProtocols.Read())
                    {
                        var protocols = new Protocols();
                        protocols.DataRefSpectrum = new DataRefSpectrum();
                        protocols.DataRefSpectrum.DateMeas = readerProtocols.GetValue(c => c.DateMeas);
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

                        protocols.DataSynchronizationProcess = loadSynchroProcessData.CurrentSynchronizationProcesByIds(readerProtocols.GetValue(c => c.SYNCHRO_PROCESS.Id));

                        protocols.ProtocolsLinkedWithEmittings = new ProtocolsWithEmittings();


                        var builderLinkProtocolsWithEmittings = this._dataLayer.GetBuilder<MD.ILinkProtocolsWithEmittings>().From();
                        builderLinkProtocolsWithEmittings.Select(c => c.Bandwidth_kHz, c => c.Contravention, c => c.CorrectnessEstimations, c => c.CurentPower_dBm, c => c.Freq_kHz, c => c.Id, c => c.LevelsDistributionCount, c => c.LevelsDistributionLvl, c => c.Levels_dBm, c => c.Loss_dB, c => c.MarkerIndex, c => c.MeanDeviationFromReference, c => c.Probability, c => c.ReferenceLevel_dBm, c => c.RollOffFactor, c => c.SignalLevel_dBm, c => c.SpectrumStartFreq_MHz, c => c.SpectrumSteps_kHz, c => c.StandardBW, c => c.StartFrequency_MHz, c => c.StopFrequency_MHz, c => c.T1, c => c.T2, c => c.TraceCount, c => c.TriggerDeviationFromReference, c => c.WorkTimeStart, c => c.WorkTimeStop, c => c.RefLevels, c => c.RefLevelsStartFrequency_Hz, c => c.RefLevelsStepFrequency_Hz);
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
                                protocols.ProtocolsLinkedWithEmittings.ReferenceLevels = new ReferenceLevels();
                                if (readerLinkProtocolsWithEmittings.GetValue(c => c.RefLevels) != null)
                                {
                                    protocols.ProtocolsLinkedWithEmittings.ReferenceLevels.levels = readerLinkProtocolsWithEmittings.GetValue(c => c.RefLevels);
                                }
                                if (readerLinkProtocolsWithEmittings.GetValue(c => c.RefLevelsStartFrequency_Hz) != null)
                                {
                                    protocols.ProtocolsLinkedWithEmittings.ReferenceLevels.StartFrequency_Hz = readerLinkProtocolsWithEmittings.GetValue(c => c.RefLevelsStartFrequency_Hz).Value;
                                }
                                if (readerLinkProtocolsWithEmittings.GetValue(c => c.RefLevelsStepFrequency_Hz) != null)
                                {
                                    protocols.ProtocolsLinkedWithEmittings.ReferenceLevels.StepFrequency_Hz = readerLinkProtocolsWithEmittings.GetValue(c => c.RefLevelsStepFrequency_Hz).Value;
                                }
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
    }
}

