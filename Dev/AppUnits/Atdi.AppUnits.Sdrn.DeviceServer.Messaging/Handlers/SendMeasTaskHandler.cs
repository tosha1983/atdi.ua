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
using Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Convertor;
using Atdi.DataModels.Sdrn.DeviceServer;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendMeasTaskHandler : MessageHandlerBase<DM.MeasTask, SendMeasTaskMessage>
    {
        private readonly ILogger _logger;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskStarter _taskStarter;
        private readonly IRepository<TaskParameters, string> _repositoryTaskParameters;
        private readonly ITimeService _timeService;
        private readonly ConfigMessaging _config;
        private readonly IRepository<DM.DeviceCommand, string> _repositoryDeviceCommand;


        public SendMeasTaskHandler(
           ITimeService timeService,
           IProcessingDispatcher processingDispatcher,
           IRepository<TaskParameters, string> repositoryTaskParameters,
           IRepository<DM.DeviceCommand, string> repositoryDeviceCommand,
           ITaskStarter taskStarter,
           ConfigMessaging config,
           ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._repositoryTaskParameters = repositoryTaskParameters;
            this._timeService = timeService;
            this._config = config;
            this._repositoryDeviceCommand = repositoryDeviceCommand;
        }


        public override void OnHandle(IReceivedMessage<DM.MeasTask> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            try
            {
                if ((message.Data != null) && (message.Data.SdrnServer != null) && (message.Data.SensorName != null) && (message.Data.EquipmentTechId != null))
                {
                    if ((message.Data.Measurement == DataModels.Sdrns.MeasurementType.SpectrumOccupation)
                        || (message.Data.Measurement == DataModels.Sdrns.MeasurementType.Signaling)
                        || (message.Data.Measurement == DataModels.Sdrns.MeasurementType.BandwidthMeas))
                    {

                        var taskParameters = message.Data.Convert(_config);
                        var idTaskParameters = this._repositoryTaskParameters.Create(taskParameters);

                        this._repositoryDeviceCommand.Create(new DM.DeviceCommand()
                        {
                            Command = "CreateMeasTask",
                            CustDate1 = DateTime.Now,
                            CustTxt1 = message.Data.Measurement.ToString()+"_"+message.Data.TaskId+"_",
                            EquipmentTechId = message.Data.EquipmentTechId,
                            SdrnServer = message.Data.SdrnServer,
                            SensorName = message.Data.SensorName
                        });

                        this._logger.Info(Contexts.ThisComponent, Categories.SendMeasTaskHandlerStart, Events.CreateNewTaskParameters);

                        message.Result = MessageHandlingResult.Confirmed;

                    }
                    else if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.Level)
                    {
                        message.Result = MessageHandlingResult.Trash;
                        throw new NotImplementedException("Not supported MeasurementType  'Level'");
                    }
                    else if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.MonitoringStations)
                    {
                        message.Result = MessageHandlingResult.Trash;
                        throw new NotImplementedException("Not supported MeasurementType 'MonitoringStations'");
                    }
                    else
                    {
                        message.Result = MessageHandlingResult.Trash;
                        throw new NotImplementedException("Not supported MeasurementType");
                    }
                }
                else
                {
                    message.Result = MessageHandlingResult.Trash;
                    this._logger.Error(Contexts.ThisComponent, Exceptions.IncorrectMessageParams);
                }
            }
            catch (Exception e)
            {
                message.Result = MessageHandlingResult.Error;
                this._logger.Error(Contexts.ThisComponent, Exceptions.UnknownErrorsInSendMeasTaskHandler, e.Message);
            }
        }
    }
}
