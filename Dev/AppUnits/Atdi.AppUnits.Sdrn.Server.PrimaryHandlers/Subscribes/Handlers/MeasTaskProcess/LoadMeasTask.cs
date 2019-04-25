using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;




namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{

    public class LoadMeasTask
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public LoadMeasTask(IDataLayer<EntityDataOrm> dataLayer,  ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }


        public MeasTask ReadTask(int id)
        {
            var measTask = new MeasTask();
            try
            {
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>().From();
                builderMeasTask.Select(c => c.CreatedBy);
                builderMeasTask.Select(c => c.DateCreated);
                builderMeasTask.Select(c => c.ExecutionMode);
                builderMeasTask.Select(c => c.Id);
                builderMeasTask.Select(c => c.IdStart);
                builderMeasTask.Select(c => c.MaxTimeBs);
                builderMeasTask.Select(c => c.Name);
                builderMeasTask.Select(c => c.OrderId);
                builderMeasTask.Select(c => c.PerInterval);
                builderMeasTask.Select(c => c.PerStart);
                builderMeasTask.Select(c => c.PerStop);
                builderMeasTask.Select(c => c.Prio);
                builderMeasTask.Select(c => c.ResultType);
                builderMeasTask.Select(c => c.Status);
                builderMeasTask.Select(c => c.Task);
                builderMeasTask.Select(c => c.TimeStart);
                builderMeasTask.Select(c => c.TimeStop);
                builderMeasTask.Select(c => c.Type);
                builderMeasTask.Where(c => c.Id, ConditionOperator.Equal, id);
                builderMeasTask.Where(c => c.Status, ConditionOperator.NotEqual, "Z");
                builderMeasTask.Where(c => c.Status, ConditionOperator.IsNotNull);
                queryExecuter.Fetch(builderMeasTask, readerMeasTask =>
                {
                    while (readerMeasTask.Read())
                    {
                        measTask.CreatedBy = readerMeasTask.GetValue(c => c.CreatedBy);
                        measTask.DateCreated = readerMeasTask.GetValue(c => c.DateCreated);
                        MeasTaskExecutionMode ExecutionMode;
                        if (Enum.TryParse<MeasTaskExecutionMode>(readerMeasTask.GetValue(c => c.ExecutionMode), out ExecutionMode))
                        {
                            measTask.ExecutionMode = ExecutionMode;
                        }
                        measTask.Id = new MeasTaskIdentifier();
                        measTask.Id.Value = readerMeasTask.GetValue(c => c.Id);
                        measTask.MaxTimeBs = readerMeasTask.GetValue(c => c.MaxTimeBs);
                        measTask.Name = readerMeasTask.GetValue(c => c.Name);
                        measTask.OrderId = readerMeasTask.GetValue(c => c.OrderId);
                        measTask.Prio = readerMeasTask.GetValue(c => c.Prio);
                        MeasTaskResultType ResultType;
                        if (Enum.TryParse<MeasTaskResultType>(readerMeasTask.GetValue(c => c.ResultType), out ResultType))
                        {
                            measTask.ResultType = ResultType;
                        }
                        measTask.Status = readerMeasTask.GetValue(c => c.Status);
                        MeasTaskType Task;
                        if (Enum.TryParse<MeasTaskType>(readerMeasTask.GetValue(c => c.Task), out Task))
                        {
                            measTask.Task = Task;
                        }
                        measTask.Type = readerMeasTask.GetValue(c => c.Type);


                        var listReferenceSituation = new List<ReferenceSituation>();
                        var builderReferenceSituationRaw = this._dataLayer.GetBuilder<MD.IReferenceSituation>().From();
                        builderReferenceSituationRaw.Select(c => c.Id);
                        builderReferenceSituationRaw.Select(c => c.SensorId);
                        builderReferenceSituationRaw.Select(c => c.MeasTaskId);
                        builderReferenceSituationRaw.Where(c => c.MeasTaskId, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderReferenceSituationRaw, readerReferenceSituationRaw =>
                        {
                            while (readerReferenceSituationRaw.Read())
                            {
                                var refSituation = new ReferenceSituation();
                                if (readerReferenceSituationRaw.GetValue(c => c.SensorId).HasValue)
                                {
                                    refSituation.SensorId = readerReferenceSituationRaw.GetValue(c => c.SensorId).Value;
                                }

                                var referenceSignals = new List<ReferenceSignal>();
                                var builderReferenceSignalRaw = this._dataLayer.GetBuilder<MD.IReferenceSignal>().From();
                                builderReferenceSignalRaw.Select(c => c.Id);
                                builderReferenceSignalRaw.Select(c => c.Bandwidth_kHz);
                                builderReferenceSignalRaw.Select(c => c.Frequency_MHz);
                                builderReferenceSignalRaw.Select(c => c.LevelSignal_dBm);
                                builderReferenceSignalRaw.Select(c => c.RefSituationId);
                                builderReferenceSignalRaw.Select(c => c.IcsmId);
                                builderReferenceSignalRaw.Select(c => c.IcsmTable);
                                builderReferenceSignalRaw.Where(c => c.RefSituationId, ConditionOperator.Equal, readerReferenceSituationRaw.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderReferenceSignalRaw, readerReferenceSignalRaw =>
                                {
                                    while (readerReferenceSignalRaw.Read())
                                    {

                                        var referenceSignal = new ReferenceSignal();
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
                                        if (readerReferenceSignalRaw.GetValue(c => c.IcsmId) != null)
                                        {
                                            referenceSignal.IcsmId = readerReferenceSignalRaw.GetValue(c => c.IcsmId).Value;
                                        }
                                        
                                        referenceSignal.IcsmTable = readerReferenceSignalRaw.GetValue(c => c.IcsmTable);

                                        referenceSignal.SignalMask = new SignalMask();
                                        List<double> freqs = new List<double>();
                                        List<float> loss = new List<float>();
                                        var builderSignalMaskRaw = this._dataLayer.GetBuilder<MD.ISignalMask>().From();
                                        builderSignalMaskRaw.Select(c => c.Id);
                                        builderSignalMaskRaw.Select(c => c.EmittingId);
                                        builderSignalMaskRaw.Select(c => c.Freq_kHz);
                                        builderSignalMaskRaw.Select(c => c.Loss_dB);
                                        builderSignalMaskRaw.Select(c => c.ReferenceSignalId);
                                        builderSignalMaskRaw.Where(c => c.ReferenceSignalId, ConditionOperator.Equal, readerReferenceSignalRaw.GetValue(c => c.Id));
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
                            measTask.RefSituation = listReferenceSituation.ToArray();
                        }


                        // MeasTimeParamList

                        var timeParamList = new MeasTimeParamList();
                        timeParamList.PerInterval = readerMeasTask.GetValue(c => c.PerInterval);
                        if (readerMeasTask.GetValue(c => c.PerStart) != null)
                        {
                            timeParamList.PerStart = readerMeasTask.GetValue(c => c.PerStart).Value;
                        }
                        if (readerMeasTask.GetValue(c => c.PerStop) != null)
                        {
                            timeParamList.PerStop = readerMeasTask.GetValue(c => c.PerStop).Value;
                        }
                        timeParamList.TimeStart = readerMeasTask.GetValue(c => c.TimeStart);
                        timeParamList.TimeStop = readerMeasTask.GetValue(c => c.TimeStop);
                        measTask.MeasTimeParamList = timeParamList;

                        // IMeasStation

                        var measStations = new List<MeasStation>();
                        var builderMeasstation = this._dataLayer.GetBuilder<MD.IMeasStation>().From();
                        builderMeasstation.Select(c => c.Id);
                        builderMeasstation.Select(c => c.StationId);
                        builderMeasstation.Select(c => c.MeasTaskId);
                        builderMeasstation.Select(c => c.StationType);
                        builderMeasstation.Where(c => c.MeasTaskId, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasstation, readerMeasStation =>
                        {
                            while (readerMeasStation.Read())
                            {
                                var measStation = new MeasStation();
                                measStation.StationId = new MeasStationIdentifier();
                                if (readerMeasStation.GetValue(c => c.StationId) != null) measStation.StationId.Value = readerMeasStation.GetValue(c => c.StationId).Value;
                                measStation.StationType = readerMeasStation.GetValue(c => c.StationType);
                                measStations.Add(measStation);
                            }
                            return true;
                        });
                        measTask.Stations = measStations.ToArray();

                        // IMeasDtParam

                        var builderMeasDtParam = this._dataLayer.GetBuilder<MD.IMeasDtParam>().From();
                        builderMeasDtParam.Select(c => c.Id);
                        builderMeasDtParam.Select(c => c.Demod);
                        builderMeasDtParam.Select(c => c.DetectType);
                        builderMeasDtParam.Select(c => c.Ifattenuation);
                        builderMeasDtParam.Select(c => c.MeasTaskId);
                        builderMeasDtParam.Select(c => c.MeasTime);
                        builderMeasDtParam.Select(c => c.Mode);
                        builderMeasDtParam.Select(c => c.Preamplification);
                        builderMeasDtParam.Select(c => c.Rbw);
                        builderMeasDtParam.Select(c => c.Rfattenuation);
                        builderMeasDtParam.Select(c => c.TypeMeasurements);
                        builderMeasDtParam.Select(c => c.Vbw);
                        builderMeasDtParam.Where(c => c.MeasTaskId, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasDtParam, readerMeasDtParam =>
                        {
                            while (readerMeasDtParam.Read())
                            {
                                var dtx = new MeasDtParam();
                                dtx.Demod = readerMeasDtParam.GetValue(c => c.Demod);
                                DetectingType detectType;
                                if (Enum.TryParse<DetectingType>(readerMeasDtParam.GetValue(c => c.DetectType), out detectType))
                                    dtx.DetectType = detectType;

                                dtx.IfAttenuation = readerMeasDtParam.GetValue(c => c.Ifattenuation).HasValue ? readerMeasDtParam.GetValue(c => c.Ifattenuation).Value : 0;
                                dtx.MeasTime = readerMeasDtParam.GetValue(c => c.MeasTime);
                                MeasurementMode mode;
                                if (Enum.TryParse<MeasurementMode>(readerMeasDtParam.GetValue(c => c.Mode), out mode))
                                    dtx.Mode = mode;

                                dtx.Preamplification = readerMeasDtParam.GetValue(c => c.Preamplification).HasValue ? readerMeasDtParam.GetValue(c => c.Preamplification).Value : -1;
                                dtx.RBW = readerMeasDtParam.GetValue(c => c.Rbw);
                                dtx.RfAttenuation = readerMeasDtParam.GetValue(c => c.Rfattenuation).HasValue ? readerMeasDtParam.GetValue(c => c.Rfattenuation).Value : 0;
                                MeasurementType typeMeasurements;
                                if (Enum.TryParse<MeasurementType>(readerMeasDtParam.GetValue(c => c.TypeMeasurements), out typeMeasurements))
                                    dtx.TypeMeasurements = typeMeasurements;
                                dtx.VBW = readerMeasDtParam.GetValue(c => c.Vbw);
                                measTask.MeasDtParam = dtx;

                            }
                            return true;
                        });
                        measTask.Stations = measStations.ToArray();


                        // IMeasFreqParam


                        var builderMeasFreqParam = this._dataLayer.GetBuilder<MD.IMeasFreqParam>().From();
                        builderMeasFreqParam.Select(c => c.Id);
                        builderMeasFreqParam.Select(c => c.MeasTaskId);
                        builderMeasFreqParam.Select(c => c.Mode);
                        builderMeasFreqParam.Select(c => c.Rgl);
                        builderMeasFreqParam.Select(c => c.Rgu);
                        builderMeasFreqParam.Select(c => c.Step);
                        builderMeasFreqParam.Where(c => c.MeasTaskId, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasFreqParam, readerMeasFreqParam =>
                        {
                            while (readerMeasFreqParam.Read())
                            {
                                var freqParam = new MeasFreqParam();
                                FrequencyMode Mode;
                                if (Enum.TryParse<FrequencyMode>(readerMeasFreqParam.GetValue(x => x.Mode), out Mode))
                                    freqParam.Mode = Mode;
                                freqParam.RgL = readerMeasFreqParam.GetValue(x => x.Rgl);
                                freqParam.RgU = readerMeasFreqParam.GetValue(x => x.Rgu);
                                freqParam.Step = readerMeasFreqParam.GetValue(x => x.Step);


                                var listmeasFreq = new List<MeasFreq>();
                                var builderMeasFreq = this._dataLayer.GetBuilder<MD.IMeasFreq>().From();
                                builderMeasFreq.Select(c => c.Id);
                                builderMeasFreq.Select(c => c.Freq);
                                builderMeasFreq.Select(c => c.MeasFreqParamId);
                                builderMeasFreq.Where(c => c.MeasFreqParamId, ConditionOperator.Equal, readerMeasFreqParam.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderMeasFreq, readerMeasFreq =>
                                {
                                    while (readerMeasFreq.Read())
                                    {
                                        var measFreq = new MeasFreq();
                                        if (readerMeasFreq.GetValue(c => c.Freq) != null)
                                        {
                                            measFreq.Freq = readerMeasFreq.GetValue(c => c.Freq).Value;
                                            listmeasFreq.Add(measFreq);
                                        }
                                    }
                                    return true;
                                });
                                freqParam.MeasFreqs = listmeasFreq.ToArray();
                                measTask.MeasFreqParam = freqParam;

                            }
                            return true;
                        });


                        // IMeasLocationParam

                        var measLocParams = new List<MeasLocParam>();
                        var builderMeasLocationParam = this._dataLayer.GetBuilder<MD.IMeasLocationParam>().From();
                        builderMeasLocationParam.Select(c => c.Id);
                        builderMeasLocationParam.Select(c => c.Asl);
                        builderMeasLocationParam.Select(c => c.Lat);
                        builderMeasLocationParam.Select(c => c.Lon);
                        builderMeasLocationParam.Select(c => c.MaxDist);
                        builderMeasLocationParam.Select(c => c.MeasTaskId);
                        builderMeasLocationParam.Where(c => c.MeasTaskId, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasLocationParam, readermeasLocParam =>
                        {
                            while (readermeasLocParam.Read())
                            {
                                var measLocParam = new MeasLocParam();
                                measLocParam.ASL = readermeasLocParam.GetValue(c => c.Asl);
                                measLocParam.Lat = readermeasLocParam.GetValue(c => c.Lat);
                                measLocParam.Lon = readermeasLocParam.GetValue(c => c.Lon);
                                measLocParam.MaxDist = readermeasLocParam.GetValue(c => c.MaxDist);
                                measLocParams.Add(measLocParam);
                            }
                            return true;
                        });
                        measTask.MeasLocParams = measLocParams.ToArray();



                        var measOther = new MeasOther();
                        var builderMeasOther = this._dataLayer.GetBuilder<MD.IMeasOther>().From();
                        builderMeasOther.Select(c => c.Id);
                        builderMeasOther.Select(c => c.LevelMinOccup);
                        builderMeasOther.Select(c => c.MeasTaskId);
                        builderMeasOther.Select(c => c.Nchenal);
                        builderMeasOther.Select(c => c.SwNumber);
                        builderMeasOther.Select(c => c.TypeSpectrumOccupation);
                        builderMeasOther.Select(c => c.TypeSpectrumscan);
                        builderMeasOther.Where(c => c.MeasTaskId, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasOther, readerMeasOther =>
                        {
                            while (readerMeasOther.Read())
                            {
                                measOther.LevelMinOccup = readerMeasOther.GetValue(c => c.LevelMinOccup);
                                measOther.NChenal = readerMeasOther.GetValue(c => c.Nchenal);
                                measOther.SwNumber = readerMeasOther.GetValue(c => c.SwNumber);

                                SpectrumOccupationType typeSpectrumOccupation;
                                if (Enum.TryParse<SpectrumOccupationType>(readerMeasOther.GetValue(c => c.TypeSpectrumOccupation), out typeSpectrumOccupation))
                                {
                                    measOther.TypeSpectrumOccupation = typeSpectrumOccupation;
                                }

                                SpectrumScanType typeSpectrumscan;
                                if (Enum.TryParse<SpectrumScanType>(readerMeasOther.GetValue(c => c.TypeSpectrumscan), out typeSpectrumscan))
                                {
                                    measOther.TypeSpectrumScan = typeSpectrumscan;
                                }

                            }
                            return true;
                        });
                        measTask.MeasOther = measOther;


                        var listmeasSubTask = new List<MeasSubTask>();
                        var builderMeasSubTask = this._dataLayer.GetBuilder<MD.IMeasSubTask>().From();
                        builderMeasSubTask.Select(c => c.Id);
                        builderMeasSubTask.Select(c => c.Interval);
                        builderMeasSubTask.Select(c => c.MeasTaskId);
                        builderMeasSubTask.Select(c => c.Status);
                        builderMeasSubTask.Select(c => c.TimeStart);
                        builderMeasSubTask.Select(c => c.TimeStop);
                        builderMeasSubTask.Where(c => c.MeasTaskId, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                        queryExecuter.Fetch(builderMeasSubTask, readerMeasSubTask =>
                        {
                            while (readerMeasSubTask.Read())
                            {
                                var measSubTask = new MeasSubTask();
                                measSubTask.Id = new MeasTaskIdentifier();
                                measSubTask.Id.Value = readerMeasSubTask.GetValue(c => c.Id);
                                measSubTask.Interval = readerMeasSubTask.GetValue(c => c.Interval);
                                measSubTask.Status = readerMeasSubTask.GetValue(c => c.Status);
                                if (readerMeasSubTask.GetValue(c => c.TimeStart) != null) measSubTask.TimeStart = readerMeasSubTask.GetValue(c => c.TimeStart).Value;
                                if (readerMeasSubTask.GetValue(c => c.TimeStop) != null) measSubTask.TimeStop = readerMeasSubTask.GetValue(c => c.TimeStop).Value;
                                var listMeasSubTaskStation = new List<MeasSubTaskStation>();
                                var builderMeasSubTaskSta = this._dataLayer.GetBuilder<MD.IMeasSubTaskSta>().From();
                                builderMeasSubTaskSta.Select(c => c.Id);
                                builderMeasSubTaskSta.Select(c => c.Count);
                                builderMeasSubTaskSta.Select(c => c.MeasSubTaskId);
                                builderMeasSubTaskSta.Select(c => c.SensorId);
                                builderMeasSubTaskSta.Select(c => c.Status);
                                builderMeasSubTaskSta.Select(c => c.TimeNextTask);
                                builderMeasSubTaskSta.Where(c => c.MeasSubTaskId, ConditionOperator.Equal, readerMeasSubTask.GetValue(c => c.Id));
                                queryExecuter.Fetch(builderMeasSubTaskSta, readerMeasSubTaskSta =>
                                {
                                    while (readerMeasSubTaskSta.Read())
                                    {
                                        var measSubTaskStation = new MeasSubTaskStation();
                                        measSubTaskStation.Count = readerMeasSubTaskSta.GetValue(c => c.Count);
                                        measSubTaskStation.Id = readerMeasSubTaskSta.GetValue(c => c.Id);
                                        measSubTaskStation.StationId = new SensorIdentifier();
                                        if (readerMeasSubTaskSta.GetValue(c => c.SensorId) != null) measSubTaskStation.StationId.Value = readerMeasSubTaskSta.GetValue(c => c.SensorId).Value;
                                        measSubTaskStation.Status = readerMeasSubTaskSta.GetValue(c => c.Status);
                                        measSubTaskStation.TimeNextTask = readerMeasSubTaskSta.GetValue(c => c.TimeNextTask);
                                        listMeasSubTaskStation.Add(measSubTaskStation);
                                    }
                                    return true;
                                });
                                measSubTask.MeasSubTaskStations = listMeasSubTaskStation.ToArray();
                                listmeasSubTask.Add(measSubTask);
                            }
                            return true;
                        });
                        measTask.MeasSubTasks = listmeasSubTask.ToArray();
                        measTask.StationsForMeasurements = GetStationDataForMeasurementsByTaskId(id);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return measTask;
        }

        public StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(int taskId)
        {
            var listStationData = new List<StationDataForMeasurements>();
            try
            {
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderStation = this._dataLayer.GetBuilder<MD.IStation>().From();
                builderStation.Select(c => c.MeasTaskId);
                builderStation.Select(c => c.Id);
                builderStation.Select(c => c.StationSiteId);
                builderStation.Select(c => c.CloseDate);
                builderStation.Select(c => c.DozvilName);
                builderStation.Select(c => c.EndDate);
                builderStation.Select(c => c.GlobalSID);
                builderStation.Select(c => c.MeasTaskId);
                builderStation.Select(c => c.OwnerDataId);
                builderStation.Select(c => c.Standart);
                builderStation.Select(c => c.StartDate);
                builderStation.Select(c => c.StationId);
                builderStation.Select(c => c.StationSiteId);
                builderStation.Select(c => c.Status);
                builderStation.Where(c => c.MeasTaskId, ConditionOperator.Equal, taskId);
                builderStation.OrderByAsc(c => c.Id);
                queryExecuter.Fetch(builderStation, readerStation =>
                {
                    while (readerStation.Read())
                    {
                        var measStation = new StationDataForMeasurements();
                        measStation.IdStation = readerStation.GetValue(c => c.StationId).HasValue ? readerStation.GetValue(c => c.StationId).Value : -1;
                        measStation.GlobalSID = readerStation.GetValue(c => c.GlobalSID);
                        measStation.Standart = readerStation.GetValue(c => c.Standart);
                        measStation.Status = readerStation.GetValue(c => c.Status);
                        var perm = new PermissionForAssignment();
                        perm.CloseDate = readerStation.GetValue(c => c.CloseDate);
                        perm.DozvilName = readerStation.GetValue(c => c.DozvilName);
                        perm.EndDate = readerStation.GetValue(c => c.EndDate);
                        perm.Id = null;
                        perm.StartDate = readerStation.GetValue(c => c.StartDate);
                        measStation.LicenseParameter = perm;
                        measStation.IdSite = readerStation.GetValue(c => c.StationSiteId) != null ? readerStation.GetValue(c => c.StationSiteId).Value : -1;
                        measStation.IdOwner = readerStation.GetValue(c => c.OwnerDataId) != null ? readerStation.GetValue(c => c.OwnerDataId).Value : -1;

                        var ownerData = new OwnerData();
                        var builderOwnerData = this._dataLayer.GetBuilder<MD.IOwnerData>().From();
                        builderOwnerData.Select(c => c.Address);
                        builderOwnerData.Select(c => c.CODE);
                        builderOwnerData.Select(c => c.Id);
                        builderOwnerData.Select(c => c.OKPO);
                        builderOwnerData.Select(c => c.OwnerName);
                        builderOwnerData.Select(c => c.ZIP);
                        builderOwnerData.Where(c => c.Id, ConditionOperator.Equal, measStation.IdOwner);
                        builderOwnerData.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderOwnerData, readerOwnerData =>
                        {
                            while (readerOwnerData.Read())
                            {
                                ownerData.Addres = readerOwnerData.GetValue(c => c.Address);
                                ownerData.Code = readerOwnerData.GetValue(c => c.CODE);
                                ownerData.OKPO = readerOwnerData.GetValue(c => c.OKPO);
                                ownerData.OwnerName = readerOwnerData.GetValue(c => c.OwnerName);
                                ownerData.Zip = readerOwnerData.GetValue(c => c.ZIP);
                                ownerData.Id = readerOwnerData.GetValue(c => c.Id);
                            }
                            return true;
                        });

                        measStation.Owner = ownerData;


                        var siteStationForMeas = new SiteStationForMeas();
                        var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().From();
                        builderStationSite.Select(c => c.Address);
                        builderStationSite.Select(c => c.Id);
                        builderStationSite.Select(c => c.Lat);
                        builderStationSite.Select(c => c.Lon);
                        builderStationSite.Select(c => c.Region);
                        builderStationSite.Where(c => c.Id, ConditionOperator.Equal, measStation.IdSite);
                        builderStationSite.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderStationSite, readerStationSite =>
                        {
                            while (readerStationSite.Read())
                            {
                                siteStationForMeas.Adress = readerStationSite.GetValue(c => c.Address);
                                siteStationForMeas.Lat = readerStationSite.GetValue(c => c.Lat);
                                siteStationForMeas.Lon = readerStationSite.GetValue(c => c.Lon);
                                siteStationForMeas.Region = readerStationSite.GetValue(c => c.Region);
                            }
                            return true;
                        });

                        measStation.Site = siteStationForMeas;


                        List<SectorStationForMeas> listSector = new List<SectorStationForMeas>();
                        var builderISector = this._dataLayer.GetBuilder<MD.ISector>().From();
                        builderISector.Select(c => c.Agl);
                        builderISector.Select(c => c.Azimut);
                        builderISector.Select(c => c.Bw);
                        builderISector.Select(c => c.ClassEmission);
                        builderISector.Select(c => c.Eirp);
                        builderISector.Select(c => c.Id);
                        builderISector.Select(c => c.SectorId);
                        builderISector.Select(c => c.StationId);
                        builderISector.Where(c => c.StationId, ConditionOperator.Equal, readerStation.GetValue(c => c.Id));
                        builderISector.OrderByAsc(c => c.Id);
                        queryExecuter.Fetch(builderISector, readerSector =>
                        {
                            while (readerSector.Read())
                            {
                                var sectM = new SectorStationForMeas();
                                sectM.AGL = readerSector.GetValue(c => c.Agl);
                                sectM.Azimut = readerSector.GetValue(c => c.Azimut);
                                sectM.BW = readerSector.GetValue(c => c.Bw);
                                sectM.ClassEmission = readerSector.GetValue(c => c.ClassEmission);
                                sectM.EIRP = readerSector.GetValue(c => c.Eirp);
                                sectM.IdSector = readerSector.GetValue(c => c.SectorId).HasValue ? readerSector.GetValue(c => c.SectorId).Value : -1;



                                var lFreqICSM = new List<FrequencyForSectorFormICSM>();
                                var builderLinkSectorFreq = this._dataLayer.GetBuilder<MD.ILinkSectorFreq>().From();
                                builderLinkSectorFreq.Select(c => c.Id);
                                builderLinkSectorFreq.Select(c => c.SectorFreqId);
                                builderLinkSectorFreq.Select(c => c.SECTORFREQ.ChannelNumber);
                                builderLinkSectorFreq.Select(c => c.SECTORFREQ.Frequency);
                                builderLinkSectorFreq.Select(c => c.SECTORFREQ.Id);
                                builderLinkSectorFreq.Select(c => c.SECTORFREQ.PlanId);
                                builderLinkSectorFreq.Where(c => c.SectorId, ConditionOperator.Equal, readerSector.GetValue(c => c.Id));
                                builderLinkSectorFreq.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderLinkSectorFreq, readerLinkSectorFreq =>
                                {
                                    while (readerLinkSectorFreq.Read())
                                    {
                                        var freqM = new FrequencyForSectorFormICSM();
                                        freqM.ChannalNumber = readerLinkSectorFreq.GetValue(x => x.SECTORFREQ.ChannelNumber);
                                        freqM.Frequency = (decimal)readerLinkSectorFreq.GetValue(x => x.SECTORFREQ.Frequency);
                                        freqM.Id = null;
                                        freqM.IdPlan = readerLinkSectorFreq.GetValue(x => x.SECTORFREQ.PlanId);
                                        lFreqICSM.Add(freqM);
                                    }
                                    return true;
                                });

                                sectM.Frequencies = lFreqICSM.ToArray();

                                var lMask = new List<MaskElements>();
                                var builderLinkSectorMaskElement = this._dataLayer.GetBuilder<MD.ILinkSectorMaskElement>().From();
                                builderLinkSectorMaskElement.Select(c => c.Id);
                                builderLinkSectorMaskElement.Select(c => c.SECTORMASKELEMENT.Bw);
                                builderLinkSectorMaskElement.Select(c => c.SECTORMASKELEMENT.Id);
                                builderLinkSectorMaskElement.Select(c => c.SECTORMASKELEMENT.Level);
                                builderLinkSectorMaskElement.Where(c => c.SectorId, ConditionOperator.Equal, readerSector.GetValue(c => c.Id));
                                builderLinkSectorMaskElement.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderLinkSectorMaskElement, readerLinkSectorMaskElement =>
                                {
                                    while (readerLinkSectorMaskElement.Read())
                                    {
                                        MaskElements maskElementsM = new MaskElements();
                                        maskElementsM.BW = readerLinkSectorMaskElement.GetValue(c => c.SECTORMASKELEMENT.Bw);
                                        maskElementsM.level = readerLinkSectorMaskElement.GetValue(c => c.SECTORMASKELEMENT.Level);
                                        lMask.Add(maskElementsM);
                                    }
                                    return true;
                                });
                                sectM.MaskBW = lMask.ToArray();
                                listSector.Add(sectM);
                            }
                            return true;
                        });

                        measStation.Sectors = listSector.ToArray();
                        listStationData.Add(measStation);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listStationData.ToArray();
        }

        public  List<Atdi.DataModels.Sdrns.Device.MeasTask> CreateeasTaskSDRsApi(MeasTask task, string SensorName, string SdrnServer, string EquipmentTechId, int? MeasTaskId, string Type = "New")
        {
            var saveMeasTask = new SaveMeasTask(_dataLayer, _logger);
            List<Atdi.DataModels.Sdrns.Device.MeasTask> ListMTSDR = new List<Atdi.DataModels.Sdrns.Device.MeasTask>();
            if (task.MeasSubTasks == null) return ListMTSDR;

            for (int f=0; f < task.MeasSubTasks.Length; f++)
            { 
                var SubTask = task.MeasSubTasks[f];
                if (SubTask.MeasSubTaskStations != null)
                {
                    for (int g = 0; g < SubTask.MeasSubTaskStations.Length; g++)
                    { 
                        var SubTaskStation = SubTask.MeasSubTaskStations[g];
                   
                        if ((Type == "New") || ((Type == "Stop") && ((SubTaskStation.Status == "F") || (SubTaskStation.Status == "P"))) || ((Type == "Run") && ((SubTaskStation.Status == "O") || (SubTaskStation.Status == "A"))) ||
                            ((Type == "Del") && (SubTaskStation.Status == "Z")))
                        {
                            Atdi.DataModels.Sdrns.Device.MeasTask MTSDR = new Atdi.DataModels.Sdrns.Device.MeasTask();
                            //int? IdentValueTaskSDR = saveMeasTask.SaveTaskSDRToDB(SubTask.Id.Value, SubTaskStation.Id, task.Id.Value, SubTaskStation.StationId.Value);
                            MTSDR.TaskId = MeasTaskId.ToString();//IdentValueTaskSDR.GetValueOrDefault().ToString();
                            if (task.Id == null) task.Id = new MeasTaskIdentifier();
                            if (task.MeasOther == null) task.MeasOther = new MeasOther();
                            if (task.MeasDtParam == null) { task.MeasDtParam = new MeasDtParam(); }
                            if (task.Prio != null) { MTSDR.Priority = task.Prio.GetValueOrDefault(); } else { MTSDR.Priority = 10; }
                            MTSDR.SensorName = SensorName;
                            MTSDR.SdrnServer = SdrnServer;
                            MTSDR.EquipmentTechId = EquipmentTechId;
                            if (Type == "New")
                            {
                                MTSDR.SOParam = new DEV.SpectrumOccupationMeasParam();
                                if (task.MeasOther.LevelMinOccup != null) { MTSDR.SOParam.LevelMinOccup_dBm = task.MeasOther.LevelMinOccup.GetValueOrDefault(); } else { MTSDR.SOParam.LevelMinOccup_dBm = -70; }
                                if (task.MeasOther.NChenal != null) { MTSDR.SOParam.MeasurmentNumber = task.MeasOther.NChenal.GetValueOrDefault(); } else { MTSDR.SOParam.MeasurmentNumber = 10; }
                                switch (task.MeasOther.TypeSpectrumOccupation)
                                {
                                    case SpectrumOccupationType.FreqBandwidthOccupation:
                                        MTSDR.SOParam.Type = DataModels.Sdrns.SpectrumOccupationType.FreqBandOccupancy;
                                        break;
                                    case SpectrumOccupationType.FreqChannelOccupation:
                                        MTSDR.SOParam.Type = DataModels.Sdrns.SpectrumOccupationType.FreqChannelOccupancy;
                                        break;
                                    default:
                                        throw new NotImplementedException($"Type '{task.MeasOther.TypeSpectrumOccupation}' not supported");
                                }
                                switch (task.MeasDtParam.TypeMeasurements)
                                {
                                    case MeasurementType.MonitoringStations:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.MonitoringStations;
                                        break;
                                    case MeasurementType.Level:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.Level;
                                        break;
                                    case MeasurementType.SpectrumOccupation:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.SpectrumOccupation;
                                        break;
                                    case MeasurementType.BandwidthMeas:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.BandwidthMeas;
                                        break;
                                    case MeasurementType.Signaling:
                                        MTSDR.Measurement = DataModels.Sdrns.MeasurementType.Signaling;
                                        break;
                                    default:
                                        throw new NotImplementedException($"Type '{task.MeasDtParam.TypeMeasurements}' not supported");
                                }
                                if (SubTask.Interval != null) { MTSDR.Interval_sec = SubTask.Interval.GetValueOrDefault(); }
                                MTSDR.DeviceParam = new DEV.DeviceMeasParam();
                                if (task.MeasDtParam.MeasTime != null) { MTSDR.DeviceParam.MeasTime_sec = task.MeasDtParam.MeasTime.GetValueOrDefault(); } else { MTSDR.DeviceParam.MeasTime_sec = 0.001; }


                                switch (task.MeasDtParam.DetectType)
                                {
                                    case DetectingType.Avarage:
                                        MTSDR.DeviceParam.DetectType = DataModels.Sdrns.DetectingType.Average;
                                        break;
                                    case DetectingType.MaxPeak:
                                        MTSDR.DeviceParam.DetectType = DataModels.Sdrns.DetectingType.MaxPeak;
                                        break;
                                    case DetectingType.MinPeak:
                                        MTSDR.DeviceParam.DetectType = DataModels.Sdrns.DetectingType.MinPeak;
                                        break;
                                    case DetectingType.Peak:
                                        MTSDR.DeviceParam.DetectType = DataModels.Sdrns.DetectingType.Peak;
                                        break;
                                    default:
                                        throw new NotImplementedException($"Type '{task.MeasDtParam.DetectType}' not supported");
                                }

                                MTSDR.DeviceParam.Preamplification_dB = task.MeasDtParam.Preamplification;
                                if (task.MeasDtParam.RBW != null) { MTSDR.DeviceParam.RBW_kHz = task.MeasDtParam.RBW.GetValueOrDefault(); } else { MTSDR.DeviceParam.RBW_kHz= 10; }
                                MTSDR.DeviceParam.RfAttenuation_dB = (int)task.MeasDtParam.RfAttenuation;
                                if (task.MeasDtParam.VBW != null) { MTSDR.DeviceParam.VBW_kHz = task.MeasDtParam.VBW.GetValueOrDefault(); } else { MTSDR.DeviceParam.VBW_kHz = 10; }
                                MTSDR.DeviceParam.RefLevel_dBm = -30;

                                MTSDR.Frequencies = new DEV.MeasuredFrequencies();
                                if (task.MeasFreqParam != null)
                                {
                                    var freqs = task.MeasFreqParam.MeasFreqs;
                                    if (freqs != null)
                                    {
                                        Double[] listFreqs = new double[freqs.Length];
                                        for (int j = 0; j < freqs.Length; j++)
                                        {
                                            listFreqs[j] = freqs[j].Freq;
                                        }
                                        MTSDR.Frequencies.Values_MHz = listFreqs;
                                    }

                                    if (task.MeasFreqParam.Mode == FrequencyMode.FrequencyList)
                                    {
                                        MTSDR.Frequencies.Mode = DataModels.Sdrns.FrequencyMode.FrequencyList;
                                    }
                                    else if (task.MeasFreqParam.Mode == FrequencyMode.FrequencyRange)
                                    {
                                        MTSDR.Frequencies.Mode = DataModels.Sdrns.FrequencyMode.FrequencyRange;
                                    }
                                    else if (task.MeasFreqParam.Mode == FrequencyMode.SingleFrequency)
                                    {
                                        MTSDR.Frequencies.Mode = DataModels.Sdrns.FrequencyMode.SingleFrequency;
                                    }
                                    else
                                    {
                                        throw new NotImplementedException($"Type '{MTSDR.Frequencies.Mode}' not supported");
                                    }
                                    MTSDR.Frequencies.RgL_MHz = task.MeasFreqParam.RgL;
                                    MTSDR.Frequencies.RgU_MHz = task.MeasFreqParam.RgU;
                                    MTSDR.Frequencies.Step_kHz = task.MeasFreqParam.Step;
                                }

                                double subFreqMaxMin = 0;
                                if ((MTSDR.Frequencies != null) && (MTSDR.Frequencies.Values_MHz != null) && (MTSDR.Frequencies.Values_MHz.Length > 0))
                                {
                                    var minFreq = MTSDR.Frequencies.Values_MHz.Min();
                                    var maxFreq = MTSDR.Frequencies.Values_MHz.Max();
                                    subFreqMaxMin = maxFreq - minFreq;
                                }
                                if ((subFreqMaxMin >= 0) && (MTSDR.Frequencies.Step_kHz>0))
                                {
                                    MTSDR.DeviceParam.ScanBW_kHz = subFreqMaxMin * 1000 + MTSDR.Frequencies.Step_kHz;
                                }

                                MTSDR.ScanParameters = new DataModels.Sdrns.Device.StandardScanParameter[] { };
                                MTSDR.StartTime = SubTask.TimeStart;
                                MTSDR.StopTime = SubTask.TimeStop;
                                MTSDR.Status = SubTask.Status;
                                MTSDR.MobEqipmentMeasurements = new DataModels.Sdrns.MeasurementType[3];
                                MTSDR.MobEqipmentMeasurements[0] = DataModels.Sdrns.MeasurementType.MonitoringStations;
                                MTSDR.MobEqipmentMeasurements[1] = DataModels.Sdrns.MeasurementType.BandwidthMeas;
                                MTSDR.MobEqipmentMeasurements[2] = DataModels.Sdrns.MeasurementType.Level;
                                if (task.MeasOther.SwNumber != null) { MTSDR.ScanPerTaskNumber = task.MeasOther.SwNumber.GetValueOrDefault(); }
                                if (task.StationsForMeasurements != null)
                                {
                                    MTSDR.Stations = new DataModels.Sdrns.Device.MeasuredStation[task.StationsForMeasurements.Count()];
                                    if (task.MeasDtParam.TypeMeasurements == MeasurementType.MonitoringStations)
                                    { // 21_02_2018 в данном случае мы передаем станции  исключительно для системы мониторинга станций т.е. один таск на месяц Надо проверить.
                                        if (task.StationsForMeasurements != null)
                                        {
                                            ///MTSDR.StationsForMeasurements = task.StationsForMeasurements;
                                            // далее сформируем переменную GlobalSID 
                                            for (int i = 0; i < task.StationsForMeasurements.Count(); i++)
                                            {
                                                MTSDR.Stations[i] = new DataModels.Sdrns.Device.MeasuredStation();
                                                var stationI = MTSDR.Stations[i];
                                                string CodeOwener = "0";
                                                stationI.Owner = new DataModels.Sdrns.Device.StationOwner();
                                                var station = task.StationsForMeasurements[i];
                                                if (station.Owner != null)
                                                {
                                                    var owner = stationI.Owner;
                                                    owner.Address = station.Owner.Addres;
                                                    owner.Code = station.Owner.Code;
                                                    owner.Id = station.Owner.Id;
                                                    owner.OKPO = station.Owner.OKPO;
                                                    owner.OwnerName = station.Owner.OwnerName;
                                                    owner.Zip = station.Owner.Zip;


                                                    if (owner.OKPO == "14333937") { CodeOwener = "1"; };
                                                    if (owner.OKPO == "22859846") { CodeOwener = "6"; };
                                                    if (owner.OKPO == "21673832") { CodeOwener = "3"; };
                                                    if (owner.OKPO == "37815221") { CodeOwener = "7"; };
                                                }
                                                stationI.GlobalSid = "255 " + CodeOwener + " 00000 " + string.Format("{0:00000}", station.IdStation);
                                                station.GlobalSID = stationI.GlobalSid;

                                                stationI.OwnerGlobalSid = task.StationsForMeasurements[i].GlobalSID;//работать с таблицей (доп. создасть в БД по GlobalSID и Standard)
                                                                                                                    //
                                                stationI.License = new DataModels.Sdrns.Device.StationLicenseInfo();
                                                if (station.LicenseParameter != null)
                                                {
                                                    stationI.License.CloseDate = station.LicenseParameter.CloseDate;
                                                    stationI.License.EndDate = station.LicenseParameter.EndDate;
                                                    stationI.License.IcsmId = station.LicenseParameter.Id;
                                                    stationI.License.Name = station.LicenseParameter.DozvilName;
                                                    stationI.License.StartDate = station.LicenseParameter.StartDate;
                                                }

                                                stationI.Site = new DataModels.Sdrns.Device.StationSite();
                                                if (station.Site != null)
                                                {
                                                    stationI.Site.Adress = station.Site.Adress;
                                                    stationI.Site.Lat = station.Site.Lat;
                                                    stationI.Site.Lon = station.Site.Lon;
                                                    stationI.Site.Region = station.Site.Region;
                                                }
                                                stationI.Standard = station.Standart;
                                                stationI.StationId = station.IdStation.ToString();
                                                stationI.Status = station.Status;


                                                if (station.Sectors != null)
                                                {
                                                    stationI.Sectors = new DataModels.Sdrns.Device.StationSector[station.Sectors.Length];
                                                    for (int j = 0; j < station.Sectors.Length; j++)
                                                    {
                                                        var sector = station.Sectors[j];
                                                        stationI.Sectors[j] = new DataModels.Sdrns.Device.StationSector();
                                                        var statSector = stationI.Sectors[j];
                                                        statSector.AGL = sector.AGL;
                                                        statSector.Azimuth = sector.Azimut;

                                                        if (sector.MaskBW != null)
                                                        {
                                                            statSector.BWMask = new DataModels.Sdrns.Device.ElementsMask[sector.MaskBW.Length];
                                                            for (int k = 0; k < sector.MaskBW.Length; k++)
                                                            {
                                                                statSector.BWMask[k] = new DataModels.Sdrns.Device.ElementsMask();
                                                                statSector.BWMask[k].BW_kHz = sector.MaskBW[k].BW;
                                                                statSector.BWMask[k].Level_dB = sector.MaskBW[k].level;
                                                            }
                                                        }
                                                        statSector.BW_kHz = sector.BW;
                                                        statSector.ClassEmission = sector.ClassEmission;
                                                        statSector.EIRP_dBm = sector.EIRP;

                                                        if (sector.Frequencies != null)
                                                        {
                                                            statSector.Frequencies = new DataModels.Sdrns.Device.SectorFrequency[sector.Frequencies.Length];
                                                            for (int k = 0; k < sector.Frequencies.Length; k++)
                                                            {
                                                                statSector.Frequencies[k] = new DataModels.Sdrns.Device.SectorFrequency();
                                                                statSector.Frequencies[k].ChannelNumber = sector.Frequencies[k].ChannalNumber;
                                                                statSector.Frequencies[k].Frequency_MHz = sector.Frequencies[k].Frequency;
                                                                statSector.Frequencies[k].Id = sector.Frequencies[k].Id;
                                                                statSector.Frequencies[k].PlanId = sector.Frequencies[k].IdPlan;
                                                            }
                                                        }
                                                        statSector.SectorId = sector.IdSector.ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            ListMTSDR.Add(MTSDR);
                        }
                    }
                }
            }
            return ListMTSDR;
        }

      
    }
}

