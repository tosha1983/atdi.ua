using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;

namespace Atdi.WebApiServices.Identity
{
    public sealed class IdentityComponent : WebApiServicesComponent
    {
        public IdentityComponent() : base("IdentityWebApiServices", ComponentBehavior.Simple)
        {
        }
    }
}
