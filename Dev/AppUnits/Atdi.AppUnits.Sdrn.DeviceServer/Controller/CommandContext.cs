using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Data;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal class CommandContext : IDisposable
    {
        public readonly Dictionary<ValueTuple<Type, CommandType, string>, IObjectPool> ResultsPool;

        public CommandContext(Dictionary<ValueTuple<Type, CommandType, string>, IObjectPool> resultsPool)
        {
            this.ResultsPool = resultsPool;
        }

        public CommandType Type;
        public ExecutionContext ExecutionContext;
        public CommandHandler Handler;
        public IResultBuffer Buffer;
        public ResultWorker Worker;
        private IObjectPool<CommandContext> _pool;

        public void Use(CommandDescriptor descriptor)
        {
            lock (this.Worker.Locker)
            {
                this._pool = null;
                this.Worker.Use(descriptor, this);
                this.ExecutionContext.Use(descriptor, this);
            }
        }

        public void FinishResultProcessing()
        {
            lock (this.Worker.Locker)
            {
                _pool?.Put(this);
                _pool = null;
            }
        }

        public void Free(IObjectPool<CommandContext> pool)
        {
            lock (this.Worker.Locker)
            {
                if (!this.Worker.Processing)
                {
                    pool.Put(this);
                    return;
                }
                this._pool = pool;
            }
            
        }

        public void Dispose()
        {
            if (this.Worker != null)
            {
                this.Worker.Dispose();
                this.Worker = null;
            }

            if (this.Buffer != null)
            {
                this.Buffer.Dispose();
                this.Buffer = null;
            }
        }
    }
}
