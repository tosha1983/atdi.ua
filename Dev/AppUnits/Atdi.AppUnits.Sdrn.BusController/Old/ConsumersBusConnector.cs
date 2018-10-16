////using Atdi.Platform.Logging;
////using System;
////using System.Collections.Generic;
////using System.Collections.Concurrent;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;
////using RabbitMQ.Client;
////using RabbitMQ.Client.Events;

////namespace Atdi.AppUnits.Sdrn.BusController
////{
////    public class ConsumersBusConnector : LoggedObject, IDisposable
////    {
////        private readonly SdrnServerDescriptor _serverDescriptor;
////        private readonly ConsumersRabbitMQConnection _connection;
////        private readonly string _deviceExchangeName;

////        public ConsumersBusConnector(ConsumersRabbitMQConnection connection, SdrnServerDescriptor serverDescriptor, ILogger logger) : base(logger)
////        {
////            this._connection = connection;
////            this._serverDescriptor = serverDescriptor;

////            this._deviceExchangeName = $"{this._serverDescriptor.DeviceExchange}.[v{this._serverDescriptor.ApiVersion}]";
////            this._connection.DeclareDurableDirectExchange(this._deviceExchangeName);

////            this.DeclareConsumers();
////        }

////        private void DeclareConsumers()
////        {
////            var serverQueues = this._serverDescriptor.ServerQueueus.Values;
////            var consumers = new List<QueueConsumer[]>();

////            foreach (var serverQueueDescriptor in serverQueues)
////            {
////                var queue = new QueueDescriptor
////                {
////                    Name = $"{this._serverDescriptor.ServerQueueNamePart}.[{this._serverDescriptor.ServerInstance}].[{serverQueueDescriptor.RoutingKey}].[v{this._serverDescriptor.ApiVersion}]",
////                    RoutingKey = $"[{this._serverDescriptor.ServerInstance}].[{serverQueueDescriptor.RoutingKey}]",
////                    Exchange = this._deviceExchangeName
////                };
////                this._connection.DeclareDurableQueue(queue);

////                consumers.Add(this._connection.CreateConsumers(serverQueueDescriptor.RoutingKey, queue.Name, serverQueueDescriptor.ConsumerCount));
////            }

////            var tasks = new List<Task>();
////            foreach (var consumerPart in consumers)
////            {
////                tasks.Add(Task.Run(() => 
////                    {
////                        foreach (var consumer in consumerPart)
////                        {
////                            consumer.Activate();
////                        }
////                    }));
////            }
////            Task.WaitAll(tasks.ToArray());

////            this.Logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The consumers are declared successfully: count = {consumers.Count}");
////        }


////        public void Dispose()
////        {
////            _connection.Dispose();
////        }
////    }
////}
