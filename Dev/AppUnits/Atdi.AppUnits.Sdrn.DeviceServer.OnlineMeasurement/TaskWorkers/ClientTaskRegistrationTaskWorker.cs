using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Results;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.TaskWorkers
{
    public class ClientTaskRegistrationTaskWorker : ITaskWorker<ClientTaskRegistrationTask, OnlineMeasurementProcess, PerThreadTaskWorkerLifetime>
    {
        private readonly AppServerComponentConfig _config;
        private readonly IController _controller;
        private readonly ILogger _logger;

        public ClientTaskRegistrationTaskWorker(AppServerComponentConfig config, IController controller, ILogger logger)
        {
            this._config = config;
            this._controller = controller;
            this._logger = logger;
        }

        public void Run(ITaskContext<ClientTaskRegistrationTask, OnlineMeasurementProcess> context)
        {
            try
            {
                _logger.Verbouse(Contexts.ThisComponent, Categories.ClientTaskRegistrationTaskWorker, Events.StartedClientTaskRegistrationTaskWorker);

                if (context.Process.MeasTask.OnlineMeasType == OnlineMeasType.Level)
                {
                    var sendCommandForRegistrationTaskWorker = new SendCommandForRegistrationTaskWorker(this._config, this._controller, this._logger);
                    var measTraceParameter = ConvertToMesureTraceParameterForLevel.ConvertForLevel(context.Process.MeasTask);
                    var deviceCommand = new MesureTraceCommand(measTraceParameter);
                    deviceCommand.Timeout = this._config.maximumDurationMeasLevel_ms;
                    deviceCommand.Options = CommandOption.StartImmediately;
                    sendCommandForRegistrationTaskWorker.Handle(context, deviceCommand, out DeviceServerParametersDataLevel deviceServerParametersDataLevel);
                }
                else
                {
                    throw new NotImplementedException($"Type {context.Process.MeasTask.OnlineMeasType} is not supported");
                }
                context.Finish();

                _logger.Verbouse(Contexts.ThisComponent, Categories.ClientTaskRegistrationTaskWorker, Events.FinishClientTaskRegistrationTaskWorker);
            }
            catch (Exception e)
            {
                context.Abort(e);
                _logger.Exception(Contexts.ThisComponent, Categories.ClientTaskRegistrationTaskWorker, e);
            }
        }
    }
}
