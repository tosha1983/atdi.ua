using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Workflows;
using SdrnsServer = Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using Atdi.Platform;
using Atdi.Contracts.Api.EventSystem;


namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.Subscribes
{
    
    public sealed class OnOnlineMeasResponseDeviceHandler : IMessageHandler<OnlineMeasResponseDeviceToMasterServer, OnlineMeasurementResponseData>
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        public OnOnlineMeasResponseDeviceHandler(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }


        public void Handle(IIncomingEnvelope<OnlineMeasResponseDeviceToMasterServer, OnlineMeasurementResponseData> envelope, IHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnOnlineMeasResponseDeviceHandler, this))
            {
                try
                {
                    var response = envelope.DeliveryObject;
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        if (response.Conformed)
                        {
                            var updateOnlineMesurement = this._dataLayer.GetBuilder<MD.IOnlineMesurement>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, response.OnlineMeasId)
                                .Where(c => c.StatusCode, ConditionOperator.Equal, (byte)0)
                                .SetValue(c => c.StatusCode, response.StatusCode)
                                .SetValue(c => c.StatusNote, "")
                                .SetValue(c => c.StartTime, DateTimeOffset.Now)
                                .SetValue(c => c.SensorToken, response.Token)
                                .SetValue(c => c.WebSocketUrl, response.WebSocketUrl);
                            scope.Executor.Execute(updateOnlineMesurement);


                            var update = this._dataLayer.GetBuilder<MD.IOnlineMesurement>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, response.OnlineMeasId)
                                .Where(c => c.StatusCode, ConditionOperator.Equal, (byte)1) // WaiteSensor 
                                .SetValue(c => c.StatusCode, (byte)4) // SonsorReady 
                                .SetValue(c => c.StatusNote, $"SonsorReady: The sensor is waiting the client request")
                                .SetValue(c => c.StartTime, DateTimeOffset.Now)
                                .SetValue(c => c.SensorToken, response.Token)
                                .SetValue(c => c.WebSocketUrl, response.WebSocketUrl);

                            scope.Executor.Execute(update);
                        }
                        else
                        {
                            var update = this._dataLayer.GetBuilder<MD.IOnlineMesurement>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, response.OnlineMeasId)
                                .Where(c => c.StatusCode, ConditionOperator.Equal, (byte)1)  // WaiteSensor
                                .SetValue(c => c.StatusCode, (byte)3) //DeniedBySensor
                                .SetValue(c => c.StatusNote, $"DeniedBySensor: The device did not confirm the online measurement: {response.Message}") //DeniedBySensor
                                .SetValue(c => c.SensorToken, null)
                                .SetValue(c => c.WebSocketUrl, null);

                            scope.Executor.Execute(update);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Exception(Contexts.ThisComponent, (EventCategory)"OnInitOnlineMeasurement", e, this);
                    throw;
                }
                result.Status = MessageHandlingStatus.Confirmed;
            }
        }
    }

  
}
