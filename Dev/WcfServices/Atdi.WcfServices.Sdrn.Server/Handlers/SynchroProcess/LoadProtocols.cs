/*
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




        public long[] GetHeadRefSpectrumIdsBySDRN(long? synchronizationId)
        {
            var headRefSpectrumIds = new List<long>();
            if (synchronizationId != null)
            {
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var queryLinkHeadRefSpectrumFrom = this._dataLayer.GetBuilder<MD.ILinkHeadRefSpectrum>()
                .From()
                .Select(c => c.Id, c => c.HEAD_REF_SPECTRUM.Id)
                .Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, synchronizationId);
                queryExecuter.Fetch(queryLinkHeadRefSpectrumFrom, readerLinkHeadRefSpectrum =>
                {
                    while (readerLinkHeadRefSpectrum.Read())
                    {
                        headRefSpectrumIds.Add(readerLinkHeadRefSpectrum.GetValue(c => c.HEAD_REF_SPECTRUM.));
                    }
                    return true;
                });
            }
            return headRefSpectrumIds.ToArray();
        }

        public Protocols[] GetProtocolsByParameters(
                                 string createdBy,
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
        var listProtocols = new List<Protocols>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetProtocolsByParametersMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderProtocols = this._dataLayer.GetBuilder<MD.IProtocols>().From();
                builderProtocols.Select(c => c.DateMeas, c => c.DispersionLow, c => c.DispersionUp, c => c.Freq_MHz, c => c.GlobalSID, c => c.Id, c => c.Level_dBm, c => c.Percent, c => c.PermissionNumber, c => c.PermissionStart, c => c.PermissionStop, c => c.SensorLat, c => c.SensorLon, c => c.SensorName, c => c.SensorId);
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
                if ((freq == null) && (permissionStart == null) && (permissionStop == null) && (DateMeas == null) && string.IsNullOrEmpty(permissionNumber))
                {
                    builderProtocols.Where(c => c.Id, ConditionOperator.GreaterThan, 0);
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
                        protocols.DataRefSpectrum.SensorId = readerProtocols.GetValue(c => c.SensorId);


                        protocols.StationExtended = new StationExtended();
                        

                        var stationExtended = new StationExtended();

                        var protocolsWithEmittings = new ProtocolsWithEmittings();
                        var dataRefSpectrum = new DataRefSpectrum();
                        var dataSynchronizationProcess = new DataSynchronizationProcess();
                        var sensor = new Sensor();

                        var builderDataRefSpectrum = this._dataLayer.GetBuilder<MD.IRefSpectrum>().From();
                        builderDataRefSpectrum.Select(c => c.DateMeas, c => c.DispersionLow, c => c.DispersionUp, c => c.Freq_MHz, c => c.GlobalSID, c => c.Id, c => c.IdNum, c => c.Level_dBm, c => c.Percent, c => c.SensorId, c => c.TableId, c => c.TableName);
                        builderDataRefSpectrum.Where(c => c.HEAD_REF_SPECTRUM.Id, ConditionOperator.Equal, readerRefSpectrum.GetValue(c => c.Id));
                        builderDataRefSpectrum.Where(c => c.SensorId, ConditionOperator.In, sensorIdsBySDRN);
                        queryExecuter.Fetch(builderDataRefSpectrum, readerDataRefSpectrum =>
                        {

                            while (readerDataRefSpectrum.Read())
                            {
                                var dataSpectrum = new DataRefSpectrum();
                                dataSpectrum.DateMeas = readerDataRefSpectrum.GetValue(c => c.DateMeas);
                                dataSpectrum.DispersionLow = readerDataRefSpectrum.GetValue(c => c.DispersionLow);
                                dataSpectrum.DispersionUp = readerDataRefSpectrum.GetValue(c => c.DispersionUp);
                                dataSpectrum.Freq_MHz = readerDataRefSpectrum.GetValue(c => c.Freq_MHz);
                                dataSpectrum.GlobalSID = readerDataRefSpectrum.GetValue(c => c.GlobalSID);
                                dataSpectrum.IdNum = readerDataRefSpectrum.GetValue(c => c.IdNum);
                                dataSpectrum.Level_dBm = readerDataRefSpectrum.GetValue(c => c.Level_dBm);
                                dataSpectrum.Percent = readerDataRefSpectrum.GetValue(c => c.Percent);
                                dataSpectrum.SensorId = readerDataRefSpectrum.GetValue(c => c.SensorId);
                                dataSpectrum.TableId = readerDataRefSpectrum.GetValue(c => c.TableId);
                                dataSpectrum.TableName = readerDataRefSpectrum.GetValue(c => c.TableName);
                                dataSpectrum.Id = readerDataRefSpectrum.GetValue(c => c.Id);
                                listDataSpectrum.Add(dataSpectrum);
                            }
                            return true;
                        });
                        refSpectrum.DataRefSpectrum = listDataSpectrum.ToArray();
                        listRefSpectrum.Add(refSpectrum);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listRefSpectrum.ToArray();
        }
    }
}
*/


