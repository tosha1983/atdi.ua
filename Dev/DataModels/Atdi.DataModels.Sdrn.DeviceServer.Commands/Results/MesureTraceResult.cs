using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Results
{
    public class MesureTraceResult : CommandResultPartBase
    {
        public MesureTraceResult(ulong partIndex, CommandResultStatus status)
               : base(partIndex, status)
        {
        }
        public float[] Level;
        public double[] Freq_Hz;
        public long TimeStamp; //Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
    }
}
