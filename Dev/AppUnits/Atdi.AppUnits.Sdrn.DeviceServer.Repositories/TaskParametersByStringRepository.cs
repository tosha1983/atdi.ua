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
    public sealed class TaskParametersByStringRepository : IRepository<TaskParameters, string>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;

        public TaskParametersByStringRepository()
        {
        }

        public TaskParametersByStringRepository(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public string Create(TaskParameters item)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public TaskParameters[] LoadAllObjects()
        {
            throw new NotImplementedException();
        }

        public TaskParameters LoadObject(string SDRTaskId)
        {
            TaskParameters taskParameters = null;
            try
            {
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
                builderInsertTaskParameters.Select(c => c.NCount);
                builderInsertTaskParameters.Select(c => c.StartTime);
                builderInsertTaskParameters.Select(c => c.Status);
                builderInsertTaskParameters.Select(c => c.StepSO_kHz);
                builderInsertTaskParameters.Select(c => c.StopTime);
                builderInsertTaskParameters.Select(c => c.TypeTechnology);
                builderInsertTaskParameters.Select(c => c.Type_of_SO);
                builderInsertTaskParameters.Select(c => c.VBW_Hz);
                builderInsertTaskParameters.Select(c => c.SweepTime_ms);
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

                        if (readerMeasTask.GetValue(c => c.SweepTime_ms) != null)
                        {
                            taskParameters.SweepTime_s = readerMeasTask.GetValue(c => c.SweepTime_ms).Value;
                        }


                        TypeTechnology typeTechnology;
                        if (Enum.TryParse<TypeTechnology>(readerMeasTask.GetValue(c => c.TypeTechnology) != null ? readerMeasTask.GetValue(c => c.TypeTechnology).ToString() : "", out typeTechnology))
                        {
                            taskParameters.TypeTechnology = typeTechnology;
                        }

                        SOType sOType;
                        if (Enum.TryParse<SOType>(readerMeasTask.GetValue(c => c.Type_of_SO) != null ? readerMeasTask.GetValue(c => c.Type_of_SO).ToString() : "", out sOType))
                        {
                            taskParameters.TypeOfSO = sOType;
                        }


                        if (readerMeasTask.GetValue(c => c.VBW_Hz) != null)
                        {
                            taskParameters.VBW_Hz = readerMeasTask.GetValue(c => c.VBW_Hz).Value;
                        }

                        taskParameters.ListFreqCH = new List<double>();

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
                                    taskParameters.ListFreqCH.Add(readerTaskParametersFreq.GetValue(c => c.FreqCH).Value);
                                }
                            }
                            return true;
                        });

                        //ReferenceSignal

                        var listReferenceSituation = new List<DataModels.Sdrns.Device.ReferenceSituation>();
                        var builderReferenceSituationRaw = this._dataLayer.GetBuilder<MD.IReferenceSituation>().From();
                        builderReferenceSituationRaw.Select(c => c.Id);
                        builderReferenceSituationRaw.Select(c => c.SensorId);
                        builderReferenceSituationRaw.Select(c => c.MeasTaskId);
                        builderReferenceSituationRaw.Where(c => c.MeasTaskId, DataModels.DataConstraint.ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderReferenceSituationRaw, readerReferenceSituationRaw =>
                        {
                            while (readerReferenceSituationRaw.Read())
                            {
                                var refSituation = new DataModels.Sdrns.Device.ReferenceSituation();
                                if (readerReferenceSituationRaw.GetValue(c => c.SensorId).HasValue)
                                {
                                    refSituation.SensorId = readerReferenceSituationRaw.GetValue(c => c.SensorId).Value;
                                }

                                var referenceSignals = new List<DataModels.Sdrns.Device.ReferenceSignal>();
                                var builderReferenceSignalRaw = this._dataLayer.GetBuilder<MD.IReferenceSignalRaw>().From();
                                builderReferenceSignalRaw.Select(c => c.Id);
                                builderReferenceSignalRaw.Select(c => c.Bandwidth_kHz);
                                builderReferenceSignalRaw.Select(c => c.Frequency_MHz);
                                builderReferenceSignalRaw.Select(c => c.LevelSignal_dBm);
                                builderReferenceSignalRaw.Select(c => c.RefSituationId);
                                builderReferenceSignalRaw.Where(c => c.RefSituationId, DataModels.DataConstraint.ConditionOperator.Equal, readerReferenceSituationRaw.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderReferenceSignalRaw, readerReferenceSignalRaw =>
                                {
                                    while (readerReferenceSignalRaw.Read())
                                    {

                                        var referenceSignal = new DataModels.Sdrns.Device.ReferenceSignal();
                                        if (readerReferenceSignalRaw.GetValue(c => c.Bandwidth_kHz) != null)
                                        {
                                            referenceSignal.Bandwidth_kHz = readerReferenceSignalRaw.GetValue(c => c.Bandwidth_kHz).Value;
                                        }
                                        if (readerReferenceSignalRaw.GetValue(c => c.Frequency_MHz) != null)
                                        {
                                            referenceSignal.Frequency_MHz = readerReferenceSignalRaw.GetValue(c => c.Frequency_MHz).Value;
                                        }
                                        if (readerReferenceSignalRaw.GetValue(c => c.LevelSignal_dBm) != null)
                                        {
                                            referenceSignal.LevelSignal_dBm = readerReferenceSignalRaw.GetValue(c => c.LevelSignal_dBm).Value;
                                        }

                                        referenceSignal.SignalMask = new DataModels.Sdrns.Device.SignalMask();
                                        List<double> freqs = new List<double>();
                                        List<float> loss = new List<float>();
                                        var builderSignalMaskRaw = this._dataLayer.GetBuilder<MD.ISignalMaskRaw>().From();
                                        builderSignalMaskRaw.Select(c => c.Id);
                                        builderSignalMaskRaw.Select(c => c.ReferenceSignalId);
                                        builderSignalMaskRaw.Select(c => c.Freq_kHz);
                                        builderSignalMaskRaw.Select(c => c.Loss_dB);
                                        builderSignalMaskRaw.Where(c => c.ReferenceSignalId, DataModels.DataConstraint.ConditionOperator.Equal, readerReferenceSignalRaw.GetValue(c => c.Id));
                                        queryExecuter.Fetch(builderSignalMaskRaw, readerSignalMaskRaw =>
                                        {
                                            while (readerSignalMaskRaw.Read())
                                            {
                                                if (readerSignalMaskRaw.GetValue(c => c.Freq_kHz) != null)
                                                {
                                                    freqs.Add(readerSignalMaskRaw.GetValue(c => c.Freq_kHz).Value);
                                                }
                                                if (readerSignalMaskRaw.GetValue(c => c.Loss_dB) != null)
                                                {
                                                    loss.Add((float)readerSignalMaskRaw.GetValue(c => c.Loss_dB).Value);
                                                }
                                            }
                                            return true;
                                        });

                                        referenceSignal.SignalMask.Freq_kHz = freqs.ToArray();
                                        referenceSignal.SignalMask.Loss_dB = loss.ToArray();

                                        referenceSignals.Add(referenceSignal);
                                    }
                                    return true;
                                });

                                refSituation.ReferenceSignal = referenceSignals.ToArray();

                                listReferenceSituation.Add(refSituation);
                            }
                            return true;
                        });

                        if (listReferenceSituation.Count > 0)
                        {
                            taskParameters.RefSituation = listReferenceSituation[0];
                        }
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return taskParameters;
        }

        public TaskParameters[] LoadObjectsWithRestrict()
        {
            throw new NotImplementedException();
        }

        public bool Update(TaskParameters item)
        {
            throw new NotImplementedException();
        }
    }
}



