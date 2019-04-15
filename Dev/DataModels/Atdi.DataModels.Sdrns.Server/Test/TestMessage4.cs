using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Test
{
    public class TestMessage4 : MessageTypeBase
    {
        public TestMessage4() 
            : base("test_message_4", QueueType.Specific, "specific_queue_2")
        {
        }
    }
}
