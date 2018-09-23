using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class MessageToken : IMessageToken
    {
        public MessageToken(string id, string type)
        {
            this.Id = id;
            this.Type = type;
        }

        public string Id { get; }

        public string Type { get; }
    }
}
