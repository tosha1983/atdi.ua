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

        private readonly int _timeout = 10000;
        private Stopwatch _timer;
        private DateTime _started;
        private CounterRecord[] _buffer;
        private int _index;
        private int _count;
        private readonly string _processName;

        private readonly PerformanceCounter _privateBytesSysCounter;
        private readonly PerformanceCounter _gen0SysCounter;
        private readonly PerformanceCounter _gen1SysCounter;
        private readonly PerformanceCounter _gen2SysCounter;
        private readonly PerformanceCounter _gen0HeapSizeSysCounter;
        private readonly PerformanceCounter _gen1HeapSizeSysCounter;
        private readonly PerformanceCounter _gen2HeapSizeSysCounter;
        private readonly PerformanceCounter _lohHeapSizeSysCounter;
        private readonly PerformanceCounter _threadsLogicalSysCounter;
        private readonly PerformanceCounter _threadsPhysicalSysCounter;
        private readonly PerformanceCounter _threadsQueueSysCounter;

        public StatisticCollector(IStatistics statistic, AppComponentConfig config, ILogger logger)
        {
            this._statistic = statistic;
            this._config = config;
            this._logger = logger;
            this._timeout = config.CollectorTimeout??10000;
            this._tokenSource = new CancellationTokenSource();
            this._started = DateTime.Now;
            this._timer = Stopwatch.StartNew();

            this._buffer = new CounterRecord[config.CollectorBufferSize??10000];
            this._index = -1;
            this._count = 0;
            this._processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            this._privateBytesSysCounter = new PerformanceCounter("Process", "Private Bytes", _processName);
            this._gen0SysCounter = new PerformanceCounter(".NET CLR Memory", "# Gen 0 Collections", _processName);
            this._gen1SysCounter = new PerformanceCounter(".NET CLR Memory", "# Gen 1 Collections", _processName);
            this._gen2SysCounter = new PerformanceCounter(".NET CLR Memory", "# Gen 2 Collections", _processName);
            this._gen0HeapSizeSysCounter = new PerformanceCounter(".NET CLR Memory", "Gen 0 heap size", _processName);
            this._gen1HeapSizeSysCounter = new PerformanceCounter(".NET CLR Memory", "Gen 1 heap size", _processName);
            this._gen2HeapSizeSysCounter = new PerformanceCounter(".NET CLR Memory", "Gen 2 heap size", _processName);
            this._lohHeapSizeSysCounter = new PerformanceCounter(".NET CLR Memory", "Large Object Heap size", _processName);

            this._threadsLogicalSysCounter = new PerformanceCounter(".NET CLR LocksAndThreads", "# of current logical Threads", _processName);
            this._threadsPhysicalSysCounter = new PerformanceCounter(".NET CLR LocksAndThreads", "# of current physical Threads", _processName);
            this._threadsQueueSysCounter = new PerformanceCounter(".NET CLR LocksAndThreads", "Current Queue Length", _processName);

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
                    var process = System.Diagnostics.Process.GetCurrentProcess();

                    _statistic.Counter(STS.Counter.Process.Handles)?.Change(process.HandleCount);
                    _statistic.Counter(STS.Counter.Process.Threads)?.Change(process.Threads.Count);

                    _statistic.Counter(STS.Counter.Memory.NonpagedSystem)?.Change(process.NonpagedSystemMemorySize64);
                    _statistic.Counter(STS.Counter.Memory.Paged)?.Change(process.PagedSystemMemorySize64);
                    _statistic.Counter(STS.Counter.Memory.PagedSystem)?.Change(process.PagedSystemMemorySize64);

                    _statistic.Counter(STS.Counter.Memory.Private)?.Change(process.PrivateMemorySize64);
                    _statistic.Counter(STS.Counter.Memory.Virtual)?.Change(process.VirtualMemorySize64);
                    _statistic.Counter(STS.Counter.Memory.WorkingSet)?.Change(process.WorkingSet64);

                    _statistic.Entry(STS.Process.Memory.PeakPaged)?.Set(process.PeakPagedMemorySize64);
                    _statistic.Entry(STS.Process.Memory.PeakVirtual)?.Set(process.PeakVirtualMemorySize64);
                    _statistic.Entry(STS.Process.Memory.PeakWorkingSet)?.Set(process.PeakWorkingSet64);

                    _statistic.Counter(STS.Counter.Memory.GC.Private)?.Change(this._privateBytesSysCounter.RawValue);
                    _statistic.Counter(STS.Counter.Memory.GC.Gen0)?.Change(this._gen0SysCounter.RawValue);
                    _statistic.Counter(STS.Counter.Memory.GC.Gen1)?.Change(this._gen1SysCounter.RawValue);
                    _statistic.Counter(STS.Counter.Memory.GC.Gen2)?.Change(this._gen2SysCounter.RawValue);
                    _statistic.Counter(STS.Counter.Memory.GC.Gen0Heap)?.Change(this._gen0HeapSizeSysCounter.RawValue);
                    _statistic.Counter(STS.Counter.Memory.GC.Gen1Heap)?.Change(this._gen1HeapSizeSysCounter.RawValue);
                    _statistic.Counter(STS.Counter.Memory.GC.Gen2Heap)?.Change(this._gen2HeapSizeSysCounter.RawValue);
                    _statistic.Counter(STS.Counter.Memory.GC.LOHeap)?.Change(this._lohHeapSizeSysCounter.RawValue);

                    _statistic.Counter(STS.Counter.Process.ThreadsLogical)?.Change(this._threadsLogicalSysCounter.RawValue);
                    _statistic.Counter(STS.Counter.Process.ThreadsPhysical)?.Change(this._threadsPhysicalSysCounter.RawValue);
                    _statistic.Counter(STS.Counter.Process.ThreadsQueue)?.Change(this._threadsQueueSysCounter.RawValue);
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
