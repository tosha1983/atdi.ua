using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.ConfigElements;
using Atdi.Platform.DependencyInjection;

namespace Atdi.Platform
{
    public sealed class PlatformConfigurator : IPlatformConfigurator
    {
	    private static AppServer.IServerHost SingleHost;
		private static object _lockObject = new object();

        private readonly AtdiPlatformConfigElement _config;
        private readonly ITypeResolver _typeResolver;

        private PlatformConfigurator()
        {
            this._config = (AtdiPlatformConfigElement)System.Configuration.ConfigurationManager.GetSection("atdi.platform");
            this._typeResolver = new TypeResolver(this._config);
        }

        public IServicesContainer BuildContainer()
        {
            if (this._config.ServicesContainerSection == null)
            {
                throw new InvalidOperationException("Not found configuration section with name 'servicesContainer'");
            }

            var containerType = this._config.ServicesContainerSection.TypeProperty;

            if (string.IsNullOrEmpty(containerType))
            {
                throw new InvalidOperationException("Undefined a type of the services container into the config file");
            }

            var realType = _typeResolver.ResolveType(containerType);
            var containerInstance = (IServicesContainer)Activator.CreateInstance(realType);

            // regestry some services
            containerInstance.RegisterInstance<ITypeResolver>(this._typeResolver);

            // prepare components
            var containerComponents = this._config.ServicesContainerSection.ComponentsSection;
            if (containerComponents != null && containerComponents.Count > 0)
            {
                foreach (ComponentConfigElement containerComponent in containerComponents)
                {
                    this.RegistryComponent(containerInstance, containerComponent);
                }
            }

            // to install othe components
            var installers = this._config.InstallersSection;
            if (installers  != null && installers.Count > 0)
            {
                foreach (InstallConfigElement installer in installers)
                {
                    this.InstallPlatformComponent(containerInstance, installer);
                }
            }

            return containerInstance;
        }   

        private void RegistryComponent(IServicesContainer container, ComponentConfigElement component)
        {
            var serviceType = default(Type);
            if (!string.IsNullOrEmpty(component.ServiceProperty))
            {
                serviceType = _typeResolver.ResolveType(component.ServiceProperty);
            }

            if (string.IsNullOrEmpty(component.TypeProperty))
            {
                throw new InvalidOperationException("Undefined a type of service implementation for component");
            }

            var implementType = _typeResolver.ResolveType(component.TypeProperty);

            container.Register(serviceType, implementType, component.LifetimeProperty);
        }

        private void InstallPlatformComponent(IServicesContainer container, InstallConfigElement installConfig)
        {
            if (string.IsNullOrEmpty(installConfig.TypeProperty))
            {
                throw new InvalidOperationException("Undefined a type of the installer implementation for the platform component");
            }
            var installerType = _typeResolver.ResolveType(installConfig.TypeProperty);

            var installerInstance = (IPlatformInstaller)Activator.CreateInstance(installerType);

            var parameters = new Configurator.ConfigParameters(installConfig.ParametersSection);
            installerInstance.Install(container, parameters);
        }

        public static IPlatformConfigurator Create()
        {
            var configurator = new PlatformConfigurator();
            return configurator;
        }

        public AppServer.IServerConfig ServerConfig()
        {
            return new AppServer.ServerConfig(this._config.AppServerSection);
        }

        public static AppServer.IServerHost BuildHost()
        {
            var configurator = new PlatformConfigurator();
            var container = configurator.BuildContainer();
            var serverConfig = configurator.ServerConfig();
            container.RegisterInstance<AppServer.IServerConfig>(serverConfig);
            return container.GetResolver<IServicesResolver>().Resolve<AppServer.IServerHost>();
        }

        public static AppServer.IServerHost GetSingleHost()
        {
	        if (PlatformConfigurator.SingleHost != null)
	        {
		        return PlatformConfigurator.SingleHost;

	        }
			lock (_lockObject)
			{
				PlatformConfigurator.SingleHost = PlatformConfigurator.BuildHost();
				PlatformConfigurator.SingleHost.Start();
			}
			return PlatformConfigurator.SingleHost;
        }
    }
}
