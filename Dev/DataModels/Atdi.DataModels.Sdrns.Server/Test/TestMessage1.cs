using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Test
{
    public class TestMessage1 : MessageTypeBase
    {
        public TestMessage1() 
            : base("test_message_1")
        {
        }
    }
}
