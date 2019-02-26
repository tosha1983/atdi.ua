﻿using System.Collections.Generic;
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
    public class LoadMeasTask 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public LoadMeasTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }


        public ShortMeasTask[] GetShortMeasTasks()
        {
            var listMeasTask = new List<ShortMeasTask>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasTasksMethod.Text);
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
                queryExecuter.Fetch(builderMeasTask, readerMeasTask =>
                {

                    while (readerMeasTask.Read())
                    {
                        var measTask = new ShortMeasTask();
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
                        measTask.OrderId = readerMeasTask.GetValue(c => c.OrderId).HasValue ? readerMeasTask.GetValue(c => c.OrderId).Value : -1;
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
                                MeasurementType typeMeasurements;
                                if (Enum.TryParse<MeasurementType>(readerMeasDtParam.GetValue(c => c.TypeMeasurements), out typeMeasurements))
                                    measTask.TypeMeasurements = typeMeasurements;
                            }
                            return true;
                        });

                        listMeasTask.Add(measTask);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listMeasTask.ToArray();
        }

        public ShortMeasTask GetShortMeasTask(int taskId)
        {
            var measTask = new ShortMeasTask();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetShortMeasTaskMethod.Text);
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
                builderMeasTask.Where(c => c.Id, ConditionOperator.Equal, taskId);
                builderMeasTask.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
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
                        measTask.OrderId = readerMeasTask.GetValue(c => c.OrderId).HasValue ? readerMeasTask.GetValue(c => c.OrderId).Value : -1;
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
                            var resultMeasDtParam = true;
                            while (readerMeasDtParam.Read())
                            {
                                MeasurementType typeMeasurements;
                                if (Enum.TryParse<MeasurementType>(readerMeasDtParam.GetValue(c => c.TypeMeasurements), out typeMeasurements))
                                    measTask.TypeMeasurements = typeMeasurements;
                            }
                            return resultMeasDtParam;
                        });
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

        public MeasTask GetMeasTaskHeader(MeasTaskIdentifier taskId)
        {
            MeasTask result = null;
            if (taskId != null)
            {
                var loadMeasTask = new LoadMeasTask(_dataLayer, _logger);
                var Res = loadMeasTask.ShortReadTask(taskId.Value);
                if (Res != null)
                {
                    if (Res.Count > 0)
                    {
                        result = Res[0];
                    }
                    else
                    {
                        result = null;
                    }
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                return null;
            }
            return result;
        }


        public List<MeasTask> ShortReadTask(int id)
        {
            var listMeasTask = new List<MeasTask>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallShortReadTaskMethod.Text);
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
                builderMeasTask.Where(c => c.Status, ConditionOperator.NotEqual, Status.Z.ToString());
                builderMeasTask.Where(c => c.Status, ConditionOperator.IsNotNull);
                queryExecuter.Fetch(builderMeasTask, readerMeasTask =>
                {
                    var measTask = new MeasTask();
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
                        listMeasTask.Add(measTask);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listMeasTask;
        }

        /*
        public StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(int taskId)
        {
            var listStationData = new List<StationDataForMeasurements>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetStationDataForMeasurementsByTaskIdMethod.Text);
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
                                builderLinkSectorFreq.Where(c => c.SECTOR.STATION.Id, ConditionOperator.Equal, readerStation.GetValue(c => c.Id));
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
                                builderLinkSectorMaskElement.Where(c => c.SECTOR.STATION.Id, ConditionOperator.Equal, readerStation.GetValue(c => c.Id));
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
        */

        public StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(int taskId)
        {
            var listStationData = new List<StationDataForMeasurements>();
            try
            {
                List<KeyValuePair<int, StationDataForMeasurements>> idStations = new List<KeyValuePair<int, StationDataForMeasurements>>();
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetStationDataForMeasurementsByTaskIdMethod.Text);
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
                        idStations.Add(new KeyValuePair<int, StationDataForMeasurements>(readerStation.GetValue(c => c.Id), measStation));
                    }
                    return true;
                });



                var listOwnerData = new List<OwnerData>();
                var idOwners = idStations.ToList().Select(c => c.Value.IdOwner);
                var listOwnerDataTemp = new List<int>();
                if ((idOwners != null))
                {
                    var arrIds = idOwners.ToArray();
                    if (arrIds.Length > 0)
                    {
                        for (int i = 0; i < arrIds.Length; i++)
                        {
                            listOwnerDataTemp.Add(arrIds[i]);
                            if (listOwnerDataTemp.Count == 1000)
                            {
                                var builderOwnerData = this._dataLayer.GetBuilder<MD.IOwnerData>().From();
                                builderOwnerData.Select(c => c.Address);
                                builderOwnerData.Select(c => c.CODE);
                                builderOwnerData.Select(c => c.Id);
                                builderOwnerData.Select(c => c.OKPO);
                                builderOwnerData.Select(c => c.OwnerName);
                                builderOwnerData.Select(c => c.ZIP);
                                builderOwnerData.Where(c => c.Id, ConditionOperator.In, listOwnerDataTemp.ToArray());
                                builderOwnerData.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderOwnerData, readerOwnerData =>
                                {
                                    while (readerOwnerData.Read())
                                    {
                                        var ownerData = new OwnerData();
                                        ownerData.Addres = readerOwnerData.GetValue(c => c.Address);
                                        ownerData.Code = readerOwnerData.GetValue(c => c.CODE);
                                        ownerData.OKPO = readerOwnerData.GetValue(c => c.OKPO);
                                        ownerData.OwnerName = readerOwnerData.GetValue(c => c.OwnerName);
                                        ownerData.Zip = readerOwnerData.GetValue(c => c.ZIP);
                                        ownerData.Id = readerOwnerData.GetValue(c => c.Id);
                                        listOwnerData.Add(ownerData);
                                    }
                                    return true;
                                });
                                listOwnerDataTemp.Clear();
                            }
                        }

                        if (listOwnerDataTemp.Count > 0)
                        {
                            var builderOwnerDataAnother = this._dataLayer.GetBuilder<MD.IOwnerData>().From();
                            builderOwnerDataAnother.Select(c => c.Address);
                            builderOwnerDataAnother.Select(c => c.CODE);
                            builderOwnerDataAnother.Select(c => c.Id);
                            builderOwnerDataAnother.Select(c => c.OKPO);
                            builderOwnerDataAnother.Select(c => c.OwnerName);
                            builderOwnerDataAnother.Select(c => c.ZIP);
                            builderOwnerDataAnother.Where(c => c.Id, ConditionOperator.In, listOwnerDataTemp.ToArray());
                            builderOwnerDataAnother.OrderByAsc(c => c.Id);
                            queryExecuter.Fetch(builderOwnerDataAnother, readerOwnerData =>
                            {
                                while (readerOwnerData.Read())
                                {
                                    var ownerData = new OwnerData();
                                    ownerData.Addres = readerOwnerData.GetValue(c => c.Address);
                                    ownerData.Code = readerOwnerData.GetValue(c => c.CODE);
                                    ownerData.OKPO = readerOwnerData.GetValue(c => c.OKPO);
                                    ownerData.OwnerName = readerOwnerData.GetValue(c => c.OwnerName);
                                    ownerData.Zip = readerOwnerData.GetValue(c => c.ZIP);
                                    ownerData.Id = readerOwnerData.GetValue(c => c.Id);
                                    listOwnerData.Add(ownerData);
                                }
                                return true;
                            });
                        }
                    }
                }


                var listSitesDataTemp = new List<int>();
                var idSites = idStations.ToList().Select(c => c.Value.IdSite);
                var listSiteStationForMeas = new List<SiteStationForMeas>();
                if ((idSites != null))
                {
                    var arrIds = idSites.ToArray();
                    if (arrIds.Length > 0)
                    {
                        for (int i = 0; i < arrIds.Length; i++)
                        {
                            listSitesDataTemp.Add(arrIds[i]);
                            if (listSitesDataTemp.Count == 1000)
                            {

                                var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().From();
                                builderStationSite.Select(c => c.Address);
                                builderStationSite.Select(c => c.Id);
                                builderStationSite.Select(c => c.Lat);
                                builderStationSite.Select(c => c.Lon);
                                builderStationSite.Select(c => c.Region);
                                builderStationSite.Where(c => c.Id, ConditionOperator.In, listSitesDataTemp.ToArray());
                                builderStationSite.OrderByAsc(c => c.Id);
                                queryExecuter.Fetch(builderStationSite, readerStationSite =>
                                {
                                    while (readerStationSite.Read())
                                    {
                                        var siteStationForMeas = new SiteStationForMeas();
                                        siteStationForMeas.Adress = readerStationSite.GetValue(c => c.Address);
                                        siteStationForMeas.Lat = readerStationSite.GetValue(c => c.Lat);
                                        siteStationForMeas.Lon = readerStationSite.GetValue(c => c.Lon);
                                        siteStationForMeas.Region = readerStationSite.GetValue(c => c.Region);
                                        siteStationForMeas.Id = readerStationSite.GetValue(c => c.Id);
                                        listSiteStationForMeas.Add(siteStationForMeas);
                                    }
                                    return true;
                                });
                                listSitesDataTemp.Clear();
                            }
                        }
                    }
                }

                if (listSitesDataTemp.Count > 0)
                {
                    var builderStationSite = this._dataLayer.GetBuilder<MD.IStationSite>().From();
                    builderStationSite.Select(c => c.Address);
                    builderStationSite.Select(c => c.Id);
                    builderStationSite.Select(c => c.Lat);
                    builderStationSite.Select(c => c.Lon);
                    builderStationSite.Select(c => c.Region);
                    builderStationSite.Where(c => c.Id, ConditionOperator.In, listSitesDataTemp.ToArray());
                    builderStationSite.OrderByAsc(c => c.Id);
                    queryExecuter.Fetch(builderStationSite, readerStationSite =>
                    {
                        while (readerStationSite.Read())
                        {
                            var siteStationForMeas = new SiteStationForMeas();
                            siteStationForMeas.Adress = readerStationSite.GetValue(c => c.Address);
                            siteStationForMeas.Lat = readerStationSite.GetValue(c => c.Lat);
                            siteStationForMeas.Lon = readerStationSite.GetValue(c => c.Lon);
                            siteStationForMeas.Region = readerStationSite.GetValue(c => c.Region);
                            siteStationForMeas.Id = readerStationSite.GetValue(c => c.Id);
                            listSiteStationForMeas.Add(siteStationForMeas);
                        }
                        return true;
                    });
                }


                /*
                var listSectorDataTemp = new List<int?>();
                var idSectors = idStations.ToList().Select(c => c.Key);
                List<SectorStationForMeas> listSector = new List<SectorStationForMeas>();
                if ((idSectors != null))
                {
                    var arrIds = idSectors.ToArray();
                    if (arrIds.Length > 0)
                    {
                        for (int i = 0; i < arrIds.Length; i++)
                        {
                            listSectorDataTemp.Add(arrIds[i]);
                            if (listSectorDataTemp.Count == 1000)
                            {
                                var builderISector = this._dataLayer.GetBuilder<MD.ISector>().From();
                                builderISector.Select(c => c.Agl);
                                builderISector.Select(c => c.Azimut);
                                builderISector.Select(c => c.Bw);
                                builderISector.Select(c => c.ClassEmission);
                                builderISector.Select(c => c.Eirp);
                                builderISector.Select(c => c.Id);
                                builderISector.Select(c => c.SectorId);
                                builderISector.Select(c => c.StationId);
                                builderISector.Where(c => c.StationId, ConditionOperator.In, listSectorDataTemp.ToArray());
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
                                listSectorDataTemp.Clear();
                            }
                        }
                    }
                }


                if (listSectorDataTemp.Count > 0)
                {
                    var builderISector = this._dataLayer.GetBuilder<MD.ISector>().From();
                    builderISector.Select(c => c.Agl);
                    builderISector.Select(c => c.Azimut);
                    builderISector.Select(c => c.Bw);
                    builderISector.Select(c => c.ClassEmission);
                    builderISector.Select(c => c.Eirp);
                    builderISector.Select(c => c.Id);
                    builderISector.Select(c => c.SectorId);
                    builderISector.Select(c => c.StationId);
                    builderISector.Where(c => c.StationId, ConditionOperator.In, listSectorDataTemp.ToArray());
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
                }
                */


                foreach (var item in idStations)
                {

                    var findIdOwner = listOwnerData.Find(c => c.Id == item.Value.IdOwner);
                    if (findIdOwner != null)
                    {
                        item.Value.Owner = findIdOwner;
                    }

                    var findIdSite = listSiteStationForMeas.Find(c => c.Id == item.Value.IdSite);
                    if (findIdSite != null)
                    {
                        item.Value.Site = findIdSite;
                    }

                    listStationData.Add(item.Value);
                }

            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listStationData.ToArray();
        }

    }
}



