using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    static class BrokerEvents
    {
        public static readonly int VebouseEvent = 0;
        public static readonly int ExceptionEvent = 1;
        public static readonly int ConfigParameterError = 2;
        public static readonly int EstablishConnectionException = 3;
        public static readonly int EstablishChannelException = 4;
        public static readonly int DeclareExchangeException = 5;
        public static readonly int DeclareQueueException = 6;
        public static readonly int JoinConsumerException = 7;
        public static readonly int UnjoinConsumerException = 8;
        public static readonly int CloseConnectionException = 9;
        public static readonly int DisposeConnectionException = 10;
        public static readonly int CloseChannelException = 11;
        public static readonly int DisposeChannelException = 12;
        public static readonly int PublishException = 13;
        public static readonly int UnjoinConsumersException = 14;
        public static readonly int DeclareQueueBindingException = 15;


    }
}
