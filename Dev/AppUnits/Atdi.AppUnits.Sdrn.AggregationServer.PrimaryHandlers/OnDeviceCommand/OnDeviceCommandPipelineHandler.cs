using System;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.DataModels.Api.EventSystem;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers;
using Atdi.Contracts.Api.DataBus;
using Atdi.Common;



namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Subscribes
{
    public class OnDeviceCommandPipelineHandler : IPipelineHandler<DeviceCommandResultEvent, DeviceCommandResultEvent>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IPublisher _publisher;


        public OnDeviceCommandPipelineHandler(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, IPublisher publisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
            this._environment = environment;
            this._publisher = publisher;
        }


        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public DeviceCommandResultEvent Handle(DeviceCommandResultEvent data, IPipelineContext<DeviceCommandResultEvent, DeviceCommandResultEvent> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnDeviceCommandPipelineHandler, this))
            {
                var retEnvelope = this._publisher.CreateEnvelope<SendDeviceCommandFromAggregationToMasterServer, DeviceCommandResultEvent>();
                retEnvelope.To = this._environment.MasterServerInstance;
                long aggregationSubTaskId = -1;
                long masterSubTaskId = -1;
                var constSubtataskName = "SDRN.SubTaskSensorId.";
                var val = data.CustTxt1.Replace(constSubtataskName, "").ConvertStringToDouble();
                if (val != null)
                {
                    aggregationSubTaskId = (long)(val.Value);
                    if (aggregationSubTaskId > 0)
                    {
                        var loadMeasTask = new LoadMeasTask(this._dataLayer, this._logger);
                        masterSubTaskId = loadMeasTask.GetMasterSubTaskId(aggregationSubTaskId);
                        data.CustTxt1 = masterSubTaskId.ToString();
                    }
                }

                retEnvelope.DeliveryObject = new DeviceCommandResultEvent()
                {
                    CommandId = data.CommandId,
                    CustDate1 = data.CustDate1,
                    CustNbr1 = data.CustNbr1,
                    CustTxt1 = data.CustTxt1,
                    Status = data.Status,
                    SensorName = data.SensorName,
                    SensorTechId = data.SensorTechId
                };
                this._publisher.Send(retEnvelope);
                return retEnvelope.DeliveryObject;
            }
        }
    }
}
