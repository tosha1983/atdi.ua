using Atdi.Platform;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.CoreServices.Monitoring.Statistics
{
    public class StatisticCollector : IDisposable
    {
        private readonly object _locker = new object();

        private readonly IStatistics _statistic;
        private readonly AppComponentConfig _config;
        private readonly ILogger _logger;
        private volatile bool _isDisposing;
        private Thread _thread;
        private CancellationTokenSource _tokenSource;

        private readonly int _timeout = 100;
        private Stopwatch _timer;
        private DateTime _started;
        private CounterRecord[] _buffer;
        private int _index;
        private int _count;

        public StatisticCollector(IStatistics statistic, AppComponentConfig config, ILogger logger)
        {
            this._statistic = statistic;
            this._config = config;
            this._logger = logger;
            this._timeout = config.CollectorTimeout??1000;
            this._tokenSource = new CancellationTokenSource();
            this._started = DateTime.Now;
            this._timer = Stopwatch.StartNew();

            this._buffer = new CounterRecord[config.CollectorBufferSize??10000];
            this._index = -1;
            this._count = 0;
        }

        public void Dispose()
        {
            if (!_isDisposing)
            {
                _isDisposing = true;
                _thread.Abort();
            }

        }

        public void Run()
        {
            this._thread = new Thread(this.Process)
            {
                Name = $"ATDI.Platform.StatisticCollector",
                Priority = ThreadPriority.Lowest
            };

            this._thread.Start();


        }

        private void Process()
        {
            try
            {
                while (!_tokenSource.Token.IsCancellationRequested)
                {
                    var counters = _statistic.GetCounters();
                    for (int i = 0; i < counters.Length; i++)
                    {
                        var counter = counters[i];
                        lock (_locker)
                        {
                            if (_count + 1 <= _buffer.Length)
                            {
                                ++_count;
                            }
                            if (_index + 1 < _buffer.Length)
                            {
                                ++_index;
                            }
                            else
                            {
                                _index = 0;
                            }
                            _buffer[_index].Name = counter.Key.Name;
                            _buffer[_index].Data = counter.Data;
                            _buffer[_index].Time = _started.AddMilliseconds(_timer.ElapsedMilliseconds);
                        }

                    }
                    Thread.Sleep(_timeout);
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                if (_isDisposing)
                {
                    // this is normal process
                }
            }
            catch (Exception)
            {
                //this.Logger.Exception(Contexts.EntityOrm, Categories.Processing, e, this);
            }
        }

        public IStatisticEntryKey[] GetEntryKeys()
        {
            return _statistic.GetEntryKeys();
        }

        public IStatisticCounterKey[] GetCounterKeys()
        {
            return _statistic.GetCounterKeys();
        }

        public EntryRecord[] GetEntryRecords()
        {
            var entries = _statistic.GetEntries();
            return entries.Select(e => new EntryRecord { Data = e.GetData(), Name = e.Key.Name }).ToArray();
        }

        public CounterRecord[] GetCurrentCounterRecords()
        {
            var counters = _statistic.GetCounters();
            var now = DateTime.Now;
            return counters.Select(c => new CounterRecord { Name = c.Key.Name, Data = c.Data, Time = now }).ToArray();
        }
        public CounterRecord[] GetCounterRecords()
        {
            lock (_locker)
            {
                var result = new CounterRecord[_count];
                var start = 0;
                if (_count >= _buffer.Length)
                {
                    start = _index + 1;
                    if (start + 1 >= _buffer.Length)
                    {
                        start = 0;
                    }
                }
                for (int i = 0; i < result.Length; i++)
                {
                    var index = start + i;
                    if (index >= _buffer.Length)
                    {
                        index = index - _buffer.Length;
                    }
                    result[i] = _buffer[index];
                }
                return result;
            }

        }
    }
}
