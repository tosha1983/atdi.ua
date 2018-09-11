﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Atdi.DataModels.Sdrns.Device;
using Castle.Windsor;
using RabbitMQ.Client;



namespace Atdi.AppServer.ConfigurationSdrnController
{
    public class Concumer
    {
        public void HandleMessage(RabbitMQ.Client.IModel channel, RabbitMQ.Client.Events.BasicDeliverEventArgs message, IWindsorContainer container, string StartName, string Exchangepoint, IConnection connection, string ExchangePointFromServer, string StartNameQueueDevice)
        {
            var messageType = message.BasicProperties.Type;
            if (messageType == null)
            {
                return;
            }

            
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
                        var dataRegisterSensor = JsonConvert.DeserializeObject(UTF8Encoding.UTF8.GetString(message.Body), typeof(Sensor)) as Sensor;
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
                                    PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceResult)), message.BasicProperties.CorrelationId);
                                }
                                else
                                {
                                    deviceResult.Status = "Fault";
                                    deviceResult.Message = string.Format("Error registration sensor Name = {0}, TechId = {1}", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId);
                                    PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceResult)), message.BasicProperties.CorrelationId);
                                }
                            }
                            catch (Exception ex)
                            {
                                deviceResult.Status = "Fault";
                                deviceResult.Message = string.Format("Error registration sensor Name = {0}, TechId = {1}: {2}", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId, ex.Message);
                                PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceResult)), message.BasicProperties.CorrelationId);
                            }
                        }
                        else
                        {
                            deviceResult.Status = "Fault";
                            deviceResult.Message = string.Format("Error registration sensor Name = {0}, TechId = {1} (Duplicate) ", dataRegisterSensor.Name, dataRegisterSensor.Equipment.TechId);
                            PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendRegistrationResult", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceResult)), message.BasicProperties.CorrelationId);
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
                        var dataUpdateSensor = JsonConvert.DeserializeObject(UTF8Encoding.UTF8.GetString(message.Body), typeof(Sensor)) as Sensor;
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
                                    PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendSensorUpdatingResult", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceResultUpdate)), message.BasicProperties.CorrelationId);
                                }
                                else
                                {
                                    deviceResultUpdate.Status = "Fault";
                                    deviceResultUpdate.Message = string.Format("Error updated sensor Name = {0}, TechId = {1}", dataUpdateSensor.Name, dataUpdateSensor.Equipment.TechId);
                                    PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendSensorUpdatingResult", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceResultUpdate)), message.BasicProperties.CorrelationId);
                                }
                            }
                            catch (Exception ex)
                            {
                                deviceResultUpdate.Status = "Fault";
                                deviceResultUpdate.Message = string.Format("Error updated sensor Name = {0}, TechId = {1}: {2}", dataUpdateSensor.Name, dataUpdateSensor.Equipment.TechId, ex.Message);
                                PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendSensorUpdatingResult", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceResultUpdate)), message.BasicProperties.CorrelationId);
                            }

                        result = true;
                        break;
                    case "SendMeasResults":
                        Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassesDBGetResult DbGetRes = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassesDBGetResult>();
                        var data = JsonConvert.DeserializeObject(UTF8Encoding.UTF8.GetString(message.Body), typeof(Atdi.DataModels.Sdrns.Device.MeasResults)) as Atdi.DataModels.Sdrns.Device.MeasResults;
                        int? ID = -1;
                        string Status_Original = data.Status;
                        Atdi.AppServer.Contracts.Sdrns.MeasurementResults msReslts = Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassConvertToSDRResults.GenerateMeasResults2_0(data);
                        if (msReslts.TypeMeasurements == Atdi.AppServer.Contracts.Sdrns.MeasurementType.SpectrumOccupation) msReslts.Status = Status_Original;
                        if (msReslts.MeasurementsResults != null)
                        {
                            if (msReslts.MeasurementsResults.Count() > 0)
                            {
                                if (msReslts.MeasurementsResults[0] is Atdi.AppServer.Contracts.Sdrns.LevelMeasurementOnlineResult)
                                {
                                    msReslts.Status = "O";
                                    ID = DbGetRes.SaveResultToDB(msReslts);
                                }
                                else
                                {
                                    ID = DbGetRes.SaveResultToDB(msReslts);
                                }
                            }
                        }
                        else
                        {
                            ID = DbGetRes.SaveResultToDB(msReslts);
                        }
                        if (ID > 0)
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandResSendMeasResults = new DeviceCommand();
                            commandResSendMeasResults.Command = "SendMeasResultsResult";
                            commandResSendMeasResults.CommandId = "SendCommand";
                            commandResSendMeasResults.SensorName = sensorName;
                            commandResSendMeasResults.EquipmentTechId = techId;
                            commandResSendMeasResults.SdrnServer = sdrnServer;
                            commandResSendMeasResults.CustTxt1 = "Successfully saved Results to DB";
                            PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(commandResSendMeasResults)), message.BasicProperties.CorrelationId);
                        }
                        else
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandResSendMeasResults = new DeviceCommand();
                            commandResSendMeasResults.Command = "SendMeasResultsResult";
                            commandResSendMeasResults.CommandId = "SendCommand";
                            commandResSendMeasResults.SensorName = sensorName;
                            commandResSendMeasResults.EquipmentTechId = techId;
                            commandResSendMeasResults.SdrnServer = sdrnServer;
                            commandResSendMeasResults.CustTxt1 = "Error saved Results to DB";
                            PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(commandResSendMeasResults)), message.BasicProperties.CorrelationId);
                        }
                        result = true;
                        break;
                    case "SendEntityPart":
                        var datapartEntityPart = JsonConvert.DeserializeObject(UTF8Encoding.UTF8.GetString(message.Body), typeof(Atdi.DataModels.Sdrns.Device.EntityPart)) as Atdi.DataModels.Sdrns.Device.EntityPart;
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
                            commandRes.CustTxt1 = "Successfully saved SendEntityPart to DB";
                            PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(commandRes)), message.BasicProperties.CorrelationId);
                        }
                        else
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandRes = new DeviceCommand();
                            commandRes.Command = "SendEntityPartResult";
                            commandRes.CommandId = "SendCommand";
                            commandRes.SensorName = sensorName;
                            commandRes.EquipmentTechId = techId;
                            commandRes.SdrnServer = sdrnServer;
                            commandRes.CustTxt1 = "Fail saved SendEntityPart to DB";
                            PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(commandRes)), message.BasicProperties.CorrelationId);
                        }

                        result = true;
                        break;
                    case "SendEntity":
                        var datapartEntity = JsonConvert.DeserializeObject(UTF8Encoding.UTF8.GetString(message.Body), typeof(Atdi.DataModels.Sdrns.Device.Entity)) as Atdi.DataModels.Sdrns.Device.Entity;
                        Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBEntity DbGetResEntity = container.Resolve<Atdi.AppServer.AppService.SdrnsControllerv2_0.ClassDBEntity>();
                        int? ID2 = DbGetResEntity.SaveEntity(datapartEntity);
                        if (ID2 > 0)
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandResSendEntity = new DeviceCommand();
                            commandResSendEntity.Command = "SendEntityResult";
                            commandResSendEntity.CommandId = "SendCommand";
                            commandResSendEntity.CustTxt1 = "Successfully saved SendEntity to DB";
                            commandResSendEntity.SensorName = sensorName;
                            commandResSendEntity.EquipmentTechId = techId;
                            commandResSendEntity.SdrnServer = sdrnServer;
                            PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(commandResSendEntity)), message.BasicProperties.CorrelationId);
                        }
                        else
                        {
                            Atdi.DataModels.Sdrns.Device.DeviceCommand commandResSendEntity = new DeviceCommand();
                            commandResSendEntity.Command = "SendEntityResult";
                            commandResSendEntity.CommandId = "SendCommand";
                            commandResSendEntity.CustTxt1 = "Fail saved SendEntity to DB";
                            commandResSendEntity.SensorName = sensorName;
                            commandResSendEntity.EquipmentTechId = techId;
                            commandResSendEntity.SdrnServer = sdrnServer;
                            PublishMessage(sdrnServer, Exchangepoint, routingKey, sensorName, techId, "SendCommand", channel, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(commandResSendEntity)), message.BasicProperties.CorrelationId);
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
            }

            // подтверждение обработки сообщения
            if (result)
            {
                channel.BasicAck(message.DeliveryTag, false);
            }
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
