using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using System.Collections.Generic;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Handlers
{
    public class RegisterSensorHandler : ISdrnMessageHandler<MSG.Device.RegisterSensorMessage, Sensor>
    {
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ILogger _logger;

        public RegisterSensorHandler(ISdrnMessagePublisher messagePublisher, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, IEventEmitter eventEmitter, ILogger logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._eventEmitter = eventEmitter;
            this._logger = logger;
        }

        public void Handle(ISdrnIncomingEnvelope<Sensor> incomingEnvelope, ISdrnMessageHandlingResult result)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.MessageProcessing, this))
            {
                this._eventEmitter.Emit("OnEvent1", "RegisterSensorProcess");
                result.Status = SdrnMessageHandlingStatus.Rejected;
                //var sensorRegistration = false;
                //var sensorExistsInDb = false;

                ///////////////////////////////////
              /*
                    var query = this._dataLayer.GetBuilder<MD.IAntenna>()
                      .From()
                      .Select(c => c.FrequencyMHz,
                               //c => c.POS.Id,
                               c => c.Name,
                               c => c.EXT1.FullName,
                               c => c.EXT1.EXT2.FullName2,
                               c => c.TYPE.TYPE2.TYPE3.Name3,
                               //c => c.EXT1.EXTENDED.EXT1.FullName,
                               c => c.PROP1.TableName,
                               c => c.POS.PosX,
                               c => c.POS.PSS2.PosType
                             )
                      .OrderByDesc(x => x.FrequencyMHz)
                      //.Where(c => c.POS.PosX, ConditionOperator.Equal, 2.35)
                      //.Where(c => c.PROP1.PropName, ConditionOperator.Equal, "i")
                      .Where(c => c.Id, ConditionOperator.Equal, 2)
                      .OnTop(1);


                var sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                  .Execute<MD.IAntenna>(query)
                  ;
                 */
             
                //.Where(c => c.POS.PosX, ConditionOperator.Equal, 2.35);





                ///////////////////////////////////


                //try
                //{
                //    // поиск сенсора в БД
               /*
                    var query = this._dataLayer.GetBuilder<MD.ISensorAntenna>()
                        .From()
                        .Select(c=>c.Name)
                        .Select(c => c.Id)
                        .Select(c => c.TechId)
                        .Select(c => c.SENSOR.Name)
                        .Where(c => c.Name, ConditionOperator.Equal, incomingEnvelope.DeliveryObject.Name)
                        //.Where(c => c.TechId, ConditionOperator.Equal, incomingEnvelope.DeliveryObject.Equipment.TechId)
                        .OnTop(1);
                        */
            
                   var query = this._dataLayer.GetBuilder<MD.IAntennaPattern>()
                       .From()
                       .Select(c => c.Gain)
                       .Select(c => c.Id)
                       .Select(c => c.SENSORANT.SENSOR.Name)
                       .Where(c => c.Id, ConditionOperator.Equal, 182)
                       .OnTop(1);

               

               string Name = "";


               var sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                   .Fetch<MD.IAntennaPattern, string>(query, reader =>
                   {
                       while (reader.Read())
                       {
                           Name = reader.GetValue(c=>c.Gain).ToString();
                       }
                       return Name;
                   });
                
                  /*
                var query = this._dataLayer.GetBuilder<MD.ISensor>()
                      .Update()
                      .SetValue(c => c.Name, "NEW1")
                      .SetValue(c => c.TechId, "TECH1")
                      .Where(c => c.Id, ConditionOperator.Equal, 341);
                      ;

  */


                //.Execute<MD.ISensor>(query);





                //var sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
                //    .Execute<MD.ISensor>(query);

                //    if (!sensorExistsInDb)
                //    {
                //        var insertCommand = this._dataLayer.GetBuilder<MD.ISensor>()
                //            .Insert()
                //            .SetValue(c => c.Name, incomingEnvelope.DeliveryObject.Name)
                //            .SetValue(c => c.TechId, incomingEnvelope.DeliveryObject.Equipment.TechId)
                //            // тут нужно дописать код для остальных полей
                //            .SetValue(c => c.Status, "NEW");

                //        sensorRegistration = this._dataLayer.Executor<SdrnServerDataContext>()
                //            .Execute(insertCommand) == 1;
                //    }

                //    // с этого момента нужно считать что сообщение удачно обработано
                //    result.Status = SdrnMessageHandlingStatus.Confirmed;

                //    // отправка события если новый сенсор создан в БД
                //    if (sensorRegistration)
                //    {
                //        this._eventEmitter.Emit("OnNewSensorRegistartion", "RegisterSensorProccesing");
                //    }

                //}
                //catch (Exception e)
                //{
                //    this._logger.Exception(Contexts.PrimaryHandler, Categories.MessageProcessing, e, this);
                //    if (result.Status == SdrnMessageHandlingStatus.Unprocessed)
                //    {
                //        result.Status = SdrnMessageHandlingStatus.Error;
                //        result.ReasonFailure = e.ToString();
                //    }
                //}
                //finally
                //{
                //    // независимо упали мы по ошибке мы обязаны отправить ответ клиенту
                //    // формируем объект подтвержденяи регистрации
                var registrationResult = new SensorRegistrationResult
                {
                    EquipmentTechId = incomingEnvelope.SensorTechId,
                    SensorName = incomingEnvelope.SensorName,
                    SdrnServer = this._environment.ServerInstance
                };

                //    if (result.Status == SdrnMessageHandlingStatus.Error)
                //    {
                //        registrationResult.Status = "ERROR";
                //        registrationResult.Message = "Something went wrong on the server";
                //    }
                //    else if (sensorExistsInDb)
                //    {
                //        registrationResult.Status = "REJECT";
                //        registrationResult.Message = "The sensor has already been registered earlier";
                //    }
                //    else if (sensorRegistration)
                //    {
                registrationResult.Status = "OK";
                //    }
                //    else
                //    {
                //        registrationResult.Status = "ERROR";
                //        registrationResult.Message = "Something went wrong on the server during the registration of a new sensor";
                //    }

                var envelop = _messagePublisher.CreateOutgoingEnvelope<MSG.Server.SendRegistrationResultMessage, SensorRegistrationResult>();

                envelop.SensorName = incomingEnvelope.SensorName;
                envelop.SensorTechId = incomingEnvelope.SensorTechId;
                envelop.DeliveryObject = registrationResult;

                _messagePublisher.Send(envelop);
            
                // }
            }
        }
        
    }
}
