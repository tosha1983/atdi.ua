using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Contracts.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server;
using Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnNewMeasTaskEvent", SubscriberName = "SubscriberOnNewMeasTaskEvent")]
    public class OnReceivedNewMeasTaskEvent : IEventSubscriber<OnMeasTaskEvent>
    {
        private readonly ILogger _logger;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public OnReceivedNewMeasTaskEvent(IDataLayer<EntityDataOrm> dataLayer, ISdrnMessagePublisher messagePublisher, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._logger = logger;
            this._environment = environment;
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
        }
        /// <summary>
        /// Отправка новых сгенерированных подтасков  в шину DeviceBus
        /// </summary>
        /// <param name="event"></param>

        public void Notify(OnMeasTaskEvent @event)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.Notify, this))
            {
                if ((@event.MeasTaskId>0) && (@event.SensorId >0) && (@event.SensorName != null) && (@event.EquipmentTechId != null))
                {
                    try
                    {
                        var loadMeasTask = new LoadMeasTask(this._dataLayer, this._logger);
                        var saveMeasTask = new SaveMeasTask(this._dataLayer, this._logger);
                        var status = loadMeasTask.GetStatusTask(@event.MeasTaskId);
                        if (status == Status.N.ToString())
                        {
                            var loadTask = loadMeasTask.ReadTask(@event.MeasTaskId);
                            var listMeasTask = saveMeasTask.CreateeasTaskSDRsApi(loadTask, @event.SensorName, this._environment.ServerInstance, @event.EquipmentTechId, @event.MeasTaskId, @event.SensorId, "New");
                            if (listMeasTask != null)
                            {
                                for (int i = 0; i < listMeasTask.Length; i++)
                                {
                                    var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendMeasTaskMessage, DEV.MeasTask>();
                                    envelop.SensorName = @event.SensorName;
                                    envelop.SensorTechId = @event.EquipmentTechId;
                                    listMeasTask[i].SensorId = (int)@event.SensorId;
                                    envelop.DeliveryObject = listMeasTask[i];
                                    _messagePublisher.Send(envelop);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.Exception(Contexts.PrimaryHandler, ex);
                    }
                }
            }
        }
    }
}
