using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class BusBufferConfig : IBusBufferConfig
    {
        public BusBufferConfig()
        {
            this.Type = BufferType.None;
        }

        public BufferType Type { get; set; }

        public string OutboxFolder { get; set; }

        public string ConnectionString { get; set; }

        public ContentType ContentType { get; set; }

        public override string ToString()
        {
            return $"Type = '{Type}', ContentType = '{ContentType}', OutboxFolder = '{OutboxFolder}'";
        }
    }
}
