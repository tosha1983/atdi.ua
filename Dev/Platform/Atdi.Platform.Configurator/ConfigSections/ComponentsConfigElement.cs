using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    [ConfigurationCollection(typeof(ComponentConfigElement), AddItemName = "component")]
    public class ComponentsConfigElement : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ComponentConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ComponentConfigElement)element).ServiceProperty;
        }

    }
}
