using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.DataBus.Messages
{
    class SpecificMessageType1 : MessageTypeBase
    {
        public SpecificMessageType1()
            : base("specific_message_type_1", QueueType.Specific, "specific_queue_1")
        {
        }
    }

    class SpecificMessageType2 : MessageTypeBase
    {
        public SpecificMessageType2()
            : base("specific_message_type_2", QueueType.Specific, "specific_queue_2")
        {
        }
    }

    class SpecificMessageType3 : MessageTypeBase
    {
        public SpecificMessageType3() 
            : base("specific_message_type_3", QueueType.Specific, "specific_queue_3")
        { 
        }
    }
}
