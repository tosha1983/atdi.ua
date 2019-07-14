using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class SysInfoTask : MeasurementTaskBase
    {
        public DateTime? LastTimeSend = null;
        public SysInfoResult  sysInfoResult; //  результат измерения
        public long maximumTimeForWaitingResultBandWidth; // (максимальное время ожидания результата)
    }
}
