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
        private readonly IRepository<DM.MeasTask> _repositoryMeasTask;
        private readonly IRepository<TaskParameters> _repositoryTaskParameters;
        private readonly IRepository<DM.Sensor> _repositorySensor;



        public SendMeasTaskHandler(
           IProcessingDispatcher processingDispatcher,
           IRepository<DM.MeasTask> repositoryMeasTask,
           IRepository<TaskParameters> repositoryTaskParameters,
           IRepository<DM.Sensor> repositorySensor,
           ITaskStarter taskStarter,
           ILogger logger)
        {
            this._processingDispatcher = processingDispatcher;
            this._taskStarter = taskStarter;
            this._logger = logger;
            this._repositoryMeasTask = repositoryMeasTask;
            this._repositoryTaskParameters = repositoryTaskParameters;
            this._repositorySensor = repositorySensor;
        }


        public override void OnHandle(IReceivedMessage<DM.MeasTask> message)
        {
            _logger.Verbouse(Contexts.ThisComponent, Categories.Handling, Events.MessageIsBeingHandled.With(message.Token.Type));
            if ((message.Data != null) && (message.Data.SdrnServer != null) && (message.Data.SensorName != null) && (message.Data.EquipmentTechId != null))
            {
                // здесь предварительная проверка(валидация) таска на возможность физической обработки
                if (Validation(message.Data)) // пока заглушка
                {
                    if (message.Data.Measurement == DataModels.Sdrns.MeasurementType.SpectrumOccupation)
                    {
                        this._logger.Error(Contexts.ThisComponent, Categories.SendMeasTaskHandlerStart, Events.StartProcessSendMeasTask);
                        // Старт процесса MeasProcess
                        var measProcess = this._processingDispatcher.Start<MeasProcess>();
                        // пишем ссылку на входящее сообщение в свойство MeasTask процесса MeasProcess
                        measProcess.MeasTask = message.Data;
                        var soTask = new SOTask();
                        var allSensor = this._repositorySensor.LoadAllObjects();
                        if ((allSensor != null) && (allSensor.Length > 0))
                        {
                            var activeObject = allSensor[0];
                            if (activeObject != null)
                            {
                                soTask.sensorParameters = activeObject.Convert();
                            }
                        }

                        soTask.taskParameters = measProcess.MeasTask.Convert();
                        // форммрование набора параметров для передачи в контроллер и затем в адаптер
                        soTask.mesureTraceParameter = soTask.taskParameters.Convert();
                        // Сохранение объекта MeasTask в БД
                        var saveMeasTask = this._repositoryMeasTask.Create(message.Data);
                        // Сохранение объекта SensorParameters в БД
                        var idTaskParameters = this._repositoryTaskParameters.Create(soTask.taskParameters);
                        // запуск таска SOTask на выполнение
                        _taskStarter.RunParallel(soTask, measProcess);
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

        /// <summary>
        /// Предварительная валидация measTask
        /// </summary>
        /// <param name="measTask"></param>
        /// <returns></returns>
        public bool Validation(DM.MeasTask measTask)
        {
            return true;
        }
    }
}
