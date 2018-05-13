using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;

namespace Atdi.Platform.Logging
{
    public class LoggerInstaller : IPlatformInstaller
    {
        public void Install(IServicesContainer container, IConfigParameters parameters)
        {
            var logConfig = new LogConfig();
            if (parameters.Has(LogConfig.EventsCapacityConfigKey))
            {
                logConfig[LogConfig.EventsCapacityConfigKey] = parameters[LogConfig.EventsCapacityConfigKey];
            }

            container.RegisterInstance<ILogConfig>(logConfig, ServiceLifetime.Singleton);
            container.Register<IEventDataConvertor, DataConvertors.SimpleDataConvertor>(ServiceLifetime.Singleton);

            container.Register<ILogger, IEventsProducer, AsyncLogger>(ServiceLifetime.Singleton);
        }
    }
}
