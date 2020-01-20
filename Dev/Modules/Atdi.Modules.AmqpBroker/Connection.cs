using System;
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

            this._connectionFactory.SocketReadTimeout *= 100;
            this._connectionFactory.SocketWriteTimeout *= 100;
            this._connectionFactory.RequestedConnectionTimeout *= 100;
            //this._connectionFactory.
            //this._connectionFactory.RequestedFrameMax = 1024 * 1024; 

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

        internal ConnectionConfig Config => _config;

        public Channel CreateChannel()
        {
            return new Channel(this, _logger);
        }

        public RBC.IModel CreateAmqpChannel()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException($"The connection is not opened: {_config}");
            }
            return _connection.CreateModel();
        }

        public bool IsOpen
        {
            get
            {
                if (this._connection != null)
                {
                    return this._connection.IsOpen;
                }
                return false;
            }
        }

        public override string ToString()
        {
            if (IsOpen)
            {
                try
                {
                    return $"{_config}, Opened={this.IsOpen} LocalPort=#{_connection?.LocalPort}, RemotePort=#{_connection?.RemotePort}";
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return $"{_config}, Opened={this.IsOpen}";
        }

        private void EstablishConnection()
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
                //var c = 0;
                //var v = 1 / c;
                this._connection = this._connectionFactory.CreateConnection(_config.ConnectionName);

                this._connection.ConnectionRecoveryError += _connection_ConnectionRecoveryError;
                this._connection.CallbackException += _connection_CallbackException;
                this._connection.ConnectionShutdown += _connection_ConnectionShutdown;
                this._connection.RecoverySucceeded += _connection_RecoverySucceeded;
                this._connection.ConnectionBlocked += _connection_ConnectionBlocked;
                this._connection.ConnectionUnblocked += _connection_ConnectionUnblocked;

                this._logger.Info("AmqpBroker.ConnectionEstablishment", $"The connection is established successfully: {this}", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.EstablishConnectionException, "AmqpBroker.ConnectionEstablishment", e, this);
                throw new InvalidOperationException($"The connection is not established: {_config}", e);
            }
        }

        private void _connection_ConnectionUnblocked(object sender, EventArgs e)
        {
            this._logger.Verbouse("AmqpBroker.ConnectionUnblocked", $"The connection is unblocked: {this}", this);
        }

        private void _connection_ConnectionBlocked(object sender, RBE.ConnectionBlockedEventArgs e)
        {
            this._logger.Verbouse("AmqpBroker.ConnectionBlocked", $"The connection is blocked: {this}", this);
        }

        private void _connection_ConnectionRecoveryError(object sender, RBE.ConnectionRecoveryErrorEventArgs e)
        {
            this._logger.Verbouse("AmqpBroker.ConnectionRecoveryError", $"The exception is occurred: {this}, Message = '{e.Exception.Message}'", this);
            this._logger.Exception(BrokerEvents.ExceptionEvent, "AmqpBroker.ConnectionRecoveryError", e.Exception, this);
        }

        private void _connection_CallbackException(object sender, RBE.CallbackExceptionEventArgs e)
        {
            this._logger.Verbouse("AmqpBroker.CallbackException", $"The exception is occurred: {_config}, Message = '{e.Exception.Message}'", this);
            this._logger.Exception(BrokerEvents.ExceptionEvent, "AmqpBroker.CallbackException", e.Exception, this);
        }

        private void _connection_RecoverySucceeded(object sender, EventArgs e)
        {
            this._logger.Info("AmqpBroker.ConnectionRecovery", $"The connection is recovered successfully: {this}", this);
        }

        private void _connection_ConnectionShutdown(object sender, RBC.ShutdownEventArgs e)
        {
            this._logger.Info("AmqpBroker.ConnectionShutdown", $"The connection is shutdown: Initiator='{e.Initiator}', Reason='{e.ReplyText}', Code=#{e.ReplyCode}, {this}", this);
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
                        this._logger.Info("AmqpBroker.ConnectionClosing", $"The connection is closed successfully: {this}", this);
                    }
                    catch (Exception e)
                    {
                        this._logger.Exception(BrokerEvents.CloseConnectionException, "AmqpBroker.ConnectionClosing", e, this);
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
                    this._connection.ConnectionRecoveryError -= _connection_ConnectionRecoveryError;
                    this._connection.CallbackException -= _connection_CallbackException;
                    this._connection.ConnectionShutdown -= _connection_ConnectionShutdown;
                    this._connection.RecoverySucceeded -= _connection_RecoverySucceeded;
                    this._connection.ConnectionBlocked -= _connection_ConnectionBlocked;
                    this._connection.ConnectionUnblocked -= _connection_ConnectionUnblocked;
                    this._connection.Dispose();

                    this._logger.Verbouse("AmqpBroker.ConnectionDisposing", $"The connection is disposed successfully: {_config}", this);
                }
                catch (Exception e)
                {
                    this._logger.Exception(BrokerEvents.DisposeConnectionException, "AmqpBroker.ConnectionDisposing", e, this);
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
