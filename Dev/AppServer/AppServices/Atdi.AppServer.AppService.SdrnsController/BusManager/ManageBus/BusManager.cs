using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Atdi.SDNRS.AppServer;
using RabbitMQ.Client;

namespace Atdi.SDNRS.AppServer.BusManager
{
    /// <summary>
    /// Шаблон для отправки сообщений в очереди сообщений
    /// </summary>
    /// <typeparam name="T">Класс основного объекта</typeparam>
    /// <typeparam name="S">Класс стоп-листа для основного объекта</typeparam>
    public class BusManager<T, S>
        where T : class
        where S : class
    {


           

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
                    if (ClassStaticBus.bus.IsConnected)
                    {
                        if (!ClassStaticBus.List_Queue.Contains(name_queue)) { ClassStaticBus.List_Queue.Add(name_queue); ClassStaticBus.bus.Advanced.QueueDeclare(name_queue); }
                        ClassStaticBus.bus.Advanced.PublishAsync(EasyNetQ.Topology.Exchange.GetDefault(), name_queue, true, z)
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
        // метод для отправки стоп-листа в шину
        public bool SendDataObjectStop(S obj, string name_queue, string Expriration)
        {
            bool is_Success = false;
            try
            {
                    IMessage<S> z = new Message<S>(obj);
                    z.Properties.ExpirationPresent = true;
                    z.Properties.Expiration = Expriration;
                    z.Properties.DeliveryModePresent = true;
                    z.Properties.DeliveryMode = 2;
                    if (ClassStaticBus.bus.IsConnected)
                    {
                        if (!ClassStaticBus.List_Queue.Contains(name_queue)) { ClassStaticBus.List_Queue.Add(name_queue); ClassStaticBus.bus.Advanced.QueueDeclare(name_queue); }
                        ClassStaticBus.bus.Advanced.PublishAsync(EasyNetQ.Topology.Exchange.GetDefault(), name_queue, true, z)
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
                //CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SendDataObjectStop]:" + ex.Message);
            }
            return is_Success;
        }
    }

    /// <summary>
    /// Шаблон для отправки сообщений в очереди сообщений
    /// </summary>
    /// <typeparam name="T">Класс основного объекта</typeparam>
    public class BusManager<T>
        where T : class
    {

        public bool SendDataToServer(string sensorName, string techId, byte[] data, string apiVer)
        {
            bool isSendSuccess = false;
            try
            {
                var factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword };
                {
                    using (var connection = factory.CreateConnection())
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
                        props.AppId = "SDRN Server";
                        props.DeliveryMode = 2;
                        props.Headers = new Dictionary<string, object>();
                        props.Headers["SdrnServer"] = GlobalInit.NameServer;
                        props.Headers["SensorName"] = sensorName;
                        props.Headers["TechId"] = techId;

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
                    if (ClassStaticBus.bus.IsConnected)
                    {
                        if (!ClassStaticBus.List_Queue.Contains(name_queue)) { ClassStaticBus.List_Queue.Add(name_queue); ClassStaticBus.bus.Advanced.QueueDeclare(name_queue); }
                        ClassStaticBus.bus.Advanced.PublishAsync(EasyNetQ.Topology.Exchange.GetDefault(), name_queue, true, z)
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
