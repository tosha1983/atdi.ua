using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using XMLLibrary;

namespace Atdi.SDNRS.AppServer.BusManager
{
    public class ClassStaticBus
    {
        // основная шина для обмена данными
        public static IBus bus = RabbitHutch.CreateBus(GlobalInit.MainRabbitMQServices);
        public static List<string> List_Queue = new List<string>();
     
    }
}
