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
    public class LoadMeasTask
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;



        public LoadMeasTask(ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._environment = environment;
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }


        public List<MeasTask> ShortReadTask(int id)
        {
            var listMeasTask = new List<MeasTask>();
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
                var measTask = new MeasTask();
                var resultMeasTask = false;
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
                    timeParamList.PerStart = readerMeasTask.GetValue(c => c.PerStart);
                    timeParamList.PerStop = readerMeasTask.GetValue(c => c.PerStop);
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
                        var resultMeasStation = false;
                        while (readerMeasStation.Read())
                        {
                            var measStation = new MeasStation();
                            measStation.StationId = new MeasStationIdentifier();
                            if (readerMeasStation.GetValue(c => c.StationId) != null) measStation.StationId.Value = readerMeasStation.GetValue(c => c.StationId).Value;
                            measStation.StationType = readerMeasStation.GetValue(c => c.StationType);
                            measStations.Add(measStation);
                        }
                        return resultMeasStation;
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
                    builderMeasstation.Where(c => c.MeasTaskId, ConditionOperator.Equal, readerMeasTask.GetValue(c => c.Id));
                    queryExecuter.Fetch(builderMeasDtParam, readerMeasDtParam =>
                    {
                        var resultMeasDtParam = false;
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
                        return resultMeasDtParam;
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
                        var resultMeasFreqParam = false;
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
                                var resultMeasFreq = false;
                                while (readerMeasFreq.Read())
                                {
                                    var measFreq = new MeasFreq();
                                    if (readerMeasFreq.GetValue(c => c.Freq) != null)
                                    {
                                        measFreq.Freq = readerMeasFreq.GetValue(c => c.Freq).Value;
                                        listmeasFreq.Add(measFreq);
                                    }
                                }
                                return resultMeasFreq;
                            });
                            freqParam.MeasFreqs = listmeasFreq.ToArray();
                            measTask.MeasFreqParam = freqParam;

                        }
                        return resultMeasFreqParam;
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
                        var resultmeasLocParam = false;
                        while (readermeasLocParam.Read())
                        {
                            var measLocParam = new MeasLocParam();
                            measLocParam.ASL = readermeasLocParam.GetValue(c => c.Asl);
                            measLocParam.Lat = readermeasLocParam.GetValue(c => c.Lat);
                            measLocParam.Lon = readermeasLocParam.GetValue(c => c.Lon);
                            measLocParam.MaxDist = readermeasLocParam.GetValue(c => c.MaxDist);
                            measLocParams.Add(measLocParam);
                        }
                        return resultmeasLocParam;
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
                        var resultMeasOther = false;
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
                        return resultMeasOther;
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
                        var resultMeasSubTask = false;
                        while (readerMeasSubTask.Read())
                        {
                            var measSubTask = new MeasSubTask();
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
                                var resultIMeasSubTaskSta = false;
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
                                return resultIMeasSubTaskSta;
                            });
                            measSubTask.MeasSubTaskStations = listMeasSubTaskStation.ToArray();
                            listmeasSubTask.Add(measSubTask);
                        }
                        return resultMeasSubTask;
                    });

                    measTask.MeasSubTasks = listmeasSubTask.ToArray();
                    listMeasTask.Add(measTask);
                }
                return resultMeasTask;
            });
            return listMeasTask;
        }
    }
}




