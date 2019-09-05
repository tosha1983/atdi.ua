using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public class ConfigMeasurements
    {
        /// <summary>
        /// Директория для хранения эталонных сигналов
        /// </summary>
        public string FolderSignalGSM { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CountMaxEmission { get; set; }
        
    }
}
