using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging.EventsConsumers
{
    public sealed class FileEventsConsumer : IEventsConsumer
    {
        private readonly IEventFormatter _formatter;
        private readonly FileEventsConsumerConfig _config;
        private readonly object _locker = new object();
        private readonly string _rootFolder;
        public FileEventsConsumer(IEventFormatter formatter, FileEventsConsumerConfig config)
        {
            this._formatter = formatter;
            this._config = config;
            this._rootFolder = config.FolderPath; // Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }

        public void Push(IEvent[] events)
        {
            if (events == null || events.Length == 0)
                return;

            //lock (this._locker)
            //{
                if (this._config.HasFilters)
                {
                    events = events.Where(@event => _config.Check(@event)).ToArray();
                }
                var folderName = string.Empty; // 2018_01_31
                var fileName = string.Empty; // 14
                var buffer = new StringBuilder();

                var lastTime = events[0].Time;
                for (int i = 0; i < events.Length; i++)
                {
                    var @event = events[i];

                    var eventTime = @event.Time;
                    if (lastTime.Year != eventTime.Year 
                        || lastTime.Month != eventTime.Month 
                        || lastTime.Day != eventTime.Day 
                        || lastTime.Hour != eventTime.Hour )
                    {
                        if (buffer.Length > 0)
                        {
                            SaveEventsToFile(buffer, lastTime);
                            buffer = new StringBuilder();
                        }
                    }

                    lastTime = eventTime;
                    buffer.AppendLine(this._formatter.Format(@event));
                }

                if (buffer.Length > 0)
                {
                    SaveEventsToFile(buffer, lastTime);
                }
            //}
        }

        private void SaveEventsToFile(StringBuilder buffer, DateTime time)
        {
            var folderPath = Path.Combine(this._rootFolder,  time.ToString("yyyy-MM-dd"));
            var filePath = Path.Combine(folderPath, this._config.FilePrefix + time.ToString("HH.00.000 - HH.59.999") + ".log");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.AppendAllText(filePath, buffer.ToString(), Encoding.Unicode);
        }
    }
}
