using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.Platform.DependencyInjection;

namespace Atdi.Platform.AppServer
{
    class ServerInstaller : IPlatformInstaller
    {
        public void Install(IServicesContainer container, IConfigParameters parameters)
        {
            var resolver = container.GetResolver<IServicesResolver>();
            var logger = resolver.Resolve<ILogger>();

            logger.Info(Contexts.AppServerInstaller, Categories.Installation, Events.StartedInstall);

            container.Register<IServerHost, ServerHost>(ServiceLifetime.Transient);

            logger.Info(Contexts.AppServerInstaller, Categories.Installation, Events.FinishedInstall);
        }
    }
}
