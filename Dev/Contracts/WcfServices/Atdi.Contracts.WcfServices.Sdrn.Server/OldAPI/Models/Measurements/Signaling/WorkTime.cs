using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    public class WorkTime
    {
        public DateTime StartEmitting;
        public DateTime StopEmitting;
        public int HitCount;
        public float PersentAvailability;
    }
}
