using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.BusMessages.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.EntityOrm;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.DeviceBusHandlers
{
    class InitOnlineMeasurementHandler : MessageHandlerBase<DM.InitOnlineMeasurementOptions, InitOnlineMeasurementMessage>
    {
        private readonly MeasurementDispatcher _dispatcher;
        private readonly IBusGate _busGate;
        private readonly ILogger _logger;

        public InitOnlineMeasurementHandler(
            MeasurementDispatcher dispatcher,
            IBusGate busGate,
            ILogger logger)
        {
            this._dispatcher = dispatcher;
            this._busGate = busGate;
            this._logger = logger;
        }

        public override void OnHandle(IReceivedMessage<DM.InitOnlineMeasurementOptions> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            try
            {
                var response = _dispatcher.CaptureDevice(message.Data);
                var publisher = _busGate.CreatePublisher("SDRN.DeviceServer.OnlineMeas.Publisher");
                publisher.Send("OnlineMeasurementResponse", response);

                message.Result = MessageHandlingResult.Confirmed;
            }
            catch (Exception e)
            {
                message.Result = MessageHandlingResult.Error;
                this._logger.Exception(Contexts.ThisComponent, Categories.Handling, e, this);
            }
        }
    }
}
