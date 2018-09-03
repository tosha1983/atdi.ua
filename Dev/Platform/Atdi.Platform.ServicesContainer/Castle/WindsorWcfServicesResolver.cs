using Atdi.Platform.DependencyInjection;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.ServicesContainer.Castle
{
    class WindsorWcfServicesResolver : WindsorServicesResolver, IWcfServicesResolver
    {
        private DefaultServiceHostFactory _wcfHostFactory;

        public WindsorWcfServicesResolver(IWindsorContainer container) : base(container)
        {
        }

        public TServiceHost CreateWcfServiceHost<TServiceHost>(string constructorString, Uri[] baseAddresses)
             where TServiceHost : class
        {
            if (this._wcfHostFactory == null)
            {
                this._wcfHostFactory = new DefaultServiceHostFactory(this._container.Kernel);
            }

            return (TServiceHost)(object)this._wcfHostFactory.CreateServiceHost(constructorString, baseAddresses);
        }
    }
}
