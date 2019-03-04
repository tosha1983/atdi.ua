using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    /// <summary>
    /// Таск для обработки входящих сообщений типа DM.DeviceCommand
    /// </summary>
    public class DeviceCommandTask : TaskBase
    {
       public DM.DeviceCommand  deviceCommand { get; set; }
    }
}
