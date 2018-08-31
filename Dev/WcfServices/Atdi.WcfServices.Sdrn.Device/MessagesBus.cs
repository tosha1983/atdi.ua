using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using RabbitMQ.Client;
using Newtonsoft;
using Newtonsoft.Json;

namespace Atdi.WcfServices.Sdrn.Device
{
    public class MessagesBus : IDisposable
    {
        private bool disposedValue = false; 
        private readonly ILogger _logger;
        private readonly SdrnServerDescriptor _serverDescriptor;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private readonly string _exchangeName;

        
        public MessagesBus(SdrnServerDescriptor serverDescriptor, ILogger logger)
        {
            this._logger = logger;
            this._serverDescriptor = serverDescriptor;
            this._connectionFactory= new ConnectionFactory()
            {
                HostName = this._serverDescriptor.RabbitMqHost,
                UserName = this._serverDescriptor.RabbitMqUser,
                Password = this._serverDescriptor.RabbitMqPassword
            };

            this._connection = this._connectionFactory.CreateConnection();

            this._exchangeName = $"{this._serverDescriptor.MessagesExchange}.[{this._serverDescriptor.ApiVersion}]";
        }

        public void SendObject<IObject>(SensorDescriptor descriptor, string messageType, IObject msgObject)
        {
            try
            {
                var data = this.SerializeObjectToByteArray(msgObject);

                var msgKey = this._serverDescriptor.QueueBindings[messageType].RoutingKey;

                var routingKey = $"[{this._serverDescriptor.ServerInstance}].[{msgKey}]";


                if (!this._connection.IsOpen)
                {
                    this._connection.Dispose();
                    this._connection = this._connectionFactory.CreateConnection();
                }

                using (var channel = this._connection.CreateModel())
                {
                    var props = channel.CreateBasicProperties();

                    props.Persistent = true;
                    props.AppId = "Atdi.WcfServices.Sdrn.Device.dll";
                    props.Headers = new Dictionary<string, object>();
                    props.Headers["SdrnServer"] = descriptor.SdrnServer;
                    props.Headers["SensorName"] = descriptor.SensorName;
                    props.Headers["SensorTechId"] = descriptor.EquipmentTechId;
                    props.Headers["MessageType"] = messageType;

                    channel.BasicPublish(exchange: this._exchangeName,
                                     routingKey: routingKey,
                                     basicProperties: props,
                                     body: data);

                    this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The message was sent successfuly: MessageType = '{messageType}', RoutingKey = '{routingKey}', SDRN Server = '{descriptor.SdrnServer}', Sensor = '{descriptor.SensorName}'");
                }

            }
            catch (Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"Object sending", e);
            }
        }

        private byte[] SerializeObjectToByteArray<TObject>(TObject source)
        {
            byte[] result = null;

            var json = JsonConvert.SerializeObject(source);

            result = Encoding.UTF8.GetBytes(json);

            return result;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this._connection != null)
                    {
                        if (this._connection.IsOpen)
                        {
                            this._connection.Close();
                        }
                        this._connection.Dispose();
                        this._connection = null;
                    }
                }
                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

    }
}
