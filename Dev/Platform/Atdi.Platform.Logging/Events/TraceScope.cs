using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Atdi.Platform.Logging
{
    class TraceScope : ITraceScope, IDisposable
    {
        private bool _disposedValue = false;
        private readonly AsyncLogger _writer;
        private readonly Lazy<Dictionary<string, object>> _data = new Lazy<Dictionary<string, object>>(() => new Dictionary<string, object>());
        private readonly Stopwatch _mainTimer;
        private readonly Stopwatch _innerTraceTimer;
        public TraceScope(AsyncLogger writer, IBeginTraceEvent beginEvent, TraceScopeData scopeData)
        {
            this._writer = writer;
            this.BeginEvent = beginEvent;
            this.ScopeData = scopeData;
            this._mainTimer = new Stopwatch();
            this._mainTimer.Start();

            this._innerTraceTimer = new Stopwatch();
            this._innerTraceTimer.Start();
        }

        // To detect redundant calls

        public IBeginTraceEvent BeginEvent { get; set; }

        public ITraceScopeData ScopeData { get; set; }

        public TimeSpan? CurrentDuration
        {
            get
            {
                this._mainTimer.Stop();
                var duration = this._mainTimer.Elapsed;
                this._mainTimer.Start();
                return duration;
            }
        }

        public TimeSpan? LastDuration
        {
            get
            {
                this._innerTraceTimer.Stop();
                var duration = this._innerTraceTimer.Elapsed;
                this._innerTraceTimer.Start();
                return duration;
            }
        }

        public void Trace(EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            this._innerTraceTimer.Stop();
            this._writer.Trace(this, eventText, source, this._innerTraceTimer.Elapsed, data);
            this._innerTraceTimer.Restart();
        }


        public void SetData<T>(string key, T value)
        {
            _data.Value[key] = value;
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    this._mainTimer.Stop()
;                    if (!_data.IsValueCreated)
                    {
                        _writer.StopTrace(this, this._mainTimer.Elapsed, null);
                    }
                    else
                    {
                        _writer.StopTrace(this, this._mainTimer.Elapsed, _data.Value);
                    }
                }
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }




        #endregion
    }

    class TraceScopeDummy : ITraceScope, IDisposable
    {
        public TimeSpan? CurrentDuration => null;

        public TimeSpan? LastDuration => null;

        public IBeginTraceEvent BeginEvent => null;

        public ITraceScopeData ScopeData => null;

        public void Dispose()
        {
        }

        public void SetData<T>(string key, T value)
        {
        }

        public void Trace(EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
        }
    }
}
