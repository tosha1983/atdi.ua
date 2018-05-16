using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices
{
    class Endpoint
    {

        // {binding=basicHttpBinding, address=http://localhost:8734/Atdi.WcfServices/AuthenticationManager/}
        public Endpoint(string endpointString)
        {
            
            if (string.IsNullOrEmpty(endpointString))
            {
                throw new ArgumentNullException(nameof(endpointString));
            }
            var value = endpointString;
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
                throw new ArgumentException($"Incorrect endpoint string '{endpointString}'");
            }

            foreach (var part in parts)
            {
                var attrs = part.Split(new string[] { "=", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (attrs.Length != 2)
                {
                    throw new ArgumentException($"Incorrect endpoint string '{endpointString}' in part '{part}'");
                }

                if("binding".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                {
                    this.Binding = attrs[1];
                }
                else if ("address".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                {
                    this.Address = attrs[1];
                }
            }
        }

        public string Address { get; private set; }

        public string Binding { get; private set; }
    }
}
