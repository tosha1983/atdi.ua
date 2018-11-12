using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Atdi.SDNRS.AppServer;
using RabbitMQ.Client;
using Atdi.Modules.Sdrn.MessageBus;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
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
                ConnectionFactory factory = null;
                if (string.IsNullOrEmpty(GlobalInit.RabbitVirtualHost))
                {
                    factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword };
                }
                else
                {
                    factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword, VirtualHost = GlobalInit.RabbitVirtualHost };
                }
                if (factory!=null)
                {
                    using (var connection = factory.CreateConnection($"SDRN device (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
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
                        isSuccessRegister = true;
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
            messageConvertSettings.UseСompression = true;
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
                ConnectionFactory factory = null;
                if (string.IsNullOrEmpty(GlobalInit.RabbitVirtualHost))
                {
                    factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword };
                }
                else
                {
                    factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword, VirtualHost = GlobalInit.RabbitVirtualHost };
                }
                if (factory != null)
                {
                    
                    using (var connection = factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
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
                        messageConvertSettings.UseСompression = GlobalInit.UseСompression;
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
                        isSendSuccess = true;
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
                ConnectionFactory factory = null;
                if (string.IsNullOrEmpty(GlobalInit.RabbitVirtualHost))
                {
                    factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword };
                }
                else
                {
                    factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword, VirtualHost = GlobalInit.RabbitVirtualHost };
                }
                if (factory != null)
                {
                 
                    using (var connection = factory.CreateConnection($"SDRN service (Activate) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
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
                        props.Type = typeMessage;;
                        props.Headers = new Dictionary<string, object>();
                        props.Headers["SdrnServer"] = GlobalInit.NameServer;
                        props.Headers["SensorName"] = sensorName;
                        props.Headers["SensorTechId"] = techId;
                        props.DeliveryMode = 2;
                        channel.BasicPublish(exchange: exchange,
                                             routingKey: routingKey,
                                             basicProperties: props,
                                             body: data);
                        isSendSuccess = true;

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
            try {
                EasyNetQ.Topology.IQueue q = new EasyNetQ.Topology.Queue(name_queue, false);
                return ClassStaticBus.bus.Advanced.MessageCount(q);
            }
            catch (Exception)
            { return 0; }
        }

        
        public object GetDataObject(string name_queue)
        {
            try {
                EasyNetQ.Topology.IQueue q = new EasyNetQ.Topology.Queue(name_queue, false);
                var getResult = ClassStaticBus.bus.Advanced.Get<T>(q);
                if (getResult.MessageAvailable) 
                    return getResult.Message.Body;
                else return null;
            }
            catch (Exception)
            { return null; }
        }
       
      

        // метод для отправки основного объекта в шину
        public bool SendDataObject(T obj, string name_queue, string Expriration)
        {
            bool is_Success = false;
            try {
                    IMessage<T> z = new Message<T>(obj);
                    z.Properties.ExpirationPresent = true;
                    z.Properties.Expiration = Expriration;
                    z.Properties.DeliveryModePresent = true;
                    z.Properties.DeliveryMode = 2;

                    MessageProperties messageProperties = new MessageProperties();
                    messageProperties.ExpirationPresent = true;
                    messageProperties.Expiration = Expriration;
                    messageProperties.DeliveryModePresent = true;
                    messageProperties.DeliveryMode = 2;

                if (ClassStaticBus.bus.IsConnected)
                    {
                    if (!ClassStaticBus.List_Queue.Contains(name_queue)) { ClassStaticBus.List_Queue.Add(name_queue); ClassStaticBus.bus.Advanced.QueueDeclare(name_queue); }//, false, true, false, false, null,null,null,null,null,null, 2147000000);  }
                        EasyNetQ.Topology.IExchange exchange = EasyNetQ.Topology.Exchange.GetDefault();
                     ClassStaticBus.bus.Advanced.PublishAsync(exchange, name_queue, true, z)
                            .ContinueWith(task =>
                                {
                                    if (task.IsCompleted)
                                    {
                                        is_Success = true;
                                    }
                                    if (task.IsFaulted)
                                    {
                                        is_Success = false;
                                    }
                                }).Wait();
                    }
                    else
                    {
                        ClassStaticBus.bus.Dispose();
                        ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                    }
            }
            catch (Exception ex) { is_Success = false;
                //CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SendDataObject]:" + ex.Message); 
            }
            return is_Success;
        }




        public bool DeleteQueue(string name_queue)
        {
            bool is_Success = false;
            try
            {
                    EasyNetQ.Topology.Queue Q = new EasyNetQ.Topology.Queue(name_queue, false);
                    if (ClassStaticBus.bus.IsConnected)
                    {
                        if (ClassStaticBus.List_Queue.Contains(name_queue)) { ClassStaticBus.List_Queue.Remove(name_queue); ClassStaticBus.bus.Advanced.QueueDelete(Q); is_Success = true; }
                    }
                    else
                    {
                        ClassStaticBus.bus.Dispose();
                        ClassStaticBus.bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
                    }
            }
            catch (Exception ex) { is_Success = false;
                //CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[DeleteQueue]:" + ex.Message); 
            }
            return is_Success;
        }

    }





}
