using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace Atdi.WcfServices
{
    public class WcfServicesComponent : ComponentBase

    {
        private List<ServiceHost> _serviceHosts;
        private IWcfServicesResolver _servicesResolver;

        public WcfServicesComponent(string name) : base(name, ComponentType.WcfServices, ComponentBehavior.SingleInstance)
        {

        }

        public WcfServicesComponent(string name, ComponentBehavior behavior) : base(name, ComponentType.WcfServices, behavior)
        {
            
        }

        protected override void OnInstall()
        {
            this._serviceHosts = new List<ServiceHost>();
            this._servicesResolver = this.Container.GetResolver<IWcfServicesResolver>();
            var typeResolver = this._servicesResolver.Resolve<ITypeResolver>();

            var servicesTypes = typeResolver.ResolveTypes(this.GetType().Assembly, typeof(WcfServiceBase<>));

            foreach (var serviceType in servicesTypes)
            {
                var descriptor = new ServiceDescriptor(serviceType, this.Config);
                this.Container.Register(descriptor.ContractType, descriptor.ServiceType, ServiceLifetime.PerWcfOperation);
                var serviceHost = new ServiceHost(descriptor, this._servicesResolver);
                this._serviceHosts.Add(serviceHost);
            }
        }

        protected override void OnActivate()
        {
            foreach (var serviceHost in this._serviceHosts)
            {
                try
                {
                    using (var scope = this.Logger.StartTrace(Contexts.WcfServicesComponent, Categories.Opening, TraceScopeNames.OpenHost))
                    {
                        serviceHost.Open();
                        scope.Trace(Events.ServiceHostDescriptor.With(serviceHost.ToString()));
                    }
                        
                }
                catch(Exception e)
                {
                    this.Logger.Exception(Contexts.WcfServicesComponent, Categories.Opening, Events.UnableToOpenHost.With(serviceHost.ToString()), e, this);
                }
            }
        }

        protected override void OnDeactivate()
        {
            foreach (var serviceHost in this._serviceHosts)
            {
                try
                {
                    serviceHost.Close();
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.WcfServicesComponent, Categories.Closing, Events.UnableToCloseHost.With(serviceHost.ToString()), e, this);
                }
            }
        }

        protected override void OnUninstall()
        {
            foreach (var serviceHost in this._serviceHosts)
            {
                try
                {
                    serviceHost.Dispose();
                }
                catch (Exception e)
                {
                    this.Logger.Exception(Contexts.WcfServicesComponent, Categories.Disposabling, Events.UnableToCloseHost.With(serviceHost.ToString()), e, this);
                }
            }

            this._serviceHosts = null;
            this._servicesResolver = null;
        }
    }
}
