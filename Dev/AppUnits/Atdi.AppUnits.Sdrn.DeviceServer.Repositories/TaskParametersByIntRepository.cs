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
    public sealed class TaskParametersByIntRepository : IRepository<TaskParameters, int?>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public TaskParametersByIntRepository(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public TaskParametersByIntRepository()
        {
        }

        public TaskParameters LoadObject(string SDRTaskId)
        {
            TaskParameters taskParameters = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();

            var builderInsertTaskParameters = this._dataLayer.GetBuilder<MD.ITaskParameters>().From();
            builderInsertTaskParameters.Select(c => c.LevelMinOccup_dBm);
            builderInsertTaskParameters.Select(c => c.MaxFreq_MHz);
            builderInsertTaskParameters.Select(c => c.MeasurementType);
            builderInsertTaskParameters.Select(c => c.MinFreq_MHz);
            builderInsertTaskParameters.Select(c => c.NChenal);
            builderInsertTaskParameters.Select(c => c.RBW_Hz);
            builderInsertTaskParameters.Select(c => c.ReceivedIQStreemDuration_sec);
            builderInsertTaskParameters.Select(c => c.SDRTaskId);
            builderInsertTaskParameters.Select(c => c.StartTime);
            builderInsertTaskParameters.Select(c => c.Status);
            builderInsertTaskParameters.Select(c => c.NCount);
            builderInsertTaskParameters.Select(c => c.StepSO_kHz);
            builderInsertTaskParameters.Select(c => c.StopTime);
            builderInsertTaskParameters.Select(c => c.TypeTechnology);
            builderInsertTaskParameters.Select(c => c.Type_of_SO);
            builderInsertTaskParameters.Select(c => c.VBW_Hz);
            builderInsertTaskParameters.Select(c => c.Id);
            builderInsertTaskParameters.Where(c => c.SDRTaskId, DataModels.DataConstraint.ConditionOperator.Equal, SDRTaskId);
            queryExecuter.Fetch(builderInsertTaskParameters, readerMeasTask =>
            {
                if (readerMeasTask.Read())
                {
                    taskParameters = new TaskParameters();

                    if (readerMeasTask.GetValue(c => c.LevelMinOccup_dBm) != null)
                    {
                        taskParameters.LevelMinOccup_dBm = readerMeasTask.GetValue(c => c.LevelMinOccup_dBm).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.MaxFreq_MHz) != null)
                    {
                        taskParameters.MaxFreq_MHz = readerMeasTask.GetValue(c => c.MaxFreq_MHz).Value;
                    }

                    MeasType measurementType;

                    if (Enum.TryParse<MeasType>(readerMeasTask.GetValue(c => c.MeasurementType) != null ? readerMeasTask.GetValue(c => c.MeasurementType).ToString() : "", out measurementType))
                    {
                        taskParameters.MeasurementType = measurementType;
                    }

                    if (readerMeasTask.GetValue(c => c.MinFreq_MHz) != null)
                    {
                        taskParameters.MinFreq_MHz = readerMeasTask.GetValue(c => c.MinFreq_MHz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.NChenal) != null)
                    {
                        taskParameters.NChenal = readerMeasTask.GetValue(c => c.NChenal).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.NCount) != null)
                    {
                        taskParameters.NCount = readerMeasTask.GetValue(c => c.NCount).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.RBW_Hz) != null)
                    {
                        taskParameters.RBW_Hz = readerMeasTask.GetValue(c => c.RBW_Hz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.ReceivedIQStreemDuration_sec) != null)
                    {
                        taskParameters.ReceivedIQStreemDuration_sec = readerMeasTask.GetValue(c => c.ReceivedIQStreemDuration_sec).Value;
                    }


                    taskParameters.SDRTaskId = readerMeasTask.GetValue(c => c.SDRTaskId);
                    taskParameters.status = readerMeasTask.GetValue(c => c.Status);

                    if (readerMeasTask.GetValue(c => c.StartTime) != null)
                    {
                        taskParameters.StartTime = readerMeasTask.GetValue(c => c.StartTime).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.StepSO_kHz) != null)
                    {
                        taskParameters.StepSO_kHz = readerMeasTask.GetValue(c => c.StepSO_kHz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.StopTime) != null)
                    {
                        taskParameters.StopTime = readerMeasTask.GetValue(c => c.StopTime).Value;
                    }


                    TypeTechnology typeTechnology;
                    if (Enum.TryParse<TypeTechnology>(readerMeasTask.GetValue(c => c.TypeTechnology) != null ? readerMeasTask.GetValue(c => c.TypeTechnology).ToString() : "", out typeTechnology))
                    {
                        taskParameters.TypeTechnology = typeTechnology;
                    }

                    SOType sOType;
                    if (Enum.TryParse<SOType>(readerMeasTask.GetValue(c => c.Type_of_SO) != null ? readerMeasTask.GetValue(c => c.Type_of_SO).ToString() : "", out sOType))
                    {
                        taskParameters.Type_of_SO = sOType;
                    }


                    if (readerMeasTask.GetValue(c => c.VBW_Hz) != null)
                    {
                        taskParameters.VBW_Hz = readerMeasTask.GetValue(c => c.VBW_Hz).Value;
                    }

                    taskParameters.List_freq_CH = new List<double>();

                    var builderInsertTaskParametersFreq = this._dataLayer.GetBuilder<MD.ITaskParametersFreq>().From();
                    builderInsertTaskParametersFreq.Select(c => c.FreqCH);
                    builderInsertTaskParametersFreq.Select(c => c.IdTaskParameters);
                    builderInsertTaskParametersFreq.Select(c => c.Id);
                    builderInsertTaskParametersFreq.Where(c => c.IdTaskParameters, DataModels.DataConstraint.ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                    queryExecuter.Fetch(builderInsertTaskParametersFreq, readerTaskParametersFreq =>
                    {
                        while (readerTaskParametersFreq.Read())
                        {
                            if (readerTaskParametersFreq.GetValue(c => c.FreqCH) != null)
                            {
                                taskParameters.List_freq_CH.Add(readerTaskParametersFreq.GetValue(c => c.FreqCH).Value);
                            }
                        }
                        return true;
                    });
                }
                return true;
            });

            return taskParameters;
        }

        public TaskParameters LoadObject(int? id)
        {
            TaskParameters taskParameters = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();

            var builderInsertTaskParameters = this._dataLayer.GetBuilder<MD.ITaskParameters>().From();
            builderInsertTaskParameters.Select(c => c.LevelMinOccup_dBm);
            builderInsertTaskParameters.Select(c => c.MaxFreq_MHz);
            builderInsertTaskParameters.Select(c => c.MeasurementType);
            builderInsertTaskParameters.Select(c => c.MinFreq_MHz);
            builderInsertTaskParameters.Select(c => c.NChenal);
            builderInsertTaskParameters.Select(c => c.RBW_Hz);
            builderInsertTaskParameters.Select(c => c.ReceivedIQStreemDuration_sec);
            builderInsertTaskParameters.Select(c => c.SDRTaskId);
            builderInsertTaskParameters.Select(c => c.StartTime);
            builderInsertTaskParameters.Select(c => c.Status);
            builderInsertTaskParameters.Select(c => c.StepSO_kHz);
            builderInsertTaskParameters.Select(c => c.NCount);
            builderInsertTaskParameters.Select(c => c.StopTime);
            builderInsertTaskParameters.Select(c => c.TypeTechnology);
            builderInsertTaskParameters.Select(c => c.Type_of_SO);
            builderInsertTaskParameters.Select(c => c.VBW_Hz);
            builderInsertTaskParameters.Select(c => c.Id);
            builderInsertTaskParameters.Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.GreaterThan, 0);
            queryExecuter.Fetch(builderInsertTaskParameters, readerMeasTask =>
            {
                if (readerMeasTask.Read())
                {
                    taskParameters = new TaskParameters();

                    if (readerMeasTask.GetValue(c => c.LevelMinOccup_dBm) != null)
                    {
                        taskParameters.LevelMinOccup_dBm = readerMeasTask.GetValue(c => c.LevelMinOccup_dBm).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.MaxFreq_MHz) != null)
                    {
                        taskParameters.MaxFreq_MHz = readerMeasTask.GetValue(c => c.MaxFreq_MHz).Value;
                    }

                    MeasType measurementType;

                    if (Enum.TryParse<MeasType>(readerMeasTask.GetValue(c => c.MeasurementType) != null ? readerMeasTask.GetValue(c => c.MeasurementType).ToString() : "", out measurementType))
                    {
                        taskParameters.MeasurementType = measurementType;
                    }

                    if (readerMeasTask.GetValue(c => c.MinFreq_MHz) != null)
                    {
                        taskParameters.MinFreq_MHz = readerMeasTask.GetValue(c => c.MinFreq_MHz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.NChenal) != null)
                    {
                        taskParameters.NChenal = readerMeasTask.GetValue(c => c.NChenal).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.NCount) != null)
                    {
                        taskParameters.NCount = readerMeasTask.GetValue(c => c.NCount).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.RBW_Hz) != null)
                    {
                        taskParameters.RBW_Hz = readerMeasTask.GetValue(c => c.RBW_Hz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.ReceivedIQStreemDuration_sec) != null)
                    {
                        taskParameters.ReceivedIQStreemDuration_sec = readerMeasTask.GetValue(c => c.ReceivedIQStreemDuration_sec).Value;
                    }


                    taskParameters.SDRTaskId = readerMeasTask.GetValue(c => c.SDRTaskId);
                    taskParameters.status = readerMeasTask.GetValue(c => c.Status);

                    if (readerMeasTask.GetValue(c => c.StartTime) != null)
                    {
                        taskParameters.StartTime = readerMeasTask.GetValue(c => c.StartTime).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.StepSO_kHz) != null)
                    {
                        taskParameters.StepSO_kHz = readerMeasTask.GetValue(c => c.StepSO_kHz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.StopTime) != null)
                    {
                        taskParameters.StopTime = readerMeasTask.GetValue(c => c.StopTime).Value;
                    }


                    TypeTechnology typeTechnology;
                    if (Enum.TryParse<TypeTechnology>(readerMeasTask.GetValue(c => c.TypeTechnology) != null ? readerMeasTask.GetValue(c => c.TypeTechnology).ToString() : "", out typeTechnology))
                    {
                        taskParameters.TypeTechnology = typeTechnology;
                    }

                    SOType sOType;
                    if (Enum.TryParse<SOType>(readerMeasTask.GetValue(c => c.Type_of_SO) != null ? readerMeasTask.GetValue(c => c.Type_of_SO).ToString() : "", out sOType))
                    {
                        taskParameters.Type_of_SO = sOType;
                    }


                    if (readerMeasTask.GetValue(c => c.VBW_Hz) != null)
                    {
                        taskParameters.VBW_Hz = readerMeasTask.GetValue(c => c.VBW_Hz).Value;
                    }

                    taskParameters.List_freq_CH = new List<double>();

                    var builderInsertTaskParametersFreq = this._dataLayer.GetBuilder<MD.ITaskParametersFreq>().From();
                    builderInsertTaskParametersFreq.Select(c => c.FreqCH);
                    builderInsertTaskParametersFreq.Select(c => c.IdTaskParameters);
                    builderInsertTaskParametersFreq.Select(c => c.Id);
                    builderInsertTaskParametersFreq.Where(c => c.IdTaskParameters, DataModels.DataConstraint.ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                    queryExecuter.Fetch(builderInsertTaskParametersFreq, readerTaskParametersFreq =>
                    {
                        while (readerTaskParametersFreq.Read())
                        {
                            if (readerTaskParametersFreq.GetValue(c => c.FreqCH) != null)
                            {
                                taskParameters.List_freq_CH.Add(readerTaskParametersFreq.GetValue(c => c.FreqCH).Value);
                            }
                        }
                        return true;
                    });
                }
                return true;
            });

            return taskParameters;
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
                    builderInsertTaskParameters.SetValue(c => c.NCount, item.NCount);
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
            bool isSuccessUpdate = false;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();
            if (item != null)
            {
                try
                {
                    queryExecuter.BeginTransaction();
                    var builderInsertTaskParameters = this._dataLayer.GetBuilder<MD.ITaskParameters>().Update();
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
                    builderInsertTaskParameters.SetValue(c => c.NCount, item.NCount);
                    builderInsertTaskParameters.SetValue(c => c.StepSO_kHz, item.StepSO_kHz);
                    builderInsertTaskParameters.SetValue(c => c.StopTime, item.StopTime);
                    builderInsertTaskParameters.SetValue(c => c.TypeTechnology, item.TypeTechnology.ToString());
                    builderInsertTaskParameters.SetValue(c => c.Type_of_SO, item.Type_of_SO.ToString());
                    builderInsertTaskParameters.SetValue(c => c.VBW_Hz, item.VBW_Hz);
                    builderInsertTaskParameters.Where(c => c.SDRTaskId, DataModels.DataConstraint.ConditionOperator.Equal, item.SDRTaskId);
                    int cntUpdate = queryExecuter.Execute(builderInsertTaskParameters);
                    if (cntUpdate > 0)
                    {
                        isSuccessUpdate = true;
                    }
                    queryExecuter.CommitTransaction();
                }
                catch (Exception)
                {
                    queryExecuter.RollbackTransaction();
                }
            }
            return isSuccessUpdate;
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
            List<TaskParameters> listTaskParameters = new List<TaskParameters>();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();

            var builderInsertTaskParameters = this._dataLayer.GetBuilder<MD.ITaskParameters>().From();
            builderInsertTaskParameters.Select(c => c.LevelMinOccup_dBm);
            builderInsertTaskParameters.Select(c => c.MaxFreq_MHz);
            builderInsertTaskParameters.Select(c => c.MeasurementType);
            builderInsertTaskParameters.Select(c => c.MinFreq_MHz);
            builderInsertTaskParameters.Select(c => c.NChenal);
            builderInsertTaskParameters.Select(c => c.RBW_Hz);
            builderInsertTaskParameters.Select(c => c.ReceivedIQStreemDuration_sec);
            builderInsertTaskParameters.Select(c => c.SDRTaskId);
            builderInsertTaskParameters.Select(c => c.StartTime);
            builderInsertTaskParameters.Select(c => c.Status);
            builderInsertTaskParameters.Select(c => c.NCount);
            builderInsertTaskParameters.Select(c => c.StepSO_kHz);
            builderInsertTaskParameters.Select(c => c.StopTime);
            builderInsertTaskParameters.Select(c => c.TypeTechnology);
            builderInsertTaskParameters.Select(c => c.Type_of_SO);
            builderInsertTaskParameters.Select(c => c.VBW_Hz);
            builderInsertTaskParameters.Select(c => c.Id);
            builderInsertTaskParameters.Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.GreaterThan, 0);
            queryExecuter.Fetch(builderInsertTaskParameters, readerMeasTask =>
            {
                while (readerMeasTask.Read())
                {
                    TaskParameters taskParameters = new TaskParameters();

                    if (readerMeasTask.GetValue(c => c.LevelMinOccup_dBm) != null)
                    {
                        taskParameters.LevelMinOccup_dBm = readerMeasTask.GetValue(c => c.LevelMinOccup_dBm).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.MaxFreq_MHz) != null)
                    {
                        taskParameters.MaxFreq_MHz = readerMeasTask.GetValue(c => c.MaxFreq_MHz).Value;
                    }

                    MeasType measurementType;

                    if (Enum.TryParse<MeasType>(readerMeasTask.GetValue(c => c.MeasurementType) != null ? readerMeasTask.GetValue(c => c.MeasurementType).ToString() : "", out measurementType))
                    {
                        taskParameters.MeasurementType = measurementType;
                    }

                    if (readerMeasTask.GetValue(c => c.MinFreq_MHz) != null)
                    {
                        taskParameters.MinFreq_MHz = readerMeasTask.GetValue(c => c.MinFreq_MHz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.NChenal) != null)
                    {
                        taskParameters.NChenal = readerMeasTask.GetValue(c => c.NChenal).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.NCount) != null)
                    {
                        taskParameters.NCount = readerMeasTask.GetValue(c => c.NCount).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.RBW_Hz) != null)
                    {
                        taskParameters.RBW_Hz = readerMeasTask.GetValue(c => c.RBW_Hz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.ReceivedIQStreemDuration_sec) != null)
                    {
                        taskParameters.ReceivedIQStreemDuration_sec = readerMeasTask.GetValue(c => c.ReceivedIQStreemDuration_sec).Value;
                    }


                    taskParameters.SDRTaskId = readerMeasTask.GetValue(c => c.SDRTaskId);
                    taskParameters.status = readerMeasTask.GetValue(c => c.Status);

                    if (readerMeasTask.GetValue(c => c.StartTime) != null)
                    {
                        taskParameters.StartTime = readerMeasTask.GetValue(c => c.StartTime).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.StepSO_kHz) != null)
                    {
                        taskParameters.StepSO_kHz = readerMeasTask.GetValue(c => c.StepSO_kHz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.StopTime) != null)
                    {
                        taskParameters.StopTime = readerMeasTask.GetValue(c => c.StopTime).Value;
                    }


                    TypeTechnology typeTechnology;
                    if (Enum.TryParse<TypeTechnology>(readerMeasTask.GetValue(c => c.TypeTechnology) != null ? readerMeasTask.GetValue(c => c.TypeTechnology).ToString() : "", out typeTechnology))
                    {
                        taskParameters.TypeTechnology = typeTechnology;
                    }

                    SOType sOType;
                    if (Enum.TryParse<SOType>(readerMeasTask.GetValue(c => c.Type_of_SO) != null ? readerMeasTask.GetValue(c => c.Type_of_SO).ToString() : "", out sOType))
                    {
                        taskParameters.Type_of_SO = sOType;
                    }


                    if (readerMeasTask.GetValue(c => c.VBW_Hz) != null)
                    {
                        taskParameters.VBW_Hz = readerMeasTask.GetValue(c => c.VBW_Hz).Value;
                    }

                    taskParameters.List_freq_CH = new List<double>();

                    var builderInsertTaskParametersFreq = this._dataLayer.GetBuilder<MD.ITaskParametersFreq>().From();
                    builderInsertTaskParametersFreq.Select(c => c.FreqCH);
                    builderInsertTaskParametersFreq.Select(c => c.IdTaskParameters);
                    builderInsertTaskParametersFreq.Select(c => c.Id);
                    builderInsertTaskParametersFreq.Where(c => c.IdTaskParameters, DataModels.DataConstraint.ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                    queryExecuter.Fetch(builderInsertTaskParametersFreq, readerTaskParametersFreq =>
                    {
                        while (readerTaskParametersFreq.Read())
                        {
                            if (readerTaskParametersFreq.GetValue(c => c.FreqCH) != null)
                            {
                                taskParameters.List_freq_CH.Add(readerTaskParametersFreq.GetValue(c => c.FreqCH).Value);
                            }
                        }
                        return true;
                    });
                    listTaskParameters.Add(taskParameters);
                }

                return true;
            });


            return listTaskParameters.ToArray();
        }

        public TaskParameters[] LoadObjectsWithRestrict()
        {
            List<TaskParameters> listTaskParameters = new List<TaskParameters>();
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();

            var builderInsertTaskParameters = this._dataLayer.GetBuilder<MD.ITaskParameters>().From();
            builderInsertTaskParameters.Select(c => c.LevelMinOccup_dBm);
            builderInsertTaskParameters.Select(c => c.MaxFreq_MHz);
            builderInsertTaskParameters.Select(c => c.MeasurementType);
            builderInsertTaskParameters.Select(c => c.MinFreq_MHz);
            builderInsertTaskParameters.Select(c => c.NChenal);
            builderInsertTaskParameters.Select(c => c.RBW_Hz);
            builderInsertTaskParameters.Select(c => c.ReceivedIQStreemDuration_sec);
            builderInsertTaskParameters.Select(c => c.SDRTaskId);
            builderInsertTaskParameters.Select(c => c.StartTime);
            builderInsertTaskParameters.Select(c => c.Status);
            builderInsertTaskParameters.Select(c => c.NCount);
            builderInsertTaskParameters.Select(c => c.StepSO_kHz);
            builderInsertTaskParameters.Select(c => c.StopTime);
            builderInsertTaskParameters.Select(c => c.TypeTechnology);
            builderInsertTaskParameters.Select(c => c.Type_of_SO);
            builderInsertTaskParameters.Select(c => c.VBW_Hz);
            builderInsertTaskParameters.Select(c => c.Id);
            builderInsertTaskParameters.Where(c => c.Status, DataModels.DataConstraint.ConditionOperator.NotEqual, StatusTask.C.ToString());
            builderInsertTaskParameters.Where(c => c.Status, DataModels.DataConstraint.ConditionOperator.NotEqual, StatusTask.Z.ToString());
            queryExecuter.Fetch(builderInsertTaskParameters, readerMeasTask =>
            {
                while (readerMeasTask.Read())
                {
                    TaskParameters taskParameters = new TaskParameters();

                    if (readerMeasTask.GetValue(c => c.LevelMinOccup_dBm) != null)
                    {
                        taskParameters.LevelMinOccup_dBm = readerMeasTask.GetValue(c => c.LevelMinOccup_dBm).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.MaxFreq_MHz) != null)
                    {
                        taskParameters.MaxFreq_MHz = readerMeasTask.GetValue(c => c.MaxFreq_MHz).Value;
                    }

                    MeasType measurementType;

                    if (Enum.TryParse<MeasType>(readerMeasTask.GetValue(c => c.MeasurementType) != null ? readerMeasTask.GetValue(c => c.MeasurementType).ToString() : "", out measurementType))
                    {
                        taskParameters.MeasurementType = measurementType;
                    }

                    if (readerMeasTask.GetValue(c => c.MinFreq_MHz) != null)
                    {
                        taskParameters.MinFreq_MHz = readerMeasTask.GetValue(c => c.MinFreq_MHz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.NChenal) != null)
                    {
                        taskParameters.NChenal = readerMeasTask.GetValue(c => c.NChenal).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.NCount) != null)
                    {
                        taskParameters.NCount = readerMeasTask.GetValue(c => c.NCount).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.RBW_Hz) != null)
                    {
                        taskParameters.RBW_Hz = readerMeasTask.GetValue(c => c.RBW_Hz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.ReceivedIQStreemDuration_sec) != null)
                    {
                        taskParameters.ReceivedIQStreemDuration_sec = readerMeasTask.GetValue(c => c.ReceivedIQStreemDuration_sec).Value;
                    }


                    taskParameters.SDRTaskId = readerMeasTask.GetValue(c => c.SDRTaskId);
                    taskParameters.status = readerMeasTask.GetValue(c => c.Status);

                    if (readerMeasTask.GetValue(c => c.StartTime) != null)
                    {
                        taskParameters.StartTime = readerMeasTask.GetValue(c => c.StartTime).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.StepSO_kHz) != null)
                    {
                        taskParameters.StepSO_kHz = readerMeasTask.GetValue(c => c.StepSO_kHz).Value;
                    }

                    if (readerMeasTask.GetValue(c => c.StopTime) != null)
                    {
                        taskParameters.StopTime = readerMeasTask.GetValue(c => c.StopTime).Value;
                    }


                    TypeTechnology typeTechnology;
                    if (Enum.TryParse<TypeTechnology>(readerMeasTask.GetValue(c => c.TypeTechnology) != null ? readerMeasTask.GetValue(c => c.TypeTechnology).ToString() : "", out typeTechnology))
                    {
                        taskParameters.TypeTechnology = typeTechnology;
                    }

                    SOType sOType;
                    if (Enum.TryParse<SOType>(readerMeasTask.GetValue(c => c.Type_of_SO) != null ? readerMeasTask.GetValue(c => c.Type_of_SO).ToString() : "", out sOType))
                    {
                        taskParameters.Type_of_SO = sOType;
                    }


                    if (readerMeasTask.GetValue(c => c.VBW_Hz) != null)
                    {
                        taskParameters.VBW_Hz = readerMeasTask.GetValue(c => c.VBW_Hz).Value;
                    }

                    taskParameters.List_freq_CH = new List<double>();

                    var builderInsertTaskParametersFreq = this._dataLayer.GetBuilder<MD.ITaskParametersFreq>().From();
                    builderInsertTaskParametersFreq.Select(c => c.FreqCH);
                    builderInsertTaskParametersFreq.Select(c => c.IdTaskParameters);
                    builderInsertTaskParametersFreq.Select(c => c.Id);
                    builderInsertTaskParametersFreq.Where(c => c.IdTaskParameters, DataModels.DataConstraint.ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                    queryExecuter.Fetch(builderInsertTaskParametersFreq, readerTaskParametersFreq =>
                    {
                        while (readerTaskParametersFreq.Read())
                        {
                            if (readerTaskParametersFreq.GetValue(c => c.FreqCH) != null)
                            {
                                taskParameters.List_freq_CH.Add(readerTaskParametersFreq.GetValue(c => c.FreqCH).Value);
                            }
                        }
                        return true;
                    });
                    listTaskParameters.Add(taskParameters);
                }

                return true;
            });


            return listTaskParameters.ToArray();
        }
    }
}



