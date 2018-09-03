using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.WcfServices.Identity
{
    public sealed class IdentityComponent : WcfServicesComponent
    {
        public IdentityComponent() 
            : base("IdentityWcfServices", ComponentBehavior.SingleInstance)
        {
        }
    }
}
