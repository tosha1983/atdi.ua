using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;

namespace Atdi.AppServer.Models.TechServices
{
    public interface ITechServiceHost
    {
        void Install(IWindsorContainer container);

        void Open();

        void Close();
    }
}
