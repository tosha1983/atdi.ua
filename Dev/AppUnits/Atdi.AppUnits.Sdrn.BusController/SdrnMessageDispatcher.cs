using Atdi.Contracts.Sdrn.Server;
using Atdi.Modules.Sdrn.AmqpBroker;
using Atdi.Modules.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnMessageDispatcher : ISdrnMessageDispatcher
    {
        private readonly SdrnBusControllerConfig _busControllerConfig;
        private readonly ISdrnServerEnvironment _environment;
        private readonly BusConnectionFactory _busConnectionFactory;
        private readonly MessageConverter _messageConverter;
        private readonly IServicesResolver _servicesResolver;
        private readonly ILogger _logger;
        private BusConnection _busConnection;
        private readonly string _deviceExchangeName;
        private readonly string _serverExchangeName;
        private readonly string _serverInnerExchangeName;
        private readonly SdrnHandlerLibrary _handlerLibrary;
        private readonly List<BusConnection> _consumerConnections;
        private readonly SdrnQueueConsumer[] _consumers; 

        public SdrnMessageDispatcher(SdrnBusControllerConfig busControllerConfig, ISdrnServerEnvironment environment, BusConnectionFactory busConnectionFactory, MessageConverter messageConverter, IServicesResolver servicesResolver, ILogger logger)
        {
            this._busControllerConfig = busControllerConfig;
            this._environment = environment;
            this._busConnectionFactory = busConnectionFactory;
            this._messageConverter = messageConverter;
            this._servicesResolver = servicesResolver;
            this._logger = logger;
            this._handlerLibrary = new SdrnHandlerLibrary(this._servicesResolver);
            this._consumerConnections = new List<BusConnection>();

            this.State = SdrnMessageDispatcherState.Deactivated;

            this.Tag = $"[SDRN.Server].[{environment.ServerInstance}].[Dispatcher].[#{System.Threading.Thread.CurrentThread.ManagedThreadId}]";

            var busConfig = new BusConnectionConfig
            {
                ApplicationName = environment.GetAppName(),
                ConnectionName = this.Tag,
                AutoRecovery = true,
                HostName = busControllerConfig.BusHost,
                VirtualHost = busControllerConfig.BusVirtualHost,
                Port = busControllerConfig.BusPort,
                UserName = busControllerConfig.BusUser,
                Password = busControllerConfig.BusPassword
            };

            this._busConnection = busConnectionFactory.Create(busConfig);

            this._deviceExchangeName = this._busControllerConfig.GetDeviceExchangeName(); // $"{this._busControllerConfig.DeviceExchange}.[v{this._busControllerConfig.ApiVersion}]";
            this._serverExchangeName = this._busControllerConfig.GetServerExchangeName(); // $"{this._busControllerConfig.ServerExchange}.[v{this._busControllerConfig.ApiVersion}]";
            this._serverInnerExchangeName = this._busControllerConfig.GetServerInnerExchangeName();

            this._busConnection.DeclareDurableDirectExchange(this._deviceExchangeName);
            this._busConnection.DeclareDurableDirectExchange(this._serverExchangeName);
            this._busConnection.DeclareDurableDirectExchange(this._serverInnerExchangeName);

            this._consumers = this.DeclareConsumers();
        }

        private SdrnQueueConsumer[] DeclareConsumers()
        {
            var serverQueues = this._busControllerConfig.ServerQueueus.Values;
            var consumers = new List<SdrnQueueConsumer>();

            foreach (var serverQueueDescriptor in serverQueues)
            {
                var workQueue = new QueueDescriptor
                {
                    Name = _busControllerConfig.BuildServerQueueName(serverQueueDescriptor.RoutingKey), // $"{this._busControllerConfig.ServerQueueNamePart}.[{this._environment.ServerInstance}].[{serverQueueDescriptor.RoutingKey}].[v{this._busControllerConfig.ApiVersion}]",
                    RoutingKey = _busControllerConfig.BuildServerQueueRoute(serverQueueDescriptor.RoutingKey), //  $"[{this._environment.ServerInstance}].[{serverQueueDescriptor.RoutingKey}]",
                    Exchange = this._deviceExchangeName
                };
                this._busConnection.DeclareDurableQueue(workQueue.Name, workQueue.Exchange, workQueue.RoutingKey);

                var unprocessedQueue = new QueueDescriptor
                {
                    Name = _busControllerConfig.BuildServerUnprocessedQueueName(serverQueueDescriptor.RoutingKey),
                    RoutingKey = _busControllerConfig.BuildServerUnprocessedQueueRoute(serverQueueDescriptor.RoutingKey),
                    Exchange = this._serverInnerExchangeName
                };
                this._busConnection.DeclareDurableQueue(unprocessedQueue.Name, unprocessedQueue.Exchange, unprocessedQueue.RoutingKey);

                var rejectedQueue = new QueueDescriptor
                {
                    Name = _busControllerConfig.BuildServerRejectedQueueName(serverQueueDescriptor.RoutingKey),
                    RoutingKey = _busControllerConfig.BuildServerRejectedQueueRoute(serverQueueDescriptor.RoutingKey),
                    Exchange = this._serverInnerExchangeName
                };
                this._busConnection.DeclareDurableQueue(rejectedQueue.Name, rejectedQueue.Exchange, rejectedQueue.RoutingKey);

                var errorsQueue = new QueueDescriptor
                {
                    Name = _busControllerConfig.BuildServerErrorsQueueName(serverQueueDescriptor.RoutingKey),
                    RoutingKey = _busControllerConfig.BuildServerErrorQueueRoute(serverQueueDescriptor.RoutingKey),
                    Exchange = this._serverInnerExchangeName
                };
                this._busConnection.DeclareDurableQueue(errorsQueue.Name, errorsQueue.Exchange, errorsQueue.RoutingKey);

                var trashQueue = new QueueDescriptor
                {
                    Name = _busControllerConfig.BuildServerTrashQueueName(serverQueueDescriptor.RoutingKey),
                    RoutingKey = _busControllerConfig.BuildServerTrashQueueRoute(serverQueueDescriptor.RoutingKey),
                    Exchange = this._serverInnerExchangeName
                };
                this._busConnection.DeclareDurableQueue(trashQueue.Name, trashQueue.Exchange, trashQueue.RoutingKey);

                

                var busConfig = new BusConnectionConfig
                {
                    ApplicationName = _environment.GetAppName(),
                    ConnectionName = this.Tag + $".[Consumers].[{serverQueueDescriptor.RoutingKey}]",
                    AutoRecovery = true,
                    HostName = _busControllerConfig.BusHost,
                    VirtualHost = _busControllerConfig.BusVirtualHost,
                    Port = _busControllerConfig.BusPort,
                    UserName = _busControllerConfig.BusUser,
                    Password = _busControllerConfig.BusPassword
                };
                var consumerConnection = _busConnectionFactory.Create(busConfig);
                _consumerConnections.Add(consumerConnection);

                for (int i = 0; i < serverQueueDescriptor.ConsumerCount; i++)
                {
                    var consumer = new SdrnQueueConsumer(
                        tag: $"CS.[{serverQueueDescriptor.RoutingKey}].[#{i}]",
                        routingKey: serverQueueDescriptor.RoutingKey,
                        queue: workQueue.Name,
                        messageConverter: _messageConverter,
                        handlerLibrary: _handlerLibrary,
                        busConnection: consumerConnection,
                        busControllerConfig: _busControllerConfig,
                        servicesResolver: _servicesResolver,
                        logger: _logger);
                    consumers.Add(consumer);
                }
            }

            this._logger.Verbouse(Contexts.ThisComponent, Categories.Declaring, $"The consumers are declared successfully: count = {consumers.Count}");

            return consumers.ToArray();
        }

        public SdrnMessageDispatcherState State { get; private set; }

        public string Tag { get; }

        public void Activate()
        {
            if (this.State == SdrnMessageDispatcherState.Activated)
            {
                return;
            }

            foreach (var consumer in _consumers)
            {
                consumer.Join();
            }

            this.State = SdrnMessageDispatcherState.Activated;
        }

        public void Deactivate()
        {
            if (this.State == SdrnMessageDispatcherState.Deactivated)
            {
                return;
            }

            foreach (var consumer in _consumers)
            {
                consumer.Unjoin();
            }

            this.State = SdrnMessageDispatcherState.Deactivated;
        }

        public void Dispose()
        {
            if (_consumerConnections != null && _consumerConnections.Count > 0)
            {
                foreach (var connection in _consumerConnections)
                {
                    connection.Dispose();
                }
                _consumerConnections.Clear();
            }

            if (_busConnection != null)
            {
                _busConnection.Dispose();
                _busConnection = null;
            }
        }

        public void RegistryHandler(Type handlerType)
        {
            _handlerLibrary.RegistryHandler(handlerType);
        }
    }
}
