using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;

namespace Atdi.Platform.Logging.EventsConsumers
{
    public class ConsoleEventsConsumerInstaller : IPlatformInstaller
    {
        public void Install(IServicesContainer container, IConfigParameters parameters)
        {
            var resolver = container.GetResolver<IServicesResolver>();

            var config = new ConsoleEventsConsumerConfig(parameters);

            container.RegisterInstance(typeof(ConsoleEventsConsumerConfig), config, ServiceLifetime.Singleton);
            container.Register<IConsoleEventWriter, ColorConsoleEventWriter>(ServiceLifetime.Singleton);
            container.Register<ConsoleEventsConsumer>(ServiceLifetime.Singleton);


            // connect to producer
            //var rs = resolver.Resolve<IResourceResolver>();
            var logger = resolver.Resolve<ILogger>();
            var producer = resolver.Resolve<IEventsProducer>();
            var consumer = resolver.Resolve<ConsoleEventsConsumer>();

            producer.AddConsumer(consumer);

            logger.Info("Platform", "Initialization", "The console events consumer was installed");

        }
    }
}
