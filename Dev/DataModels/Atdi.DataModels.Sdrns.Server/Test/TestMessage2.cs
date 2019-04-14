using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Test
{
    public class TestMessage2 : MessageTypeBase
    {
        public TestMessage2() 
            : base("test_message_2", QueueType.Private)
        {
        }
    }
}
