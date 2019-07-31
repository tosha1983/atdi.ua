using Atdi.DataModels.Api.EventSystem;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendCommandResultDeviceBusEvent", SubscriberName = "SendCommandResultSubscriber")]
    public class SendCommandResultSubscriber : SubscriberBase<DM.DeviceCommandResult>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IStatistics _statistics;
        private readonly IStatisticCounter _messageProcessingHitsCounter;
        private readonly IStatisticCounter _sendCommandResultHitsCounter;
        private readonly IStatisticCounter _sendCommandResultErrorsCounter;

        public SendCommandResultSubscriber(
            ISdrnMessagePublisher messagePublisher, 
            IMessagesSite messagesSite, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment,
            IStatistics statistics,
            ILogger logger) 
            : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._statistics = statistics;
            if (this._statistics != null)
            {
                this._messageProcessingHitsCounter = _statistics.Counter(Monitoring.Counters.MessageProcessingHits);
                this._sendCommandResultHitsCounter = _statistics.Counter(Monitoring.Counters.SendCommandResultHits);
                this._sendCommandResultErrorsCounter = _statistics.Counter(Monitoring.Counters.SendCommandResultErrors);
            }
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.DeviceCommandResult deliveryObject, long messageId)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                this._messageProcessingHitsCounter?.Increment();
                this._sendCommandResultHitsCounter?.Increment();

                //var status = SdrnMessageHandlingStatus.Unprocessed;
                var sensorExistsInDb = false;
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        scope.BeginTran();
                        switch (deliveryObject.CommandId)
                        {
                            case "SendActivitySensor":
                                var query = this._dataLayer.GetBuilder<MD.ISensor>()
                           .From()
                           .Select(c => c.Name)
                           .Select(c => c.Id)
                           .Where(c => c.Name, ConditionOperator.Equal, sensorName)
                           .Where(c => c.TechId, ConditionOperator.Equal, sensorTechId)
                           .OrderByAsc(c => c.Id);
                                sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                                .Execute(query) == 1;

                                if (sensorExistsInDb == true)
                                {
                                    var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensor>().Update();
                                    builderUpdateSensor.Where(c => c.TechId, ConditionOperator.Equal, sensorTechId);
                                    builderUpdateSensor.Where(c => c.Name, ConditionOperator.Equal, sensorName);
                                    builderUpdateSensor.SetValue(c => c.Status, "A");
                                    builderUpdateSensor.SetValue(c => c.LastActivity, DateTime.Now);
                                    var cnt = scope.Executor
                                     .Execute(builderUpdateSensor);
                                    if (cnt == 1)
                                    {
                                        scope.Commit();
                                    }
                                }
                                //status = SdrnMessageHandlingStatus.Confirmed;
                                break;
                            case "UpdateStatusMeasTask":
                                long subTaskStationId = -1;
                                long taskIds = -1;
                                if (deliveryObject.CustTxt1 != null)
                                {
                                    if (long.TryParse(deliveryObject.CustTxt1.Replace("SDRN.SubTaskSensorId.",""), out subTaskStationId))
                                    {
                                        if (subTaskStationId > -1)
                                        {
                                            var querySubTaskSensor = this._dataLayer.GetBuilder<MD.ISubTaskSensor>()
                                                .Update()
                                                .Where(c => c.Id, ConditionOperator.Equal, subTaskStationId)
                                                .SetValue(c => c.Status, deliveryObject.Status);
                                            var updated = scope.Executor
                                            .Execute(querySubTaskSensor) == 1;
                                            

                                           var queryMeasTaskSelect = this._dataLayer.GetBuilder<MD.ISubTaskSensor>()
                                         .From()
                                         .Select(c => c.Id)
                                         .Select(c => c.Status)
                                         .Select(c => c.SUBTASK.MEAS_TASK.Id)
                                         .Where(c => c.Id, ConditionOperator.Equal, subTaskStationId);
                                            scope.Executor
                                           .Fetch(queryMeasTaskSelect, reader =>
                                           {
                                               while (reader.Read())
                                               {
                                                   taskIds = reader.GetValue(c => c.SUBTASK.MEAS_TASK.Id);
                                                   break;
                                               }
                                               return true;
                                           });


                                            int cntCompleteTask = 0;
                                            int cntAllTask = 0;
                                            var querySubTaskSensorSelect = this._dataLayer.GetBuilder<MD.ISubTaskSensor>()
                                           .From()
                                           .Select(c => c.Id)
                                           .Select(c => c.Status)
                                           .Select(c => c.SUBTASK.MEAS_TASK.Id)
                                           .Where(c => c.SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, taskIds);
                                            scope.Executor
                                           .Fetch(querySubTaskSensorSelect, reader =>
                                           {
                                               while (reader.Read())
                                               {
                                                   if (reader.GetValue(c => c.Status) == "C")
                                                   {
                                                       cntCompleteTask++;
                                                   }
                                                   cntAllTask++;
                                               }
                                               return true;
                                           });

                                            if (cntAllTask == cntCompleteTask)
                                            {
                                                var queryMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>()
                                                .Update()
                                                .Where(c => c.Id, ConditionOperator.Equal, taskIds)
                                                .SetValue(c => c.Status, deliveryObject.Status);
                                                var updatedMeasTask = scope.Executor
                                                .Execute(queryMeasTask) == 1;
                                            }

                                            scope.Commit();
                                        }
                                    }
                                }
                                //status = SdrnMessageHandlingStatus.Confirmed;
                                break;
                            case "UpdateSensorLocation":
                                long? Id = -1;
                                var queryUpdateSensorLocation = this._dataLayer.GetBuilder<MD.ISensor>()
                                .From()
                                .Select(c => c.Name)
                                .Select(c => c.Id)
                                .Where(c => c.Name, ConditionOperator.Equal, sensorName)
                                .Where(c => c.TechId, ConditionOperator.Equal, sensorTechId)
                                .OrderByAsc(c => c.Id);
                                scope.Executor
                               .Fetch(queryUpdateSensorLocation, reader =>
                           {
                               var res = reader.Read();
                               if (res)
                               {
                                   Id = reader.GetValue(c => c.Id);
                               }
                               return res;
                           });


                                if (Id > 0)
                                {
                                    var values = deliveryObject.CustTxt1.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                    if ((values != null) && (values.Length > 0))
                                    {
                                        if (values.Length == 3)
                                        {

                                            double Lon = -1;
                                            double Lat = -1;
                                            double Asl = -1;
                                            double.TryParse(values[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out Lon);
                                            double.TryParse(values[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out Lat);
                                            double.TryParse(values[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out Asl);

                                            if ((Lon != -1) && (Lat != -1) && (Asl != -1))
                                            {
                                                var queryCheck = this._dataLayer.GetBuilder<MD.ISensorLocation>()
                                               .From()
                                               .Select(c => c.Id)
                                               .Where(c => c.Lon, ConditionOperator.Equal, Lon)
                                               .Where(c => c.Lat, ConditionOperator.Equal, Lat);
                                                var cnt = scope.Executor.Execute(queryCheck);
                                                if (cnt == 0)
                                                {
                                                    var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensorLocation>().Update();
                                                    builderUpdateSensor.Where(c => c.SENSOR.Id, ConditionOperator.Equal, Id);
                                                    builderUpdateSensor.Where(c => c.Status, ConditionOperator.NotEqual, "Z");
                                                    builderUpdateSensor.SetValue(c => c.Status, "Z");
                                                    scope.Executor
                                                     .Execute(builderUpdateSensor);

                                                    var builderInsertSensor = this._dataLayer.GetBuilder<MD.ISensorLocation>().Insert();
                                                    builderInsertSensor.SetValue(c => c.SENSOR.Id, Id);
                                                    builderInsertSensor.SetValue(c => c.Lon, Lon);
                                                    builderInsertSensor.SetValue(c => c.Lat, Lat);
                                                    builderInsertSensor.SetValue(c => c.Asl, Asl);
                                                    builderInsertSensor.SetValue(c => c.DateCreated, DateTime.Now);
                                                    builderInsertSensor.SetValue(c => c.DateFrom, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1));
                                                    builderInsertSensor.SetValue(c => c.DateTo, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));
                                                    builderInsertSensor.SetValue(c => c.Status, "A");
                                                    
                                                    scope.Executor
                                                    .Execute(builderInsertSensor);

                                                }
                                            }
                                        }
                                    }

                                    scope.Commit();
                                }
                                //status = SdrnMessageHandlingStatus.Confirmed;
                                break;
                            default:
                                throw new NotImplementedException($"Handle for CommandId {deliveryObject.CommandId} not implemented");
                        }
                    }
                }
                catch (Exception e)
                {
                    this._sendCommandResultErrorsCounter?.Increment();
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    //status = SdrnMessageHandlingStatus.Error;
                }
            }
        }
    }
}
