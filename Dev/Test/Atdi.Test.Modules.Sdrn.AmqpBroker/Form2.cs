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
using Atdi.Modules.Sdrn.AmqpBroker;
using Atdi.Modules.Sdrn.MessageBus;
using Message = Atdi.Modules.Sdrn.MessageBus.Message;

namespace Atdi.Test.Modules.Sdrn.AmqpBroker
{
    public partial class Form2 : Form, IBrokerObserver, IMessageHandler
    {
        private BusConnectionFactory _factory;
        private BusConnection _consumersConnection = null;
        private BusConnection _publisherConnection = null;
        private SynchronizationContext _synchronizationContext;

        public Form2()
        {
            InitializeComponent();
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;

            _factory = new BusConnectionFactory(this);
        }

        public void OnEvent(IBrokerEvent busEvent)
        {
            _synchronizationContext.Post(
            (o) => { txtLog.AppendText(busEvent.ToString()); }, null);

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var config = new BusConnectionConfig
            {
                HostName = txtRabbitMQHost.Text,
                VirtualHost = txtRabbitMQVisrtHost.Text,
                ApplicationName = txtRabbitMQAppName.Text,
                ConnectionName = txtRabbitMQConnectName.Text + ".[Publisher]",
                UserName = txtRabbitMQUser.Text,
                Password = txtRabbitMQPassword.Text
            };

            this._publisherConnection = _factory.Create(config);

           
            config = new BusConnectionConfig
            {
                HostName = txtRabbitMQHost.Text,
                VirtualHost = txtRabbitMQVisrtHost.Text,
                ApplicationName = txtRabbitMQAppName.Text,
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
            if (_publisherConnection != null)
            {
                _publisherConnection.DeclareDurableDirectExchange(txtExchange.Text);
            }
                
        }

        Atdi.Modules.Sdrn.AmqpBroker.MessageHandlingResult IMessageHandler.Handle(Atdi.Modules.Sdrn.MessageBus.Message message, IDeliveryContext deliveryContext)
        {
            var tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            _synchronizationContext.Post(
            (o) => 
            {
                txtMsg.AppendText($"#{tid} >>> Consumer: '{deliveryContext.ConsumerTag}'; DeliveryTag: '{deliveryContext.DeliveryTag}'; Exchange: '{deliveryContext.Exchange}'; RoutingKey: '{deliveryContext.RoutingKey}'; AppID: '{message.AppId}'; Message ID: '{message.Id}'" + Environment.NewLine);

            }, null);

            

            return Atdi.Modules.Sdrn.AmqpBroker.MessageHandlingResult.Confirm;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_consumersConnection != null)
            {
                _consumersConnection.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#1]", this);
                _consumersConnection.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#2]", this);
                _consumersConnection.JoinConsumer("Q.SDRN.Server.[Test]", "CS.[Test].[#3]", this);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_publisherConnection != null)
            {
                var msg = new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "SomeType",
                    AppId = "Test: "  + Guid.NewGuid().ToString()
                };
                for (int i = 0; i < 10000; i++)
                {
                    msg.AppId = $"[#{i}]";
                    _publisherConnection.Publish("[SDRN.Test].[1]", "RK.[Test]", msg);
                }
                
            }
        }
    }
}
