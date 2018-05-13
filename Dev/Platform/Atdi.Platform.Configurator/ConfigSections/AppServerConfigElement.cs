using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    public class AppServerConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("instance", DefaultValue = "", IsRequired = false)]   
        public string InstanceProperty
        {
            get => (string)base["instance"]; 
            set => base["instance"] = value;
        }

        [ConfigurationProperty("properties")]
        public PropertiesConfigElement PropertiesSection => ((PropertiesConfigElement)(base["properties"]));

        [ConfigurationProperty("components")]
        public AppServerComponentsConfigElement ComponentsSection => ((AppServerComponentsConfigElement)(base["components"]));
    }
}
