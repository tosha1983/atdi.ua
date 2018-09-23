using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal sealed class MessagesBindingDecriptor
    {
        public MessagesBindingDecriptor(string bindingPart)
        {
            if (string.IsNullOrEmpty(bindingPart))
            {
                throw new ArgumentNullException(nameof(bindingPart));
            }
            var value = bindingPart;
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
                throw new ArgumentException($"Incorrect binding part string '{bindingPart}'");
            }

            foreach (var part in parts)
            {
                var attrs = part.Split(new string[] { "=", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (attrs.Length != 2)
                {
                    throw new ArgumentException($"Incorrect binding part string '{bindingPart}' in part '{part}'");
                }

                if ("messageType".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                {
                    this.MessageType = attrs[1];
                }
                else if ("routingKey".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                {
                    this.RoutingKey = attrs[1];
                }
            }
        }

        public string MessageType { get; set; }

        public string RoutingKey { get; set; }

        public override string ToString()
        {
            return $"{MessageType} => {RoutingKey}";
        }
    }
}
