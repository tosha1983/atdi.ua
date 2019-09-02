using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    [Serializable]
    public class DeviceServerResultLevel : DeviceServerResult
    {
        /// <summary>
        /// Level
        /// </summary>
        public float[] Level { get; set; }

        /// <summary>
        /// индикатор overload 
        /// </summary>
        public bool Overload { get; set; }
    }

}
