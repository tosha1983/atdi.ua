using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    public class WorkTime
    {
        public DateTime StartEmitting;
        public DateTime StopEmitting;
        public int HitCount;
        public float PersentAvailability;
        public int ScanCount;
        public int TempCount;
    }
}
