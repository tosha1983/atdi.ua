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


namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging.Handlers
{
    class SendMeasTaskHandler : MessageHandlerBase<DM.MeasTask, SendMeasTaskMessage>
    {
        private readonly ILogger _logger;
        private readonly IProcessingDispatcher _processingDispatcher;
        private readonly ITaskStarter _taskStarter;
        private readonly IRepository<DM.MeasTask,int?> _repositoryMeasTask;
        private readonly IRepository<TaskParameters, int?> _repositoryTaskParameters;
        private readonly IRepository<DM.Sensor, int?> _repositorySensor;
        private readonly ITimeService _timeService;
        private readonly IRepository<LastUpdate, int?> _repositoryLastUpdateByInt;


        public SendMeasTaskHandler(
           ITimeService timeService,
           IProcessingDispatcher processingDispatcher,
           IRepository<DM.MeasTask, int?> repositoryMeasTask,
           IRepository<TaskParameters, int?> repositoryTaskParameters,
           IRepository<DM.Sensor, int?> repositorySensor,
           IRepository<LastUpdate, int?> repositoryLastUpdateByInt,
           ITaskStarter taskStarter,
           ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._repositoryMeasTask = repositoryMeasTask;
            this._repositoryTaskParameters = repositoryTaskParameters;
            this._repositorySensor = repositorySensor;
            this._timeService = timeService;
            this._repositoryLastUpdateByInt = repositoryLastUpdateByInt;
        }


        public override void OnHandle(IReceivedMessage<DM.MeasTask> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            try
            {
                if ((message.Data != null) && (message.Data.SdrnServer != null) && (message.Data.SensorName != null) && (message.Data.EquipmentTechId != null))
                {
                    // здесь предварительная проверка(валидация) таска на возможность физической обработки
                    if (Validation(message.Data)) // пока заглушка
                    {

                        var lastUpdate = new LastUpdate()
                        {
                            TableName = "XBS_TASKPARAMETERS",
                            LastDateTimeUpdate = DateTime.Now,
                            Status = "N"
                        };
                        var allTablesLastUpdated = this._repositoryLastUpdateByInt.LoadAllObjects();
                        if ((allTablesLastUpdated != null) && (allTablesLastUpdated.Length > 0))
                        {
                            var listAlTables = allTablesLastUpdated.ToList();
                            var findTableProperties = listAlTables.Find(z => z.TableName == "XBS_TASKPARAMETERS");
                            if (findTableProperties != null)
                            {
                                this._repositoryLastUpdateByInt.Update(lastUpdate);
                            }
                            else
                            {
                                this._repositoryLastUpdateByInt.Create(lastUpdate);
                            }
                        }
                        else
                        {
                            this._repositoryLastUpdateByInt.Create(lastUpdate);
                        }

                        if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.SpectrumOccupation)
                        {

                            var taskParameters = message.Data.Convert();
                            var idTaskParameters = this._repositoryTaskParameters.Create(taskParameters);

                            this._logger.Info(Contexts.ThisComponent, Categories.SendMeasTaskHandlerStart, Events.CreateNewTaskParameters);

                            message.Result = MessageHandlingResult.Confirmed;

                        }
                        if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.Signaling)
                        {

                            var taskParameters = message.Data.Convert();
                            var idTaskParameters = this._repositoryTaskParameters.Create(taskParameters);

                            this._logger.Info(Contexts.ThisComponent, Categories.SendMeasTaskHandlerStart, Events.CreateNewTaskParameters);

                            message.Result = MessageHandlingResult.Confirmed;

                        }
                        if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.BandwidthMeas)
                        {

                            var taskParameters = message.Data.Convert();
                            var idTaskParameters = this._repositoryTaskParameters.Create(taskParameters);

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
                        else if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.Signaling)
                        {
                            message.Result = MessageHandlingResult.Trash;
                            throw new NotImplementedException("Not supported MeasurementType 'Signaling'");
                        }
                        else if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.BandwidthMeas)
                        {
                            message.Result = MessageHandlingResult.Trash;
                            throw new NotImplementedException("Not supported MeasurementType 'BandwidthMeas'");
                        }
                        else
                        {
                            message.Result = MessageHandlingResult.Trash;
                            throw new NotImplementedException("Not supported MeasurementType");
                        }
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

        /// <summary>
        /// Предварительная валидация measTask
        /// </summary>
        /// <param name="measTask"></param>
        /// <returns></returns>
        public bool Validation(DM.MeasTask measTask/*, DM.Sensor sensor*/)
        {
            /*
            bool isSuccessValidation = false;
            if ((measTask.SensorName == sensor.Name) && (measTask.EquipmentTechId == sensor.Equipment.TechId))
            {
                isSuccessValidation = true;
            }
            return isSuccessValidation;
            */
            return true;
        }
    }
}
