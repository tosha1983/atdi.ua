﻿using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
{
    public class UpdateSensorHandler : SdrnPrimaryHandlerBase<Sensor>
    {
        private readonly IBusGate _busGate;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public UpdateSensorHandler(IBusGate busGate, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, IEventEmitter eventEmitter, ILogger logger)
            : base(DeviceBusMessages.UpdateSensorMessage.Name)
        {
            this._busGate = busGate;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }

        public override void OnHandle(ISdrnReceivedMessage<Sensor> message)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
            {
                var sensorRegistration = false;
                var sensorExistsInDb = false;

                try
                {
                    // поиск сенсора в БД
                    var query = this._dataLayer.GetBuilder<MD.ISensor>()
                        .From()
                        .Where(c => c.Name, ConditionOperator.Equal, message.Data.Name)
                        .Where(c => c.TechId, ConditionOperator.Equal, message.Data.Equipment.TechId)
                        .OnTop(1);

                    sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                        .Execute(query) == 1;

                    if (sensorExistsInDb)
                    {
                        var updateCommand = this._dataLayer.GetBuilder<MD.ISensor>()
                            .Update()
                            .SetValue(c => c.Name, message.Data.Name)
                            .SetValue(c => c.TechId, message.Data.Equipment.TechId)
                            // тут нужно дописать код для остальных полей
                            .SetValue(c => c.Status, "UPDATETD")
                            .Where(c => c.Name, ConditionOperator.Equal, message.Data.Name)
                            .Where(c => c.TechId, ConditionOperator.Equal, message.Data.Equipment.TechId);

                        sensorRegistration = this._dataLayer.Executor<SdrnServerDataContext>()
                            .Execute(updateCommand) == 1;
                    }

                    // с этого момента нужно считать что сообщение удачно обработано
                    message.Result = MessageHandlingResult.Confirmed;

                    // отправка события если новый сенсор создан в БД
                    if (sensorRegistration)
                    {
                        this._eventEmitter.Emit("OnSensorInfoUpdated", "UpdateSensorProccesing");
                    }

                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
                    if (message.Result == MessageHandlingResult.Received)
                    {
                        message.Result = MessageHandlingResult.Error;
                        message.ReasonFailure = e.ToString();
                    }
                }
                finally
                {
                    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                    // формируем объект подтвержденяи регистрации
                    var registrationResult = new SensorUpdatingResult
                    {
                        EquipmentTechId = message.DeviceSensorTechId,
                        SensorName = message.DeviceSensorName,
                        SdrnServer = this._environment.ServerInstance
                    };

                    if (message.Result == MessageHandlingResult.Error)
                    {
                        registrationResult.Status = "ERROR";
                        registrationResult.Message = "Something went wrong on the server";
                    }
                    else if (sensorExistsInDb)
                    {
                        registrationResult.Status = "REJECT";
                        registrationResult.Message = "The sensor has already been registered earlier";
                    }
                    else if (sensorRegistration)
                    {
                        registrationResult.Status = "OK";
                    }
                    else
                    {
                        registrationResult.Status = "ERROR";
                        registrationResult.Message = "Something went wrong on the server during the registration of a new sensor";
                    }

                    var publisher = this._busGate.CreatePublisher(this);
                    publisher.Send(ServerBusMessages.SendSensorUpdatingResultMessage.Name, registrationResult, message.Token.Id);
                }
            }
        }


    }
}



//using Atdi.Contracts.Api.Sdrn.MessageBus;
//using Atdi.Contracts.CoreServices.DataLayer;
//using Atdi.Contracts.CoreServices.EntityOrm;
//using Atdi.DataModels.Sdrns.Device;
//using Atdi.Platform.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
//{
//    public class UpdateSensorHandler : SdrnPrimaryHandlerBase<Sensor>
//    {
//        private readonly IDataLayer<EntityDataOrm> _dataLayer;
//        private readonly ILogger _logger;

//        public UpdateSensorHandler(ILogger logger) 
//            : base(DeviceBusMessages.UpdateSensorMessage.Name)
//        {
//            this._logger = logger;
//        }

//        public override void OnHandle(IReceivedMessage<Sensor> message)
//        {
//            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
//            {
//                try
//                {

//                }
//                catch (Exception e)
//                {
//                    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
//                }
//            }
//        }
//    }
//}