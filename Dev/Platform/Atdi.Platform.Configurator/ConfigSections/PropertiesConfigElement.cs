using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    [ConfigurationCollection(typeof(PropertyConfigElement), AddItemName = "property")]
    public class PropertiesConfigElement : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PropertyConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PropertyConfigElement)element).NameProperty;
        }

        public PropertyConfigElement GetProperty(string name)
        {
            return (PropertyConfigElement)BaseGet(name);
        }
    }
}
