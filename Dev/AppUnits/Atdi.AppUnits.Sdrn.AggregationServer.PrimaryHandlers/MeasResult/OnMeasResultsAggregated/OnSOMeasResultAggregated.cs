using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Common;
using Atdi.Platform;
using Atdi.Platform.Caching;
using Atdi.DataModels.Sdrns.Device;
using DMS = Atdi.DataModels.Sdrns.Server;
using Atdi.Contracts.Api.DataBus;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers
{
    [SubscriptionEvent(EventName = "OnSOMeasResultAggregated", SubscriberName = "SOMeasResultSubscriber")]
    public class OnSOMeasResultAggregated : IEventSubscriber<SOMeasResultAggregated>
    {
        private readonly ILogger _logger;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IPublisher _publisher;
        public OnSOMeasResultAggregated(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ILogger logger, ISdrnServerEnvironment environment, IPublisher publisher)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._publisher = publisher;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
        }
        public void Notify(SOMeasResultAggregated @event)
        {
            try
            {
                this.Handle(@event.MeasResultId);
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.EventProcessing, e, (object)this);
            }
        }
        private void Handle(long measResultId)
        {
            var measResult = new MeasResults();

            // IResMeas
            long subTaskSensorId = 0;
            var builderResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().From();
            builderResMeas.Select(c => c.MeasResultSID, c => c.TimeMeas, c => c.Status, c => c.StartTime, c => c.StopTime, c => c.ScansNumber, c => c.TypeMeasurements, c => c.SUBTASK_SENSOR.Id);
            builderResMeas.Where(c => c.Id, ConditionOperator.Equal, measResultId);
            this._queryExecutor.Fetch(builderResMeas, readerResMeas =>
            {
                while (readerResMeas.Read())
                {
                    subTaskSensorId = (readerResMeas.GetValue(c => c.SUBTASK_SENSOR.Id));
                    measResult.ResultId = (readerResMeas.GetValue(c => c.MeasResultSID));
                    measResult.Status = (readerResMeas.GetValue(c => c.Status));
                    measResult.Measured = readerResMeas.GetValue(c => c.TimeMeas).GetValueOrDefault();
                    measResult.StartTime = readerResMeas.GetValue(c => c.StartTime).GetValueOrDefault();
                    measResult.StopTime = readerResMeas.GetValue(c => c.StopTime).GetValueOrDefault();
                    measResult.ScansNumber = readerResMeas.GetValue(c => c.ScansNumber).GetValueOrDefault();
                    measResult.Measurement = (Enum.TryParse<MeasurementType>(readerResMeas.GetValue(c => c.TypeMeasurements), out MeasurementType outResType)) ? outResType : MeasurementType.SpectrumOccupation;
                }
                return true;
            });

            // ILinkSubTaskSensorMasterId
            var builderResMeasTaskId = this._dataLayer.GetBuilder<MD.ILinkSubTaskSensorMasterId>().From();
            builderResMeasTaskId.Select(c => c.SubtaskSensorMasterId);
            builderResMeasTaskId.Where(c => c.SUBTASK_SENSOR.Id, ConditionOperator.Equal, subTaskSensorId);
            this._queryExecutor.Fetch(builderResMeasTaskId, readerResMeasTaskId =>
            {
                while (readerResMeasTaskId.Read())
                {
                    var subtaskSensorMasterId = readerResMeasTaskId.GetValue(c => c.SubtaskSensorMasterId);
                    if (subtaskSensorMasterId.HasValue)
                        measResult.TaskId = subtaskSensorMasterId.Value.ToString();
                }
                return true;
            });

            // IResLevels
            var freqSampleList = new List<FrequencySample>();
            var builderResMeasLevels = this._dataLayer.GetBuilder<MD.IResLevels>().From();
            builderResMeasLevels.Select(c => c.VMMaxLvl, c => c.VMinLvl, c => c.ValueLvl, c => c.ValueSpect, c => c.OccupancySpect, c => c.FreqMeas, c => c.LevelMinArr, c => c.SpectrumOccupationArr);
            builderResMeasLevels.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, measResultId);
            this._queryExecutor.Fetch(builderResMeasLevels, readerResMeasLevels =>
            {
                while (readerResMeasLevels.Read())
                {
                    var freqSample = new FrequencySample();
                    freqSample.Freq_MHz = readerResMeasLevels.GetValue(c => c.FreqMeas).GetValueOrDefault();
                    freqSample.Occupation_Pt = readerResMeasLevels.GetValue(c => c.OccupancySpect).GetValueOrDefault();
                    freqSample.LevelMax_dBm = readerResMeasLevels.GetValue(c => c.VMMaxLvl).GetValueOrDefault();
                    freqSample.LevelMin_dBm = readerResMeasLevels.GetValue(c => c.VMinLvl).GetValueOrDefault();
                    freqSample.Level_dBm = readerResMeasLevels.GetValue(c => c.ValueLvl).GetValueOrDefault();
                    freqSample.Level_dBmkVm = readerResMeasLevels.GetValue(c => c.ValueSpect).GetValueOrDefault();
                    freqSample.LevelMinArr = readerResMeasLevels.GetValue(c => c.LevelMinArr);
                    freqSample.SpectrumOccupationArr = readerResMeasLevels.GetValue(c => c.SpectrumOccupationArr);
                    freqSampleList.Add(freqSample);
                }
                return true;
            });
            if (freqSampleList.Count > 0) measResult.FrequencySamples = freqSampleList.ToArray();

            // IResLocSensorMeas
            var location = new GeoLocation();
            var builderResMeasLocation = this._dataLayer.GetBuilder<MD.IResLocSensorMeas>().From();
            builderResMeasLocation.Select(c => c.Agl, c => c.Asl, c => c.Lon, c => c.Lat);
            builderResMeasLocation.Where(c => c.RES_MEAS.Id, ConditionOperator.Equal, measResultId);
            this._queryExecutor.Fetch(builderResMeasLocation, readerResMeasLocation =>
            {
                while (readerResMeasLocation.Read())
                {
                    location.AGL = readerResMeasLocation.GetValue(c => c.Agl).GetValueOrDefault();
                    location.ASL = readerResMeasLocation.GetValue(c => c.Asl).GetValueOrDefault();
                    location.Lon = readerResMeasLocation.GetValue(c => c.Lon).GetValueOrDefault();
                    location.Lat = readerResMeasLocation.GetValue(c => c.Lat).GetValueOrDefault();
                }
                return true;
            });
            measResult.Location = location;

            var deliveryObject = new DMS.MeasResultContainer() { MeasResult = measResult };
            var envelope = this._publisher.CreateEnvelope<DMS.SendMeasResultSOToMasterServer, DMS.MeasResultContainer>();
            envelope.To = this._environment.MasterServerInstance;
            envelope.DeliveryObject = deliveryObject;
            this._publisher.Send(envelope);
        }
    }
}
