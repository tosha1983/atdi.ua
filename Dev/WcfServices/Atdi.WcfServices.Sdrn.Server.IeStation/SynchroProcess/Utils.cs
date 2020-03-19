using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities.IeStation;
using System.Xml;
using System.Linq;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;


namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    public class Utils
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public Utils(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        /// <summary>
        ///Получение сведений о процессе синхронизации по заданному идентификатору
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Получение сведений о текущем "активном" процессе синхронизации 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Получение сведений о всех процессах синхронизации 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Извлечение данных о всех RefSpectrum
        /// </summary>
        /// <returns></returns>
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
                        builderDataRefSpectrum.Select(c => c.DateMeas, c => c.DispersionLow, c => c.DispersionUp, c => c.Freq_MHz, c => c.GlobalSID, c => c.Id, c => c.IdNum, c => c.Level_dBm, c => c.Percent, c => c.SensorId, c => c.TableId, c => c.TableName, c => c.StatusMeas);
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
                                dataSpectrum.HeadId = readerRefSpectrum.GetValue(c => c.Id);
                                dataSpectrum.StatusMeas = readerDataRefSpectrum.GetValue(c => c.StatusMeas);
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

        /// <summary>
        /// Удаление заданного RefSpectrum по идентификатору
        /// </summary>
        /// <param name="headRefSpectrumId"></param>
        /// <returns></returns>
        public bool DeleteHeadRefSpectrum(long headRefSpectrumId)
        {
            var isSuccess = false;
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderDeleteHeadRefSpectrum = this._dataLayer.GetBuilder<MD.IHeadRefSpectrum>().Delete();
                    builderDeleteHeadRefSpectrum.Where(c => c.Id, ConditionOperator.Equal, headRefSpectrumId);
                    scope.Executor.Execute(builderDeleteHeadRefSpectrum);

                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        /// <summary>
        /// Очистка всех линков связанных с текущим идентификатором процесса синхронизации
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public bool ClearAllLinksByProcessId(long processId)
        {
            var isSuccess = false;
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderDeleteLinkHeadRefSpectrum = this._dataLayer.GetBuilder<MD.ILinkHeadRefSpectrum>().Delete();
                    builderDeleteLinkHeadRefSpectrum.Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, processId);
                    scope.Executor.Execute(builderDeleteLinkHeadRefSpectrum);

                    var builderDeleteLinkSensorsWithSynchroProcess = this._dataLayer.GetBuilder<MD.ILinkSensorsWithSynchroProcess>().Delete();
                    builderDeleteLinkSensorsWithSynchroProcess.Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, processId);
                    scope.Executor.Execute(builderDeleteLinkSensorsWithSynchroProcess);

                    var builderDeleteLinkArea = this._dataLayer.GetBuilder<MD.ILinkArea>().Delete();
                    builderDeleteLinkArea.Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, processId);
                    scope.Executor.Execute(builderDeleteLinkArea);

                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        /// <summary>
        /// Удаление записи с таблицы LinkHeadRefSpectrum по заданному идентфикатору headRefSpectrumId
        /// </summary>
        /// <param name="headRefSpectrumId"></param>
        /// <returns></returns>
        public bool DeleteLinkHeadRefSpectrumByHeadId(long headRefSpectrumId)
        {
            var isSuccess = false;
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderDeleteLinkHeadRefSpectrum = this._dataLayer.GetBuilder<MD.ILinkHeadRefSpectrum>().Delete();
                    builderDeleteLinkHeadRefSpectrum.Where(c => c.HEAD_REF_SPECTRUM.Id, ConditionOperator.Equal, headRefSpectrumId);
                    scope.Executor.Execute(builderDeleteLinkHeadRefSpectrum);

                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        /// <summary>
        /// Удаление тех записей с таблицы LinkSensorsWithSynchroProcess, у которых значение поля SensorId не содержит значений передаваемых в массиве sensorId
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public bool DeleteLinkSensors(long[] sensorId, long synchroProcessId)
        {
            var isSuccess = false;
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderDeleteLinkSensorsWithSynchroProcess = this._dataLayer.GetBuilder<MD.ILinkSensorsWithSynchroProcess>().Delete();
                    builderDeleteLinkSensorsWithSynchroProcess.Where(c => c.SensorId, ConditionOperator.NotIn, sensorId);
                    builderDeleteLinkSensorsWithSynchroProcess.Where(c => c.SYNCHRO_PROCESS.Id, ConditionOperator.Equal, synchroProcessId);
                    scope.Executor.Execute(builderDeleteLinkSensorsWithSynchroProcess);

                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        /// <summary>
        /// Удаление тех записей с таблицы IRefSpectrum, у которых значение поля SensorId не содержит значений передаваемых в массиве sensorId
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        public bool DeleteRefSpectrumBySensorId(long[] sensorId, long[] headRefSpectrumIdsBySDRN)
        {
            var isSuccess = false;
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    var builderDeleteRefSpectrum = this._dataLayer.GetBuilder<MD.IRefSpectrum>().Delete();
                    builderDeleteRefSpectrum.Where(c => c.SensorId, ConditionOperator.NotIn, sensorId);
                    builderDeleteRefSpectrum.Where(c => c.HEAD_REF_SPECTRUM.Id, ConditionOperator.In, headRefSpectrumIdsBySDRN);
                    scope.Executor.Execute(builderDeleteRefSpectrum);

                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        /// <summary>
        /// Удаление записей из таблицы HeadRefSpectrum, для случая когда нет ни одной связанной записи в таблице RefSpectrum
        /// </summary>
        /// <param name="headRefSpectrumIdsBySDRN"></param>
        /// <param name="sensorIdsBySDRN"></param>
        public void RemoveEmptyHeadRefSpectrum(long[] headRefSpectrumIdsBySDRN, long[] sensorIdsBySDRN)
        {
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetRefSpectrumByIdsMethod.Text);
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderRefSpectrum = this._dataLayer.GetBuilder<MD.IHeadRefSpectrum>().From();
                builderRefSpectrum.Select(c =>c.Id);
                builderRefSpectrum.Where(c => c.Id, ConditionOperator.In, headRefSpectrumIdsBySDRN);
                queryExecuter.Fetch(builderRefSpectrum, readerRefSpectrum =>
                {
                    while (readerRefSpectrum.Read())
                    {
                        bool isEmptyData = true;
                        var builderDataRefSpectrum = this._dataLayer.GetBuilder<MD.IRefSpectrum>().From();
                        builderDataRefSpectrum.Select(c =>c.Id);
                        builderDataRefSpectrum.Where(c => c.HEAD_REF_SPECTRUM.Id, ConditionOperator.Equal, readerRefSpectrum.GetValue(c => c.Id));
                        builderDataRefSpectrum.Where(c => c.SensorId, ConditionOperator.In, sensorIdsBySDRN);
                        queryExecuter.Fetch(builderDataRefSpectrum, readerDataRefSpectrum =>
                        {
                            while (readerDataRefSpectrum.Read())
                            {
                                isEmptyData = false;
                                break;
                            }
                            return true;
                        });

                        if (isEmptyData==true)
                        {
                            DeleteLinkHeadRefSpectrumByHeadId(readerRefSpectrum.GetValue(c => c.Id));
                            DeleteHeadRefSpectrum(readerRefSpectrum.GetValue(c => c.Id));
                        }
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
        }

        /// <summary>
        /// Получение массива значений RefSpectrum[], по перечню идентификаторов headRefSpectrumIdsBySDRN и sensorIdsBySDRN
        /// </summary>
        /// <param name="headRefSpectrumIdsBySDRN"></param>
        /// <param name="sensorIdsBySDRN"></param>
        /// <returns></returns>
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
                        builderDataRefSpectrum.Select(c => c.DateMeas, c => c.DispersionLow, c => c.DispersionUp, c => c.Freq_MHz, c => c.GlobalSID, c => c.Id, c => c.IdNum, c => c.Level_dBm, c => c.Percent, c => c.SensorId, c => c.TableId, c => c.TableName, c => c.StatusMeas);
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
                                dataSpectrum.HeadId = readerRefSpectrum.GetValue(c => c.Id);
                                dataSpectrum.StatusMeas = readerDataRefSpectrum.GetValue(c => c.StatusMeas);
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

        /// <summary>
        /// Обновление статуса записи таблицы RefSpectrum
        /// </summary>
        /// <param name="dataRefSpectrum"></param>
        /// <returns></returns>
        public bool UpdateStatusRefSpectrum(DataRefSpectrum dataRefSpectrum)
        {
            bool isSuccess = false;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.ImportData, Events.UpdateStatusStationExtended.Text);
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();
                    var builderUpdateRefSpectrum = this._dataLayer.GetBuilder<MD.IRefSpectrum>().Update();
                    builderUpdateRefSpectrum.Where(c => c.TableId, ConditionOperator.Equal, dataRefSpectrum.TableId);
                    builderUpdateRefSpectrum.Where(c => c.TableName, ConditionOperator.Equal, dataRefSpectrum.TableName);
                    builderUpdateRefSpectrum.Where(c => c.SensorId, ConditionOperator.Equal, dataRefSpectrum.SensorId);
                    if (!string.IsNullOrEmpty(dataRefSpectrum.StatusMeas))
                    {
                        builderUpdateRefSpectrum.SetValue(c => c.StatusMeas, dataRefSpectrum.StatusMeas);
                        scope.Executor.Execute(builderUpdateRefSpectrum);
                    }
                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

        /// <summary>
        /// Обновление статуса записи таблицы StationExtended
        /// </summary>
        /// <param name="stationExtended"></param>
        /// <returns></returns>
        public bool UpdateStatusStationExtended(StationExtended stationExtended)
        {
            bool isSuccess = false;
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.ImportData, Events.UpdateStatusStationExtended.Text);
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();
                    var builderUpdateStationExtended = this._dataLayer.GetBuilder<MD.IStationExtended>().Update();
                    builderUpdateStationExtended.Where(c => c.TableId, ConditionOperator.Equal, stationExtended.TableId);
                    builderUpdateStationExtended.Where(c => c.TableName, ConditionOperator.Equal, stationExtended.TableName);
                    if (!string.IsNullOrEmpty(stationExtended.StatusMeas))
                    {
                        builderUpdateStationExtended.SetValue(c => c.StatusMeas, stationExtended.StatusMeas);
                        scope.Executor.Execute(builderUpdateStationExtended);
                    }
                    scope.Commit();
                }
                isSuccess = true;
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return isSuccess;
        }

    }
}




