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
        private string[] _buffer;
        private int _position;

        public FileEventsConsumer(IEventFormatter formatter, FileEventsConsumerConfig config)
        {
            this._formatter = formatter;
            this._config = config;
            this._rootFolder = config.FolderPath; // Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            this._buffer = new string[1000];
            

        }

        public void Push(IEvent[] events)
        {
            if (events == null || events.Length == 0)
                return;

			if (this._buffer.Length < events.Length)
			{
				this._buffer = new string[events.Length];
			}
			this._position = -1;

			var hasFilters = this._config.HasFilters;


            var lastTime = events[0].Time;
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
                if (lastTime.Year != eventTime.Year 
                    || lastTime.Month != eventTime.Month 
                    || lastTime.Day != eventTime.Day 
                    || lastTime.Hour != eventTime.Hour )
                {
					SaveEventsToFile(lastTime);
				}

                lastTime = eventTime;
                _buffer[++this._position] = this._formatter.Format(@event);

				//buffer.AppendLine(this._formatter.Format(@event));
            }

			SaveEventsToFile(lastTime);

		}

        private void SaveEventsToFile( DateTime time)
        {
	        if (this._position == -1)
	        {
				return;
	        }

	        try
	        {
		        var folderPath = Path.Combine(this._rootFolder, time.ToString("yyyy-MM-dd"));
		        var filePath = Path.Combine(folderPath,
			        this._config.FilePrefix + time.ToString("HH.00.000 - HH.59.999") + ".log");

		        if (!Directory.Exists(folderPath))
		        {
			        Directory.CreateDirectory(folderPath);
		        }

		        //File.AppendAllText(filePath, buffer.ToString(), Encoding.Unicode);
		        using (var stream = (TextWriter) new StreamWriter(filePath, true, Encoding.Unicode))
		        {
			        for (var i = 0; i <= this._position; i++)
			        {
				        stream.WriteLine(_buffer[i]);
			        }
		        }
	        }
	        catch (Exception)
	        {
		        throw;
	        }
	        finally
	        {
		        this._position = -1;
			}
			
			
        }
    }
}
