using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DM = Atdi.DataModels.Sdrns.Server.Entities;
using System.Threading.Tasks;
using Atdi.DataModels.DataConstraint;
using Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Models;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnInitOnlineMeasurement", SubscriberName = "SubscriberOnInitOnlineMeasurementEvent")]
    public class OnInitOnlineMeasurementEvent : IEventSubscriber<OnInitOnlineMeasurement>
    {
        private readonly ILogger _logger;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnMessagePublisher _messagePublisher;

        public OnInitOnlineMeasurementEvent(IDataLayer<EntityDataOrm> dataLayer, ISdrnMessagePublisher messagePublisher, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._logger = logger;
            this._environment = environment;
            this._dataLayer = dataLayer;
            this._messagePublisher = messagePublisher;
        }

        public void Notify(OnInitOnlineMeasurement @event)
        {
            try
            {
                using (this._logger.StartTrace(Contexts.EventSubscriber, Categories.Notify, this))
                {
                    using (var dbScope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        this.Handle(@event.OnlineMeasId, dbScope);
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.EventSubscriber, Categories.OnInitOnlineMeasurement, e, this);
            }
        }

        private void Handle(long measId, IDataLayerScope dbScope)
        {
            var query = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                .From()
                .Where(c => c.Id, ConditionOperator.Equal, measId)
                .Select(
                    c => c.Id,
                    c => c.SENSOR.Name,
                    c => c.SENSOR.TechId,
                    c => c.StatusCode,
                    c => c.ServerToken);

            var onlineMeas = new InitOnlineMeasurementModel();

            var measExists = dbScope.Executor.ExecuteAndFetch(query, reader =>
            {
                var exists = reader.Read();
                if (exists)
                {
                    onlineMeas.Id = reader.GetValue(c => c.Id);
                    onlineMeas.SensorName = reader.GetValue(c => c.SENSOR.Name);
                    onlineMeas.SensorTechId = reader.GetValue(c => c.SENSOR.TechId);
                    onlineMeas.Status = (OnlineMeasurementStatus)reader.GetValue(c => c.StatusCode);
                    onlineMeas.ServerToken = reader.GetValue(c => c.ServerToken);
                }
                return exists;
            });

            if (!measExists)
            {
                throw new InvalidOperationException($"Not found an online measurement with ID #{measId}");
            }

            if (onlineMeas.Status != OnlineMeasurementStatus.Initiation)
            {
                throw new InvalidOperationException($"Incorrect status ({onlineMeas.Status}) to init online measurement to Sensor");
            }

            var cancelled = false;
            var note = new StringBuilder();

            if (string.IsNullOrEmpty(onlineMeas.SensorName))
            {
                cancelled = true;
                note.AppendLine("Undefined value of the Sensor Name");
            }
            if (string.IsNullOrEmpty(onlineMeas.SensorTechId))
            {
                cancelled = true;
                note.AppendLine("Undefined value of the Sensor Tech ID");
            }
            if (cancelled)
            {
                var update = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)OnlineMeasurementStatus.CanceledByServer)
                .SetValue(c => c.StatusNote, note.ToString())
                .SetValue(c => c.FinishTime, DateTimeOffset.Now)
                .Where(c => c.Id, ConditionOperator.Equal, onlineMeas.Id);

                dbScope.Executor.Execute(update);

                throw new InvalidOperationException($"Processing of a request for initiating an online measurement  with ID #{measId} was interrupted by the server due to detected incorrect conditions: {note.ToString()}");
            }

            var options = new DEV.InitOnlineMeasurementOptions()
            {
                OnlineMeasId = onlineMeas.Id
            };

            var deviceBusEnvelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.InitOnlineMeasurementMessage, DEV.InitOnlineMeasurementOptions>();
            deviceBusEnvelop.SensorName = onlineMeas.SensorName;
            deviceBusEnvelop.SensorTechId = onlineMeas.SensorTechId;
            deviceBusEnvelop.DeliveryObject = options;

            _messagePublisher.Send(deviceBusEnvelop);

            var updateQuery = _dataLayer.GetBuilder<DM.IOnlineMesurement>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)OnlineMeasurementStatus.WaitSensor)
                .Where(c => c.Id, ConditionOperator.Equal, onlineMeas.Id);

            dbScope.Executor.Execute(updateQuery);

        }
    }
}
