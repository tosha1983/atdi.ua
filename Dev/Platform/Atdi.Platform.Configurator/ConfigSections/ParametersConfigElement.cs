using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    [ConfigurationCollection(typeof(ParameterConfigElement), AddItemName = "parameter")]
    public class ParametersConfigElement : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ParameterConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParameterConfigElement)element).NameProperty;
        }

        public PropertyConfigElement GetParameter(string name)
        {
            return (PropertyConfigElement)BaseGet(name);
        }
    }
}
