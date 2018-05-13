using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    public class ComponentConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("service", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string ServiceProperty
        {
            get => (string)base["service"];
            set => base["service"] = value;
        }

        [ConfigurationProperty("type", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string TypeProperty
        {
            get => (string)base["type"];
            set => base["type"] = value;
        }

        [ConfigurationProperty("lifetime", DefaultValue = "Transient", IsKey = false, IsRequired = true)]
        public DependencyInjection.ServiceLifetime LifetimeProperty
        {
            get => (DependencyInjection.ServiceLifetime)base["lifetime"];
            set => base["lifetime"] = value;
        }

        [ConfigurationProperty("parameters")]
        public ParametersConfigElement ParametersSection => ((ParametersConfigElement)(base["parameters"]));

    }
}
