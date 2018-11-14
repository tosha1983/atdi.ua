using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using XMLLibrary;
using RabbitMQ.Client;

namespace Atdi.SDNRS.AppServer.BusManager
{
    public class ClassStaticBus
    {
        // основная шина для обмена данными

        public static ConnectionFactory factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword, VirtualHost = GlobalInit.RabbitVirtualHost, SocketReadTimeout = 2147000000, SocketWriteTimeout = 2147000000 };
        public static IBus bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
        public static List<string> List_Queue = new List<string>();

    }
}
