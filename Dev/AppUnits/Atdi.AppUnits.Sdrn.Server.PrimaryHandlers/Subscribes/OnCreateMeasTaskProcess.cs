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
    [SubscriptionEvent(EventName = "OnNewMeasTaskEvent", SubscriberName = "SubscriberMeasTaskProcess")]
    [SubscriptionEvent(EventName = "OnDelMeasTaskEvent", SubscriberName = "SubscriberMeasTaskProcess")]
    [SubscriptionEvent(EventName = "OnStopMeasTaskEvent", SubscriberName = "SubscriberMeasTaskProcess")]
    [SubscriptionEvent(EventName = "OnRunMeasTaskEvent", SubscriberName = "SubscriberMeasTaskProcess")]
    public class OnCreateMeasTaskProcess : IEventSubscriber<Event>
    {
        private readonly ILogger _logger;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;

        public OnCreateMeasTaskProcess(ISdrnMessagePublisher messagePublisher, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger)
        {
            this._logger = logger;
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
        }

        public void Notify(Event @event)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.Notify, this))
            {
                if (@event.Name == "OnNewMeasTaskEvent")
                {
                    int? idTask = null;
                    string sensorName = null;
                    string sensorTechId = null;
                    if (@event.Source!=null)
                    {
                        string[] words =  @event.Source.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if ((words != null) && (words.Length == 3))
                        {
                            idTask = int.Parse(words[0]);
                            sensorName = words[1];
                            sensorTechId = words[2];
                            if ((idTask != null) && (sensorName != null) && (sensorTechId != null))
                            {
                                var loadTask = ReadTask(idTask.Value);
                                var listMeasTask = CreateeasTaskSDRsApi(loadTask, this._environment.ServerInstance, sensorName, sensorTechId, idTask.Value, "New");
                                foreach (var item in listMeasTask)
                                {
                                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendMeasTaskMessage, DEV.MeasTask>();
                                    envelop.SensorName = sensorName;
                                    envelop.SensorTechId = sensorTechId;
                                    envelop.DeliveryObject = item;
                                    _messagePublisher.Send(envelop);
                                }
                            }
                        }
                    }
                }
                else if (@event.Name == "OnStopMeasTaskEvent")
                {
                    int? idTask = null;
                    string sensorName = null;
                    string sensorTechId = null;
                    if (@event.Source != null)
                    {
                        string[] words = @event.Source.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if ((words != null) && (words.Length == 3))
                        {
                            idTask = int.Parse(words[0]);
                            sensorName = words[1];
                            sensorTechId = words[2];
                            if ((idTask != null) && (sensorName != null) && (sensorTechId != null))
                            {
                                var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DEV.DeviceCommand>();
                                envelop.SensorName = sensorName;
                                envelop.SensorTechId = sensorTechId;
                                envelop.DeliveryObject = new DEV.DeviceCommand();
                                envelop.DeliveryObject.Command = "StopMeasTask";
                                envelop.DeliveryObject.SensorName = sensorName;
                                envelop.DeliveryObject.SdrnServer = this._environment.ServerInstance;
                                envelop.DeliveryObject.EquipmentTechId = sensorTechId;
                                envelop.DeliveryObject.CustNbr1 = idTask; 
                                _messagePublisher.Send(envelop);
                            }
                        }
                    }
                }
                else if (@event.Name == "OnRunMeasTaskEvent")
                {
                    int? idTask = null;
                    string sensorName = null;
                    string sensorTechId = null;
                    if (@event.Source != null)
                    {
                        string[] words = @event.Source.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if ((words != null) && (words.Length == 3))
                        {
                            idTask = int.Parse(words[0]);
                            sensorName = words[1];
                            sensorTechId = words[2];
                            if ((idTask != null) && (sensorName != null) && (sensorTechId != null))
                            {
                                var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendCommandMessage, DEV.DeviceCommand>();
                                envelop.SensorName = sensorName;
                                envelop.SensorTechId = sensorTechId;
                                envelop.DeliveryObject = new DEV.DeviceCommand();
                                envelop.DeliveryObject.Command = "RunMeasTask";
                                envelop.DeliveryObject.SensorName = sensorName;
                                envelop.DeliveryObject.SdrnServer = this._environment.ServerInstance;
                                envelop.DeliveryObject.EquipmentTechId = sensorTechId;
                                envelop.DeliveryObject.CustNbr1 = idTask; 
                                _messagePublisher.Send(envelop);
                            }
                        }
                    }
                }
            }
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
            List<Atdi.DataModels.Sdrns.Device.MeasTask> ListMTSDR = new List<Atdi.DataModels.Sdrns.Device.MeasTask>();
            if (task.MeasSubTasks == null) return ListMTSDR;
            foreach (MeasSubTask SubTask in task.MeasSubTasks)
            {
                if (SubTask.MeasSubTaskStations != null)
                {
                    foreach (MeasSubTaskStation SubTaskStation in SubTask.MeasSubTaskStations)
                    {
                        if ((Type == "New") || ((Type == "Stop") && ((SubTaskStation.Status == "F") || (SubTaskStation.Status == "P"))) || ((Type == "Run") && ((SubTaskStation.Status == "O") || (SubTaskStation.Status == "A"))) ||
                            ((Type == "Del") && (SubTaskStation.Status == "Z")))
                        {
                            Atdi.DataModels.Sdrns.Device.MeasTask MTSDR = new Atdi.DataModels.Sdrns.Device.MeasTask();
                            int? IdentValueTaskSDR = SaveTaskSDRToDB(SubTask.Id.Value, SubTaskStation.Id, task.Id.Value, SubTaskStation.StationId.Value);
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
                                //MTSDR.DeviceParam
                                //MTSDR.Interval_sec
                                //MTSDR.Measurement
                                //MTSDR.SOParam

                                MTSDR.ScanParameters = new DataModels.Sdrns.Device.StandardScanParameter[] { };
                                MTSDR.StartTime = SubTask.TimeStart;
                                MTSDR.StopTime = SubTask.TimeStop;
                                MTSDR.Status = SubTask.Status;
                                MTSDR.MobEqipmentMeasurements = new DataModels.Sdrns.MeasurementType[5];
                                MTSDR.MobEqipmentMeasurements[0] = DataModels.Sdrns.MeasurementType.MonitoringStations;
                                MTSDR.MobEqipmentMeasurements[1] = DataModels.Sdrns.MeasurementType.BandwidthMeas;
                                MTSDR.MobEqipmentMeasurements[4] = DataModels.Sdrns.MeasurementType.Level;
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
                                                string CodeOwener = "0";
                                                MTSDR.Stations[i].Owner = new DataModels.Sdrns.Device.StationOwner();
                                                if (task.StationsForMeasurements[i].Owner != null)
                                                {
                                                    MTSDR.Stations[i].Owner.Address = task.StationsForMeasurements[i].Owner.Addres;
                                                    MTSDR.Stations[i].Owner.Code = task.StationsForMeasurements[i].Owner.Code;
                                                    MTSDR.Stations[i].Owner.Id = task.StationsForMeasurements[i].Owner.Id;
                                                    MTSDR.Stations[i].Owner.OKPO = task.StationsForMeasurements[i].Owner.OKPO;
                                                    MTSDR.Stations[i].Owner.OwnerName = task.StationsForMeasurements[i].Owner.OwnerName;
                                                    MTSDR.Stations[i].Owner.Zip = task.StationsForMeasurements[i].Owner.Zip;


                                                    if (MTSDR.Stations[i].Owner.OKPO == "14333937") { CodeOwener = "1"; };
                                                    if (MTSDR.Stations[i].Owner.OKPO == "22859846") { CodeOwener = "6"; };
                                                    if (MTSDR.Stations[i].Owner.OKPO == "21673832") { CodeOwener = "3"; };
                                                    if (MTSDR.Stations[i].Owner.OKPO == "37815221") { CodeOwener = "7"; };
                                                }
                                                MTSDR.Stations[i].GlobalSid = "255 " + CodeOwener + " 00000 " + string.Format("{0:00000}", task.StationsForMeasurements[i].IdStation);
                                                task.StationsForMeasurements[i].GlobalSID = MTSDR.Stations[i].GlobalSid;

                                                MTSDR.Stations[i].OwnerGlobalSid = task.StationsForMeasurements[i].GlobalSID;//работать с таблицей (доп. создасть в БД по GlobalSID и Standard)
                                                                                                                             //
                                                MTSDR.Stations[i].License = new DataModels.Sdrns.Device.StationLicenseInfo();
                                                if (task.StationsForMeasurements[i].LicenseParameter != null)
                                                {
                                                    MTSDR.Stations[i].License.CloseDate = task.StationsForMeasurements[i].LicenseParameter.CloseDate;
                                                    MTSDR.Stations[i].License.EndDate = task.StationsForMeasurements[i].LicenseParameter.EndDate;
                                                    MTSDR.Stations[i].License.IcsmId = task.StationsForMeasurements[i].LicenseParameter.Id;
                                                    MTSDR.Stations[i].License.Name = task.StationsForMeasurements[i].LicenseParameter.DozvilName;
                                                    MTSDR.Stations[i].License.StartDate = task.StationsForMeasurements[i].LicenseParameter.StartDate;
                                                }

                                                MTSDR.Stations[i].Site = new DataModels.Sdrns.Device.StationSite();
                                                if (task.StationsForMeasurements[i].Site != null)
                                                {
                                                    MTSDR.Stations[i].Site.Adress = task.StationsForMeasurements[i].Site.Adress;
                                                    MTSDR.Stations[i].Site.Lat = task.StationsForMeasurements[i].Site.Lat;
                                                    MTSDR.Stations[i].Site.Lon = task.StationsForMeasurements[i].Site.Lon;
                                                    MTSDR.Stations[i].Site.Region = task.StationsForMeasurements[i].Site.Region;
                                                }
                                                MTSDR.Stations[i].Standard = task.StationsForMeasurements[i].Standart;
                                                MTSDR.Stations[i].StationId = task.StationsForMeasurements[i].IdStation.ToString();
                                                MTSDR.Stations[i].Status = task.StationsForMeasurements[i].Status;


                                                if (task.StationsForMeasurements[i].Sectors != null)
                                                {
                                                    MTSDR.Stations[i].Sectors = new DataModels.Sdrns.Device.StationSector[task.StationsForMeasurements[i].Sectors.Length];
                                                    for (int j = 0; j < task.StationsForMeasurements[i].Sectors.Length; j++)
                                                    {
                                                        MTSDR.Stations[i].Sectors[j] = new DataModels.Sdrns.Device.StationSector();
                                                        MTSDR.Stations[i].Sectors[j].AGL = task.StationsForMeasurements[i].Sectors[j].AGL;
                                                        MTSDR.Stations[i].Sectors[j].Azimuth = task.StationsForMeasurements[i].Sectors[j].Azimut;

                                                        if (task.StationsForMeasurements[i].Sectors[j].MaskBW != null)
                                                        {
                                                            MTSDR.Stations[i].Sectors[j].BWMask = new DataModels.Sdrns.Device.ElementsMask[task.StationsForMeasurements[i].Sectors[j].MaskBW.Length];
                                                            for (int k = 0; k < task.StationsForMeasurements[i].Sectors[j].MaskBW.Length; k++)
                                                            {
                                                                MTSDR.Stations[i].Sectors[j].BWMask[k] = new DataModels.Sdrns.Device.ElementsMask();
                                                                MTSDR.Stations[i].Sectors[j].BWMask[k].BW_kHz = task.StationsForMeasurements[i].Sectors[j].MaskBW[k].BW;
                                                                MTSDR.Stations[i].Sectors[j].BWMask[k].Level_dB = task.StationsForMeasurements[i].Sectors[j].MaskBW[k].level;
                                                            }
                                                        }
                                                        MTSDR.Stations[i].Sectors[j].BW_kHz = task.StationsForMeasurements[i].Sectors[j].BW;
                                                        MTSDR.Stations[i].Sectors[j].ClassEmission = task.StationsForMeasurements[i].Sectors[j].ClassEmission;
                                                        MTSDR.Stations[i].Sectors[j].EIRP_dBm = task.StationsForMeasurements[i].Sectors[j].EIRP;

                                                        if (task.StationsForMeasurements[i].Sectors[j].Frequencies != null)
                                                        {
                                                            MTSDR.Stations[i].Sectors[j].Frequencies = new DataModels.Sdrns.Device.SectorFrequency[task.StationsForMeasurements[i].Sectors[j].Frequencies.Length];
                                                            for (int k = 0; k < task.StationsForMeasurements[i].Sectors[j].Frequencies.Length; k++)
                                                            {
                                                                MTSDR.Stations[i].Sectors[j].Frequencies[k] = new DataModels.Sdrns.Device.SectorFrequency();
                                                                MTSDR.Stations[i].Sectors[j].Frequencies[k].ChannelNumber = task.StationsForMeasurements[i].Sectors[j].Frequencies[k].ChannalNumber;
                                                                MTSDR.Stations[i].Sectors[j].Frequencies[k].Frequency_MHz = task.StationsForMeasurements[i].Sectors[j].Frequencies[k].Frequency;
                                                                MTSDR.Stations[i].Sectors[j].Frequencies[k].Id = task.StationsForMeasurements[i].Sectors[j].Frequencies[k].Id;
                                                                MTSDR.Stations[i].Sectors[j].Frequencies[k].PlanId = task.StationsForMeasurements[i].Sectors[j].Frequencies[k].IdPlan;
                                                            }
                                                        }
                                                        MTSDR.Stations[i].Sectors[j].SectorId = task.StationsForMeasurements[i].Sectors[j].IdSector.ToString();
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

        public int? SaveTaskSDRToDB(int SubTaskId, int SubTaskStationId, int TaskId, int SensorId)
        {
            int? numVal = null;
            int? Num = null;
            bool isNew = false;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            try
            {
                queryExecuter.BeginTransaction();
                var builderMeasTaskSDR = this._dataLayer.GetBuilder<MD.IMeasTaskSDR>().From();
                builderMeasTaskSDR.Select(c => c.Id);
                builderMeasTaskSDR.Select(c => c.Num);
                builderMeasTaskSDR.OrderByDesc(c => c.Num);
                queryExecuter.Fetch(builderMeasTaskSDR, readerMeasTaskSDR =>
                {
                    if (readerMeasTaskSDR.Read())
                    {
                        Num = readerMeasTaskSDR.GetValue(c=>c.Num);
                    }
                    return true;
                });
                
                if (Num==null)
                {
                    Num = 0;
                }
                ++Num;

                builderMeasTaskSDR = this._dataLayer.GetBuilder<MD.IMeasTaskSDR>().From();
                builderMeasTaskSDR.Select(c => c.Id);
                builderMeasTaskSDR.Select(c => c.Num);
                builderMeasTaskSDR.Where(c=>c.MeasTaskId, ConditionOperator.Equal, TaskId);
                builderMeasTaskSDR.Where(c => c.MeasSubTaskId, ConditionOperator.Equal, SubTaskId);
                builderMeasTaskSDR.Where(c => c.MeasSubTaskStaId, ConditionOperator.Equal, SubTaskStationId);
                builderMeasTaskSDR.Where(c => c.SensorId, ConditionOperator.Equal, SensorId);
                builderMeasTaskSDR.OrderByDesc(c => c.Id);
                queryExecuter.Fetch(builderMeasTaskSDR, readerMeasTaskSDR =>
                {
                    if (readerMeasTaskSDR.Read())
                    {
                        isNew = true;
                        numVal = readerMeasTaskSDR.GetValue(c=>c.Num);
                    }
                    return true;
                });

               

                if (!isNew)
                {
                    var builderInsertMeasTaskSDR = this._dataLayer.GetBuilder<MD.IMeasTaskSDR>().Insert();
                    builderInsertMeasTaskSDR.SetValue(c => c.MeasTaskId, TaskId);
                    builderInsertMeasTaskSDR.SetValue(c => c.MeasSubTaskId, SubTaskId);
                    builderInsertMeasTaskSDR.SetValue(c => c.MeasSubTaskStaId, SubTaskStationId);
                    builderInsertMeasTaskSDR.SetValue(c => c.SensorId, SensorId);
                    builderInsertMeasTaskSDR.SetValue(c => c.Num, Num);
                    builderInsertMeasTaskSDR.Select(c => c.Id);
                    queryExecuter.ExecuteAndFetch(builderInsertMeasTaskSDR, reader =>
                    {
                        return true;
                    });
                    numVal = Num;
                    queryExecuter.CommitTransaction();
                }
            }
            catch (Exception)
            {
                queryExecuter.RollbackTransaction();
            }
            return numVal;
        }
    }
}
