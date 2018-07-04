using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;

namespace Atdi.Platform.Logging.EventsConsumers
{
    public class FileEventsConsumerInstaller : IPlatformInstaller
    {
        public void Install(IServicesContainer container, IConfigParameters parameters)
        {
            var resolver = container.GetResolver<IServicesResolver>();

            var config = new FileEventsConsumerConfig(parameters);
            var formatter = new FileEventFormatter(config, resolver.Resolve<IResourceResolver>());
            var consumer = new FileEventsConsumer(formatter, config);
            
            var logger = resolver.Resolve<ILogger>();
            var producer = resolver.Resolve<IEventsProducer>();
           

            producer.AddConsumer(consumer);

            logger.Info("Platform", "Initialization", "The file events consumer was installed");

        }
    }
}
