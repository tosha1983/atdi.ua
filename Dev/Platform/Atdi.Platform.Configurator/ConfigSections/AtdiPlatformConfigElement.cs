using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    public class AtdiPlatformConfigElement : ConfigurationSection
    {
        [ConfigurationProperty("properties")]
        public PropertiesConfigElement PropertiesSection => ((PropertiesConfigElement)(base["properties"]));

        [ConfigurationProperty("servicesContainer")]
        public ServicesContainerConfigElement ServicesContainerSection
        {
            get => (ServicesContainerConfigElement)base["servicesContainer"];
            set =>  base["servicesContainer"] = value;
        }

        [ConfigurationProperty("installers")]
        public InstallersConfigElement InstallersSection => ((InstallersConfigElement)(base["installers"]));

        [ConfigurationProperty("appServer")]
        public AppServerConfigElement AppServerSection
        {
            get => (AppServerConfigElement)base["appServer"];
            set => base["appServer"] = value;
        }
    }
}
