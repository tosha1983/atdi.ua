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
    public class LoadSynchroProcessData
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public LoadSynchroProcessData(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public DataSynchronizationProcess CurrentSynchronizationProcesByIds(long? processId)
        {
            DataSynchronizationProcess dataSynchronizationProcess = null;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCurrentDataSynchronizationProcessMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderSynchroProcess = this._dataLayer.GetBuilder<MD.ISynchroProcess>().From();
                builderSynchroProcess.Select(c => c.CreatedBy, c => c.CreatedDate, c => c.DateEnd, c => c.DateStart, c => c.Status, c => c.Id);
                builderSynchroProcess.Where(c => c.Id, ConditionOperator.Equal, processId);
                builderSynchroProcess.OrderByDesc(c => c.CreatedDate);
                queryExecuter.Fetch(builderSynchroProcess, readerSynchroProcess =>
                {
                    while (readerSynchroProcess.Read())
                    {
                        dataSynchronizationProcess = new DataSynchronizationProcess();
                        dataSynchronizationProcess.CreatedBy = readerSynchroProcess.GetValue(c => c.CreatedBy);
                        dataSynchronizationProcess.DateCreated = readerSynchroProcess.GetValue(c => c.CreatedDate);
                        dataSynchronizationProcess.DateStart = readerSynchroProcess.GetValue(c => c.DateStart);
                        dataSynchronizationProcess.DateEnd = readerSynchroProcess.GetValue(c => c.DateEnd);
                        dataSynchronizationProcess.Id = readerSynchroProcess.GetValue(c => c.Id);

                        Status status;
                        if (Enum.TryParse<Status>(readerSynchroProcess.GetValue(c => c.Status), out status))
                        {
                            dataSynchronizationProcess.Status = status;
                        }
                        else
                        {
                            dataSynchronizationProcess.Status = Status.E;
                        }
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return dataSynchronizationProcess;
        }

        public DataSynchronizationProcess CurrentDataSynchronizationProcess()
        {
            DataSynchronizationProcess dataSynchronizationProcess = null;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCurrentDataSynchronizationProcessMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderSynchroProcess = this._dataLayer.GetBuilder<MD.ISynchroProcess>().From();
                builderSynchroProcess.Select(c => c.CreatedBy, c => c.CreatedDate, c => c.DateEnd, c => c.DateStart, c => c.Status, c => c.Id);
                builderSynchroProcess.Where(c => c.Status, ConditionOperator.Equal, Status.A.ToString());
                builderSynchroProcess.OrderByDesc(c => c.CreatedDate);
                queryExecuter.Fetch(builderSynchroProcess, readerSynchroProcess =>
                {
                    while (readerSynchroProcess.Read())
                    {
                        dataSynchronizationProcess =  new DataSynchronizationProcess();
                        dataSynchronizationProcess.CreatedBy = readerSynchroProcess.GetValue(c => c.CreatedBy);
                        dataSynchronizationProcess.DateCreated = readerSynchroProcess.GetValue(c => c.CreatedDate);
                        dataSynchronizationProcess.DateStart = readerSynchroProcess.GetValue(c => c.DateStart);
                        dataSynchronizationProcess.DateEnd = readerSynchroProcess.GetValue(c => c.DateEnd);
                        dataSynchronizationProcess.Id = readerSynchroProcess.GetValue(c => c.Id);

                        Status status;
                        if (Enum.TryParse<Status>(readerSynchroProcess.GetValue(c => c.Status), out status))
                        {
                            dataSynchronizationProcess.Status = status;
                        }
                        else
                        {
                            dataSynchronizationProcess.Status =  Status.E;
                        }
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return dataSynchronizationProcess;
        }

        public DataSynchronizationProcess[] GetAllDataSynchronizationProcess()
        {
            var lstDataSynchronizationProcess = new List<DataSynchronizationProcess>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCurrentDataSynchronizationProcessMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderSynchroProcess = this._dataLayer.GetBuilder<MD.ISynchroProcess>().From();
                builderSynchroProcess.Select(c => c.CreatedBy, c => c.CreatedDate, c => c.DateEnd, c => c.DateStart, c => c.Status, c => c.Id);
                builderSynchroProcess.Where(c => c.Id, ConditionOperator.GreaterThan,0);
                builderSynchroProcess.OrderByDesc(c => c.CreatedDate);
                queryExecuter.Fetch(builderSynchroProcess, readerSynchroProcess =>
                {
                    while (readerSynchroProcess.Read())
                    {
                        var dataSynchronizationProcess = new DataSynchronizationProcess();
                        dataSynchronizationProcess.CreatedBy = readerSynchroProcess.GetValue(c => c.CreatedBy);
                        dataSynchronizationProcess.DateCreated = readerSynchroProcess.GetValue(c => c.CreatedDate);
                        dataSynchronizationProcess.DateStart = readerSynchroProcess.GetValue(c => c.DateStart);
                        dataSynchronizationProcess.DateEnd = readerSynchroProcess.GetValue(c => c.DateEnd);
                        dataSynchronizationProcess.Id = readerSynchroProcess.GetValue(c => c.Id);

                        Status status;
                        if (Enum.TryParse<Status>(readerSynchroProcess.GetValue(c => c.Status), out status))
                        {
                            dataSynchronizationProcess.Status = status;
                        }
                        else
                        {
                            dataSynchronizationProcess.Status = Status.E;
                        }
                        lstDataSynchronizationProcess.Add(dataSynchronizationProcess);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return lstDataSynchronizationProcess.ToArray();
        }

        public RefSpectrum[] GetAllRefSpectrum()
        {
            var listRefSpectrum = new List<RefSpectrum>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetAllRefSpectrumMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderRefSpectrum = this._dataLayer.GetBuilder<MD.IHeadRefSpectrum>().From();
                builderRefSpectrum.Select(c => c.CountImportRecords, c => c.CountSensors, c => c.CreatedBy, c => c.CreatedDate, c => c.FileName, c => c.Id, c => c.MaxFreqMHz, c => c.MinFreqMHz);
                builderRefSpectrum.Where(c => c.Id, ConditionOperator.GreaterThan, 0);
                queryExecuter.Fetch(builderRefSpectrum, readerRefSpectrum =>
                {
                    while (readerRefSpectrum.Read())
                    {
                        var refSpectrum = new RefSpectrum();
                        refSpectrum.CreatedBy = readerRefSpectrum.GetValue(c => c.CreatedBy);
                        refSpectrum.DateCreated = readerRefSpectrum.GetValue(c => c.CreatedDate);
                        refSpectrum.FileName = readerRefSpectrum.GetValue(c => c.FileName);
                        refSpectrum.Id = readerRefSpectrum.GetValue(c => c.Id);
                        refSpectrum.MaxFreqMHz = readerRefSpectrum.GetValue(c => c.MaxFreqMHz);
                        refSpectrum.MinFreqMHz = readerRefSpectrum.GetValue(c => c.MinFreqMHz);
                        refSpectrum.CountSensors = readerRefSpectrum.GetValue(c => c.CountSensors);
                        refSpectrum.CountImportRecords = readerRefSpectrum.GetValue(c => c.CountImportRecords);
                        var listDataSpectrum = new List<DataRefSpectrum>();

                        var builderDataRefSpectrum = this._dataLayer.GetBuilder<MD.IRefSpectrum>().From();
                        builderDataRefSpectrum.Select(c => c.DateMeas, c => c.DispersionLow, c => c.DispersionUp, c => c.Freq_MHz, c => c.GlobalSID, c => c.Id, c => c.IdNum, c => c.Level_dBm, c => c.Percent, c => c.SensorId, c => c.TableId, c => c.TableName);
                        builderDataRefSpectrum.Where(c => c.HEAD_REF_SPECTRUM.Id, ConditionOperator.Equal, readerRefSpectrum.GetValue(c => c.Id));
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

        public RefSpectrum[] GetRefSpectrumByIds(long[] headRefSpectrumIdsBySDRN, long[] sensorIdsBySDRN)
        {
            var listRefSpectrum = new List<RefSpectrum>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetRefSpectrumByIdsMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderRefSpectrum = this._dataLayer.GetBuilder<MD.IHeadRefSpectrum>().From();
                builderRefSpectrum.Select(c => c.CountImportRecords, c => c.CountSensors, c => c.CreatedBy, c => c.CreatedDate, c => c.FileName, c => c.Id, c => c.MaxFreqMHz, c => c.MinFreqMHz);
                builderRefSpectrum.Where(c => c.Id, ConditionOperator.In, headRefSpectrumIdsBySDRN);
                queryExecuter.Fetch(builderRefSpectrum, readerRefSpectrum =>
                {
                    while (readerRefSpectrum.Read())
                    {
                        var refSpectrum = new RefSpectrum();
                        refSpectrum.CreatedBy = readerRefSpectrum.GetValue(c => c.CreatedBy);
                        refSpectrum.DateCreated = readerRefSpectrum.GetValue(c => c.CreatedDate);
                        refSpectrum.FileName = readerRefSpectrum.GetValue(c => c.FileName);
                        refSpectrum.Id = readerRefSpectrum.GetValue(c => c.Id);
                        refSpectrum.MaxFreqMHz = readerRefSpectrum.GetValue(c => c.MaxFreqMHz);
                        refSpectrum.MinFreqMHz = readerRefSpectrum.GetValue(c => c.MinFreqMHz);
                        refSpectrum.CountSensors = readerRefSpectrum.GetValue(c => c.CountSensors);
                        refSpectrum.CountImportRecords = readerRefSpectrum.GetValue(c => c.CountImportRecords);
                        var listDataSpectrum = new List<DataRefSpectrum>();

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




