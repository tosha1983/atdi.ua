using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.AppComponent
{
    public interface IComponent
    {
        ComponentBehavior Behavior { get; }

        ComponentType Type { get; }

        string Name { get; }

        void Install(DependencyInjection.IServicesContainer container, IComponentConfig config);

        void Activate();

        void Deactivate();

        void Uninstall();
    }
}
