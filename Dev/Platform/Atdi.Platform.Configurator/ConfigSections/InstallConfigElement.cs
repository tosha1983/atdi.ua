using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    public class InstallConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("type", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string TypeProperty
        {
            get => (string)base["type"];
            set => base["type"] = value;
        }

        [ConfigurationProperty("parameters")]
        public ParametersConfigElement ParametersSection => ((ParametersConfigElement)(base["parameters"]));

    }
}
