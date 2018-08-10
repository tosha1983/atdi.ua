using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Atdi.SDNRS.AppServer;


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
