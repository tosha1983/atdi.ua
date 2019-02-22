using Atdi.Contracts.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.DeviceServer
{
    public class TimeService : ITimeService
    {
        private readonly ITimeStamp _timeStamp;

        public TimeService()
        {
            this._timeStamp = new TimeStamp();
        }

        public ITimeStamp TimeStamp => this._timeStamp;
    }
}
