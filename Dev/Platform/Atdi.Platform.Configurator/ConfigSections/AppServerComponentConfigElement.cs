using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    public class AppServerComponentConfigElement : ConfigurationElement
    {

        [ConfigurationProperty("type", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string TypeProperty
        {
            get => (string)base["type"];
            set => base["type"] = value;
        }

        [ConfigurationProperty("assembly", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string AssemblyProperty
        {
            get => (string)base["assembly"];
            set => base["assembly"] = value;
        }

        [ConfigurationProperty("instance", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string InstanceProperty
        {
            get => (string)base["instance"];
            set => base["instance"] = value;
        }

        [ConfigurationProperty("parameters")]
        public ParametersConfigElement ParametersSection => ((ParametersConfigElement)(base["parameters"]));

    }
}
