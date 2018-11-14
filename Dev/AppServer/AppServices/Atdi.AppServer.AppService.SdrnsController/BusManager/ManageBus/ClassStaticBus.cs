using System.Collections.Generic;
using RabbitMQ.Client;

namespace Atdi.SDNRS.AppServer.BusManager
{
    public class ClassStaticBus
    {
        public static ConnectionFactory factory = new ConnectionFactory() { HostName = GlobalInit.RabbitHostName, UserName = GlobalInit.RabbitUserName, Password = GlobalInit.RabbitPassword, VirtualHost = GlobalInit.RabbitVirtualHost, SocketReadTimeout = 2147000000, SocketWriteTimeout = 2147000000 };
        public static List<string> List_Queue = new List<string>();
    }
}
