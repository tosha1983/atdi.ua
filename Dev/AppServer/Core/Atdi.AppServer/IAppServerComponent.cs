using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

namespace Atdi.AppServer
{
    public interface IAppServerComponent
    {
        AppServerComponentType Type { get;  }

        string Name { get; }

        void Install(IWindsorContainer container, IAppServerContext serverContext);

        void Activate();

        void Deactivate();

        void Uninstall(IWindsorContainer container, IAppServerContext serverContext);
    }
}
