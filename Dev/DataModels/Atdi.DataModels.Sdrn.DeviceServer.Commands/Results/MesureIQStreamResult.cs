using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Results
{
    public class MesureIQStreamResult : CommandResultPartBase
    {
        public MesureIQStreamResult(ulong partIndex, CommandResultStatus status)
               : base(partIndex, status)
        {
        }
        public float[][] iq_samples;
        public long OneSempleDuration_ns;
        public long TimeStamp; //Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
        public long PPSTimeDifference_ns; //PPS относительно ближайшей секунды TimeStamp;
    }
}
