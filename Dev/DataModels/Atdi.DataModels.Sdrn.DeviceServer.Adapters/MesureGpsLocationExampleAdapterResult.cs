using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters
{
    public class MesureGpsLocationExampleAdapterResult : CommandResultPartBase
    {
        public MesureGpsLocationExampleAdapterResult(ulong partIndex, CommandResultStatus status) 
            : base(partIndex, status)
        {
        }

        public float Lon;

        public float Lat;

        public float ASL;

        public float AGL;
    }
}
