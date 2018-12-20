using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;

namespace Atdi.CoreServices.Identity
{
    public sealed class IdentityComponent : ComponentBase
    {
        public IdentityComponent()
            : base(
                  name: "IdentityCoreServices",
                  type: ComponentType.CoreServices,
                  behavior: ComponentBehavior.Simple | ComponentBehavior.SingleInstance)
        {
        }

        protected override void OnInstall()
        {
            this.Container.Register<IUserTokenProvider, UserTokenProvider>(ServiceLifetime.PerThread);
            this.Container.Register<IAuthenticationManager, AuthenticationManager>(ServiceLifetime.PerThread);
        }
    }
}
