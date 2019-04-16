using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.DataBus.Messages
{
    class PrivateMessageType1 : MessageTypeBase
    {
        public PrivateMessageType1() 
            : base("private_message_type_1", QueueType.Private)
        {
        }
    }
    class PrivateMessageType2 : MessageTypeBase
    {
        public PrivateMessageType2()
            : base("private_message_type_2", QueueType.Private)
        {
        }
    }


    class PrivateMessageType3 : MessageTypeBase
    {
        public PrivateMessageType3()
            : base("private_message_type_3", QueueType.Private)
        {
        }
    }
}
