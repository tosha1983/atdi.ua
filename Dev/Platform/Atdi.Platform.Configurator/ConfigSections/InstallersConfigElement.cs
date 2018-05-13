using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ConfigElements
{
    [ConfigurationCollection(typeof(InstallConfigElement), AddItemName = "install")]
    public class InstallersConfigElement : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new InstallConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InstallConfigElement)element).TypeProperty;
        }

    }
}
