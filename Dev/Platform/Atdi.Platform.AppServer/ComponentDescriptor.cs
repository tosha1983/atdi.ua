using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.AppServer
{
    class ComponentDescriptor
    {
        public ComponentDescriptor(IComponent component, IComponentConfig config)
        {
            this.Component = component;
            this.Config = config;
        }

        public IComponent Component { get; private set; }
        public IComponentConfig Config { get; private set; }
    }
}
