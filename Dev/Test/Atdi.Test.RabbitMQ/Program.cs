using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Modules.AmqpBroker;

namespace Atdi.Test.RabbitMQ
{
    class Program
    {
        static ConnectionFactory _factory;
        static Connection _consumerConnection = null;
        static Connection _publisherConnection = null;
        static Channel _consumerChannel = null;
        static Channel _publisherChannel = null;

        static void Main(string[] args)
        {
            _factory = new ConnectionFactory(new BrokerObserver());

            var pubConfig = new ConnectionConfig
            {
                HostName = "10.1.1.131", //10.1.1.137", //"109.237.91.29", //"192.168.3.33", //"192.168.33.110", //"10.1.1.137",
                Port = 5672, // 5672,
                VirtualHost = "dev_2",
                ConnectionName = "[kovpak].[Publisher]",
                UserName = "SDR_Client", // "andrey",
                Password = "32Xr567", //"P@ssw0rd"
            };

            _publisherConnection = _factory.Create(pubConfig);
            _publisherChannel = _publisherConnection.CreateChannel();

            _publisherChannel.DeclareDurableDirectExchange("[ex.kovpak]");
            _publisherChannel.DeclareDurableQueue("[q.kovpak]", "[ex.kovpak]", "[r.kovpak]");

            var conConfig = new ConnectionConfig
            {
                HostName = "10.1.1.131", //10.1.1.137", //"109.237.91.29", //"192.168.3.33", //"192.168.33.110", //"10.1.1.137",
                Port = 5672, // 5672,
                VirtualHost = "dev_2",
                ConnectionName = "[kovpak].[Comsumer]",
                UserName = "SDR_Client", // "andrey",
                Password = "32Xr567", //"P@ssw0rd"
            };

            _consumerConnection = _factory.Create(conConfig);
            _consumerChannel = _consumerConnection.CreateChannel();
            _consumerChannel.JoinConsumer("[q.kovpak]", "[c.kovpak]", new DeliveryHandler());

           Task.Run(() => MakeMessages());

            Console.ReadLine();
        }

        static void MakeMessages()
        {
            var body = new byte[1024 * 1024 * 100];
            for (int i = 0; i < 4; i++)
            {
                var msg = new DeliveryMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = $"test-{i}",
                    AppId = "Test: " + Guid.NewGuid().ToString(),
                    Body = body
                };

                try
                {
                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    _publisherChannel.Publish("[ex.kovpak]", "[r.kovpak]", msg);
                    timer.Stop();
                    Console.WriteLine("  ---- send message --- : [" + msg.Type + "], Cost: " + timer.ElapsedMilliseconds.ToString() + "ms");
                }
                catch(Exception e)
                {
                    Console.WriteLine("  >>>> ERROR >>>> : " + e.Message);
                }
                
                
            }
        }
    }
}
