
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;
using Atdi.Platform.ConfigElements;

namespace Atdi.Platform.AppServer
{
    class ServerConfig : IServerConfig
    {
        private readonly AppServerConfigElement _config;
        private readonly IComponentConfig[] _components;

        public ServerConfig(AppServerConfigElement config)
        {
            this._config = config;

            if (config.ComponentsSection != null && config.ComponentsSection.Count > 0)
            {
                this._components = new IComponentConfig[config.ComponentsSection.Count];
                int index = 0;
                foreach (AppServerComponentConfigElement item in config.ComponentsSection)
                {
                    this._components[index++] = new ComponentConfig(item);
                }
            }
            else
            {
                this._components = new IComponentConfig[] { };
            }
        }
        public object this[string propertyName]
        {
            get
            {
                if (_config.PropertiesSection == null)
                {
                    return null;
                }
                var property = _config.PropertiesSection.GetProperty(propertyName);
                if (property == null)
                {
                    return null;
                }

                return property.ValueProperty;

            }
        }
            

        public string Instance => this._config.InstanceProperty;

        public IComponentConfig[] Components => _components;
    }
}
