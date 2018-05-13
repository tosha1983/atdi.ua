using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    public class ServicesContainerConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]   
        public string TypeProperty
        {
            get => (string)base["type"]; 
            set => base["type"] = value;
        }

        [ConfigurationProperty("components")]
        public ComponentsConfigElement ComponentsSection => ((ComponentsConfigElement)(base["components"]));
    }
}
