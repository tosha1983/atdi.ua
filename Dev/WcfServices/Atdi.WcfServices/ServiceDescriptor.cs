using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices
{
    class ServiceDescriptor
    {
        public ServiceDescriptor(Type serviceType, IComponentConfig config)
        {
            this.ServiceType = serviceType;
            this.ContractType = serviceType.BaseType.GenericTypeArguments[0];

            var contractParams = config[this.ContractType.Name];
            if (contractParams != null)
            {
                var endpointsString = Convert.ToString(contractParams);
                if (!string.IsNullOrEmpty(endpointsString))
                {
                    var endpoints = endpointsString.Split(new string[] { "; ", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (endpoints.Length > 0)
                    {
                        this.Endpoints = endpoints.Select(es => new Endpoint(es)).ToArray();
                    }
                }
            }
        }
        public Type ContractType { get; set; }

        public Type ServiceType { get; set; }

        public Endpoint[] Endpoints { get; }

        public override string ToString()
        {
            return $"[Contract='{ContractType.Name}', Service='{ServiceType.FullName}']";
        }
    }
}
