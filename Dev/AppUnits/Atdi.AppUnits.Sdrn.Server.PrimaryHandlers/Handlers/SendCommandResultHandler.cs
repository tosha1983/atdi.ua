
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using Atdi.Modules.Sdrn.Server.Events;
using Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
{
    public class SendCommandResultHandler : ISdrnMessageHandler<MSG.Device.SendCommandResultMessage, DeviceCommandResult>
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public SendCommandResultHandler(
            ISdrnMessagePublisher messagePublisher, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment, 
            IEventEmitter eventEmitter, ILogger logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }

        public void Handle(ISdrnIncomingEnvelope<DeviceCommandResult> incomingEnvelope, ISdrnMessageHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
            {
                result.Status = SdrnMessageHandlingStatus.Trash;
                var sensorExistsInDb = false;
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                try
                {
                    queryExecuter.BeginTransaction();
                    switch (incomingEnvelope.DeliveryObject.CommandId)
                    {
                        case "SendActivitySensor":
                            var query = this._dataLayer.GetBuilder<MD.ISensor>()
                       .From()
                       .Select(c => c.Name)
                       .Select(c => c.Id)
                       .Where(c => c.Name, ConditionOperator.Equal, incomingEnvelope.SensorName)
                       .Where(c => c.TechId, ConditionOperator.Equal, incomingEnvelope.SensorTechId)
                       .OrderByAsc(c => c.Id);
                            sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                            .Execute(query) == 1;

                            if (sensorExistsInDb == true)
                            {
                                var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensor>().Update();
                                builderUpdateSensor.Where(c => c.TechId, ConditionOperator.Equal, incomingEnvelope.SensorTechId);
                                builderUpdateSensor.Where(c => c.Name, ConditionOperator.Equal, incomingEnvelope.SensorName);
                                builderUpdateSensor.SetValue(c => c.Status, "A");
                                builderUpdateSensor.SetValue(c => c.LastActivity, DateTime.Now);
                                var cnt = queryExecuter
                                 .Execute(builderUpdateSensor);
                                if (cnt == 1)
                                {
                                    queryExecuter.CommitTransaction();
                                }
                            }
                            result.Status = SdrnMessageHandlingStatus.Confirmed;
                            break;
                        case "UpdateStatusMeasTask":
                            var queryMeasTask = this._dataLayer.GetBuilder<MD.IMeasTask>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, (int?)incomingEnvelope.DeliveryObject.CustNbr1)
                                .SetValue(c => c.Status, incomingEnvelope.DeliveryObject.Status);
                            var updated = queryExecuter
                            .Execute(queryMeasTask) == 1;
                            if (updated == true)
                            {
                                queryExecuter.CommitTransaction();
                                //this._eventEmitter.Emit("OnUpdateStatusMeasTask", "UpdateStatusMeasTaskProccesing");
                            }
                            result.Status = SdrnMessageHandlingStatus.Confirmed;
                            break;
                        case "UpdateSensorLocation":
                            int? Id = -1;
                            var queryUpdateSensorLocation = this._dataLayer.GetBuilder<MD.ISensor>()
                            .From()
                            .Select(c => c.Name)
                            .Select(c => c.Id)
                            .Where(c => c.Name, ConditionOperator.Equal, incomingEnvelope.SensorName)
                            .Where(c => c.TechId, ConditionOperator.Equal, incomingEnvelope.SensorTechId)
                            .OrderByAsc(c => c.Id);
                             queryExecuter
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
                                var values = incomingEnvelope.DeliveryObject.CustTxt1.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                if ((values!=null) && (values.Length>0))
                                {
                                    if (values.Length==3)
                                    {

                                        double Lon=-1;
                                        double Lat=-1;
                                        double Asl=-1;
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
                                            var cnt = queryExecuter.Execute(queryCheck);
                                            if (cnt == 0)
                                            {
                                                var builderUpdateSensor = this._dataLayer.GetBuilder<MD.ISensorLocation>().Update();
                                                builderUpdateSensor.Where(c => c.SensorId, ConditionOperator.Equal, Id);
                                                builderUpdateSensor.Where(c => c.Status, ConditionOperator.NotEqual, "Z");
                                                builderUpdateSensor.SetValue(c => c.Status, "Z");
                                                queryExecuter
                                                 .Execute(builderUpdateSensor);

                                                var builderInsertSensor = this._dataLayer.GetBuilder<MD.ISensorLocation>().Insert();
                                                builderInsertSensor.SetValue(c => c.SensorId, Id);
                                                builderInsertSensor.SetValue(c => c.Lon, Lon);
                                                builderInsertSensor.SetValue(c => c.Lat, Lat);
                                                builderInsertSensor.SetValue(c => c.Asl, Asl);
                                                builderInsertSensor.SetValue(c => c.DateCreated, DateTime.Now);
                                                builderInsertSensor.SetValue(c => c.DateFrom, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0 ,1));
                                                builderInsertSensor.SetValue(c => c.DateTo, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));
                                                builderInsertSensor.SetValue(c => c.Status, "A");
                                                builderInsertSensor.Select(c => c.Id);
                                                queryExecuter
                                                .Execute(builderInsertSensor);

                                            }
                                        }
                                    }
                                }

                                queryExecuter.CommitTransaction();
                            }
                            result.Status = SdrnMessageHandlingStatus.Confirmed;
                            break;
                        default:
                            throw new NotImplementedException($"Handle for CommandId {incomingEnvelope.DeliveryObject.CommandId} not implemented");
                    }
                }
                catch (Exception e)
                {
                    queryExecuter.RollbackTransaction();
                    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
                    if (result.Status == SdrnMessageHandlingStatus.Unprocessed)
                    {
                        result.Status = SdrnMessageHandlingStatus.Error;
                        result.ReasonFailure = e.ToString();
                    }
                }

            }
        }
    }
}


