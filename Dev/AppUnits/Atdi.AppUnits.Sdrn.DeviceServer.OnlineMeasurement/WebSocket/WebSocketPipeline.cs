using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Atdi.Platform.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;


namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket
{
    public class WebSocketPipeline
    {
        private static readonly UTF8Encoding encoder = new UTF8Encoding();
        private readonly ClientDescriptor _clientDescriptor;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;

        public WebSocketPipeline(
            ClientDescriptor clientDescriptor,
            IProcessingDispatcher processingDispatcher,
            ITaskStarter taskStarter,
            ILogger logger)
        {
            this._clientDescriptor = clientDescriptor;
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._logger = logger;
        }

        public void Handle(WebSocketMessage @event, WebSocketContext context)
        {
            try
            {
                this._logger.Verbouse(Contexts.WebSocket, Categories.Handling, $"The WebSocket is received data: Kind = '{@event.Kind}', Length = {@event.Length}");

                if (@event.Kind != WebSocketMessageKind.Text)
                {
                    return;
                    //throw new InvalidOperationException($"Unsupported WebSocket Message Kind '{@event.Kind}'");
                }

                var dataAsText = encoder.GetString(@event.Data, 0, (int)@event.Length);
                var message = JsonConvert.DeserializeObject<OnlineMeasMessage>(dataAsText);

                this._logger.Verbouse(Contexts.WebSocket, Categories.Handling, $"The WebSocket is received client message: Kind = '{message.Kind}'");

                var containerJson = message.Container as JObject; //ClientRegistrationData;
                if (containerJson == null)
                {
                    throw new InvalidOperationException("Incorrect the message data: Container is empty");
                }

                if (message.Kind == OnlineMeasMessageKind.ClientTaskRegistration)
                {
                    
                    var clientMeasTask = containerJson.ToObject<ClientMeasTaskData>();
                    if (clientMeasTask == null)
                    {
                        throw new InvalidOperationException("Incorrect the registration data: Container is invalid");
                    }
                    if (!_clientDescriptor.CheckToken(clientMeasTask.SensorToken))
                    {
                        throw new InvalidOperationException("Incorrect the registration data: token is invalid");
                    }

                    var process = _processingDispatcher.Start<OnlineMeasurementProcess>();
                    process.Publisher = new WebSocketPublisher(context);
                    process.MeasTask = clientMeasTask;
                    _clientDescriptor.Process = process;


                    var task = new ClientTaskRegistrationTask();
                    _taskStarter.Run(task, process);

                    var msgAsText = JsonConvert.SerializeObject(
                        new OnlineMeasMessage
                        {
                            Kind = OnlineMeasMessageKind.DeviceServerParameters,
                            Container = process.Parameters
                        });

                    var data = encoder.GetBytes(msgAsText);
                    context.SendMessage(
                        new WebSocketMessage
                        {
                            Kind = WebSocketMessageKind.Text,
                            Data = data,
                            Length = (ulong)data.Length
                        });
                }
                else if (message.Kind == OnlineMeasMessageKind.ClientReadyTakeMeasResult)
                {
                    var clientReadyData = containerJson.ToObject<ClientReadyData>();
                    if (clientReadyData == null)
                    {
                        throw new InvalidOperationException("Incorrect the ready data: Container is invalid");
                    }
                    if (!_clientDescriptor.CheckToken(clientReadyData.SensorToken))
                    {
                        throw new InvalidOperationException("Incorrect the ready data: token is invalid");
                    }

                    _clientDescriptor.Process.ReadyData = clientReadyData;

                    var task = new ClientReadyTakeMeasResultTask();
                    _clientDescriptor.TokenSource = new CancellationTokenSource();
                    _clientDescriptor.AsyncTask = task;

                    // задачу запсукаем в паралельном потоке - так как в этом нам нужно продолжать слушать сокет и принимать данные от клиента
                    // например он захочет отменить получения результатов измерения
                    _taskStarter.RunParallel(task, _clientDescriptor.Process, _clientDescriptor.TokenSource.Token);
                }
                else if (message.Kind == OnlineMeasMessageKind.ClientTaskCancellation)
                {
                    var data = containerJson.ToObject<ClientTaskCancellationData>();
                    if (data == null)
                    {
                        throw new InvalidOperationException("Incorrect the task cancellation data: Container is invalid");
                    }
                    if (!_clientDescriptor.CheckToken(data.SensorToken))
                    {
                        throw new InvalidOperationException("Incorrect the task cancellation data: token is invalid");
                    }

                    if (_clientDescriptor.TokenSource != null)
                    {
                        _clientDescriptor.TokenSource.Cancel();
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.WebSocket, Categories.Handling, e, this);
            }
        }

        
        public void OnConnect(WebSocketContext context)
        {
            this._logger.Verbouse(Contexts.WebSocket, Categories.Handling, $"The client is connected");
        }

        public void OnDisconnect(WebSocketContext context, DM.SensorOnlineMeasurementStatus status, string reason)
        {
            this._logger.Verbouse(Contexts.WebSocket, Categories.Handling, $"The client is disconnected");
            if (_clientDescriptor.TokenSource != null)
            {
                _clientDescriptor.TokenSource.Cancel();
            }
            if (_clientDescriptor.Dispatcher != null)
            {
                _clientDescriptor.Dispatcher.OnCloseConnect(status, reason);
            }
        }
    }
}
