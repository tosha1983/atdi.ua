using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.MessageBus
{
    public class MessageObject
    {
        public Type Type { get; set; }

        public object Object { get; set; }
    }
}
