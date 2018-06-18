using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging.EventsConsumers
{
    public sealed class ConsoleEventsConsumer : IEventsConsumer
    {
        public static readonly int EventsLimit = 5000;

        private readonly IConsoleEventWriter _consoleWriter;
        private readonly ConsoleEventsConsumerConfig _config;
        private readonly object _locker = new object();

        public ConsoleEventsConsumer(IConsoleEventWriter consoleWriter, ConsoleEventsConsumerConfig config)
        {
            this._consoleWriter = consoleWriter;
            this._config = config;
        }

        public void Push(IEvent[] events)
        {
            if (events == null || events.Length == 0 || this._consoleWriter == null)
                return;

            lock (this._locker)
            {
                var start = 0;
                var end = events.Length;

                if (this._config.HasFilters)
                {
                    events = events.Where(@event => _config.Check(@event)).ToArray();
                }

                if (end > ConsoleEventsConsumer.EventsLimit)
                {
                    start = end - ConsoleEventsConsumer.EventsLimit;
                    Console.WriteLine();
                    Console.WriteLine($"... was skip #{start.ToString()} events of #{end.ToString()}");
                    Console.WriteLine();
                }

                for (int i = start; i < end; i++)
                {
                    this._consoleWriter.Write(events[i]);
                }
            }
        }
    }
}
