﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    public sealed class MessageBuilder
    {
        internal MessageBuilder()
        {
        }

        public IDeliveryMessage Create()
        {
            return new DeliveryMessage();
        }

        
    }
}
