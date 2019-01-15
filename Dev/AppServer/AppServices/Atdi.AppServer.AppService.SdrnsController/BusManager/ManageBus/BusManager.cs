using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using Atdi.Modules.Sdrn.MessageBus;
using Newtonsoft.Json;
     

namespace Atdi.SDNRS.AppServer.BusManager
{
   

    /// <summary>
    /// Шаблон для отправки сообщений в очереди сообщений
    /// </summary>
    /// <typeparam name="T">Класс основного объекта</typeparam>
    public class BusManager<T>
        where T : class
    {

        public bool RegisterQueue(string sensorName, string techId, string apiVer)
        {
            bool isSuccessRegister = false;
            try
            {
                if (ClassStaticBus.factory != null)
                {
                    using (var connection = ClassStaticBus.factory.CreateConnection($"SDRN device (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
                    {
                        using (var channel = connection.CreateModel())
                        {
                            var exchange = GlobalInit.ExchangePointFromServer + string.Format(".[{0}]", apiVer);
                            var queueName = $"{GlobalInit.StartNameQueueDevice}.[{sensorName}].[{techId}].[{apiVer}]";
                            var routingKey = $"{GlobalInit.StartNameQueueDevice}.[{sensorName}].[{techId}]";

                            channel.ExchangeDeclare(
                                        exchange: exchange,
                                        type: "direct",
                                        durable: true
                                    );

                            channel.QueueDeclare(
                                queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            channel.QueueBind(queueName, exchange, routingKey);
                            channel.Close();
                            isSuccessRegister = true;
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception)
            {
                isSuccessRegister = false;
            }
            return isSuccessRegister;

        }

        

        public T UnPackObject(RabbitMQ.Client.Events.BasicDeliverEventArgs message)
        {
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
            MessageObject res = new MessageObject();
            MessageConvertSettings messageConvertSettings = new MessageConvertSettings();
            messageConvertSettings.UseEncryption = true;
            messageConvertSettings.UseCompression = true;
            var typeResolver = MessageObjectTypeResolver.CreateForApi20();
            var messageConvertor = new MessageConverter(messageConvertSettings, typeResolver);
            res = messageConvertor.Deserialize(messageResponse);
            var dataRes = res.Object as T;
            return dataRes;
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
        }

        public bool SendDataToDeviceCrypto<Tobj>(string messageType, Tobj messageObject, string sensorName, string techId, string apiVer, string correlationToken = null)
        {
            bool isSendSuccess = false;
            try
            {
                if (ClassStaticBus.factory != null)
                {

                    using (var connection = ClassStaticBus.factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
                    {
                        using (var channel = connection.CreateModel())
                        {
                            var exchange = GlobalInit.ExchangePointFromServer + string.Format(".[{0}]", apiVer);
                            var queueName = GlobalInit.StartNameQueueDevice + $".[{sensorName}].[{techId}].[{apiVer}]";
                            var routingKey = GlobalInit.StartNameQueueDevice + $".[{sensorName}].[{techId}]";

                            channel.ExchangeDeclare(
                                    exchange: exchange,
                                    type: "direct",
                                    durable: true
                                );

                            channel.QueueDeclare(
                                queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            channel.QueueBind(queueName, exchange, routingKey);

                            MessageConvertSettings messageConvertSettings = new MessageConvertSettings();
                            messageConvertSettings.UseEncryption = GlobalInit.UseEncryption;
                            messageConvertSettings.UseCompression = GlobalInit.UseСompression;
                            var typeResolver = MessageObjectTypeResolver.CreateForApi20();
                            var messageConvertor = new MessageConverter(messageConvertSettings, typeResolver);
                            var message = messageConvertor.Pack<Tobj>(messageType, messageObject);
                            message.CorrelationId = correlationToken;
                            message.Headers = new Dictionary<string, object>
                            {
                                ["SdrnServer"] = GlobalInit.NameServer,
                                ["SensorName"] = sensorName,
                                ["SensorTechId"] = techId,
                                ["Created"] = DateTime.Now.ToString("o")
                            };


                            var props = channel.CreateBasicProperties();
                            props.Persistent = true;
                            props.AppId = "Atdi.SDNRS.AppServer.BusManager.dll";
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
                            channel.Close();
                            isSendSuccess = true;
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception)
            {
                isSendSuccess = false;
            }
            return isSendSuccess;

        }

        public bool SendDataToServer(string sensorName, string techId, byte[] data, string apiVer, string typeMessage)
        {
            bool isSendSuccess = false;
            try
            {
                if (ClassStaticBus.factory != null)
                {
                    using (var connection = ClassStaticBus.factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
                    {
                        using (var channel = connection.CreateModel())
                        {
                            var exchange = GlobalInit.ExchangePointFromServer + string.Format(".[{0}]", apiVer);
                            var queueName = GlobalInit.StartNameQueueDevice + $".[{sensorName}].[{techId}].[{apiVer}]";
                            var routingKey = GlobalInit.StartNameQueueDevice + $".[{sensorName}].[{techId}]";

                            channel.ExchangeDeclare(
                                    exchange: exchange,
                                    type: "direct",
                                    durable: true
                                );

                            channel.QueueDeclare(
                                queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            var props = channel.CreateBasicProperties();
                            props.Persistent = true;
                            var messageId = Guid.NewGuid().ToString();

                            props.AppId = "Atdi.SDNRS.AppServer.BusManager.dll";
                            props.MessageId = messageId;
                            props.Type = typeMessage; ;
                            props.Headers = new Dictionary<string, object>();
                            props.Headers["SdrnServer"] = GlobalInit.NameServer;
                            props.Headers["SensorName"] = sensorName;
                            props.Headers["SensorTechId"] = techId;
                            props.DeliveryMode = 2;
                            channel.BasicPublish(exchange: exchange,
                                                 routingKey: routingKey,
                                                 basicProperties: props,
                                                 body: data);
                            channel.Close();
                            isSendSuccess = true;

                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception)
            {
                isSendSuccess = false;
            }
            return isSendSuccess;
           
        }


        public uint GetMessageCount(string name_queue)
        {
            uint MessageCount = 0;
            try {

                if (ClassStaticBus.factory != null)
                {
                    using (var connection = ClassStaticBus.factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
                    {
                        using (var channel = connection.CreateModel())
                        {
                            MessageCount = channel.MessageCount(name_queue);
                            channel.Close();
                        }

                        connection.Close();
                    }
                }
                return MessageCount;
            }
            catch (Exception)
            { return 0; }
        }

        
        public object GetDataObject<T>(string name_queue)
        {
            T obj = default(T);
            try {

                if (ClassStaticBus.factory != null)
                {
                    using (var connection = ClassStaticBus.factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
                    {
                        using (var channel = connection.CreateModel())
                        {
                           var Result = channel.BasicGet(name_queue, true);
                            obj = JsonConvert.DeserializeObject<T>(UTF8Encoding.UTF8.GetString(Result.Body));
                            channel.Close();
                        }

                        connection.Close();
                    }
                }
                return obj;
            }
            catch (Exception)
            { return null; }
        }

        public bool SendDataToQueue(string jsonString, string name_queue)
        {
            bool is_Success = false;
            try
            {
                if (ClassStaticBus.factory != null)
                {
                    using (var connection = ClassStaticBus.factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
                    {
                        using (var channel = connection.CreateModel())
                        {
                            var dictionary = new Dictionary<string, object>();
                            dictionary.Add("SdrnServer", "ServerSDRN01");
                            dictionary.Add("SensorName", "INS-DV-2018-TEST");
                            dictionary.Add("SensorTechId", "MMS-02");
                            //dictionary.Add("SensorName", "SENSOR-DBD13-G65-2314");
                            //dictionary.Add("SensorTechId", "{1645B8D8-AB87-4292-91E3-8AE2D614CEEC}");


                            channel.QueueDeclare(
                                 queue: name_queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                            var props = channel.CreateBasicProperties();
                            props.Persistent = true;
                            props.AppId = "Atdi.Sdrn";
                            props.Type = "SendMeasResults";
                            props.MessageId = Guid.NewGuid().ToString();
                            props.Headers = dictionary;
                            props.ContentType = "application/sdrn";
                            channel.BasicPublish("", name_queue, props, UTF8Encoding.UTF8.GetBytes(jsonString));
                            is_Success = true;
                            channel.Close();
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                is_Success = false;
            }

            return is_Success;
        }


        public bool SendDataToQueue(T obj, string name_queue)
        {
            bool is_Success = false;
            try
            {
                if (ClassStaticBus.factory != null)
                {
                    using (var connection = ClassStaticBus.factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(
                                 queue: name_queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                            var props = channel.CreateBasicProperties();
                            props.Persistent = true;
                            channel.BasicPublish("", name_queue, props, UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
                            is_Success = true;
                            channel.Close();
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                is_Success = false;
            }

            return is_Success;
        }

        // метод для отправки основного объекта в шину
        public bool SendDataObject(T obj, string name_queue)
        {
            return SendDataToQueue(obj, name_queue);
        }

        public bool DeleteQueue(string name_queue)
        {
            bool is_Success = false;
            try
            {
                using (var connection = ClassStaticBus.factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
                {
                    using (var channel = connection.CreateModel())
                    {
                        if (ClassStaticBus.List_Queue.Contains(name_queue)) { ClassStaticBus.List_Queue.Remove(name_queue); channel.QueueDelete(name_queue); is_Success = true; }
                        channel.Close();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex) { is_Success = false;
                //CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[DeleteQueue]:" + ex.Message); 
            }
            return is_Success;
        }

    }





}
