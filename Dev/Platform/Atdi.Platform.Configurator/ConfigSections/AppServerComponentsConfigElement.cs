using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    [ConfigurationCollection(typeof(AppServerComponentConfigElement), AddItemName = "component")]
    public class AppServerComponentsConfigElement : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AppServerComponentConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var component = element as AppServerComponentConfigElement;

            var key = component.TypeProperty;
            if (string.IsNullOrEmpty(key))
            {
                key = component.AssemblyProperty;
            }

            if (!string.IsNullOrEmpty(component.InstanceProperty) && key != null)
                key += "." + component.InstanceProperty;

            return key;
        }

    }
}
