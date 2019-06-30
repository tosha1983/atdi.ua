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

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendCommandResultDeviceBusEvent", SubscriberName = "SendCommandResultSubscriber")]
    public class SendCommandResultSubscriber : SubscriberBase<DM.DeviceCommandResult>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        public SendCommandResultSubscriber(ISdrnMessagePublisher messagePublisher, IMessagesSite messagesSite, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ILogger logger) : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.DeviceCommandResult deliveryObject)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                var status = SdrnMessageHandlingStatus.Unprocessed;
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
                                status = SdrnMessageHandlingStatus.Confirmed;
                                break;
                            case "UpdateStatusMeasTask":
                                string taskId = "";
                                int subTaskId = -1;
                                int subTaskStationId = -1;
                                int sensorId = -1;
                                if (deliveryObject.CustTxt1 != null)
                                {
                                    string[] word = deliveryObject.CustTxt1.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                    if ((word != null) && (word.Length == 4))
                                    {
                                        taskId = word[0];
                                        subTaskId = int.Parse(word[1]);
                                        subTaskStationId = int.Parse(word[2]);
                                        sensorId = int.Parse(word[3]);
                                        if (!string.IsNullOrEmpty(taskId))
                                        {
                                            var queryMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>()
                                            .Update()
                                            .Where(c => c.Id, ConditionOperator.Equal, int.Parse(taskId))
                                            .SetValue(c => c.Status, deliveryObject.Status);
                                            var updated = scope.Executor
                                            .Execute(queryMeasTask) == 1;
                                            if (updated == true)
                                            {
                                                scope.Commit();
                                            }
                                        }
                                    }
                                }
                                status = SdrnMessageHandlingStatus.Confirmed;
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
                                                    builderUpdateSensor.Where(c => c.SensorId, ConditionOperator.Equal, Id);
                                                    builderUpdateSensor.Where(c => c.Status, ConditionOperator.NotEqual, "Z");
                                                    builderUpdateSensor.SetValue(c => c.Status, "Z");
                                                    scope.Executor
                                                     .Execute(builderUpdateSensor);

                                                    var builderInsertSensor = this._dataLayer.GetBuilder<MD.ISensorLocation>().Insert();
                                                    builderInsertSensor.SetValue(c => c.SensorId, Id);
                                                    builderInsertSensor.SetValue(c => c.Lon, Lon);
                                                    builderInsertSensor.SetValue(c => c.Lat, Lat);
                                                    builderInsertSensor.SetValue(c => c.Asl, Asl);
                                                    builderInsertSensor.SetValue(c => c.DateCreated, DateTime.Now);
                                                    builderInsertSensor.SetValue(c => c.DateFrom, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1));
                                                    builderInsertSensor.SetValue(c => c.DateTo, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));
                                                    builderInsertSensor.SetValue(c => c.Status, "A");
                                                    builderInsertSensor.Select(c => c.Id);
                                                    scope.Executor
                                                    .Execute(builderInsertSensor);

                                                }
                                            }
                                        }
                                    }

                                    scope.Commit();
                                }
                                status = SdrnMessageHandlingStatus.Confirmed;
                                break;
                            default:
                                throw new NotImplementedException($"Handle for CommandId {deliveryObject.CommandId} not implemented");
                        }
                    }
                }
                catch (Exception e)
                {

                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    status = SdrnMessageHandlingStatus.Error;
                }
            }
        }
    }
}
