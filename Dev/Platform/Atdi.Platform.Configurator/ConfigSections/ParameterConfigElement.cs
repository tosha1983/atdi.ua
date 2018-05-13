using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    public class ParameterConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string NameProperty
        {
            get => (string)base["name"];
            set => base["name"] = value;
        }

        [ConfigurationProperty("value", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string ValueProperty
        {
            get => (string)base["value"];
            set => base["value"] = value;
        }
    }
}
