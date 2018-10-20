using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.AmqpBroker;


namespace Atdi.Test.Modules.Sdrn.AmqpBroker
{
    public partial class Form1 : Form, IBrokerObserver , IDeliveryHandler
    {
        private ConnectionFactory _factory;
        private Connection _consumersConnection = null;
        private Connection _publisherConnection = null;
        private Channel _consumersChannel1 = null;
        private Channel _consumersChannel2 = null;
        private Channel _consumersChannel3 = null;
        private Channel _publisherChannel = null;
        private SynchronizationContext _synchronizationContext;

        public Form1()
        {
            InitializeComponent();
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;

            _factory = new ConnectionFactory(this);
        }

        public void OnEvent(IBrokerEvent busEvent)
        {
            _synchronizationContext.Post(
            (o) => { txtLog.AppendText(busEvent.ToString()); }, null);

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var config = new ConnectionConfig
            {
                HostName = txtRabbitMQHost.Text,
                VirtualHost = txtRabbitMQVisrtHost.Text,
               // ApplicationName = txtRabbitMQAppName.Text,
                ConnectionName = txtRabbitMQConnectName.Text + ".[Publishers]",
                UserName = txtRabbitMQUser.Text,
                Password = txtRabbitMQPassword.Text
            };

            this._publisherConnection = _factory.Create(config);
            this._publisherChannel = this._publisherConnection.CreateChannel();
           
            config = new ConnectionConfig
            {
                HostName = txtRabbitMQHost.Text,
                VirtualHost = txtRabbitMQVisrtHost.Text,
               // ApplicationName = txtRabbitMQAppName.Text,
                ConnectionName = txtRabbitMQConnectName.Text + ".[Consumers]",
                UserName = txtRabbitMQUser.Text,
                Password = txtRabbitMQPassword.Text
            };

            this._consumersConnection = _factory.Create(config);

            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;

            if (_publisherChannel != null)
            {
                _publisherChannel.Dispose();
                _publisherChannel = null;
            }

            if (_publisherConnection != null)
            {
                _publisherConnection.Dispose();
                _publisherConnection = null;
            }

            if (_consumersConnection != null)
            {
                _consumersConnection.Dispose();
                _consumersConnection = null;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _synchronizationContext =
            WindowsFormsSynchronizationContext.Current;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_publisherConnection != null)
            {
                _publisherConnection.Dispose();
            }
            if (_consumersConnection != null)
            {
                _consumersConnection.Dispose();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_publisherChannel != null)
            {
                _publisherChannel.DeclareDurableDirectExchange(txtExchange.Text);
            }
                
        }

        //Atdi.Modules.Sdrn.AmqpBroker.MessageHandlingResult IMessageHandler.Handle(Atdi.Modules.Sdrn.MessageBus.Message message, IDeliveryContext deliveryContext)
        //{
        //    var tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
        //    _synchronizationContext.Post(
        //    (o) => 
        //    {
        //        txtMsg.AppendText($"#{tid} >>> Consumer: '{deliveryContext.ConsumerTag}'; DeliveryTag: '{deliveryContext.DeliveryTag}'; Exchange: '{deliveryContext.Exchange}'; RoutingKey: '{deliveryContext.RoutingKey}'; AppID: '{message.AppId}'; Message ID: '{message.Id}'" + Environment.NewLine);

        //    }, null);

            

        //    return Atdi.Modules.Sdrn.AmqpBroker.MessageHandlingResult.Confirm;
        //}

        private void button4_Click(object sender, EventArgs e)
        {
            if (_consumersConnection != null)
            {
                _consumersChannel1 = _consumersConnection.CreateChannel();
                _consumersChannel2 = _consumersConnection.CreateChannel();
                _consumersChannel3 = _consumersConnection.CreateChannel();

                _consumersChannel1.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#1]", this);
                _consumersChannel1.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#2]", this);
                _consumersChannel1.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#3]", this);

                _consumersChannel2.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#1]", this);
                _consumersChannel2.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#2]", this);
                _consumersChannel2.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#3]", this);

                _consumersChannel3.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#1]", this);
                _consumersChannel3.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#2]", this);
                _consumersChannel3.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#3]", this);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_publisherChannel != null)
            {
                var msg = new DeliveryMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "SomeType",
                    AppId = "Test: " + Guid.NewGuid().ToString()
                };
                for (int i = 0; i < 1000; i++)
                {
                    msg.AppId = $"[#{i}]";
                    _publisherChannel.Publish("[SDRN.Test].[1]", "RK.[Test]", msg);
                }

            }
        }

        HandlingResult IDeliveryHandler.Handle(IDeliveryMessage message, IDeliveryContext deliveryContext)
        {
            var tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            _synchronizationContext.Post(
            (o) =>
            {
                txtMsg.AppendText($"#{tid} >>>[Channel #{deliveryContext.Channel.Number}].Consumer: '{deliveryContext.ConsumerTag}'; DeliveryTag: '{deliveryContext.DeliveryTag}'; Exchange: '{deliveryContext.Exchange}'; RoutingKey: '{deliveryContext.RoutingKey}'; AppID: '{message.AppId}'; Message ID: '{message.Id}'" + Environment.NewLine);

            }, null);



            return Atdi.Modules.AmqpBroker.HandlingResult.Confirm;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_publisherConnection != null)
            {
                _publisherConnection.Dispose();
                _publisherConnection = null;
            }
            if (_consumersConnection != null)
            {
                _consumersConnection.Dispose();
                _consumersConnection = null;
            }
        }
    }
}
