using System;

namespace Atdi.Modules.AmqpBroker
{

    [Flags]
    public enum BrokerEventLevel
    {

        None = 0,

        Verbouse = 1,

        Info = 2,

        Warning = 4,

        Trace = 8,

        Debug = 16,

        Error = 32,

        Exception = 64,
    
        Critical = 128
    }
}
