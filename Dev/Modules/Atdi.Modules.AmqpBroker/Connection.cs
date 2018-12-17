using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RBC = RabbitMQ.Client;
using RBE = RabbitMQ.Client.Events;


namespace Atdi.Modules.AmqpBroker
{
    public class Connection : IDisposable
    {
        private readonly ConnectionConfig _config;
        private readonly IBrokerObserver _logger;
        private readonly RBC.ConnectionFactory _connectionFactory;
        private RBC.IConnection _connection;

        internal Connection(ConnectionConfig config, IBrokerObserver logger)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._logger = logger;

            this._connectionFactory = new RBC.ConnectionFactory()
            {
                HostName = this._config.HostName,
                UserName = this._config.UserName,
                Password = this._config.Password,
            };

            this._connectionFactory.SocketReadTimeout *= 10;
            this._connectionFactory.SocketWriteTimeout *= 10;
            

            if (!string.IsNullOrEmpty(this._config.VirtualHost))
            {
                this._connectionFactory.VirtualHost = this._config.VirtualHost;
            }
            if (this._config.AutoRecovery.HasValue)
            {
                this._connectionFactory.AutomaticRecoveryEnabled = this._config.AutoRecovery.Value;
            }
            if (this._config.Port.HasValue)
            {
                this._connectionFactory.Port = this._config.Port.Value;
            }

            this.EstablishConnection();
        }

        internal ConnectionConfig Config { get => _config;  }
        internal RBC.IConnection RealConnection { get => _connection; }

        public Channel CreateChannel()
        {
            return new Channel(this, _logger);
        }

        public void EstablishConnection()
        {
            if (this._connection != null)
            {
                if (this._connection.IsOpen)
                {
                    return;
                }
                this.DisposeConnection();
            }

            try
            {
                this._connection = this._connectionFactory.CreateConnection(_config.ConnectionName);
                this._connection.ConnectionRecoveryError += _connection_ConnectionRecoveryError;
                this._connection.CallbackException += _connection_CallbackException;
                this._connection.ConnectionShutdown += _connection_ConnectionShutdown;
                this._connection.RecoverySucceeded += _connection_RecoverySucceeded;
                this._connection.ConnectionBlocked += _connection_ConnectionBlocked;
                this._connection.ConnectionUnblocked += _connection_ConnectionUnblocked;

                this._logger.Verbouse("RabbitMQ.EstablishConnection", $"The connection '{this._config.ConnectionName}' is established successfully", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.EstablishConnectionException, "RabbitMQ.EstablishConnection", e, this);
                throw new InvalidOperationException($"The connection '{this._config.ConnectionName}' to RabbitMQ is not established", e);
            }
        }

        private void _connection_ConnectionUnblocked(object sender, EventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.ConnectionUnblocked", $"The connection '{this._config.ConnectionName}' is unblocked", this);
        }

        private void _connection_ConnectionBlocked(object sender, RBE.ConnectionBlockedEventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.ConnectionBlocked", $"The connection '{this._config.ConnectionName}' is blocked", this);
        }

        private void _connection_ConnectionRecoveryError(object sender, RBE.ConnectionRecoveryErrorEventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.ConnectionRecoveryError", $"The exception is occurred: Connection: '{this._config.ConnectionName}', Message: '{e.Exception.Message}'", this);
            this._logger.Exception(BrokerEvents.ExceptionEvent, "RabbitMQ.ConnectionRecoveryError", e.Exception, this);
        }

        private void _connection_CallbackException(object sender, RBE.CallbackExceptionEventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.CallbackException", $"The exception is occurred: Connection: '{this._config.ConnectionName}', Message: '{e.Exception.Message}'", this);
            this._logger.Exception(BrokerEvents.ExceptionEvent, "RabbitMQ.CallbackException", e.Exception, this);
        }

        private void _connection_RecoverySucceeded(object sender, EventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.RecoveryConnection", $"The connection '{this._config.ConnectionName}' is recovered successfully", this);
        }

        private void _connection_ConnectionShutdown(object sender, RBC.ShutdownEventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.ShutdownConnection", $"The connection '{this._config.ConnectionName}' is shutted down: Initiator: '{e.Initiator}', Reasone: '{e.ReplyText}', Code: #{e.ReplyCode}", this);
        }

        private void CloseConnection(string reason)
        {
            if (this._connection != null)
            {
                if (this._connection.IsOpen)
                {
                    try
                    {
                        this._connection.Close(200, reason);
                    }
                    catch (Exception e)
                    {
                        this._logger.Exception(BrokerEvents.CloseConnectionException, "RabbitMQ.CloseConnection", e, this);
                    }
                }
            }
        }

        private void DisposeConnection()
        {
            if (this._connection != null)
            {
                try
                {
                    this._connection.Dispose();
                }
                catch (Exception e)
                {
                    this._logger.Exception(BrokerEvents.DisposeConnectionException, "RabbitMQ.DisposeConnection", e, this);
                }
                this._connection = null;
            }
        }

        public void Dispose()
        {
            this.CloseConnection("Connection.Dispose");
            this.DisposeConnection();
        }
    }

    
}
