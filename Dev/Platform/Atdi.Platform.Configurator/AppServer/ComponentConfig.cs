using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;
using Atdi.Platform.ConfigElements;

namespace Atdi.Platform.AppServer
{
    class ComponentConfig : IComponentConfig
    {
        private readonly AppServerComponentConfigElement _config;

        public ComponentConfig(AppServerComponentConfigElement config)
        {
            this._config = config;
        }

        public object this[string paramName] => this._config.ParametersSection.GetParameter(paramName);

        public string Instance
        {
            get
            {
                if (string.IsNullOrEmpty(this._config.InstanceProperty))
                {
                    return this._config.TypeProperty;
                }
                return this._config.InstanceProperty;
            }
        }

        public string Type => this._config.TypeProperty;

        public string Assembly => this._config.AssemblyProperty;
    }
}
