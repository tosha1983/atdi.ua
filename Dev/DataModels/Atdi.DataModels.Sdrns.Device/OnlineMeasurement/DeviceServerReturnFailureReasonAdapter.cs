using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.DataModels.Sdrns.Device.OnlineMeasurement
{
    [Serializable]
    public class DeviceServerReturnFailureReasonAdapter : DeviceServerResult
    {
        /// <summary>
        /// Поле для передачи контекста ошибки при ее возникновении в адаптере 
        /// </summary>
        public string FailureReason { get; set; }

    }




}
