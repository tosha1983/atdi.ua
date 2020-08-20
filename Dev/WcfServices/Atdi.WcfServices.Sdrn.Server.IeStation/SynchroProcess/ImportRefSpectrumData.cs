using System.Collections.Generic;
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
    public class ImportRefSpectrumData
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;
        private LoadSensor _loadSensor;


        public ImportRefSpectrumData(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
            _loadSensor = new LoadSensor(this._dataLayer, this._logger);
        }

        /// <summary>
        ///Валидация отдельно взятой записи из RefSpectrum
        /// </summary>
        /// <param name="dataRefSpectrum"></param>
        /// <returns></returns>
        private bool ValidateDataRefSpectrum(DataRefSpectrum  dataRefSpectrum)
        {
            if (dataRefSpectrum == null)
                return false;

            bool result = true;
            
            if (string.IsNullOrEmpty(dataRefSpectrum.TableName))
            {
                this._logger.Error(Contexts.ThisComponent, Categories.ImportData,  $"Incorrect value DataRefSpectrum.TableName is null or empty");
                return false;
            }
            if (!dataRefSpectrum.TableName.Contains("MOB_STATION"))
            {
                this._logger.Error(Contexts.ThisComponent, Categories.ImportData, $"Incorrect value DataRefSpectrum.TableName = '{dataRefSpectrum.TableName}'");
                return false;
            }
            var sensor = _loadSensor.LoadBaseDateSensor(dataRefSpectrum.SensorId);
            if (sensor==null)
            {
                this._logger.Error(Contexts.ThisComponent, Categories.ImportData, $"Incorrect value DataRefSpectrum.SensorId = '{dataRefSpectrum.SensorId}'");
                return false;
            }
            if (string.IsNullOrEmpty(dataRefSpectrum.GlobalSID))
            {
                this._logger.Error(Contexts.ThisComponent, Categories.ImportData, $"Incorrect value DataRefSpectrum.GlobalSID is null or empty");
                return false;
            }
            if (((dataRefSpectrum.Freq_MHz >= 0.1) && (dataRefSpectrum.Freq_MHz <= 6000)) == false)
            {
                this._logger.Error(Contexts.ThisComponent, Categories.ImportData, $"Incorrect value DataRefSpectrum.Freq_MHz = {dataRefSpectrum.Freq_MHz}");
                return false;
            }
            if (((dataRefSpectrum.Level_dBm >= -200) && (dataRefSpectrum.Level_dBm <= 10)) == false)
            {
                this._logger.Error(Contexts.ThisComponent, Categories.ImportData, $"Incorrect value DataRefSpectrum.Level_dBm = {dataRefSpectrum.Level_dBm}");
                return false;
            }
            if (dataRefSpectrum.DispersionLow != null)
            {
                if (((dataRefSpectrum.DispersionLow != null) && (dataRefSpectrum.DispersionLow >= 0) && (dataRefSpectrum.DispersionLow <= 100)) == false)
                {
                    this._logger.Error(Contexts.ThisComponent, Categories.ImportData, $"Incorrect value DataRefSpectrum.DispersionLow = {dataRefSpectrum.DispersionLow}");
                    return false;
                }
            }
            if (dataRefSpectrum.DispersionUp != null)
            {
                if (((dataRefSpectrum.DispersionUp != null) && (dataRefSpectrum.DispersionUp >= 0) && (dataRefSpectrum.DispersionUp <= 100)) == false)
                {
                    this._logger.Error(Contexts.ThisComponent, Categories.ImportData, $"Incorrect value DataRefSpectrum.DispersionLow = {dataRefSpectrum.DispersionUp}");
                    return false;
                }
            }
            if (dataRefSpectrum.Percent != null)
            {
                if (((dataRefSpectrum.Percent != null) && (dataRefSpectrum.Percent >= 0) && (dataRefSpectrum.Percent <= 100)) == false)
                {
                    this._logger.Error(Contexts.ThisComponent, Categories.ImportData, $"Incorrect value DataRefSpectrum.DispersionLow = {dataRefSpectrum.Percent}");
                    return false;
                }
            }
            return result;
        }

        /// <summary>
        /// Импорт данных из csv - файла (RefSpectrum)
        /// </summary>
        /// <param name="refSpectrum"></param>
        /// <returns></returns>
        public long? ImportSpectrum(RefSpectrum refSpectrum)
        {
            long? headRefSpectrumId = null;
            int? CountImportRecords = 0;
            var listSensors = new List<long>();
            var listFreqMHz = new List<double>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.ImportData, Events.HandlerImportRefSpectrumMethod.Text);
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();
                    var builderHeadRefSpectrum = this._dataLayer.GetBuilder<MD.IHeadRefSpectrum>().Insert();
                    builderHeadRefSpectrum.SetValue(c => c.CreatedBy, refSpectrum.CreatedBy);
                    builderHeadRefSpectrum.SetValue(c => c.CreatedDate, refSpectrum.DateCreated);
                    builderHeadRefSpectrum.SetValue(c => c.FileName, refSpectrum.FileName);
                    var headRefSpectrumPK = scope.Executor.Execute<MD.IHeadRefSpectrum_PK>(builderHeadRefSpectrum);
                    headRefSpectrumId = headRefSpectrumPK.Id;

                    if (refSpectrum.DataRefSpectrum != null)
                    {
                        for (int i = 0; i < refSpectrum.DataRefSpectrum.Length; i++)
                        {
                            var dataRefSpectrum = refSpectrum.DataRefSpectrum[i];
                            var validData = ValidateDataRefSpectrum(dataRefSpectrum);
                            if (validData == false)
                            {
                                continue;
                            }

                            var builderRefSpectrum = this._dataLayer.GetBuilder<MD.IRefSpectrum>().Insert();
                            builderRefSpectrum.SetValue(c => c.DateMeas, dataRefSpectrum.DateMeas);
                            builderRefSpectrum.SetValue(c => c.DispersionLow, dataRefSpectrum.DispersionLow);
                            builderRefSpectrum.SetValue(c => c.DispersionUp, dataRefSpectrum.DispersionUp);
                            builderRefSpectrum.SetValue(c => c.Freq_MHz, dataRefSpectrum.Freq_MHz);
                            builderRefSpectrum.SetValue(c => c.GlobalSID, dataRefSpectrum.GlobalSID);
                            builderRefSpectrum.SetValue(c => c.Level_dBm, dataRefSpectrum.Level_dBm);
                            builderRefSpectrum.SetValue(c => c.Percent, dataRefSpectrum.Percent);
                            builderRefSpectrum.SetValue(c => c.IdNum, dataRefSpectrum.IdNum);
                            builderRefSpectrum.SetValue(c => c.SensorId, dataRefSpectrum.SensorId);
                            builderRefSpectrum.SetValue(c => c.TableId, dataRefSpectrum.TableId);
                            builderRefSpectrum.SetValue(c => c.TableName, dataRefSpectrum.TableName);
                            builderRefSpectrum.SetValue(c => c.StatusMeas, "N");
                            builderRefSpectrum.SetValue(c => c.HEAD_REF_SPECTRUM.Id, headRefSpectrumPK.Id);
                            var builderRefSpectrumPK = scope.Executor.Execute<MD.IRefSpectrum_PK>(builderRefSpectrum);

                            CountImportRecords++;
                            if (!listSensors.Contains(dataRefSpectrum.SensorId))
                            {
                                listSensors.Add(dataRefSpectrum.SensorId);
                            }
                            if (!listFreqMHz.Contains(dataRefSpectrum.Freq_MHz))
                            {
                                listFreqMHz.Add(dataRefSpectrum.Freq_MHz);
                            }
                        }
                    }

                    var builderUpdateHeadRefSpectrum = this._dataLayer.GetBuilder<MD.IHeadRefSpectrum>().Update();
                    builderUpdateHeadRefSpectrum.Where(c => c.Id, ConditionOperator.Equal, headRefSpectrumPK.Id);
                    builderUpdateHeadRefSpectrum.SetValue(c => c.CountImportRecords, CountImportRecords);
                    builderUpdateHeadRefSpectrum.SetValue(c => c.CountSensors, listSensors.Count);
                    builderUpdateHeadRefSpectrum.SetValue(c => c.MinFreqMHz, listFreqMHz.Min());
                    builderUpdateHeadRefSpectrum.SetValue(c => c.MaxFreqMHz, listFreqMHz.Max());
                    scope.Executor.Execute(builderUpdateHeadRefSpectrum);

                    scope.Commit();
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return headRefSpectrumId;
        }

        /// <summary>
        /// Удаление выбранных RefSpectrum
        /// </summary>
        /// <param name="RefSpectrumIdsBySDRN"></param>
        /// <returns></returns>
        public bool DeleteRefSpectrum(long[] RefSpectrumIdsBySDRN)
        {
            var isSuccess = false;
            try
            {

                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    scope.BeginTran();

                    for (int i = 0; i < RefSpectrumIdsBySDRN.Length; i++)
                    {

                        var builderDeleteLinkHeadRefSpectrum = this._dataLayer.GetBuilder<MD.ILinkHeadRefSpectrum>().Delete();
                        builderDeleteLinkHeadRefSpectrum.Where(c => c.HEAD_REF_SPECTRUM.Id, ConditionOperator.Equal, RefSpectrumIdsBySDRN[i]);
                        scope.Executor.Execute(builderDeleteLinkHeadRefSpectrum);

                        var builderDeleteRefSpectrum = this._dataLayer.GetBuilder<MD.IRefSpectrum>().Delete();
                        builderDeleteRefSpectrum.Where(c => c.HEAD_REF_SPECTRUM.Id, ConditionOperator.Equal, RefSpectrumIdsBySDRN[i]);
                        scope.Executor.Execute(builderDeleteRefSpectrum);


                        var builderDeleteHeadRefSpectrum = this._dataLayer.GetBuilder<MD.IHeadRefSpectrum>().Delete();
                        builderDeleteHeadRefSpectrum.Where(c => c.Id, ConditionOperator.Equal, RefSpectrumIdsBySDRN[i]);
                        scope.Executor.Execute(builderDeleteHeadRefSpectrum);

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




