using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

namespace Atdi.AppServer
{
    public abstract class AppServerComponentBase : IAppServerComponent
    {
        private readonly AppServerComponentType _type;
        private readonly string _name;

        public AppServerComponentBase(AppServerComponentType type, string name)
        {
            this._type = type;
            this._name = name;
        }
        AppServerComponentType IAppServerComponent.Type => this._type;

        string IAppServerComponent.Name => this._name;

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public void Install(IWindsorContainer container, IAppServerContext serverContext)
        {
            throw new NotImplementedException();
        }

        public void Uninstall(IWindsorContainer container, IAppServerContext serverContext)
        {
            throw new NotImplementedException();
        }
    }
}
