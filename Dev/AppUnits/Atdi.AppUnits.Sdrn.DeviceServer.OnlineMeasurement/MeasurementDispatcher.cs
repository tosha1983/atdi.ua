using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.Sdrn.MessageBus;


namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement
{
    public class MeasurementDispatcher : IDisposable
    {

        private readonly object _capturingLocker = new object();
        private readonly IBusGate _busGate;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly AppServerComponentConfig _config;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private bool _captured;
        private Thread _socketThread;

        private ClientDescriptor _descriptor;

        public MeasurementDispatcher(
            IBusGate busGate,
            IProcessingDispatcher processingDispatcher,
            AppServerComponentConfig config,
            ITaskStarter taskStarter,
            ILogger logger)
        {
            this._busGate = busGate;
            this._processingDispatcher = processingDispatcher;
            this._config = config;
            this._taskStarter = taskStarter;
            this._logger = logger;
        }

        public DM.OnlineMeasurementResponseData CaptureDevice(DM.InitOnlineMeasurementOptions options)
        {
            var result = new DM.OnlineMeasurementResponseData
            {
                OnlineMeasId = options.OnlineMeasId
            };

            var captured = this.TryCaptureDevice();
            if (captured)
            {
                this._descriptor = new ClientDescriptor
                {
                    OnlineMeasId = options.OnlineMeasId,
                    Token = Guid.NewGuid(),
                    Dispatcher = this
                };

                this.OpenWebSocket();

                result.Conformed = true;
                result.Token = _descriptor.Token.ToByteArray();
                result.WebSocketUrl = _config.WebSocketPublicUrl;
                result.Message = "The device has opened a socket and is waiting for a client connection.";
            }
            else
            {
                result.Conformed = false;
                result.Message = "The device is busy";
            }
            
            return result;
        }

        private void OpenWebSocket()
        {
            _descriptor.Server = new WebSocketServer(
                    new WebSocketPipeline(_descriptor, _processingDispatcher, _taskStarter, _config, _logger),
                    _config.WebSocketLocalPort, _logger, _config.WebSocketBufferSize ?? 65536
                    );

            _socketThread = new Thread(() => _descriptor.Server.Run())
            {
                Name = "ATDI.DeviceServer.SocketProcessing"
            };

            _socketThread.Start();
        }

        private bool TryCaptureDevice()
        {
            if (_captured)
            {
                return false;
            }
            lock(_capturingLocker)
            {
                if (_captured)
                {
                    return false;
                }
                _captured = true;
                return true;
            }
        }

        public void OnCloseConnect(DM.SensorOnlineMeasurementStatus status, string reason)
        {
            try
            {
                var statusData = new DM.OnlineMeasurementStatusData()
                {
                    OnlineMeasId = _descriptor.OnlineMeasId,
                    Status = status,
                    Note = reason
                };

                this.FreeDevice();

                var publisher = _busGate.CreatePublisher("SDRN.DeviceServer.OnlineMeas.Publisher");
                publisher.Send("OnlineMeasurementStatus", statusData);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.WebSocket, Categories.Handling, e, this);
            }
            
        }

        private void FreeDevice()
        {
            lock (_capturingLocker)
            {
                try
                {
                    if (_descriptor != null)
                    {
                        _descriptor.Server = null;
                        _descriptor = null;
                    }
                    _socketThread = null;
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.WebSocket, Categories.Handling, e, this);
                }
                _captured = false;
            }
        }

        public void Dispose()
        {
            try
            {
                if (_socketThread != null)
                {
                    // может зависнут в ожидании соединеняи со стороны клиента а тот никогда на связь не выйдет
                    _socketThread.Abort();
                    _socketThread = null;
                }
            }
            catch (Exception)
            {
            }
            
        }
    }
}
