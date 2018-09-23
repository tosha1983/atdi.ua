using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Modules.Licensing;
using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using RabbitMQ.Client;
using MMB = Atdi.Modules.Sdrn.MessageBus;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.WcfServices.Sdrn.Device
{
    public class SdrnDeviceComponent : WcfServicesComponent
    {
        private SdrnServerDescriptor _serverDescriptor;

        public SdrnDeviceComponent() : base("SdrnDeviceServices", ComponentBehavior.SingleInstance)
        {
           
        }

        private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetAssembly(this.GetType()).CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        protected override void OnInstall()
        {
            base.OnInstall();

            try
            {
                var productKey = this.Config.GetParameterAsDecodeString("License.ProductKey", "Atdi.WcfServices.Sdrn.Device");
                var ownerId = this.Config.GetParameterAsDecodeString("License.OwnerId", "Atdi.WcfServices.Sdrn.Device");

                var verificationData = new VerificationData
                {
                    OwnerId = ownerId,
                    ProductName = "ICS Control Device",
                    ProductKey = productKey,
                    LicenseType = "DeviceLicense",
                    Date = DateTime.Now
                };

                var licenseFileName = this.Config.GetParameterAsString("License.FileName");
                licenseFileName = Path.Combine(this.AssemblyDirectory, licenseFileName);
                var licenseBody = File.ReadAllBytes(licenseFileName);

                var verResult = LicenseVerifier.Verify(verificationData, licenseBody);

                this._serverDescriptor = new SdrnServerDescriptor(this.Config, verResult.Instance);
            }
            catch(Exception e)
            {
                throw new InvalidOperationException("The license verification failed", e);
            }


            var convertorSettings = new MMB.MessageConvertSettings
            {
                UseEncryption = this.Config.GetParameterAsBoolean("SDRN.MessageConvertor.UseEncryption"),
                UseСompression = this.Config.GetParameterAsBoolean("SDRN.MessageConvertor.ompression")
            };
            var typeResolver = MMB.MessageObjectTypeResolver.CreateForApi20();
            var messageConvertor = new MMB.MessageConverter(convertorSettings, typeResolver);
            this.Container.RegisterInstance(messageConvertor, Platform.DependencyInjection.ServiceLifetime.Singleton);

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
