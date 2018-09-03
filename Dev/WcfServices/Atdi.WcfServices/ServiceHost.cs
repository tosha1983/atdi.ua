using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;
using System.ServiceModel;

namespace Atdi.WcfServices
{
    sealed class ServiceHost : IDisposable
    {
        private readonly ServiceDescriptor _serviceDescriptor;
        private readonly IWcfServicesResolver _servicesResolver;
        private ServiceHostBase _serviceHost;

        public ServiceHost(ServiceDescriptor serviceDescriptor, IWcfServicesResolver servicesResolver)
        {
            this._serviceDescriptor = serviceDescriptor;
            this._servicesResolver = servicesResolver;

            var baseAdress = new Uri[0];
            var endpoints = serviceDescriptor.Endpoints;
            if (endpoints != null && endpoints.Length >0 )
            {
                baseAdress = endpoints
                    .Where(d => !string.IsNullOrEmpty(d.Address))
                    .Select(d => new Uri(d.Address))
                    .ToArray();
            }

            this._serviceHost = servicesResolver.CreateWcfServiceHost<ServiceHostBase>(serviceDescriptor.ContractType.AssemblyQualifiedName, baseAdress);

            
        }

        public void Open()
        {
            if (_serviceHost == null)
            {
                throw new InvalidOperationException(Exceptions.ServiceHostWasNotInitialized);
            }

            if (this._serviceHost.State == CommunicationState.Opened)
            {
                return;
            }

            if (this._serviceHost.State == CommunicationState.Created || this._serviceHost.State == CommunicationState.Closed)
            {
                _serviceHost.Open();
            }
        }

        public void Close()
        {
            if (_serviceHost == null)
            {
                throw new InvalidOperationException(Exceptions.ServiceHostWasNotInitialized);
            }

            if (this._serviceHost.State == CommunicationState.Closed || this._serviceHost.State == CommunicationState.Created)
            {
                return;
            }

            if (this._serviceHost.State == CommunicationState.Opened || this._serviceHost.State == CommunicationState.Opening)
            {
                _serviceHost.Close();
            }
        }

        public void Dispose()
        {
            if (_serviceHost == null)
            {
                return;
            }

            if (this._serviceHost.State == CommunicationState.Opened || this._serviceHost.State == CommunicationState.Opening)
            {
                _serviceHost.Close();
            }
            _serviceHost = null;

        }

        public override string ToString()
        {
            return $"Descriptor='{_serviceDescriptor.ToString()}'" ;
        }
    }
}
