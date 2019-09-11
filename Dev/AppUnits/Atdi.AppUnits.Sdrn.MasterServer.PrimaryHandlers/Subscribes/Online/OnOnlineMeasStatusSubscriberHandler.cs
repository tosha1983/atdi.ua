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
    public sealed class OnOnlineMeasStatusSubscriberHandler : IMessageHandler<OnlineMeasStatusSubscriberToMasterServer, OnlineMeasurementStatusData>
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public OnOnlineMeasStatusSubscriberHandler(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }


        public void Handle(IIncomingEnvelope<OnlineMeasStatusSubscriberToMasterServer, OnlineMeasurementStatusData> envelope, IHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnOnlineMeasStatusSubscriberHandler, this))
            {
                try
                {
                    var data = envelope.DeliveryObject;

                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        if (data.Status == SensorOnlineMeasurementStatus.CanceledByClient || data.Status == SensorOnlineMeasurementStatus.CanceledBySensor)
                        {
                            var update = this._dataLayer.GetBuilder<MD.IOnlineMesurement>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, data.OnlineMeasId)
                                .Where(c => c.StatusCode, ConditionOperator.Equal, (byte)4) // SonsorReady 
                                .SetValue(c => c.StatusCode, (byte)data.Status)
                                .SetValue(c => c.StatusNote, $"{data.Status}: {data.Note}")
                                .SetValue(c => c.FinishTime, DateTimeOffset.Now);


                            scope.Executor.Execute(update);
                        }
                        else if (data.Status == SensorOnlineMeasurementStatus.DeniedBySensor)
                        {
                            var update = this._dataLayer.GetBuilder<MD.IOnlineMesurement>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, data.OnlineMeasId)
                                .SetValue(c => c.StatusCode, (byte)data.Status)
                                .SetValue(c => c.StatusNote, $"{data.Status}: {data.Note}")
                                .SetValue(c => c.FinishTime, DateTimeOffset.Now);

                            scope.Executor.Execute(update);
                        }
                        else if (data.Status == SensorOnlineMeasurementStatus.SonsorReady)
                        {
                            var update = this._dataLayer.GetBuilder<MD.IOnlineMesurement>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, data.OnlineMeasId)
                                .Where(c => c.StatusCode, ConditionOperator.NotEqual, (byte)4)
                                .SetValue(c => c.StatusCode, (byte)data.Status)
                                .SetValue(c => c.StatusNote, $"{data.Status}: {data.Note}")
                                .SetValue(c => c.StartTime, DateTimeOffset.Now);

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
