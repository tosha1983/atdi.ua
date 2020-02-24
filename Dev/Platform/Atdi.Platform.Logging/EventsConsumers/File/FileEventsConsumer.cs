using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging.EventsConsumers
{
    public sealed class FileEventsConsumer : IEventsConsumer, IDisposable
    {
        //private readonly IEventFormatter _formatter;
        private readonly FileEventFormatter _internalFormatter;
		private readonly FileEventsConsumerConfig _config;
        private readonly object _locker = new object();
        private readonly string _rootFolder;
        private DateTime _lastTime;

		private StreamWriter _writer;
        //private char[] _charBuffer;

		public FileEventsConsumer(IEventFormatter formatter, FileEventsConsumerConfig config)
        {
            //this._formatter = formatter;
			this._internalFormatter = formatter as FileEventFormatter;
            this._config = config;
            this._rootFolder = config.FolderPath; // Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
			this._lastTime= DateTime.MinValue;
			//this._charBuffer = new char[512];
        }

        public void Push(IEvent[] events)
        {
            if (events == null || events.Length == 0)
                return;

			var hasFilters = this._config.HasFilters;

            //var buffer = new StringBuilder(events.Length * 100);

            //var lastTime = events[0].Time;
            for (var i = 0; i < events.Length; i++)
            {
                var @event = events[i];

                if (hasFilters)
                {
                    if (!_config.Check(@event))
                    {
	                    continue;
                    }
                }

				var eventTime = @event.Time;
                if (_lastTime.Year != eventTime.Year 
                    || _lastTime.Month != eventTime.Month 
                    || _lastTime.Day != eventTime.Day 
                    || _lastTime.Hour != eventTime.Hour)
                {
                    if (_writer != null)
                    {
						_writer.Flush();
						_writer.Close();
						_writer = null;
                        //SaveEventsToFile(buffer, lastTime);
                        //buffer = new StringBuilder();
                    }
                }

                _lastTime = eventTime;

                if (_writer == null)
                {
					var folderPath = string.Intern(Path.Combine(this._rootFolder, eventTime.ToString("yyyy-MM-dd")));
					var filePath = Path.Combine(folderPath, this._config.FilePrefix + eventTime.ToString("HH.00.00.000 - HH.59.59.999") + ".log");

	                if (!Directory.Exists(folderPath))
	                {
		                Directory.CreateDirectory(folderPath);
	                }

					_writer = new StreamWriter(filePath, true, Encoding.UTF8, 65535);
                }

                this._internalFormatter.Format(@event, _writer);
                _writer.WriteLine();
            }

            _writer?.Flush();
        }

        private void SaveEventsToFile(StringBuilder buffer, DateTime time)
        {
            var folderPath = string.Intern(Path.Combine(this._rootFolder,  time.ToString("yyyy-MM-dd")));
            var filePath = Path.Combine(folderPath, this._config.FilePrefix + time.ToString("HH.00.000 - HH.59.999") + ".log");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.AppendAllText(filePath, buffer.ToString(), Encoding.UTF8);
        }

        public void Dispose()
        {
			if (_writer != null)
			{
				_writer.Flush();
				_writer.Close();
				_writer = null;
			}
		}
    }
}
