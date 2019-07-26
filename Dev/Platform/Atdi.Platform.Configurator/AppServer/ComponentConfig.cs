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
        private class Parameter : IComponentConfigParameter
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }
        private readonly AppServerComponentConfigElement _config;

        public ComponentConfig(AppServerComponentConfigElement config)
        {
            this._config = config;
        }

        public object this[string paramName]
        {
            get
            {
                var parameter = this._config.ParametersSection.GetParameter(paramName);
                if (parameter == null)
                {
                    return null;
                }

                return parameter.ValueProperty;
            }
        }

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

        public IComponentConfigParameter[] Parameters
        {
            get
            {
                var result = new Parameter[this._config.ParametersSection.Count];
                int i = 0;
                foreach (ParameterConfigElement parameter in this._config.ParametersSection)
                {
                    result[i++] = new Parameter
                    {
                        Name = parameter.NameProperty,
                        Value = parameter.ValueProperty
                    };
                }
                return result;
            }
        }
    }
}
