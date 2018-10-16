using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    static class EventSystemEvents
    {
        public static readonly int VebouseEvent = 0;
        public static readonly int ExceptionEvent = 1;
        public static readonly int ConfigParameterError = 2;
        public static readonly int NotEstablishConnectionToRabbit = 3;
        public static readonly int NotEstablishRabbitSharedChannel = 4;
        public static readonly int NotDeclareExchange = 5;
        public static readonly int NotCreateConsumer = 6;


    }
}
