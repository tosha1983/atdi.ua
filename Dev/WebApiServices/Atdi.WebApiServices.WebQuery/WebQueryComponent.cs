using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebApiServices.WebQuery
{

    public sealed class WebQueryComponent : WebApiServicesComponent
    {
        public WebQueryComponent() : base("WebQueryWebApiServices", ComponentBehavior.Simple)
        {
        }
    }
}
