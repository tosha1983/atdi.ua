using Atdi.Contracts.CoreServices.Monitoring;
using Atdi.Platform.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.Monitoring
{
    public class LogEventSite : ILogEventSite, IEventsConsumer
    {
        private class Enumerator : IEnumerator
        {
            private readonly object[] _data;
            private int _index;

            public Enumerator(object[] data)
            {
                this._data = data;
                this._index = -1;
            }

            public object Current
            {
                get
                {
                    if (_index == -1 || _index >= _data.Length)
                        throw new InvalidOperationException();
                    return _data[_index];
                }
            }

            public bool MoveNext()
            {
                if (_index + 1 < _data.Length )
                {
                    _index++;
                    return true;
                }
                
                return false;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        private readonly object _locker = new object();
        private readonly AppComponentConfig _config;
        private readonly ILogger _logger;

        private readonly IEvent[] _events;
        private int _lastIndex;
        private int _count; 

        public LogEventSite(AppComponentConfig config, ILogger logger)
        {
            this._config = config;
            this._logger = logger;
            this._events = new IEvent[config.LogEventBufferSize ?? 5000];
            this._lastIndex = -1;
            this._count = 0;
        }

        public IEnumerator GetEnumerator()
        {
            lock (_locker)
            {
                var data = new object[_count];
                var start = 0;
                if (_count >= _events.Length)
                {
                    start = _lastIndex + 1;
                    if (start + 1 >= _events.Length)
                    {
                        start = 0;
                    }
                }
                for (int i = 0; i < data.Length; i++)
                {
                    var index = start + i;
                    if (index >= _events.Length)
                    {
                        index = index - _events.Length;
                    }
                    data[i] = _events[index];
                }
                return new Enumerator(data);
            }
        }

        public void Push(IEvent[] events)
        {
            lock(_locker)
            {

                int start = _events.Length >= events.Length ? 0 : events.Length - _events.Length - 1;
                for (int i = start; i < events.Length; i++)
                {
                    ++_lastIndex;
                    if (_lastIndex == _events.Length)
                    {
                        _lastIndex = 0;
                    }
                    _events[_lastIndex] = events[i];
                    if (_count < _events.Length)
                    {
                        ++_count;
                    }
                }
            }
            
        }
    }
}
