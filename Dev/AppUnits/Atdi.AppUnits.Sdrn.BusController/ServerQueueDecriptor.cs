using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class ServerQueueDecriptor
    {
        public ServerQueueDecriptor(string queuePart)
        {
            if (string.IsNullOrEmpty(queuePart))
            {
                throw new ArgumentNullException(nameof(queuePart));
            }
            var value = queuePart;
            if (value.StartsWith("{"))
            {
                value = value.Substring(1);
            }
            if (value.EndsWith("}"))
            {
                value = value.Substring(0, value.Length - 1);
            }

            var parts = value.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                throw new ArgumentException($"Incorrect queue part string '{queuePart}'");
            }

            foreach (var part in parts)
            {
                var attrs = part.Split(new string[] { "=", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (attrs.Length != 2)
                {
                    throw new ArgumentException($"Incorrect queue part string '{queuePart}' in part '{part}'");
                }

                if ("consumerCount".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                {
                    this.ConsumerCount = Convert.ToInt32(attrs[1]);
                }
                else if ("routingKey".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                {
                    this.RoutingKey = attrs[1];
                }
            }
        }

        public int ConsumerCount { get; set; }

        public string RoutingKey { get; set; }
    }
}
