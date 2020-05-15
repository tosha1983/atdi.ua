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


namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendCommandHandler : MessageHandlerBase<DM.DeviceCommand, SendCommandMessage>
    {
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITimeService _timeService;
        private readonly ITaskStarter _taskStarter;
        private readonly ILogger _logger;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParametersByString;
        private readonly ConfigMessaging _configMessaging;
        private readonly IRepository<DM.DeviceCommand, string> _repositoryDeviceCommand;


        public SendCommandHandler(ITimeService timeService,
            IProcessingDispatcher processingDispatcher,
            IRepository<TaskParameters, string> repositoryTaskParametersBystring,
            ConfigMessaging configMessaging,
            IRepository<DM.DeviceCommand, string> repositoryDeviceCommand,
            ITaskStarter taskStarter, ILogger logger)
        {
            this._configMessaging = configMessaging;
            this._processingDispatcher = processingDispatcher;
            this._timeService = timeService;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._repositoryTaskParametersByString = repositoryTaskParametersBystring;
            this._repositoryDeviceCommand = repositoryDeviceCommand;
        }

        public override void OnHandle(IReceivedMessage<DM.DeviceCommand> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            try
            {
                if ((message.Data.Command == "StopMeasTask") || (message.Data.Command == "RunMeasTask") || (message.Data.Command == "DelMeasTask"))
                {
                    DM.DeviceCommand deviceCommand = message.Data;
                    this._repositoryDeviceCommand.Create(message.Data);
                    if (deviceCommand.CustTxt1 != null)
                    {
                        string idsTask = deviceCommand.CustTxt1;
                        if (!string.IsNullOrEmpty(idsTask))
                        {
                            var taskParams = this._repositoryTaskParametersByString.LoadObject(idsTask);
                            if (taskParams != null)
                            {

                                if (deviceCommand.Command == TypeMeasTask.RunMeasTask.ToString())
                                {
                                    taskParams.status = StatusTask.A.ToString();
                                }
                                else if (deviceCommand.Command == TypeMeasTask.DelMeasTask.ToString())
                                {
                                    taskParams.status = StatusTask.Z.ToString();
                                }
                                else if (deviceCommand.Command == TypeMeasTask.StopMeasTask.ToString())
                                {
                                    taskParams.status = StatusTask.F.ToString();
                                }
                                // обновление TaskParameters в БД
                                this._repositoryTaskParametersByString.Update(taskParams);

                                this._logger.Info(Contexts.ThisComponent, Categories.SendCommandHandlerHandlerStart, Events.UpdateTaskParameters);
                                this._logger.Info(Contexts.ThisComponent, Categories.SendCommandHandlerHandlerStart, $"New command '{message.Data.Command}' for '{idsTask}' accepted in work");
                            }
                        }
                    }
                }
                message.Result = MessageHandlingResult.Confirmed;
            }
            catch (Exception e)
            {
                message.Result = MessageHandlingResult.Error;
                this._logger.Error(Contexts.ThisComponent, Exceptions.UnknownErrorsInSendCommandHandler, e.Message);
            }
        }
    }
}
