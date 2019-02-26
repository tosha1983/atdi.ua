using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.DeviceServer.Entities;
using Atdi.DataModels.EntityOrm;
using Atdi.Modules.Sdrn.DeviceServer;
using System.Xml;
using System.Linq;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    public sealed class TaskParametersRepository : IRepository<TaskParameters,int?>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public TaskParametersRepository(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }



        public TaskParameters LoadObject(int? id)
        {
            throw new NotImplementedException();
        }

        public int? Create(TaskParameters item)
        {
            int? ID = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();
            if (item != null)
            {
                try
                {
                    queryExecuter.BeginTransaction();
                    var builderInsertTaskParameters = this._dataLayer.GetBuilder<MD.ITaskParameters>().Insert();
                    builderInsertTaskParameters.SetValue(c => c.LevelMinOccup_dBm, item.LevelMinOccup_dBm);
                    builderInsertTaskParameters.SetValue(c => c.MaxFreq_MHz, item.MaxFreq_MHz);
                    builderInsertTaskParameters.SetValue(c => c.MeasurementType, item.MeasurementType.ToString());
                    builderInsertTaskParameters.SetValue(c => c.MinFreq_MHz, item.MinFreq_MHz);
                    builderInsertTaskParameters.SetValue(c => c.NChenal, item.NChenal);
                    builderInsertTaskParameters.SetValue(c => c.RBW_Hz, item.RBW_Hz);
                    builderInsertTaskParameters.SetValue(c => c.ReceivedIQStreemDuration_sec, item.ReceivedIQStreemDuration_sec);
                    builderInsertTaskParameters.SetValue(c => c.SDRTaskId, item.SDRTaskId);
                    builderInsertTaskParameters.SetValue(c => c.StartTime, item.StartTime);
                    builderInsertTaskParameters.SetValue(c => c.Status, item.status);
                    builderInsertTaskParameters.SetValue(c => c.StepSO_kHz, item.StepSO_kHz);
                    builderInsertTaskParameters.SetValue(c => c.StopTime, item.StopTime);
                    builderInsertTaskParameters.SetValue(c => c.TypeTechnology, item.TypeTechnology.ToString());
                    builderInsertTaskParameters.SetValue(c => c.Type_of_SO, item.Type_of_SO.ToString());
                    builderInsertTaskParameters.SetValue(c => c.VBW_Hz, item.VBW_Hz);
                    builderInsertTaskParameters.Select(c => c.Id);
                    queryExecuter.ExecuteAndFetch(builderInsertTaskParameters, readerMeasTask =>
                    {
                        while (readerMeasTask.Read())
                        {
                            ID = readerMeasTask.GetValue(c => c.Id);
                        }
                        return true;
                    });

                    if (ID != null)
                    {
                        for (int i = 0; i < item.List_freq_CH.Count; i++)
                        {
                            int? idTaskParametersFreq = null;
                            var builderInsertTaskParametersFreq = this._dataLayer.GetBuilder<MD.ITaskParametersFreq>().Insert();
                            builderInsertTaskParametersFreq.SetValue(c => c.FreqCH, item.List_freq_CH[i]);
                            builderInsertTaskParametersFreq.SetValue(c => c.IdTaskParameters, ID);
                            builderInsertTaskParametersFreq.Select(c => c.Id);
                            queryExecuter.ExecuteAndFetch(builderInsertTaskParametersFreq, readerTaskParametersFreq =>
                            {
                                while (readerTaskParametersFreq.Read())
                                {
                                    idTaskParametersFreq = readerTaskParametersFreq.GetValue(c => c.Id);
                                }
                                return true;
                            });
                        }
                    }
                    queryExecuter.CommitTransaction();
                }
                catch (Exception)
                {
                    queryExecuter.RollbackTransaction();
                }
            }
            return ID;
        }

        public bool Update(TaskParameters item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        TaskParameters[] IRepository<TaskParameters, int?>.LoadAllObjects()
        {
            throw new NotImplementedException();
        }
    }
}
