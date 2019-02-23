using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    public enum CommandLockState
    {
        Locked,
        Unlocked
    }
    class CommandLock
    {
        private object _locker = new object();
        private volatile int _locksCount;


        public CommandLockState State { get => this._locksCount > 0 ? CommandLockState.Locked : CommandLockState.Unlocked ; }

        public void Lock()
        {
            Interlocked.Increment(ref this._locksCount);
        }

        public void Unlock()
        {
            if (this._locksCount > 0)
            {
                lock (_locker)
                {
                    if (this._locksCount > 0)
                    {
                        Interlocked.Decrement(ref this._locksCount);
                    }
                }
            }
        }
    }
}
