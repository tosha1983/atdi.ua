using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using RabbitMQ.Client;

namespace Atdi.WcfServices.Sdrn.Device
{
    public class SdrnDeviceComponent : WcfServicesComponent
    {
        private SdrnServerDescriptor _serverDescriptor;

        public SdrnDeviceComponent() : base("SdrnDeviceServices", ComponentBehavior.SingleInstance)
        {
           
        }
        protected override void OnInstall()
        {
            base.OnInstall();
            this._serverDescriptor = new SdrnServerDescriptor(this.Config);
            this.Container.RegisterInstance(this._serverDescriptor, Platform.DependencyInjection.ServiceLifetime.Singleton);
            this.Container.Register<MessagesBus>(Platform.DependencyInjection.ServiceLifetime.PerThread);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = this._serverDescriptor.RabbitMqHost,
                    UserName = this._serverDescriptor.RabbitMqUser,
                    Password = this._serverDescriptor.RabbitMqPassword
                };

                using (var connection = factory.CreateConnection($"SDRN Device [{this._serverDescriptor.Instance}] (Declaring) #{System.Threading.Thread.CurrentThread.ManagedThreadId}"))
                using (var channel = connection.CreateModel())
                {
                    this.Logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The connection to Rabbit MQ Server was checked successfully: Host = '{this._serverDescriptor.RabbitMqHost}'");

                    var deviceExchange = $"{this._serverDescriptor.MessagesExchange}.[v{this._serverDescriptor.ApiVersion}]";

                    channel.ExchangeDeclare(
                        exchange: deviceExchange,
                        type: "direct",
                        durable: true
                    );

                    this.Logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The Exchange was declared successfully: Name = '{deviceExchange}'");

                    var bindings = this._serverDescriptor.QueueBindings.Values;
                    foreach (var binding in bindings)
                    {
                        var routingKey = $"[{this._serverDescriptor.ServerInstance}].[{binding.RoutingKey}]";
                        var queueName = $"{this._serverDescriptor.ServerQueueNamePart}.[{this._serverDescriptor.ServerInstance}].[{binding.RoutingKey}].[v{this._serverDescriptor.ApiVersion}]";

                        channel.QueueDeclare(
                           queue: queueName,
                           durable: true,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

                        channel.QueueBind(queueName, deviceExchange, routingKey);

                        this.Logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The queue was declared successfully: Name = '{queueName}', RoutingKey = '{routingKey}'");
                    }
                }
            }
            catch(Exception e)
            {
                this.Logger.Exception("SdrnDeviceServices", (EventCategory)"Activation",  e);
            }
            
        }

        protected override void OnUninstall()
        {
            this._serverDescriptor = null;
            base.OnUninstall();
        }
    }
}
