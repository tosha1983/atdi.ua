using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Test
{
    public class TestMessage3 : MessageTypeBase
    {
        public TestMessage3() 
            : base("test_message_3", QueueType.Specific, "specific_queue_1")
        {
        }
    }
}
