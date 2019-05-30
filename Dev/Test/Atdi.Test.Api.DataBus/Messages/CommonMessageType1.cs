using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Api.DataBus.Messages
{
    class CommonMessageType1 : MessageTypeBase
    {
        public CommonMessageType1() 
            : base("common_message_type_1")
        {
        }
    }

    class CommonMessageType2 : MessageTypeBase
    {
        public CommonMessageType2()
            : base("common_message_type_2")
        {
        }
    }

    class CommonMessageType3 : MessageTypeBase
    {
        public CommonMessageType3()
            : base("common_message_type_3")
        {
        }
    }
}
