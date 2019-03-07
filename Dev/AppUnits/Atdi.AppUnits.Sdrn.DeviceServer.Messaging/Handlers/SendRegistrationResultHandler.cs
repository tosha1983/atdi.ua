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
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing.Test;
using Atdi.Platform.DependencyInjection;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendRegistrationResultHandler : MessageHandlerBase<DM.SensorRegistrationResult, SendRegistrationResultMessage>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private IServicesResolver _resolver;
        private IServicesContainer _servicesContainer;



        public SendRegistrationResultHandler(ITimeService timeService, IProcessingDispatcher processingDispatcher, ITaskStarter taskStarter, ILogger logger, IServicesResolver resolver, IServicesContainer servicesContainer)
        {
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._resolver = resolver;
            this._servicesContainer = servicesContainer;
            this._logger = logger;
        }

        public override void OnHandle(IReceivedMessage<DM.SensorRegistrationResult> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            try
            {
                this._resolver = this._servicesContainer.GetResolver<IServicesResolver>();
                var baseContext = this._resolver.Resolve(typeof(MainProcess)) as MainProcess;
                baseContext.sensorRegistrationResult = message.Data;
                var process = this._processingDispatcher.Start<BaseContext>();
                var sendRegistrationResultTask = new SendRegistrationResultTask()
                {
                    TimeStamp = _timeService.TimeStamp.Milliseconds,
                    Options = TaskExecutionOption.Default,
                };
                sendRegistrationResultTask.sensorRegistrationResult = baseContext.sensorRegistrationResult;
                _taskStarter.RunParallel(sendRegistrationResultTask, process, baseContext.contextRegisterSensorTask);
                this._logger.Info(Contexts.ThisComponent, Events.StartedSendRegistrationResultTask.With(sendRegistrationResultTask.Id));
                message.Result = MessageHandlingResult.Confirmed;
            }
            catch (Exception e)
            {
                message.Result = MessageHandlingResult.Ignore;
                this._logger.Error(Contexts.ThisComponent, Exceptions.UnknownErrorsInSendRegistrationResultHandler, e.Message);
            }
        }
    }
}

