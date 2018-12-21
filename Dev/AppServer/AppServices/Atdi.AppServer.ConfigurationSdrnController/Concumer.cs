﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Atdi.DataModels.Sdrns.Device;
using Castle.Windsor;
using RabbitMQ.Client;
using Atdi.Modules.Sdrn.MessageBus;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.SDNRS.AppServer.BusManager;

namespace Atdi.AppServer.ConfigurationSdrnController
{
    public class Concumer
    {
        private ILogger _logger { get; set; }
        public void HandleMessage(RabbitMQ.Client.IModel channel, RabbitMQ.Client.Events.BasicDeliverEventArgs message, IWindsorContainer container, string StartName, string Exchangepoint, IConnection connection, string ExchangePointFromServer, string StartNameQueueDevice, ILogger logger)
        {
            this._logger = logger;
            var messageType = message.BasicProperties.Type;
            if (messageType == null)
            {
                return;
            }

            var messageResponse = new Message
            {
                Id = message.BasicProperties.MessageId,
                Type = message.BasicProperties.Type,
                ContentType = message.BasicProperties.ContentType,
                ContentEncoding = message.BasicProperties.ContentEncoding,
                CorrelationId = message.BasicProperties.CorrelationId,
                Headers = message.BasicProperties.Headers,
                Body = message.Body
            };

            var sdrnServer = Encoding.UTF8.GetString((byte[])message.BasicProperties.Headers["SdrnServer"]);
            var sensorName = Encoding.UTF8.GetString((byte[])message.BasicProperties.Headers["SensorName"]);
            var techId = Encoding.UTF8.GetString((byte[])message.BasicProperties.Headers["SensorTechId"]);
            if (string.IsNullOrEmpty(messageType))   messageType = Encoding.UTF8.GetString((byte[])message.BasicProperties.Headers["MessageType"]);

            var realObjectFromBody = UTF8Encoding.UTF8.GetString(message.Body);
            var result = false;
            try
            {
                var routingKey = $"{StartName}.[{sensorName}].[{techId}]";
                switch (messageType)
                {
                    case "RegisterSensor":
                        if (ConfigurationRabbitOptions.listRabbitOptions.ToList().Find(t => t.Value.NameSensor == sensorName && t.Value.TechId == techId).Key == null)
                        {
                            var channel_new = connection.CreateModel();
                            channel.ExchangeDeclare(exchange: ExchangePointFromServer + ".[" + ConfigurationRabbitOptions.apiVersion + "]", type: "direct", durable: true);
                            var queueName_new = $"{StartNameQueueDevice}.[{sensorName}].[{techId}].[{ConfigurationRabbitOptions.apiVersion}]";
                            var routingKey_new = $"{StartNameQueueDevice}.[{sensorName}].[{techId}]";
                            ConfigurationRabbitOptions.listRabbitOptions.Add(channel_new, new RabbitOptions(StartNameQueueDevice, routingKey, queueName_new, sensorName, techId));
                            ConfigurationRabbitOptions.QueueDeclareDevice(StartNameQueueDevice, sensorName, techId, channel_new, ExchangePointFromServer + ".[" + ConfigurationRabbitOptions.apiVersion + "]");
                        }
                        MessageObject objectRes = UnPackObject(messageResponse);
                        if (objectRes.Object is Atdi.DataModels.Sdrns.Device.Sensor)
                        {
                            var dataRegisterSensor = objectRes.Object as Atdi.DataModels.Sdrns.Device.Sensor;
                            Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor handler = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor>();
                            var queueName = routingKey + $".[{ConfigurationRabbitOptions.apiVersion}]";
                            channel.QueueDeclare(
                                  queue: queueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
                            channel.QueueBind(queueName, Exchangepoint, routingKey);
                            Atdi.DataModels.Sdrns.Device.SensorRegistrationResult deviceResult = new Atdi.DataModels.Sdrns.Device.SensorRegistrationResult();
                            deviceResult.SensorName = dataRegisterSensor.Name;
                            deviceResult.EquipmentTechId = dataRegisterSensor.Equipment.TechId;
                            deviceResult.SdrnServer = sdrnServer;
                            if (handler.IsFindSensorInDB(dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId) == false)
                            {
                                try
                                {
                                    if (handler.CreateNewObjectSensor(dataRegisterSensor, ConfigurationRabbitOptions.apiVersion))
                                    {
                                        deviceResult.Status = "Success";
                                        deviceResult.Message = string.Format("Confirm success registration sensor Name = {0}, TechId = {1}", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId);
                                        PublishMessage<SensorRegistrationResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, deviceResult, message.BasicProperties.CorrelationId);
                                    }
                                    else
                                    {
                                        deviceResult.Status = "Fault";
                                        deviceResult.Message = string.Format("Error registration sensor Name = {0}, TechId = {1}", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId);
                                        PublishMessage<SensorRegistrationResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, deviceResult, message.BasicProperties.CorrelationId);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    deviceResult.Status = "Fault";
                                    deviceResult.Message = string.Format("Error registration sensor Name = {0}, TechId = {1}: {2}", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId, ex.Message);
                                    PublishMessage<SensorRegistrationResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, deviceResult, message.BasicProperties.CorrelationId);
                                }
                            }
                            else
                            {
                                deviceResult.Status = "Fault";
                                deviceResult.Message = string.Format("Error registration sensor Name = {0}, TechId = {1} (Duplicate) ", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId);
                                PublishMessage<SensorRegistrationResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, deviceResult, message.BasicProperties.CorrelationId);
                            }
                        }
                        else if (objectRes.Object is Atdi.AppServer.Contracts.Sdrns.Sensor)
                        {
                            var dataRegisterSensor = objectRes.Object as Atdi.AppServer.Contracts.Sdrns.Sensor;
                            Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor handler = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor>();
                            var queueName = routingKey + $".[{ConfigurationRabbitOptions.apiVersion}]";
                            channel.QueueDeclare(
                                  queue: queueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
                            channel.QueueBind(queueName, Exchangepoint, routingKey);
                            Atdi.DataModels.Sdrns.Device.SensorRegistrationResult deviceResult = new Atdi.DataModels.Sdrns.Device.SensorRegistrationResult();
                            deviceResult.SensorName = dataRegisterSensor.Name;
                            deviceResult.EquipmentTechId = dataRegisterSensor.Equipment.TechId;
                            deviceResult.SdrnServer = sdrnServer;
                            if (handler.IsFindSensorInDB(dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId) == false)
                            {
                                try
                                {
                                    if (handler.CreateNewObjectSensor(dataRegisterSensor))
                                    {
                                        deviceResult.Status = "Success";
                                        deviceResult.Message = string.Format("Confirm success registration sensor Name = {0}, TechId = {1}", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId);
                                        PublishMessage<SensorRegistrationResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, deviceResult, message.BasicProperties.CorrelationId);
                                    }
                                    else
                                    {
                                        deviceResult.Status = "Fault";
                                        deviceResult.Message = string.Format("Error registration sensor Name = {0}, TechId = {1}", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId);
                                        PublishMessage<SensorRegistrationResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, deviceResult, message.BasicProperties.CorrelationId);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    deviceResult.Status = "Fault";
                                    deviceResult.Message = string.Format("Error registration sensor Name = {0}, TechId = {1}: {2}", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId, ex.Message);
                                    PublishMessage<SensorRegistrationResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, deviceResult, message.BasicProperties.CorrelationId);
                                }
                            }
                            else
                            {
                                deviceResult.Status = "Fault";
                                deviceResult.Message = string.Format("Error registration sensor Name = {0}, TechId = {1} (Duplicate) ", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId);
                                PublishMessage<SensorRegistrationResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, deviceResult, message.BasicProperties.CorrelationId);
                            }
                        }
                        result = true;
                        break;
                    case "UpdateSensor":
                        if (ConfigurationRabbitOptions.listRabbitOptions.ToList().Find(t => t.Value.NameSensor == sensorName && t.Value.TechId == techId).Key == null)
                        {
                            var channel_new = connection.CreateModel();
                            channel.ExchangeDeclare(exchange: ExchangePointFromServer + ".[" + ConfigurationRabbitOptions.apiVersion + "]", type: "direct", durable: true);
                            var queueName_new = $"{StartNameQueueDevice}.[{sensorName}].[{techId}].[{ConfigurationRabbitOptions.apiVersion}]";
                            var routingKey_new = $"{StartNameQueueDevice}.[{sensorName}].[{techId}]";
                            ConfigurationRabbitOptions.listRabbitOptions.Add(channel_new, new RabbitOptions(StartNameQueueDevice, routingKey, queueName_new, sensorName, techId));
                            ConfigurationRabbitOptions.QueueDeclareDevice(StartNameQueueDevice, sensorName, techId, channel_new, ExchangePointFromServer + ".[" + ConfigurationRabbitOptions.apiVersion + "]");
                        }

                        MessageObject dataUpdate = UnPackObject(messageResponse);
                        var dataUpdateSensor = dataUpdate.Object as Atdi.DataModels.Sdrns.Device.Sensor;
                        
                        Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor handlerUpdate = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor>();
                        var queueNameUpdate = routingKey + $".[{ConfigurationRabbitOptions.apiVersion}]";
                        channel.QueueDeclare(
                              queue: queueNameUpdate,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
                        channel.QueueBind(queueNameUpdate, Exchangepoint, routingKey);
                        Atdi.DataModels.Sdrns.Device.SensorUpdatingResult deviceResultUpdate = new Atdi.DataModels.Sdrns.Device.SensorUpdatingResult();
                        deviceResultUpdate.SensorName = dataUpdateSensor.Name;
                        deviceResultUpdate.EquipmentTechId = dataUpdateSensor.Equipment.TechId;
                        deviceResultUpdate.SdrnServer = sdrnServer;
                            try
                            {
                                if (handlerUpdate.UpdateObjectSensor(dataUpdateSensor, ConfigurationRabbitOptions.apiVersion))
                                {
                                    deviceResultUpdate.Status = "Success";
                                    deviceResultUpdate.Message = string.Format("Confirm success updated sensor Name = {0}, TechId = {1}", dataUpdateSensor.Name, dataUpdateSensor.Equipment.TechId);
                                    PublishMessage<SensorUpdatingResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendSensorUpdatingResult", channel, deviceResultUpdate, message.BasicProperties.CorrelationId);
                                }
                                else
                                {
                                    deviceResultUpdate.Status = "Fault";
                                    deviceResultUpdate.Message = string.Format("Error updated sensor Name = {0}, TechId = {1}", dataUpdateSensor.Name, dataUpdateSensor.Equipment.TechId);
                                    PublishMessage<SensorUpdatingResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendSensorUpdatingResult", channel, deviceResultUpdate, message.BasicProperties.CorrelationId);
                                }
                            }
                            catch (Exception ex)
                            {
                                deviceResultUpdate.Status = "Fault";
                                deviceResultUpdate.Message = string.Format("Error updated sensor Name = {0}, TechId = {1}: {2}", dataUpdateSensor.Name, dataUpdateSensor.Equipment.TechId, ex.Message);
                                PublishMessage<SensorUpdatingResult>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendSensorUpdatingResult", channel, deviceResultUpdate, message.BasicProperties.CorrelationId);
                            }

                        result = true;
                        break;
                    case "SendMeasResults":
                        string Symbol = "\"";

                        string textSendCommand = "";
                        string Error = "";
                        MessageObject dataU = UnPackObject(messageResponse);
                        var data = dataU.Object as Atdi.DataModels.Sdrns.Device.MeasResults;
                        Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor handler_ = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor>();
                        int? SensorId = handler_.GetIdSensor(sensorName, techId);
                        if (SensorId != null)
                        {
                            Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassesDBGetResult DbGetRes = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassesDBGetResult>();
                            //var data = JsonConvert.DeserializeObject(UTF8Encoding.UTF8.GetString(message.Body), typeof(Atdi.DataModels.Sdrns.Device.MeasResults)) as Atdi.DataModels.Sdrns.Device.MeasResults;
                            int? ID = -1;
                            string Status_Original = data.Status;
                            Atdi.AppServer.Contracts.Sdrns.MeasurementResults msReslts = Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassConvertToSDRResults.GenerateMeasResults2_0(data);
                            msReslts.StationMeasurements = new Contracts.Sdrns.StationMeasurements();
                            msReslts.StationMeasurements.StationId = new Contracts.Sdrns.SensorIdentifier();
                            msReslts.StationMeasurements.StationId.Value = SensorId.Value;
                            if (msReslts.TypeMeasurements == Atdi.AppServer.Contracts.Sdrns.MeasurementType.SpectrumOccupation) msReslts.Status = Status_Original;
                            if (msReslts.MeasurementsResults != null)
                            {
                                if (msReslts.MeasurementsResults.Count() > 0)
                                {
                                    if (msReslts.MeasurementsResults[0] is Atdi.AppServer.Contracts.Sdrns.LevelMeasurementOnlineResult)
                                    {
                                        msReslts.Status = "O";
                                        ID = DbGetRes.SaveResultToDB(msReslts, data, data.TaskId, out Error);
                                    }
                                    else
                                    {
                                        ID = DbGetRes.SaveResultToDB(msReslts, data, data.TaskId, out Error);
                                    }
                                }
                            }
                            else
                            {
                                ID = DbGetRes.SaveResultToDB(msReslts, data, data.TaskId, out Error);
                            }
                            if ((ID > 0) && (ID != Constants.NullI))
                            {
                                Atdi.DataModels.Sdrns.Device.DeviceCommand commandResSendMeasResults = new DeviceCommand();
                                commandResSendMeasResults.Command = "SendMeasResultsConfirmed";
                                commandResSendMeasResults.CommandId = "SendCommand";
                                commandResSendMeasResults.SensorName = sensorName;
                                commandResSendMeasResults.EquipmentTechId = techId;
                                commandResSendMeasResults.SdrnServer = sdrnServer;
                                commandResSendMeasResults.CustTxt1 = "{ "+ string.Format("{0}: {1}, {2}: {3}, {4}: {5} ", "\"Status\"", "\"Success\"", "\"ResultId\"", "\""+data.ResultId+"\"", "\"Message\"", "\"\"") + " }";
                                PublishMessage<DeviceCommand>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, commandResSendMeasResults, message.BasicProperties.CorrelationId);
                            }
                            else
                            {
                                Atdi.DataModels.Sdrns.Device.DeviceCommand commandResSendMeasResults = new DeviceCommand();
                                commandResSendMeasResults.Command = "SendMeasResultsConfirmed";
                                commandResSendMeasResults.CommandId = "SendCommand";
                                commandResSendMeasResults.SensorName = sensorName;
                                commandResSendMeasResults.EquipmentTechId = techId;
                                commandResSendMeasResults.SdrnServer = sdrnServer;
                                commandResSendMeasResults.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + data.ResultId + "\"", "\"Message\"", "\""+ Error + "\"") + " }";
                                PublishMessage<DeviceCommand>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, commandResSendMeasResults, message.BasicProperties.CorrelationId);
                            }
                        }
                        else
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandResSendMeasResults = new DeviceCommand();
                            commandResSendMeasResults.Command = "SendMeasResultsConfirmed";
                            commandResSendMeasResults.CommandId = "SendCommand";
                            commandResSendMeasResults.SensorName = sensorName;
                            commandResSendMeasResults.EquipmentTechId = techId;
                            commandResSendMeasResults.SdrnServer = sdrnServer;
                            commandResSendMeasResults.CustTxt1 = "{ " + string.Format("{0}: {1}, {2}: {3}, {4}: {5} ", "\"Status\"", "\"Fault\"", "\"ResultId\"", "\"" + data.ResultId + "\"", "\"Message\"", "\"" + Error + "\"") + " }";
                            PublishMessage<DeviceCommand>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, commandResSendMeasResults, message.BasicProperties.CorrelationId);
                        }
                        result = true;
                        break;

                    case "SendActivitySensor":
                        SensorActivity sensorActivity = ConfigurationSdrnController.listSensorActivity.Find(t => t.Sensor_.Name == sensorName && t.Sensor_.Equipment.TechId == techId);
                        if (sensorActivity != null)
                        {
                            if (sensorActivity.cntSeconds > 10)
                            {
                                MessageObject dataSendActivitySensor = UnPackObject(messageResponse);
                                var dataSendActivitySensorRecognize = dataSendActivitySensor.Object as Atdi.AppServer.Contracts.Sdrns.Sensor;
                                dataSendActivitySensorRecognize.Status = "A";
                                ClassDBGetSensor.UpdateStatusSensor(dataSendActivitySensorRecognize);

                                Atdi.DataModels.Sdrns.Device.DeviceCommand SendActivitySensorResultRes = new DeviceCommand();
                                SendActivitySensorResultRes.Command = "SendActivitySensorResult";
                                SendActivitySensorResultRes.CommandId = "SendCommand";
                                SendActivitySensorResultRes.SensorName = sensorName;
                                SendActivitySensorResultRes.EquipmentTechId = techId;
                                SendActivitySensorResultRes.SdrnServer = sdrnServer;
                                SendActivitySensorResultRes.CustTxt1 = "Success";
                                PublishMessage<DeviceCommand>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, SendActivitySensorResultRes, message.BasicProperties.CorrelationId);
                                sensorActivity.cntSeconds = 0;
                            }
                        }
                        result = true;
                        break;
                    case "SendMeasSdrResults":

                        MessageObject dataS = UnPackObject(messageResponse);
                        var dataS1 = dataS.Object as Atdi.AppServer.Contracts.Sdrns.MeasSdrResults;
                        // здесь обработчик
                        result = true;
                        break;
                    case "SendEntityPart":
                        MessageObject datapartEntityP = UnPackObject(messageResponse);
                        var datapartEntityPart = datapartEntityP.Object as Atdi.DataModels.Sdrns.Device.EntityPart;
                        //var datapartEntityPart = JsonConvert.DeserializeObject(UTF8Encoding.UTF8.GetString(message.Body), typeof(Atdi.DataModels.Sdrns.Device.EntityPart)) as Atdi.DataModels.Sdrns.Device.EntityPart;
                        Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBEntity DbGetResEntityPart = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBEntity>();
                        int? ID1 = DbGetResEntityPart.SaveEntityPart(datapartEntityPart);
                        if (ID1 > 0)
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandRes = new DeviceCommand();
                            commandRes.Command = "SendEntityPartResult";
                            commandRes.CommandId = "SendCommand";
                            commandRes.SensorName = sensorName;
                            commandRes.EquipmentTechId = techId;
                            commandRes.SdrnServer = sdrnServer;
                            commandRes.CustTxt1 = "Success";
                            PublishMessage<DeviceCommand>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, commandRes, message.BasicProperties.CorrelationId);
                        }
                        else
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandRes = new DeviceCommand();
                            commandRes.Command = "SendEntityPartResult";
                            commandRes.CommandId = "SendCommand";
                            commandRes.SensorName = sensorName;
                            commandRes.EquipmentTechId = techId;
                            commandRes.SdrnServer = sdrnServer;
                            commandRes.CustTxt1 = "Fault";
                            PublishMessage<DeviceCommand>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, commandRes, message.BasicProperties.CorrelationId);
                        }

                        result = true;
                        break;
                    case "SendEntity":

                        MessageObject datapartEntit = UnPackObject(messageResponse);
                        var datapartEntity = datapartEntit.Object as Atdi.DataModels.Sdrns.Device.Entity;
                        //var datapartEntity = JsonConvert.DeserializeObject(UTF8Encoding.UTF8.GetString(message.Body), typeof(Atdi.DataModels.Sdrns.Device.Entity)) as Atdi.DataModels.Sdrns.Device.Entity;
                        Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBEntity DbGetResEntity = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBEntity>();
                        int? ID2 = DbGetResEntity.SaveEntity(datapartEntity);
                        if (ID2 > 0)
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandResSendEntity = new DeviceCommand();
                            commandResSendEntity.Command = "SendEntityResult";
                            commandResSendEntity.CommandId = "SendCommand";
                            commandResSendEntity.CustTxt1 = "Success";
                            commandResSendEntity.SensorName = sensorName;
                            commandResSendEntity.EquipmentTechId = techId;
                            commandResSendEntity.SdrnServer = sdrnServer;
                            PublishMessage<DeviceCommand>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, commandResSendEntity, message.BasicProperties.CorrelationId);
                        }
                        else
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandResSendEntity = new DeviceCommand();
                            commandResSendEntity.Command = "SendEntityResult";
                            commandResSendEntity.CommandId = "SendCommand";
                            commandResSendEntity.CustTxt1 = "Fault";
                            commandResSendEntity.SensorName = sensorName;
                            commandResSendEntity.EquipmentTechId = techId;
                            commandResSendEntity.SdrnServer = sdrnServer;
                            PublishMessage<DeviceCommand>(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, commandResSendEntity, message.BasicProperties.CorrelationId);
                        }
                        result = true;
                        break;
                    case "SendCommand":

                        //ActiveSensor 
                        //ConfirmSendMeasTask
                        //ConfirmSendMeasResults
                        /*
                        var dataDeviceCommandResult = JsonConvert.DeserializeObject(UTF8Encoding.UTF8.GetString(message.Body), typeof(Atdi.DataModels.Sdrns.Device.DeviceCommandResult)) as Atdi.DataModels.Sdrns.Device.DeviceCommandResult;
                        switch (dataDeviceCommandResult.CommandId)
                        {
                            case "ActiveSensor":
                                Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor handlerx = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBGetSensor>();
                                List<Sensor> se = handlerx.LoadObjectSensor(sensorName, techId);
                                if (se != null)
                                {
                                    if (se.Count > 0)
                                    {
                                        se[0].Status = "A";
                                        handlerx.UpdateStatusSensor(se[0]);
                                        Atdi.DataModels.Sdrns.Device.DeviceCommandResult deviceComm = new Atdi.DataModels.Sdrns.Device.DeviceCommandResult();
                                        deviceComm.CommandId = "ActiveSensor";
                                        PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, messageType, channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceComm)));
                                    }
                                }
                                */
                        result = true;
                        break;
                        /*
                    case "CheckSensorActivity":
                        result = true;
                        break;
                    case "StopMeasTask":

                        result = true;
                        break;
                        */
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                result = false;
                this._logger.Error(e.Message);
            }

            // подтверждение обработки сообщения
            if (result)
            {
                channel.BasicAck(message.DeliveryTag, false);
            }
        }


        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
        }


        public MessageObject UnPackObject(Message obj)
        {
            MessageObject res = new MessageObject();
            MessageConvertSettings messageConvertSettings = new MessageConvertSettings();
            messageConvertSettings.UseEncryption = true;
            messageConvertSettings.UseСompression = true;
            var typeResolver = MessageObjectTypeResolver.CreateForApi20();
            var messageConvertor = new MessageConverter(messageConvertSettings, typeResolver);
            res = messageConvertor.Deserialize(obj);
            return res;
        }

        public bool PublishMessage<Tobj>(string NameServer, string exchange, string routingKey, string sensorName, string techId, string MessageType, RabbitMQ.Client.IModel channel, Tobj messageObject, string CorrelationId = null)
        {
            bool isSendSuccess = false;
            try
            {
                        MessageConvertSettings messageConvertSettings = new MessageConvertSettings();
                        messageConvertSettings.UseEncryption = Atdi.SDNRS.AppServer.BusManager.GlobalInit.UseEncryption;
                        messageConvertSettings.UseСompression = Atdi.SDNRS.AppServer.BusManager.GlobalInit.UseСompression;
                        var typeResolver = MessageObjectTypeResolver.CreateForApi20();
                        var messageConvertor = new MessageConverter(messageConvertSettings, typeResolver);
                        var message = messageConvertor.Pack<Tobj>(MessageType, messageObject);
                        message.CorrelationId = CorrelationId;
                        message.Headers = new Dictionary<string, object>
                        {
                            ["SdrnServer"] = NameServer,
                            ["SensorName"] = sensorName,
                            ["SensorTechId"] = techId,
                            ["Created"] = DateTime.Now.ToString("o")
                        };


                        var props = channel.CreateBasicProperties();
                        props.Persistent = true;
                        props.AppId = "Atdi.AppServer.AppService.SdrnsControllerv2_0.dll";
                        props.MessageId = message.Id;
                        props.Type = message.Type;
               
                        if (!string.IsNullOrEmpty(message.ContentType))
                        {
                            props.ContentType = message.ContentType;
                        }
                        if (!string.IsNullOrEmpty(message.ContentEncoding))
                        {
                            props.ContentEncoding = message.ContentEncoding;
                        }
                        if (!string.IsNullOrEmpty(message.CorrelationId))
                        {
                            props.CorrelationId = message.CorrelationId;
                        }
                        props.Timestamp = new AmqpTimestamp(DateTimeToUnixTimestamp(DateTime.Now));
                        props.Headers = message.Headers;

                        channel.BasicPublish(exchange, routingKey, props, message.Body);
                        isSendSuccess = true;
                  
            }
            catch (Exception e)
            {
                isSendSuccess = false;
                this._logger.Error(e.Message);
            }
            return isSendSuccess;

        }

        public void PublishMessage(string NameServer, string exchange, string routingKey, string sensorName, string techId, string MessageType, RabbitMQ.Client.IModel channel, byte[] data, string CorrelationId=null)
        {
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            var messageId = Guid.NewGuid().ToString();

            props.Persistent = true;
            props.AppId = "Atdi.AppServer.ConfigurationSdrnController.dll";
            props.MessageId = messageId;
            props.Type = MessageType;

            if (!string.IsNullOrEmpty(CorrelationId))
            {
                props.CorrelationId = CorrelationId;
            }

            props.Headers = new Dictionary<string, object>();
            props.Headers["SdrnServer"] = NameServer;
            props.Headers["SensorName"] = sensorName;
            props.Headers["SensorTechId"] = techId;
            props.DeliveryMode = 2;
            channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: props, body: data);
        }
    }
}

