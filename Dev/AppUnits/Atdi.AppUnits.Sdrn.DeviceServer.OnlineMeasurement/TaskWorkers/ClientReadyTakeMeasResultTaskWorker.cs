using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using DM = Atdi.DataModels.Sdrns.Device;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels.Sdrns.Device;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Results;



namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.TaskWorkers
{
    public class ClientReadyTakeMeasResultTaskWorker : ITaskWorker<ClientReadyTakeMeasResultTask, OnlineMeasurementProcess, PerThreadTaskWorkerLifetime>
    {
        private readonly AppServerComponentConfig _config;
        private readonly IController _controller;
        private readonly ILogger _logger;

        public ClientReadyTakeMeasResultTaskWorker(
           ILogger logger,
           IController controller,
           AppServerComponentConfig config)
        {
            this._logger = logger;
            this._controller = controller;
            this._config = config;
        }


        public void Run(ITaskContext<ClientReadyTakeMeasResultTask, OnlineMeasurementProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, Events.StartedClientReadyTakeMeasResultTaskWorker);

                if (context.Process.MeasTask.OnlineMeasType == OnlineMeasType.Level)
                {

                    var sendCommandForMeasResultTaskWorker = new SendCommandForMeasResultTaskWorker(this._config, this._controller, this._logger);
                    context.Process.SensorToken = Guid.NewGuid().ToByteArray();

                    while (true)
                    {
                        if (context.Token.IsCancellationRequested)
                        {
                            context.Cancel();
                            _logger.Info(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, Events.OnlineTaskIsCancled);
                            return;
                        }

                        DateTime currTime = DateTime.Now;
                        var measTraceParameter = ConvertToMesureTraceParameterForLevel.ConvertForLevel(context.Process.MeasTask);
                        var deviceCommand = new MesureTraceCommand(measTraceParameter);
                       
                        deviceCommand.Timeout = this._config.maximumDurationMeasLevel_ms;
                        deviceCommand.Options = CommandOption.StartImmediately;
                        sendCommandForMeasResultTaskWorker.Handle(context, deviceCommand, out DeviceServerResultLevel deviceServerResultLevel);

                        //////////////////////////////////////////////
                        // 
                        // Приостановка потока на рассчитаное время 
                        //
                        //////////////////////////////////////////////
                        var sleepTime = this._config.minimumTimeDurationLevel_ms - (DateTime.Now - currTime).TotalMilliseconds;
                        if (sleepTime >= 0)
                        {
                            Thread.Sleep((int)sleepTime);
                            _logger.Info(Contexts.ThisComponent, Categories.ClientReadyTakeMeasResultTaskWorker, Events.SleepThread.With(deviceCommand.Id, (int)sleepTime));
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException($"Type {context.Process.MeasTask.OnlineMeasType} is not supported");
                }
                
            }
            catch (Exception e)
            {
                context.Abort(e);
                _logger.Exception(Contexts.ThisComponent, Categories.ClientTaskRegistrationTaskWorker, e);
            }
        }
    }
}
